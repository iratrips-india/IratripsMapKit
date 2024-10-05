using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Gms.Maps.Utils;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using Iratrips.Mapkit;
using Iratrips.Mapkit.Api.Google;
using Iratrips.Mapkit.Interfaces;
using Iratrips.Mapkit.Models;
using Iratrips.Mapkit.Overlays;
using Iratrips.Mapkit.Utilities;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Iratrips.Mapkit.Droid
{
    public class IratripsMapView : MapView
    {
        private Action _onLayoutAction;
        private bool _isLayoutPerformed = false;

        public IratripsMapView(Action onLayoutAction, Context context) : base(context)
        {
            _onLayoutAction = onLayoutAction;
        }

        ///<inheritdoc/>
        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            if (!_isLayoutPerformed)
            {
                _isLayoutPerformed = true;
                _onLayoutAction?.Invoke();
                _onLayoutAction = null;
            }
        }

    }

    public class TKCustomMapEventListener : Java.Lang.Object, GoogleMap.ISnapshotReadyCallback, GoogleMap.IOnCameraIdleListener,
        IOnMapReadyCallback, GoogleMap.IInfoWindowAdapter, GoogleMap.IOnCameraMoveStartedListener
    {
        private TKCustomMapHandler _handler;

        public TKCustomMapEventListener(TKCustomMapHandler handler)
        {
            _handler = handler;
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            _handler.OnMapReady(googleMap);
        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            return _handler.GetInfoContents(marker);
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return _handler.GetInfoWindow(marker);
        }

        public void OnCameraIdle()
        {
            _handler.OnCameraIdle();
        }

        public void OnCameraMoveStarted(int reason)
        {
            _handler.OnCameraMoveStarted(reason);
        }

        public void OnSnapshotReady(Bitmap snapshot)
        {
            _handler.OnSnapshotReady(snapshot);
        }

        public void OnCameraChange(object sender, GoogleMap.CameraChangeEventArgs e)
        {
            _handler.OnCameraChange(sender, e);
        }

        public void CleanUp()
        {
            _handler = null;
        }

    }

    /// <summary>
    /// Android Map Handler of <see cref="Iratrips.Mapkit.TKCustomMap"/>
    /// </summary>
    public class TKCustomMapHandler : ViewHandler<TKCustomMap, IratripsMapView>, IRendererFunctions, IDisposable
    {
        object _lockObj = new object();

        bool _isInitialized;
        public bool _isLayoutPerformed;

        readonly List<TKRoute> _tempRouteList = new List<TKRoute>();

        readonly Dictionary<TKRoute, Polyline> _routes = new Dictionary<TKRoute, Polyline>();
        readonly Dictionary<TKPolyline, Polyline> _polylines = new Dictionary<TKPolyline, Polyline>();
        readonly Dictionary<TKCircle, Circle> _circles = new Dictionary<TKCircle, Circle>();
        readonly Dictionary<TKPolygon, Polygon> _polygons = new Dictionary<TKPolygon, Polygon>();
        readonly Dictionary<TKCustomMapPin, TKMarker> _markers = new Dictionary<TKCustomMapPin, TKMarker>();

        Marker _selectedMarker;
        bool _isDragging;
        bool _disposed;
        byte[] _snapShot;

        TileOverlay _tileOverlay;
        GoogleMap _googleMap;


        static Bundle s_bundle;
        internal static Bundle Bundle { set { s_bundle = value; } }

        GoogleMap Map => _googleMap;

        protected override IratripsMapView CreatePlatformView()
        {
            var mapView = new IratripsMapView(OnLayout, Context);
            mapView.OnCreate(s_bundle);
            mapView.OnResume();
            return mapView;
        }

        private TKCustomMap VirtualMap => this.VirtualView;
        private IMapFunctions MapFunctions => this.VirtualView;

        private TKCustomMapEventListener _listener = null;

        void OnLayout()
        {
            _isLayoutPerformed = true;
            _isInitialized = true;
            
            UpdateMapRegion();
            MapFunctions?.RaiseMapReady();
        }

        protected override void ConnectHandler(IratripsMapView mapView)
        {
            base.ConnectHandler(mapView);
            if (!TKGoogleMaps.IsInitialized) throw new Exception("Call MKGoogleMaps.Init first");

            lock (_lockObj)
            {
                if (mapView == null) return;

                _listener?.CleanUp();
                _listener = new TKCustomMapEventListener(this);
                mapView.GetMapAsync(_listener);

                this.VirtualView.MapFunctions = this;
            }
        }

        protected override void DisconnectHandler(IratripsMapView platformView)
        {
            base.DisconnectHandler(platformView);
            UnregisterCollections();
            this.VirtualView.MapFunctions = null;

            if (_googleMap != null)
            {
                _googleMap.MarkerClick -= OnMarkerClick;
                _googleMap.MapClick -= OnMapClick;
                _googleMap.MapLongClick -= OnMapLongClick;
                _googleMap.MarkerDragEnd -= OnMarkerDragEnd;
                _googleMap.MarkerDrag -= OnMarkerDrag;
                _googleMap.MarkerDragStart -= OnMarkerDragStart;
                _googleMap.InfoWindowClick -= OnInfoWindowClick;
                _googleMap.MyLocationChange -= OnUserLocationChange;
                _googleMap.SetOnCameraIdleListener(null);
                _googleMap.SetOnCameraMoveStartedListener(null);
                _googleMap.SetInfoWindowAdapter(null);
                _googleMap = null;
            }

            CleanUp();
            _listener?.CleanUp();
        }

        /// <summary>
        /// When the map is ready to use
        /// </summary>
        /// <param name="googleMap">The map instance</param>
        public void OnMapReady(GoogleMap googleMap)
        {
            lock (_lockObj)
            {
                _googleMap = googleMap;

                _googleMap.MarkerClick += OnMarkerClick;
                _googleMap.MapClick += OnMapClick;
                _googleMap.MapLongClick += OnMapLongClick;
                _googleMap.MarkerDragEnd += OnMarkerDragEnd;
                _googleMap.MarkerDrag += OnMarkerDrag;
                _googleMap.MarkerDragStart += OnMarkerDragStart;
                _googleMap.InfoWindowClick += OnInfoWindowClick;
                _googleMap.MyLocationChange += OnUserLocationChange;

                _googleMap.SetOnCameraIdleListener(_listener);
                _googleMap.SetOnCameraMoveStartedListener(_listener);
                _googleMap.SetInfoWindowAdapter(_listener);

                UpdateTileOptions();
                UpdateMapRegion();
                UpdatePins();
                UpdateRoutes();
                UpdateLines();
                UpdateCircles();
                UpdatePolygons();
                UpdateShowTraffic();
                UpdateMapType();
                UpdateIsShowingUser();
                UpdateHasZoomEnabled();
                UpdateHasScrollEnabled();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            CleanUp();
        }

        public void CleanUp()
        {
            UnregisterCollections();

            if (_googleMap != null)
            {
                _tempRouteList?.Clear();
                _selectedMarker = null;

                if (_polylines != null)
                {
                    foreach (var line in _polylines)
                    {
                        line.Key.PropertyChanged -= OnLinePropertyChanged;
                        line.Value.Remove();
                    }

                    _polylines.Clear();
                }

                if (_routes != null)
                {
                    foreach (var route in _routes)
                    {
                        route.Key.PropertyChanged -= OnRoutePropertyChanged;
                        route.Value.Remove();
                    }

                    _routes.Clear();
                }

                if (_circles != null)
                {
                    foreach (var circle in _circles)
                    {
                        circle.Key.PropertyChanged -= CirclePropertyChanged;
                        circle.Value.Remove();
                    }

                    _circles.Clear();
                }

                if (_polygons != null)
                {
                    foreach (var polygon in _polygons)
                    {
                        polygon.Key.PropertyChanged -= OnPolygonPropertyChanged;
                        polygon.Value.Remove();
                    }

                    _polygons.Clear();
                }

                if (_markers != null)
                {
                    foreach (var marker in _markers)
                    {
                        marker.Key.PropertyChanged -= OnPinPropertyChanged;
                        marker.Value.Marker?.Remove();
                        marker.Value.CleanUp();
                    }

                    _markers.Clear();
                }

                _googleMap.MarkerClick -= OnMarkerClick;
                _googleMap.MapClick -= OnMapClick;
                _googleMap.MapLongClick -= OnMapLongClick;
                _googleMap.MarkerDragEnd -= OnMarkerDragEnd;
                _googleMap.MarkerDrag -= OnMarkerDrag;
                _googleMap.MarkerDragStart -= OnMarkerDragStart;
                _googleMap.InfoWindowClick -= OnInfoWindowClick;
                _googleMap.MyLocationChange -= OnUserLocationChange;
                _googleMap.SetOnCameraIdleListener(null);
                _googleMap.SetOnCameraMoveStartedListener(null);
                _googleMap.SetInfoWindowAdapter(null);
                _googleMap.Dispose();
                _googleMap = null;
                _snapShot = null;
                _tileOverlay = null;
                _listener = null;
            }
        }

        #region Mappers
        public static PropertyMapper<TKCustomMap, TKCustomMapHandler> PropertyMapper = new PropertyMapper<TKCustomMap, TKCustomMapHandler>(ViewHandler.ViewMapper)
        {
            [nameof(TKCustomMap.Pins)] = MapPins,
            [nameof(TKCustomMap.SelectedPin)] = MapSelectedPin,
            [nameof(TKCustomMap.Polylines)] = MapPolylines,
            [nameof(TKCustomMap.Polygons)] = MapPolygons,
            [nameof(TKCustomMap.Circles)] = MapCircles,
            [nameof(TKCustomMap.Routes)] = MapRoutes,
            [nameof(TKCustomMap.TilesUrlOptions)] = MapTilesUrlOptions,
            [nameof(TKCustomMap.ShowTraffic)] = MapShowTraffic,
            [nameof(TKCustomMap.MapRegion)] = MapMapRegion,
            [nameof(TKCustomMap.MapType)] = MapMapType,
            [nameof(TKCustomMap.IsShowingUser)] = MapIsShowingUser,
            [nameof(TKCustomMap.HasScrollEnabled)] = MapHasScrollEnabled,
            [nameof(TKCustomMap.HasZoomEnabled)] = MapHasZoomEnabled,
        };

        public TKCustomMapHandler() : base(PropertyMapper)
        {
        }

        public static void MapPins(TKCustomMapHandler handler, TKCustomMap cfMap)
        {
            handler.UpdatePins();
        }

        public static void MapSelectedPin(TKCustomMapHandler handler, TKCustomMap cfMap)
        {
            handler.SetSelectedItem();
        }

        public static void MapPolylines(TKCustomMapHandler handler, TKCustomMap cfMap)
        {
            handler.UpdateLines();
        }

        public static void MapCircles(TKCustomMapHandler handler, TKCustomMap cfMap)
        {
            handler.UpdateCircles();
        }

        public static void MapPolygons(TKCustomMapHandler handler, TKCustomMap cfMap)
        {
            handler.UpdatePolygons();
        }

        public static void MapRoutes(TKCustomMapHandler handler, TKCustomMap cfMap)
        {
            handler.UpdateRoutes();
        }

        public static void MapTilesUrlOptions(TKCustomMapHandler handler, TKCustomMap cfMap)
        {
            handler.UpdateTileOptions();
        }
        public static void MapShowTraffic(TKCustomMapHandler handler, TKCustomMap cfMap)
        {
            handler.UpdateShowTraffic();
        }

        public static void MapMapRegion(TKCustomMapHandler handler, TKCustomMap cfMap)
        {
            handler.UpdateMapRegion();
        }

        public static void MapMapType(TKCustomMapHandler handler, TKCustomMap cfMap)
        {
            handler.UpdateMapType();
        }

        public static void MapIsShowingUser(TKCustomMapHandler handler, TKCustomMap cfMap)
        {
            handler.UpdateIsShowingUser();
        }

        public static void MapHasScrollEnabled(TKCustomMapHandler handler, TKCustomMap cfMap)
        {
            handler.UpdateHasScrollEnabled();
        }

        public static void MapHasZoomEnabled(TKCustomMapHandler handler, TKCustomMap cfMap)
        {
            handler.UpdateHasZoomEnabled();
        }
        #endregion

        /// <summary>
        /// When the location of the user changed
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        void OnUserLocationChange(object sender, GoogleMap.MyLocationChangeEventArgs e)
        {
            if (e.Location == null || VirtualMap == null) return;

            var newPosition = new Position(e.Location.Latitude, e.Location.Longitude);
            MapFunctions.RaiseUserLocationChanged(newPosition);
        }

        /// <summary>
        /// When the info window gets clicked
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        public void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            var pin = GetPinByMarker(e.Marker);

            if (pin == null || pin.Callout == null) return;

            if (pin.Callout.IsClickable)
                MapFunctions.RaiseCalloutClicked(pin);
        }

        /// <summary>
        /// Dragging process
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        void OnMarkerDrag(object sender, GoogleMap.MarkerDragEventArgs e)
        {
            var item = _markers.SingleOrDefault(i => true == i.Value.Marker?.Id.Equals(e.Marker.Id));
            if (item.Key == null) return;

            item.Key.Position = e.Marker.Position.ToPosition();
        }
        /// <summary>
        /// When a dragging starts
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        void OnMarkerDragStart(object sender, GoogleMap.MarkerDragStartEventArgs e)
        {
            _isDragging = true;
        }
        /// <summary>
        /// When the camera position changed
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        public void OnCameraChange(object sender, GoogleMap.CameraChangeEventArgs e)
        {
            if (VirtualMap == null) return;
            VirtualMap.MapRegion = GetCurrentMapRegion(e.Position.Target);
        }

        /// <summary>
        /// When a pin gets clicked
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        void OnMarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            if (VirtualMap == null) return;
            var item = _markers.SingleOrDefault(i => true == i.Value.Marker?.Id.Equals(e.Marker.Id));
            if (item.Key == null) return;

            _selectedMarker = e.Marker;
            VirtualMap.SelectedPin = item.Key;
            if (item.Key.Callout != null)
            {
                item.Value.Marker.ShowInfoWindow();
            }
        }
        /// <summary>
        /// When a drag of a marker ends
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        void OnMarkerDragEnd(object sender, GoogleMap.MarkerDragEndEventArgs e)
        {
            _isDragging = false;

            if (VirtualMap == null) return;

            var pin = _markers.SingleOrDefault(i => true == i.Value.Marker?.Id.Equals(e.Marker.Id));
            if (pin.Key == null) return;
            MapFunctions.RaisePinDragEnd(pin.Key);
        }

        /// <summary>
        /// When a long click was performed on the map
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        void OnMapLongClick(object sender, GoogleMap.MapLongClickEventArgs e)
        {
            if (VirtualMap == null) return;

            var position = e.Point.ToPosition();
            MapFunctions.RaiseMapLongPress(position);
        }
        /// <summary>
        /// When the map got tapped
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        void OnMapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            if (VirtualMap == null) return;

            var position = e.Point.ToPosition();

            if (VirtualMap.Routes != null)
            {
                foreach (var route in VirtualMap.Routes.Where(i => i.Selectable))
                {
                    var internalRoute = _routes[route];

                    if (GmsPolyUtil.IsLocationOnPath(
                        position,
                        internalRoute.Points.Select(i => i.ToPosition()),
                        true,
                        (int)_googleMap.CameraPosition.Zoom,
                        VirtualMap.MapCenter.Latitude))
                    {
                        MapFunctions.RaiseRouteClicked(route);
                        return;
                    }
                }
            }

            if (VirtualMap.Polylines != null)
            {
                foreach (var poly in VirtualMap.Polylines)
                {
                    var internalPolyline = _polylines[poly];

                    if (GmsPolyUtil.IsLocationOnPath(
                            position,
                            internalPolyline.Points.Select(i => i.ToPosition()),
                            true,
                            (int)_googleMap.CameraPosition.Zoom,
                            VirtualMap.MapCenter.Latitude))
                    {
                        MapFunctions.RaisePolylineClicked(poly);
                        return;
                    }
                }
            }
            MapFunctions.RaiseMapClicked(position);
        }
        /// <summary>
        /// Updates the markers when a pin gets added or removed in the collection
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        void OnCustomPinsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (TKCustomMapPin pin in e.NewItems)
                {
                    AddPin(pin);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (TKCustomMapPin pin in e.OldItems)
                {
                    if (!VirtualMap.Pins.Contains(pin))
                    {
                        RemovePin(pin);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                UpdatePins(false);
            }
        }

        /// <summary>
        /// When a property of a pin changed
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        async void OnPinPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var pin = sender as TKCustomMapPin;
            if (pin == null) return;


            TKMarker marker = null;
            if (!_markers.ContainsKey(pin) || (marker = _markers[pin]) == null) return;
            await marker.HandlePropertyChangedAsync(e, _isDragging);
        }

        /// <summary>
        /// Collection of routes changed
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        void OnLineCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (TKPolyline line in e.NewItems)
                {
                    AddLine(line);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (TKPolyline line in e.OldItems)
                {
                    if (!VirtualMap.Polylines.Contains(line))
                    {
                        _polylines[line].Remove();
                        line.PropertyChanged -= OnLinePropertyChanged;
                        _polylines.Remove(line);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                UpdateLines(false);
            }
        }

        /// <summary>
        /// A property of a route changed
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        void OnLinePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var line = (TKPolyline)sender;

            if (e.PropertyName == nameof(TKPolyline.LineCoordinates))
            {
                if (line.LineCoordinates != null && line.LineCoordinates.Count > 1)
                {
                    _polylines[line].Points = new List<LatLng>(line.LineCoordinates.Select(i => i.ToLatLng()));
                }
                else
                {
                    _polylines[line].Points = null;
                }
            }
            else if (e.PropertyName == nameof(TKPolyline.Color))
            {
                _polylines[line].Color = line.Color.ToMaui().ToAndroid().ToArgb();
            }
            else if (e.PropertyName == nameof(TKPolyline.LineWidth))
            {
                _polylines[line].Width = line.LineWidth;
            }
        }

        /// <summary>
        /// Creates all Markers on the map
        /// </summary>
        void UpdatePins(bool firstUpdate = true)
        {
            if (_googleMap == null) return;

            foreach (var i in _markers)
            {
                RemovePin(i.Key, false);
            }
            _markers.Clear();
            if (VirtualMap.Pins != null)
            {
                foreach (var pin in VirtualMap.Pins)
                {
                    AddPin(pin);
                }
                if (firstUpdate)
                {
                    var observable = VirtualMap.Pins as INotifyCollectionChanged;
                    if (observable != null)
                    {
                        observable.CollectionChanged += OnCustomPinsCollectionChanged;
                    }
                }
                MapFunctions.RaisePinsReady();
            }
        }

        /// <summary>
        /// Adds a marker to the map
        /// </summary>
        /// <param name="pin">The Forms Pin</param>
        void AddPin(TKCustomMapPin pin)
        {
            if (_markers.Keys.Contains(pin)) return;

            pin.PropertyChanged += OnPinPropertyChanged;

            var tkMarker = new TKMarker(pin, Context);
            var markerWithIcon = new MarkerOptions();
            tkMarker.InitializeMarkerOptions(markerWithIcon);

            _markers.Add(pin, tkMarker);
            tkMarker.Marker = _googleMap.AddMarker(markerWithIcon);
        }

        /// <summary>
        /// Remove a pin from the map and the internal dictionary
        /// </summary>
        /// <param name="pin">The pin to remove</param>
        /// <param name="removeMarker">true to remove the marker from the map</param>
        void RemovePin(TKCustomMapPin pin, bool removeMarker = true)
        {
            if (!_markers.TryGetValue(pin, out var item)) return;

            if (_selectedMarker != null)
            {
                if (item.Marker.Id.Equals(_selectedMarker.Id))
                {
                    VirtualMap.SelectedPin = null;
                }
            }

            item.Marker?.Remove();
            pin.PropertyChanged -= OnPinPropertyChanged;

            if (removeMarker)
            {
                _markers.Remove(pin);
            }
        }

        /// <summary>
        /// Set the selected item on the map
        /// </summary>
        void SetSelectedItem()
        {
            if (_selectedMarker != null)
            {
                _selectedMarker.HideInfoWindow();
                _selectedMarker = null;
            }
            if (VirtualMap.SelectedPin != null)
            {
                if (!_markers.ContainsKey(VirtualMap.SelectedPin)) return;

                var selectedPin = _markers[VirtualMap.SelectedPin];
                _selectedMarker = selectedPin.Marker;
                if (VirtualMap.SelectedPin.Callout != null)
                {
                    selectedPin.Marker.ShowInfoWindow();
                }

                MapFunctions.RaisePinSelected(VirtualMap.SelectedPin);
            }
        }

        /// <summary>
        /// Creates the routes on the map
        /// </summary>
        void UpdateLines(bool firstUpdate = true)
        {
            if (_googleMap == null) return;

            foreach (var i in _polylines)
            {
                i.Key.PropertyChanged -= OnLinePropertyChanged;
                i.Value.Remove();
            }
            _polylines.Clear();

            if (VirtualMap.Polylines != null)
            {
                foreach (var line in VirtualMap.Polylines)
                {
                    AddLine(line);
                }

                if (firstUpdate)
                {
                    var observable = VirtualMap.Polylines as INotifyCollectionChanged;
                    if (observable != null)
                    {
                        observable.CollectionChanged += OnLineCollectionChanged;
                    }
                }
            }
        }

        /// <summary>
        /// Updates all circles
        /// </summary>
        void UpdateCircles(bool firstUpdate = true)
        {
            if (_googleMap == null) return;

            foreach (var i in _circles)
            {
                i.Key.PropertyChanged -= CirclePropertyChanged;
                i.Value.Remove();
            }
            _circles.Clear();
            if (VirtualMap.Circles != null)
            {
                foreach (var circle in VirtualMap.Circles)
                {
                    AddCircle(circle);
                }
                if (firstUpdate)
                {
                    var observable = VirtualMap.Circles as INotifyCollectionChanged;
                    if (observable != null)
                    {
                        observable.CollectionChanged += CirclesCollectionChanged;
                    }
                }
            }
        }

        /// <summary>
        /// Creates the polygones on the map
        /// </summary>
        /// <param name="firstUpdate">If the collection updates the first time</param>
        void UpdatePolygons(bool firstUpdate = true)
        {
            if (_googleMap == null) return;

            foreach (var i in _polygons)
            {
                i.Key.PropertyChanged -= OnPolygonPropertyChanged;
                i.Value.Remove();
            }
            _polygons.Clear();
            if (VirtualMap.Polygons != null)
            {
                foreach (var i in VirtualMap.Polygons)
                {
                    AddPolygon(i);
                }
                if (firstUpdate)
                {
                    var observable = VirtualMap.Polygons as INotifyCollectionChanged;
                    if (observable != null)
                    {
                        observable.CollectionChanged += OnPolygonsCollectionChanged;
                    }
                }
            }
        }

        /// <summary>
        /// Create all routes
        /// </summary>
        /// <param name="firstUpdate">If first update of collection or not</param>
        void UpdateRoutes(bool firstUpdate = true)
        {
            _tempRouteList.Clear();

            if (_googleMap == null) return;

            foreach (var i in _routes)
            {
                if (i.Key != null)
                    i.Key.PropertyChanged -= OnRoutePropertyChanged;
                i.Value.Remove();
            }
            _routes.Clear();

            if (VirtualMap == null || VirtualMap.Routes == null) return;

            foreach (var i in VirtualMap.Routes)
            {
                AddRoute(i);
            }

            if (firstUpdate)
            {
                var observable = VirtualMap.Routes as INotifyCollectionChanged;
                if (observable != null)
                {
                    observable.CollectionChanged += OnRouteCollectionChanged;
                }
            }
        }

        /// <summary>
        /// When the collection of routes changed
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        void OnRouteCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (TKRoute route in e.NewItems)
                {
                    AddRoute(route);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (TKRoute route in e.OldItems)
                {
                    if (!VirtualMap.Routes.Contains(route))
                    {
                        _routes[route].Remove();
                        route.PropertyChanged -= OnRoutePropertyChanged;
                        _routes.Remove(route);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                UpdateRoutes(false);
            }
        }

        /// <summary>
        /// When a property of a route changed
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        void OnRoutePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var route = (TKRoute)sender;

            if (e.PropertyName == nameof(TKRoute.Source) ||
                e.PropertyName == nameof(TKRoute.Destination) ||
                e.PropertyName == nameof(TKRoute.TravelMode))
            {
                route.PropertyChanged -= OnRoutePropertyChanged;
                _routes[route].Remove();
                _routes.Remove(route);

                AddRoute(route);
            }
            else if (e.PropertyName == nameof(TKPolyline.Color))
            {
                _routes[route].Color = route.Color.ToMaui().ToAndroid().ToArgb();
            }
            else if (e.PropertyName == nameof(TKPolyline.LineWidth))
            {
                _routes[route].Width = route.LineWidth;
            }
        }

        /// <summary>
        /// When the polygon collection changed
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        void OnPolygonsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (TKPolygon poly in e.NewItems)
                {
                    AddPolygon(poly);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (TKPolygon poly in e.OldItems)
                {
                    if (!VirtualMap.Polygons.Contains(poly))
                    {
                        _polygons[poly].Remove();
                        poly.PropertyChanged -= OnPolygonPropertyChanged;
                        _polygons.Remove(poly);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                UpdatePolygons(false);
            }
        }

        /// <summary>
        /// Adds a polygon to the map
        /// </summary>
        /// <param name="polygon">The polygon to add</param>
        void AddPolygon(TKPolygon polygon)
        {
            polygon.PropertyChanged += OnPolygonPropertyChanged;

            var polygonOptions = new PolygonOptions();

            if (polygon.Coordinates != null && polygon.Coordinates.Any())
            {
                polygonOptions.Add(polygon.Coordinates.Select(i => i.ToLatLng()).ToArray());
            }
            if (polygon.Color != null)
            {
                polygonOptions.InvokeFillColor(polygon.Color.ToMaui().ToAndroid().ToArgb());
            }
            if (polygon.StrokeColor != null)
            {
                polygonOptions.InvokeStrokeColor(polygon.StrokeColor.ToMaui().ToAndroid().ToArgb());
            }
            polygonOptions.InvokeStrokeWidth(polygon.StrokeWidth);

            _polygons.Add(polygon, _googleMap.AddPolygon(polygonOptions));
        }

        /// <summary>
        /// When a property of a <see cref="TKPolygon"/> changed
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        void OnPolygonPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var tkPolygon = (TKPolygon)sender;

            switch (e.PropertyName)
            {
                case nameof(TKPolygon.Coordinates):
                    _polygons[tkPolygon].Points = tkPolygon.Coordinates.Select(i => i.ToLatLng()).ToList();
                    break;
                case nameof(TKPolygon.Color):
                    _polygons[tkPolygon].FillColor = tkPolygon.Color.ToMaui().ToAndroid().ToArgb();
                    break;
                case nameof(TKPolygon.StrokeColor):
                    _polygons[tkPolygon].StrokeColor = tkPolygon.StrokeColor.ToMaui().ToAndroid().ToArgb();
                    break;
                case nameof(TKPolygon.StrokeWidth):
                    _polygons[tkPolygon].StrokeWidth = tkPolygon.StrokeWidth;
                    break;
            }
        }

        /// <summary>
        /// When the circle collection changed
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        void CirclesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (TKCircle circle in e.NewItems)
                {
                    AddCircle(circle);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (TKCircle circle in e.OldItems)
                {
                    if (!VirtualMap.Circles.Contains(circle))
                    {
                        circle.PropertyChanged -= CirclePropertyChanged;
                        _circles[circle].Remove();
                        _circles.Remove(circle);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                UpdateCircles(false);
            }
        }

        /// <summary>
        /// Adds a circle to the map
        /// </summary>
        /// <param name="circle">The circle to add</param>
        void AddCircle(TKCircle circle)
        {
            circle.PropertyChanged += CirclePropertyChanged;

            var circleOptions = new CircleOptions();

            circleOptions.InvokeRadius(circle.Radius);
            circleOptions.InvokeCenter(circle.Center.ToLatLng());

            if (circle.Color != null)
            {
                circleOptions.InvokeFillColor(circle.Color.ToMaui().ToAndroid().ToArgb());
            }
            if (circle.StrokeColor != null)
            {
                circleOptions.InvokeStrokeColor(circle.StrokeColor.ToMaui().ToAndroid().ToArgb());
            }
            circleOptions.InvokeStrokeWidth(circle.StrokeWidth);
            _circles.Add(circle, _googleMap.AddCircle(circleOptions));
        }

        /// <summary>
        /// When a property of a <see cref="TKCircle"/> changed
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        void CirclePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var tkCircle = (TKCircle)sender;
            var circle = _circles[tkCircle];

            switch (e.PropertyName)
            {
                case nameof(TKCircle.Radius):
                    circle.Radius = tkCircle.Radius;
                    break;
                case nameof(TKCircle.Center):
                    circle.Center = tkCircle.Center.ToLatLng();
                    break;
                case nameof(TKCircle.Color):
                    circle.FillColor = tkCircle.Color.ToMaui().ToAndroid().ToArgb();
                    break;
                case nameof(TKCircle.StrokeColor):
                    circle.StrokeColor = tkCircle.StrokeColor.ToMaui().ToAndroid().ToArgb();
                    break;
            }
        }

        /// <summary>
        /// Adds a route to the map
        /// </summary>
        /// <param name="line">The route to add</param>
        void AddLine(TKPolyline line)
        {
            line.PropertyChanged += OnLinePropertyChanged;

            var polylineOptions = new PolylineOptions();
            if (line.Color != null)
            {
                polylineOptions.InvokeColor(line.Color.ToMaui().ToAndroid().ToArgb());
            }
            if (line.LineWidth > 0)
            {
                polylineOptions.InvokeWidth(line.LineWidth);
            }

            if (line.LineCoordinates != null)
            {
                polylineOptions.Add(line.LineCoordinates.Select(i => i.ToLatLng()).ToArray());
            }

            _polylines.Add(line, _googleMap.AddPolyline(polylineOptions));
        }

        /// <summary>
        /// Calculates and adds the route to the map
        /// </summary>
        /// <param name="route">The route to add</param>
        async void AddRoute(TKRoute route)
        {
            if (route == null) return;

            _tempRouteList.Add(route);

            route.PropertyChanged += OnRoutePropertyChanged;

            GmsDirectionResult routeData = null;
            string errorMessage = null;

            if (VirtualMap == null || Map == null || !_tempRouteList.Contains(route)) return;

            GmsRouteResult r = null;

            if (route.ProvidedRouteData == null)
            {
                routeData = await GmsDirection.Instance.CalculateRoute(route.Source, route.Destination, route.TravelMode.ToGmsTravelMode());
                if (routeData != null && routeData.Routes != null)
                {
                    if (routeData.Status == GmsDirectionResultStatus.Ok)
                        r = routeData.Routes.FirstOrDefault();
                    else
                        errorMessage = routeData.Status.ToString();
                }
                else
                    errorMessage = "Could not connect to api";
            }
            else
                r = route.ProvidedRouteData;

            if (r != null && r.Legs != null && r.Legs.Count() > 0)
            {
                SetRouteData(route, r);

                var routeOptions = new PolylineOptions();

                if (route.Color != null)
                {
                    routeOptions.InvokeColor(route.Color.ToMaui().ToAndroid().ToArgb());
                }
                if (route.LineWidth > 0)
                {
                    routeOptions.InvokeWidth(route.LineWidth);
                }

                List<LatLng> allPoints = new List<LatLng>();
                foreach (var leg in r.Legs)
                {
                    foreach (var step in leg.Steps)
                        allPoints.AddRange(step.Polyline.Positions.Select(i => i.ToLatLng()));
                }

                var pointsArray = allPoints.ToArray();
                routeOptions.Add(pointsArray);

                _routes.Add(route, _googleMap.AddPolyline(routeOptions));

                MapFunctions.RaiseRouteCalculationFinished(route);
            }
            else
                errorMessage = "Unexpected result";

            if (!string.IsNullOrEmpty(errorMessage))
            {
                var routeCalculationError = new RouteCalculationError(route, errorMessage);

                MapFunctions.RaiseRouteCalculationFailed(routeCalculationError);
            }
        }

        /// <summary>
        /// Sets the route calculation data
        /// </summary>
        /// <param name="route">The PCL route</param>
        /// <param name="routeResult">The route api result</param>
        void SetRouteData(TKRoute route, GmsRouteResult routeResult)
        {
            var latLngBounds = new LatLngBounds(
                    new LatLng(routeResult.Bounds.SouthWest.Latitude, routeResult.Bounds.SouthWest.Longitude),
                    new LatLng(routeResult.Bounds.NorthEast.Latitude, routeResult.Bounds.NorthEast.Longitude));

            var apiSteps = routeResult.Legs.First().Steps;
            var steps = new TKRouteStep[apiSteps.Count()];
            var routeFunctions = (IRouteFunctions)route;


            for (int i = 0; i < steps.Length; i++)
            {
                steps[i] = new TKRouteStep();
                var stepFunctions = (IRouteStepFunctions)steps[i];
                var apiStep = apiSteps.ElementAt(i);

                stepFunctions.SetDistance(apiStep.Distance.Value);
                stepFunctions.SetInstructions(apiStep.HtmlInstructions);
            }
            routeFunctions.SetSteps(steps);
            routeFunctions.SetDistance(routeResult.Legs.First().Distance.Value);
            routeFunctions.SetTravelTime(routeResult.Legs.First().Duration.Value);

            routeFunctions.SetBounds(
                MapSpan.FromCenterAndRadius(
                    latLngBounds.Center.ToPosition(),
                    Distance.FromKilometers(
                        new Position(latLngBounds.Southwest.Latitude, latLngBounds.Southwest.Longitude)
                        .DistanceTo(
                            new Position(latLngBounds.Northeast.Latitude, latLngBounds.Northeast.Longitude)) / 2)));
            routeFunctions.SetIsCalculated(true);
        }

        /// <summary>
        /// Updates the image of a pin
        /// </summary>
        /// <param name="pin">The forms pin</param>
        /// <param name="markerOptions">The native marker options</param>
        Task UpdateImage(TKCustomMapPin pin, MarkerOptions markerOptions)
        {
            BitmapDescriptor bitmap;
            try
            {
                if (pin.Image != null)
                {
                    bitmap = BitmapDescriptorFactory.FromBitmap(pin.Image.ToMaui().ToBitmap(Context));
                }
                else
                {
                    if (pin.DefaultPinColor != null)
                    {
                        var hue = pin.DefaultPinColor.ToMaui().ToAndroid().GetHue();
                        bitmap = BitmapDescriptorFactory.DefaultMarker(Math.Min(hue, 359.99f));
                    }
                    else
                    {
                        bitmap = BitmapDescriptorFactory.DefaultMarker();
                    }
                }
            }
            catch (Exception)
            {
                bitmap = BitmapDescriptorFactory.DefaultMarker();
            }

            markerOptions.SetIcon(bitmap);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Updates the image on a marker
        /// </summary>
        /// <param name="pin">The forms pin</param>
        /// <param name="marker">The native marker</param>
        Task UpdateImage(TKCustomMapPin pin, Marker marker)
        {
            BitmapDescriptor bitmap;
            try
            {
                if (pin.Image != null)
                {
                    bitmap = BitmapDescriptorFactory.FromBitmap(pin.Image.ToMaui().ToBitmap(Context));
                }
                else
                {
                    if (pin.DefaultPinColor != null)
                    {
                        var hue = pin.DefaultPinColor.ToMaui().ToAndroid().GetHue();
                        bitmap = BitmapDescriptorFactory.DefaultMarker(hue);
                    }
                    else
                    {
                        bitmap = BitmapDescriptorFactory.DefaultMarker();
                    }
                }
            }
            catch (Exception)
            {
                bitmap = BitmapDescriptorFactory.DefaultMarker();
            }

            marker.SetIcon(bitmap);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Updates the custom tile provider 
        /// </summary>
        void UpdateTileOptions()
        {
            if (_tileOverlay != null)
            {
                _tileOverlay.Remove();
                _googleMap.MapType = GoogleMap.MapTypeNormal;
            }

            if (VirtualMap == null || _googleMap == null) return;

            if (VirtualMap.TilesUrlOptions != null)
            {
                _googleMap.MapType = GoogleMap.MapTypeNone;

                _tileOverlay = _googleMap.AddTileOverlay(
                    new TileOverlayOptions()
                        .InvokeTileProvider(
                            new TKCustomTileProvider(VirtualMap.TilesUrlOptions))
                        .InvokeZIndex(-1));
            }
        }

        /// <summary>
        /// Updates the visible map region
        /// </summary>
        void UpdateMapRegion()
        {
            if (VirtualMap == null || _googleMap == null || !_isLayoutPerformed || VirtualMap.MapRegion == null || !VirtualMap.IsVisible) return;

            if (!VirtualMap.MapRegion.Equals(GetCurrentMapRegion(_googleMap.CameraPosition.Target)))
            {
                MoveToMapRegion(VirtualMap.MapRegion, VirtualMap.IsRegionChangeAnimated);
            }
        }

        /// <summary>
        /// Sets traffic enabled on the google map
        /// </summary>
        void UpdateShowTraffic()
        {
            if (VirtualMap == null || _googleMap == null) return;

            _googleMap.TrafficEnabled = VirtualMap.ShowTraffic;
        }

        /// <summary>
        /// Updates the map type
        /// </summary>
        void UpdateMapType()
        {
            if (VirtualMap == null || _googleMap == null) return;

            switch (VirtualMap.MapType)
            {
                case MapType.Hybrid:
                    Map.MapType = GoogleMap.MapTypeHybrid;
                    break;
                case MapType.Satellite:
                    Map.MapType = GoogleMap.MapTypeSatellite;
                    break;
                case MapType.Street:
                    Map.MapType = GoogleMap.MapTypeNormal;
                    break;
            }
        }

        /// <summary>
        /// Updates if the user location and user location button are displayed
        /// </summary>
        void UpdateIsShowingUser()
        {
            if (VirtualMap == null || _googleMap == null) return;
            Map.MyLocationEnabled = Map.UiSettings.MyLocationButtonEnabled = VirtualMap.IsShowingUser;
        }

        /// <summary>
        /// Updates scroll gesture
        /// </summary>
        void UpdateHasScrollEnabled()
        {
            if (VirtualMap == null || _googleMap == null) return;

            Map.UiSettings.ScrollGesturesEnabled = VirtualMap.HasScrollEnabled;
        }

        /// <summary>
        /// Updates zoom gesture/control
        /// </summary>
        void UpdateHasZoomEnabled()
        {
            if (VirtualMap == null || _googleMap == null) return;

            Map.UiSettings.ZoomGesturesEnabled = Map.UiSettings.ZoomControlsEnabled = VirtualMap.HasZoomEnabled;
        }

        /// <summary>
        /// Creates a <see cref="LatLngBounds"/> from a collection of <see cref="MapSpan"/>
        /// </summary>
        /// <param name="spans">The spans to get calculate the bounds from</param>
        /// <returns>The bounds</returns>
        LatLngBounds BoundsFromMapSpans(params MapSpan[] spans)
        {
            LatLngBounds.Builder builder = new LatLngBounds.Builder();

            foreach (var region in spans)
            {
                builder
                    .Include(GmsSphericalUtil.ComputeOffset(region.Center, region.Radius.Meters, 0).ToLatLng())
                    .Include(GmsSphericalUtil.ComputeOffset(region.Center, region.Radius.Meters, 90).ToLatLng())
                    .Include(GmsSphericalUtil.ComputeOffset(region.Center, region.Radius.Meters, 180).ToLatLng())
                    .Include(GmsSphericalUtil.ComputeOffset(region.Center, region.Radius.Meters, 270).ToLatLng());
            }
            return builder.Build();
        }

        /// <summary>
        /// Unregisters all collections
        /// </summary>
        void UnregisterCollections()
        {
            UnregisterCollection(VirtualMap.Pins, OnCustomPinsCollectionChanged, OnPinPropertyChanged);
            UnregisterCollection(VirtualMap.Routes, OnRouteCollectionChanged, OnRoutePropertyChanged);
            UnregisterCollection(VirtualMap.Polylines, OnLineCollectionChanged, OnLinePropertyChanged);
            UnregisterCollection(VirtualMap.Circles, CirclesCollectionChanged, CirclePropertyChanged);
            UnregisterCollection(VirtualMap.Polygons, OnPolygonsCollectionChanged, OnPolygonPropertyChanged);
        }

        /// <summary>
        /// Unregisters one collection and all of its items
        /// </summary>
        /// <param name="collection">The collection to unregister</param>
        /// <param name="observableHandler">The <see cref="NotifyCollectionChangedEventHandler"/> of the collection</param>
        /// <param name="propertyHandler">The <see cref="PropertyChangedEventHandler"/> of the collection items</param>
        void UnregisterCollection(
           IEnumerable collection,
           NotifyCollectionChangedEventHandler observableHandler,
           PropertyChangedEventHandler propertyHandler)
        {
            if (collection == null) return;

            var observable = collection as INotifyCollectionChanged;
            if (observable != null)
            {
                observable.CollectionChanged -= observableHandler;
            }
            foreach (INotifyPropertyChanged obj in collection)
            {
                obj.PropertyChanged -= propertyHandler;
            }
        }

        /// <summary>
        /// Gets the current mapregion
        /// </summary>
        /// <param name="center">Center point</param>
        /// <returns>The map region</returns>
        MapSpan GetCurrentMapRegion(LatLng center)
        {
            var map = _googleMap;
            if (map == null)
                return null;

            var projection = map.Projection;
            var width = PlatformView.Width;
            var height = PlatformView.Height;
            var ul = projection.FromScreenLocation(new global::Android.Graphics.Point(0, 0));
            var ur = projection.FromScreenLocation(new global::Android.Graphics.Point(width, 0));
            var ll = projection.FromScreenLocation(new global::Android.Graphics.Point(0, height));
            var lr = projection.FromScreenLocation(new global::Android.Graphics.Point(width, height));
            var dlat = Math.Max(Math.Abs(ul.Latitude - lr.Latitude), Math.Abs(ur.Latitude - ll.Latitude));
            var dlong = Math.Max(Math.Abs(ul.Longitude - lr.Longitude), Math.Abs(ur.Longitude - ll.Longitude));

            return new MapSpan(new Position(center.Latitude, center.Longitude), dlat, dlong);
        }

        /// <inheritdoc/>
        public async Task<byte[]> GetSnapshot()
        {
            if (_googleMap == null) return null;

            _snapShot = null;
            _googleMap.Snapshot(_listener);

            while (_snapShot == null) await Task.Delay(10);

            return _snapShot;
        }

        ///<inheritdoc/>
        public void OnSnapshotReady(Bitmap snapshot)
        {
            using (var strm = new MemoryStream())
            {
                snapshot.Compress(Bitmap.CompressFormat.Png, 100, strm);
                _snapShot = strm.ToArray();
            }
        }

        ///<inheritdoc/>
        public void FitMapRegionToPositions(IEnumerable<Position> positions, bool animate = false, int padding = 0)
        {
            if (_googleMap == null) throw new InvalidOperationException("Map not ready");
            if (positions == null) throw new InvalidOperationException("positions can't be null");

            LatLngBounds.Builder builder = new LatLngBounds.Builder();

            positions.ToList().ForEach(i => builder.Include(i.ToLatLng()));

            if (animate)
                _googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngBounds(builder.Build(), padding));
            else
                _googleMap.MoveCamera(CameraUpdateFactory.NewLatLngBounds(builder.Build(), padding));
        }

        ///<inheritdoc/>
        public void MoveToMapRegion(MapSpan region, bool animate)
        {
            if (_googleMap == null) return;

            if (region == null) return;

            var bounds = BoundsFromMapSpans(region);
            if (bounds == null) return;
            var cam = CameraUpdateFactory.NewLatLngBounds(bounds, 0);

            if (animate && _isInitialized)
                _googleMap.AnimateCamera(cam);
            else
                _googleMap.MoveCamera(cam);
        }

        //https://stackoverflow.com/questions/65482783/snap-markers-to-nearest-polyline-point-google-maps-flutter/73684671#73684671
        public Position GetSnapPosition(Position nearestPointOnRoute, Position nextPointOnRoute, Position currentPosition)
        {
            var closestPointLatLng = new LatLng(nearestPointOnRoute.Latitude, nearestPointOnRoute.Longitude);
            var nextPointLatLng = new LatLng(nextPointOnRoute.Latitude, nextPointOnRoute.Longitude);
            var positionLatLng = new LatLng(currentPosition.Latitude, currentPosition.Longitude);

            var distance = SphericalUtil.ComputeDistanceBetween(closestPointLatLng, positionLatLng);
            var heading = SphericalUtil.ComputeHeading(closestPointLatLng, nextPointLatLng);

            var extrapolated = SphericalUtil.ComputeOffset(closestPointLatLng, distance, heading);
            return new Position(extrapolated.Latitude, extrapolated.Longitude);
        }

        public bool IsOnScreen(Position current)
        {
            LatLng newPos = new LatLng(current.Latitude, current.Longitude);
            var bounds = _googleMap.Projection.VisibleRegion.LatLngBounds;
            return bounds.Contains(newPos);
        }

        public float? GetCurrentBearing()
        {
            var value = _googleMap?.CameraPosition?.Bearing;
            if (value == null) return null;

            if (value > 180)
                return (float)(value - 360);
            else
                return (float)value;
        }

        //https://stackoverflow.com/questions/52262064/animate-camera-to-position-and-set-panning-in-google-maps/52272870#52272870
        public void MoveToCurrentForDriving(Position current, double bearing)
        {
            if (_googleMap == null || !_isInitialized) return;

            var currentPosition = _googleMap.CameraPosition;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (bearing == 404)
                bearing = currentPosition.Bearing;

            LatLng newPos = new LatLng(current.Latitude, current.Longitude);

            //-----------------------------------------------
            // Next section really only needs to be done once

            // Compute distance of pixels on screen using some desirable "offset"

            Projection p = _googleMap.Projection;
            var bottomRightPoint = p.ToScreenLocation(p.VisibleRegion.NearRight);
            var center = new Android.Graphics.Point(bottomRightPoint.X / 2, bottomRightPoint.Y / 2);
            var offset = new Android.Graphics.Point(center.X, (center.Y + 300));

            LatLng centerLoc = p.FromScreenLocation(center);
            LatLng offsetNewLoc = p.FromScreenLocation(offset);

            // this computed value only changes on zoom
            double offsetDistance = SphericalUtil.ComputeDistanceBetween(centerLoc, offsetNewLoc);
            //-----------------------------------------------

            // Compute shadow target position from current position (see diagram)
            LatLng shadowTgt = SphericalUtil.ComputeOffset(newPos, offsetDistance, bearing);

            // update camera
            var b = new CameraPosition.Builder();
            b.Zoom(currentPosition.Zoom);
            b.Bearing((float)(bearing));

            var bounds = _googleMap.Projection.VisibleRegion.LatLngBounds;

            var nePoint = p.ToScreenLocation(bounds.Northeast);
            var swPoint = p.ToScreenLocation(bounds.Southwest);

            var stPoint = p.ToScreenLocation(shadowTgt);

            int padding = 50;

            if (stPoint.Y < nePoint.Y + padding || stPoint.Y > swPoint.Y - padding || stPoint.X < nePoint.X + padding || stPoint.X > swPoint.X - padding)
                b.Target(shadowTgt);
            else
                b.Target(_googleMap.CameraPosition.Target);

            var cu = CameraUpdateFactory.NewCameraPosition(b.Build());
            _googleMap.AnimateCamera(cu);
        }

        ///<inheritdoc/>
        public void FitToMapRegions(IEnumerable<MapSpan> regions, bool animate = false, int padding = 0)
        {
            if (_googleMap == null || regions == null || !regions.Any()) return;

            var bounds = BoundsFromMapSpans(regions.ToArray());
            if (bounds == null) return;
            var cam = CameraUpdateFactory.NewLatLngBounds(bounds, padding);

            if (animate)
                _googleMap.AnimateCamera(cam);
            else
                _googleMap.MoveCamera(cam);
        }

        ///<inheritdoc/>
        public IEnumerable<Position> ScreenLocationsToGeocoordinates(params TKPoint[] screenLocations)
        {
            if (_googleMap == null)
                throw new InvalidOperationException("Map not initialized");

            return screenLocations.Select(i => _googleMap.Projection.FromScreenLocation(i.ToMaui().ToAndroidPoint()).ToPosition());
        }

        /// <summary>
        /// Gets the <see cref="TKCustomMapPin"/> by the native <see cref="Marker"/>
        /// </summary>
        /// <param name="marker">The marker to search the pin for</param>
        /// <returns>The forms pin</returns>
        protected TKCustomMapPin GetPinByMarker(Marker marker)
        {
            return _markers.SingleOrDefault(i => i.Value.Marker?.Id == marker.Id).Key;
        }

        public void OnCameraIdle()
        {
            if (VirtualMap == null) return;
            VirtualMap.MapRegion = GetCurrentMapRegion(Map.CameraPosition.Target);
            MapFunctions.RaiseCameraIdeal();
        }

        public void OnCameraMoveStarted(int reason)
        {
            if (VirtualMap == null) return;
            MapFunctions.RaiseCameraMoveStarted();
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        public Android.Views.View GetInfoContents(Marker baseMarker)
        {
            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            if (inflater != null && VirtualMap.SelectedPin != null && VirtualMap.SelectedPin.Callout != null && VirtualMap.SelectedPin.Callout.HasCustomView)
            {
                if (VirtualMap.SelectedPin.Callout == null || VirtualMap.GetCalloutView == null) return null;

                Microsoft.Maui.Controls.View xfView = VirtualMap.GetCalloutView?.Invoke(VirtualMap.SelectedPin);

                var nativeView = (Android.Views.View)xfView.Handler?.PlatformView;
                //renderer.Tracker.UpdateLayout();
                xfView.Layout(new Microsoft.Maui.Graphics.Rect(0, 0, this.Context.ToPixels(xfView.WidthRequest), this.Context.ToPixels(xfView.HeightRequest)));

                LinearLayout layout = new LinearLayout(this.Context);
                layout.LayoutParameters = new LinearLayout.LayoutParams((int)this.Context.ToPixels(xfView.WidthRequest), (int)this.Context.ToPixels(xfView.HeightRequest));
                layout.SetPadding(0, 0, 0, 0);
                layout.AddView(nativeView);

                return layout;
            }

            return null;
        }

        private class OnMoveCameraComplete : Java.Lang.Object, GoogleMap.ICancelableCallback
        {
            private readonly Action _completeAction;
            public OnMoveCameraComplete(Action completeAction)
            {
                _completeAction = completeAction;
            }

            public void OnCancel()
            {

            }

            public void OnFinish()
            {
                _completeAction?.Invoke();
            }
        }

        public void AnimateMarkerPosition(TKCustomMapPin pin, Position newPosition)
        {
            if (!_markers.TryGetValue(pin, out var marker))
                return;

            marker.AnimateMarkerPosition(newPosition);
        }

        public void AnimateMarkerPosition(TKCustomMapPin pin, double speed, Position currentPosition, List<Position> nextPositions)
        {
            if (!_markers.TryGetValue(pin, out var marker))
                return;

            marker.AnimateMarkerPosition(speed, currentPosition, nextPositions);
        }

        public void StopAnimateMarkerPosition(TKCustomMapPin pin)
        {
            if (pin == null) return;
            if (!_markers.TryGetValue(pin, out var marker))
                return;

            marker.CancelAnimation();
        }
    }
}
