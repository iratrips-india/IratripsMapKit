using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Iratrips.MapKit.Interfaces;
using Iratrips.MapKit.Models;
using Iratrips.MapKit.Overlays;
using Xamarin.Forms;

namespace Iratrips.MapKit
{
    /// <summary>
    /// An extensions of the <see cref="Xamarin.Forms.Maps.Map"/>
    /// </summary>
    public class MKCustomMap : View, IMapFunctions
    {
        /// <summary>
        /// Event raised when a pin gets selected
        /// </summary>
        public event EventHandler<GenericEventArgs<MKCustomMapPin>> PinSelected;
        /// <summary>
        /// Event raised when a drag of a pin ended
        /// </summary>
        public event EventHandler<GenericEventArgs<MKCustomMapPin>> PinDragEnd;
        /// <summary>
        /// Event raised when an area of the map gets clicked
        /// </summary>
        public event EventHandler<GenericEventArgs<Position>> MapClicked;
        /// <summary>
        /// Event raised when an area of the map gets long-pressed
        /// </summary>
        public event EventHandler<GenericEventArgs<Position>> MapLongPress;
        /// <summary>
        /// Event raised when the location of the user changes
        /// </summary>
        public event EventHandler<GenericEventArgs<Position>> UserLocationChanged;
        /// <summary>
        /// Event raised when a route gets tapped
        /// </summary>
        public event EventHandler<GenericEventArgs<MKRoute>> RouteClicked;
        /// <summary>
        /// Event raised when a route calculation finished successfully
        /// </summary>
        public event EventHandler<GenericEventArgs<MKRoute>> RouteCalculationFinished;
        /// <summary>
        /// Event raised when a route calculation failed
        /// </summary>
        public event EventHandler<GenericEventArgs<RouteCalculationError>> RouteCalculationFailed;
        /// <summary>
        /// Event raised when all pins are added to the map initially
        /// </summary>
        public event EventHandler PinsReady;
        /// <summary>
        /// Event raised when a callout got tapped
        /// </summary>
        public event EventHandler<GenericEventArgs<MKCustomMapPin>> CalloutClicked;
        /// <summary>
        /// Event raised when map is ready
        /// </summary>
        public event EventHandler MapReady;

        /// <summary>
        /// Property Key for the read-only bindable Property <see cref="MapFunctions"/>
        /// </summary>
         static readonly BindablePropertyKey MapFunctionsPropertyKey = BindableProperty.CreateReadOnly(
            nameof(MapFunctions),
            typeof(IRendererFunctions),
            typeof(MKCustomMap),
            null,
            defaultBindingMode: BindingMode.OneWayToSource);
        /// <summary>
        /// Bindable Property of <see cref="MapFunctions"/>
        /// </summary>
        public static readonly BindableProperty MapFunctionsProperty = MapFunctionsPropertyKey.BindableProperty;
        /// <summary>
        /// Bindable Property of <see cref="Pins" />
        /// </summary>
        public static readonly BindableProperty PinsProperty = BindableProperty.Create(
            nameof(Pins),
            typeof(IEnumerable<MKCustomMapPin>),
            typeof(MKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="SelectedPin" />
        /// </summary>
        public static readonly BindableProperty SelectedPinProperty = BindableProperty.Create(
            nameof(SelectedPin),
            typeof(MKCustomMapPin),
            typeof(MKCustomMap),
            defaultBindingMode: BindingMode.TwoWay);
        /// <summary>
        /// Bindable Property of <see cref="PinSelectedCommand" />
        /// </summary>
        public static readonly BindableProperty PinSelectedCommandProperty = BindableProperty.Create(
            nameof(PinSelectedCommand),
            typeof(ICommand),
            typeof(MKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="MapClickedCommand"/>
        /// </summary>
        public static readonly BindableProperty MapClickedCommandProperty = BindableProperty.Create(
            nameof(MapClickedCommand),
            typeof(ICommand),
            typeof(MKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="MapLongPressCommand"/>
        /// </summary>
        public static readonly BindableProperty MapLongPressCommandProperty = BindableProperty.Create(
            nameof(MapLongPressCommand),
            typeof(ICommand),
            typeof(MKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="PinDragEndCommand"/>
        /// </summary>
        public static readonly BindableProperty PinDragEndCommandProperty = BindableProperty.Create(
            nameof(PinDragEndCommand),
            typeof(Command<MKCustomMapPin>),
            typeof(MKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="PinsReadyCommand"/>
        /// </summary>
        public static readonly BindableProperty PinsReadyCommandProperty = BindableProperty.Create(
            nameof(PinsReadyCommand),
            typeof(ICommand),
            typeof(MKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="MapCenter"/>
        /// </summary>
        public static readonly BindablePropertyKey MapCenterProperty = BindableProperty.CreateReadOnly(
            nameof(MapCenter),
            typeof(Position),
            typeof(MKCustomMap),
            default(Position));
        /// <summary>
        /// Bindable Property of <see cref="IsRegionChangeAnimated"/>
        /// </summary>
        public static readonly BindableProperty IsRegionChangeAnimatedProperty = BindableProperty.Create(
            nameof(IsRegionChangeAnimated),
            typeof(bool),
            typeof(MKCustomMap),
            default(bool));
        /// <summary>
        /// Bindable Property of <see cref="ShowTraffic"/>
        /// </summary>
        public static readonly BindableProperty ShowTrafficProperty = BindableProperty.Create(
            nameof(ShowTraffic),
            typeof(bool),
            typeof(MKCustomMap),
            default(bool));
        /// <summary>
        /// Bindable Property of <see cref="Routes"/>
        /// </summary>
        public static readonly BindableProperty PolylinesProperty = BindableProperty.Create(
            nameof(Polylines),
            typeof(IEnumerable<MKPolyline>),
            typeof(MKCustomMap),
            null);
        /// <summary>
        /// Bindable Property of <see cref="Circles"/>
        /// </summary>
        public static readonly BindableProperty CirclesProperty = BindableProperty.Create(
            nameof(Circles),
            typeof(IEnumerable<MKCircle>),
            typeof(MKCustomMap),
            null);
        /// <summary>
        /// Bindable Property of <see cref="CalloutClickedCommand"/>
        /// </summary>
        public static readonly BindableProperty CalloutClickedCommandProperty = BindableProperty.Create(
            nameof(CalloutClickedCommand),
            typeof(ICommand),
            typeof(MKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="Polygons"/>
        /// </summary>
        public static readonly BindableProperty PolygonsProperty = BindableProperty.Create(
            nameof(Polygons),
            typeof(IEnumerable<MKPolygon>),
            typeof(MKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="MapRegion"/>
        /// </summary>
        public static readonly BindableProperty MapRegionProperty = BindableProperty.Create(
            nameof(MapRegion),
            typeof(MapSpan),
            typeof(MKCustomMap),
            defaultBindingMode: BindingMode.TwoWay);
        /// <summary>
        /// Bindable Property of <see cref="Routes"/>
        /// </summary>
        public static readonly BindableProperty RoutesProperty = BindableProperty.Create(
            nameof(Routes),
            typeof(IEnumerable<MKRoute>),
            typeof(MKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="RouteClickedCommand"/>
        /// </summary>
        public static readonly BindableProperty RouteClickedCommandProperty = BindableProperty.Create(
            nameof(RouteClickedCommand),
            typeof(ICommand),
            typeof(MKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="RouteCalculationFinishedCommand"/>
        /// </summary>
        public static readonly BindableProperty RouteCalculationFinishedCommandProperty = BindableProperty.Create(
            nameof(RouteCalculationFinishedCommand),
            typeof(ICommand),
            typeof(MKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="RouteCalculationFailedCommand"/>
        /// </summary>
        public static readonly BindableProperty RouteCalculationFailedCommandProperty = BindableProperty.Create(
            nameof(RouteCalculationFailedCommand),
            typeof(ICommand),
            typeof(MKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="TilesUrlOptions"/>
        /// </summary>
        public static readonly BindableProperty TilesUrlOptionsProperty = BindableProperty.Create(
            nameof(TilesUrlOptions),
            typeof(MKTileUrlOptions),
            typeof(MKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="UserLocationChangedCommand"/>
        /// </summary>
        public static readonly BindableProperty UserLocationChangedCommandProperty = BindableProperty.Create(
            nameof(UserLocationChangedCommand),
            typeof(ICommand),
            typeof(MKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="GetCalloutView"/>
        /// </summary>
        public static readonly BindableProperty GetCalloutViewProperty = BindableProperty.Create(
            nameof(GetCalloutView),
            typeof(Func<MKCustomMapPin, Xamarin.Forms.View>),
            typeof(MKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="GetClusteredPin"/>
        /// </summary>
        public static readonly BindableProperty GetClusteredPinProperty = BindableProperty.Create(
            nameof(GetClusteredPin),
            typeof(Func<string, IEnumerable<MKCustomMapPin>, MKCustomMapPin>),
            typeof(MKCustomMap));
        /// <summary>
        /// Bindable property of <see cref="IsClusteringEnabled"/>
        /// </summary>
        public static BindableProperty IsClusteringEnabledProperty = BindableProperty.Create(
            nameof(IsClusteringEnabled),
            typeof(bool),
            typeof(MKCustomMap),
            true);
        /// <summary>
        /// Binadble property of <see cref="MapType"/>
        /// </summary>
        public static readonly BindableProperty MapTypeProperty = BindableProperty.Create(
            nameof(MapType), 
            typeof(MapType), 
            typeof(MKCustomMap), 
            default(MapType));
        /// <summary>
        /// Binadble property of <see cref="IsShowingUser"/>
        /// </summary>
        public static readonly BindableProperty IsShowingUserProperty = BindableProperty.Create(
            nameof(IsShowingUser),
            typeof(bool),
            typeof(MKCustomMap),
            default(bool));
        /// <summary>
        /// Binadble property of <see cref="HasScrollEnabled"/>
        /// </summary>
        public static readonly BindableProperty HasScrollEnabledProperty = BindableProperty.Create(
            nameof(HasScrollEnabled), 
            typeof(bool),
            typeof(MKCustomMap),
            true);
        /// <summary>
        /// Binadble property of <see cref="HasZoomEnabled"/>
        /// </summary>
        public static readonly BindableProperty HasZoomEnabledProperty = BindableProperty.Create(
            nameof(HasZoomEnabled), 
            typeof(bool), 
            typeof(MKCustomMap), 
            true);
        /// <summary>
        /// Binadble property of <see cref="MapReadyCommand"/>
        /// </summary>
        public static readonly BindableProperty MapReadyCommandProperty = BindableProperty.Create(
            nameof(MapReadyCommand),
            typeof(ICommand),
            typeof(MKCustomMap),
            default(ICommand));

        /// <summary>
        /// Gets/Sets the command which is raised when the map is ready
        /// </summary>
        public ICommand MapReadyCommand
        {
            get => (ICommand)GetValue(MapReadyCommandProperty);
            set => SetValue(MapReadyCommandProperty, value);
        }
        /// <summary>
        /// Gets/Sets the current <see cref="MapType"/>
        /// </summary>
        public MapType MapType
        {
            get => (MapType)GetValue(MapTypeProperty);
            set => SetValue(MapTypeProperty, value);
        }
        /// <summary>
        /// Gets/Sets if the user should be displayed on the map
        /// </summary>
        public bool IsShowingUser
        {
            get => (bool)GetValue(IsShowingUserProperty);
            set => SetValue(IsShowingUserProperty, value);
        }
        /// <summary>
        /// Gets/Sets whether scrolling is enabled or not
        /// </summary>
        public bool HasScrollEnabled
        {
            get => (bool)GetValue(HasScrollEnabledProperty);
            set => SetValue(HasScrollEnabledProperty, value);
        }
        /// <summary>
        /// Gets/Sets whether zooming is enabled or not
        /// </summary>
        public bool HasZoomEnabled
        {
            get => (bool)GetValue(HasZoomEnabledProperty);
            set => SetValue(HasZoomEnabledProperty, value);
        }
        /// <summary>
        /// Gets/Sets the custom pins of the Map
        /// </summary>
        public IEnumerable<MKCustomMapPin> Pins
        {
            get => (IEnumerable<MKCustomMapPin>)GetValue(PinsProperty);
            set => SetValue(PinsProperty, value);
        }
        /// <summary>
        /// Gets/Sets the currently selected pin on the map
        /// </summary>
        public MKCustomMapPin SelectedPin
        {
            get => (MKCustomMapPin)GetValue(SelectedPinProperty);
            set => SetValue(SelectedPinProperty, value);
        }
        /// <summary>
        /// Gets/Sets the command when the map was clicked/tapped
        /// </summary>
        public ICommand MapClickedCommand
        {
            get => (ICommand)GetValue(MapClickedCommandProperty);
            set => SetValue(MapClickedCommandProperty, value);
        }
        /// <summary>
        /// Gets/Sets the command when a long press was performed on the map
        /// </summary>
        public ICommand MapLongPressCommand
        {
            get => (ICommand)GetValue(MapLongPressCommandProperty);
            set => SetValue(MapLongPressCommandProperty, value);
        }
        /// <summary>
        /// Gets/Sets the command when a pin drag ended. The pin already has the updated position set
        /// </summary>
        public ICommand PinDragEndCommand
        {
            get => (ICommand)GetValue(PinDragEndCommandProperty);
            set => SetValue(PinDragEndCommandProperty, value);
        }
        /// <summary>
        /// Gets/Sets the command when a pin got selected
        /// </summary>
        public ICommand PinSelectedCommand
        {
            get => (ICommand)GetValue(PinSelectedCommandProperty);
            set => SetValue(PinSelectedCommandProperty, value);
        }
        /// <summary>
        /// Gets/Sets the command when the pins are ready
        /// </summary>
        public ICommand PinsReadyCommand
        {
            get => (ICommand)GetValue(PinsReadyCommandProperty);
            set => SetValue(PinsReadyCommandProperty, value);
        }
        /// <summary>
        /// Gets/Sets the current center of the map.
        /// </summary>
        public Position MapCenter => MapRegion.Center;
        /// <summary>
        /// Gets/Sets if a change <see cref="MapRegion"/> should be animated
        /// </summary>
        public bool IsRegionChangeAnimated
        {
            get => (bool)GetValue(IsRegionChangeAnimatedProperty);
            set => SetValue(IsRegionChangeAnimatedProperty, value);
        }
        /// <summary>
        /// Gets/Sets the lines to display on the map
        /// </summary>
        public IEnumerable<MKPolyline> Polylines
        {
            get => (IEnumerable<MKPolyline>)GetValue(PolylinesProperty);
            set => SetValue(PolylinesProperty, value);
        }
        /// <summary>
        /// Gets/Sets the circles to display on the map
        /// </summary>
        public IEnumerable<MKCircle> Circles
        {
            get => (IEnumerable<MKCircle>)GetValue(CirclesProperty);
            set => SetValue(CirclesProperty, value);
        }
        /// <summary>
        /// Gets/Sets the command when a callout gets clicked. When this is set, there will be an accessory button visible inside the callout on iOS.
        /// Android will simply raise the command by clicking anywhere inside the callout, since Android simply renders a bitmap
        /// </summary>
        public ICommand CalloutClickedCommand
        {
            get => (ICommand)GetValue(CalloutClickedCommandProperty);
            set => SetValue(CalloutClickedCommandProperty, value);
        }
        /// <summary>
        /// Gets/Sets the rectangles to display on the map
        /// </summary>
        public IEnumerable<MKPolygon> Polygons
        {
            get => (IEnumerable<MKPolygon>)GetValue(PolygonsProperty);
            set => SetValue(PolygonsProperty, value);
        }
        /// <summary>
        /// Gets/Sets the visible map region
        /// </summary>
        public MapSpan MapRegion
        {
            get => (MapSpan)GetValue(MapRegionProperty);
            set => SetValue(MapRegionProperty, value);
        }
        /// <summary>
        /// Gets/Sets the routes to calculate and display on the map
        /// </summary>
        public IEnumerable<MKRoute> Routes
        {
            get => (IEnumerable<MKRoute>)GetValue(RoutesProperty);
            set => SetValue(RoutesProperty, value);
        }
        /// <summary>
        /// Gets/Sets the command when a route gets tapped
        /// </summary>
        public ICommand RouteClickedCommand
        {
            get => (Command<MKRoute>)GetValue(RouteClickedCommandProperty);
            set => SetValue(RouteClickedCommandProperty, value);
        }
        /// <summary>
        /// Gets/Sets the command when a route calculation finished successfully
        /// </summary>
        public ICommand RouteCalculationFinishedCommand
        {
            get => (ICommand)GetValue(RouteCalculationFinishedCommandProperty);
            set => SetValue(RouteCalculationFinishedCommandProperty, value);
        }
        /// <summary>
        /// Gets/Sets the command when a route calculation failed
        /// </summary>
        public ICommand RouteCalculationFailedCommand
        {
            get => (ICommand)GetValue(RouteCalculationFailedCommandProperty);
            set => SetValue(RouteCalculationFailedCommandProperty, value);
        }
        /// <summary>
        /// Gets/Sets the options for displaying custom tiles via an url
        /// </summary>
        public MKTileUrlOptions TilesUrlOptions
        {
            get => (MKTileUrlOptions)GetValue(TilesUrlOptionsProperty);
            set => SetValue(TilesUrlOptionsProperty, value);
        }
        /// <summary>
        /// Gets/Sets the command when the user location changed
        /// </summary>
        public ICommand UserLocationChangedCommand
        {
            get => (ICommand)GetValue(UserLocationChangedCommandProperty);
            set => SetValue(UserLocationChangedCommandProperty, value);
        }
        /// <summary>
        /// Gets/Sets the avaiable functions on the map/renderer
        /// </summary>
        public IRendererFunctions MapFunctions
        {
            get => (IRendererFunctions)GetValue(MapFunctionsProperty);
             set => SetValue(MapFunctionsPropertyKey, value);
        }
        /// <summary>
        /// Gets/Sets if traffic information should be displayed
        /// </summary>
        public bool ShowTraffic
        {
            get => (bool)GetValue(ShowTrafficProperty);
            set => SetValue(ShowTrafficProperty, value);
        }
        /// <summary>
        /// Gets/Sets function to retrieve a callout view. 
        /// </summary>
        public Func<MKCustomMapPin, Xamarin.Forms.View> GetCalloutView
        {
            get => (Func<MKCustomMapPin, Xamarin.Forms.View>)GetValue(GetCalloutViewProperty);
            set => SetValue(GetCalloutViewProperty, value);
        }
        /// <summary>
        /// Gets/Sets function to retrieve a pin for clustering. You receive the group name and all pins getting clustered. 
        /// </summary>
        public Func<string, IEnumerable<MKCustomMapPin>, MKCustomMapPin> GetClusteredPin
        {
            get => (Func<string, IEnumerable<MKCustomMapPin>, MKCustomMapPin>)GetValue(GetClusteredPinProperty);
            set => SetValue(GetClusteredPinProperty, value);
        }
        /// <summary>
        /// Gets/Sets whether clustering is enabled or not
        /// </summary>
        public bool IsClusteringEnabled
        {
            get => (bool)GetValue(IsClusteringEnabledProperty);
            set => SetValue(IsClusteringEnabledProperty, value);
        }
        /// <summary>
        /// Creates a new instance of <c>MKCustomMap</c>
        /// </summary>
        public MKCustomMap() 
            : base() 
        {
            MapRegion = MapSpan.FromCenterAndRadius(new Position(40.7142700, -74.0059700), Distance.FromKilometers(2));
        }
        /// <summary>
        /// Creates a new instance of <c>MKCustomMap</c>
        /// </summary>
        /// <param name="region">The initial region of the map</param>
        public MKCustomMap(MapSpan region)
        {
            MapRegion = region;
        }
        /// <summary>
        /// Creates a new instance of <see cref="MKCustomMap"/>
        /// </summary>
        /// <param name="initialLatitude">The initial latitude value</param>
        /// <param name="initialLongitude">The initial longitude value</param>
        /// <param name="distanceInKilometers">The initial zoom distance in kilometers</param>
        public MKCustomMap(double initialLatitude, double initialLongitude, double distanceInKilometers) : 
            this(MapSpan.FromCenterAndRadius(new Position(initialLatitude, initialLongitude), Distance.FromKilometers(distanceInKilometers)))
        {
        }
        /// <summary>
        /// Returns the currently visible map as a PNG image
        /// </summary>
        /// <returns>Map as image</returns>
        public async Task<byte[]> GetSnapshot() => await MapFunctions.GetSnapshot();
        /// <summary>
        /// Moves the visible region to the specified <see cref="MapSpan"/>
        /// </summary>
        /// <param name="region">Region to move the map to</param>
        /// <param name="animate">If the region change should be animated or not</param>
        public void MoveToMapRegion(MapSpan region, bool animate = false) => MapFunctions.MoveToMapRegion(region, animate);
        /// <summary>
        /// Fits the map region to make all given positions visible
        /// </summary>
        /// <param name="positions">Positions to fit inside the MapRegion</param>
        /// <param name="animate">If the camera change should be animated</param>
        public void FitMapRegionToPositions(IEnumerable<Position> positions, bool animate = false, int padding = 0) => MapFunctions.FitMapRegionToPositions(positions, animate, padding);
        /// <summary>
        /// Fit all regions on the map
        /// </summary>
        /// <param name="regions">The regions to fit to the map</param>
        /// <param name="animate">Animation on/off</param>
        public void FitToMapRegions(IEnumerable<MapSpan> regions, bool animate = false, int padding = 0) => MapFunctions.FitToMapRegions(regions, animate, padding);
        /// <summary>
        /// Converts an array of <see cref="Point"/> into geocoordinates
        /// </summary>
        /// <param name="screenLocations">The screen locations(pixel)</param>
        /// <returns>A collection of <see cref="Position"/></returns>
        public IEnumerable<Position> ScreenLocationsToGeocoordinates(params Point[] screenLocations) => MapFunctions.ScreenLocationsToGeocoordinates(screenLocations);
        /// <summary>
        /// Raises <see cref="PinSelected"/>
        /// </summary>
        /// <param name="pin">The selected pin</param>
        protected void OnPinSelected(MKCustomMapPin pin)
        {
            PinSelected?.Invoke(this, new GenericEventArgs<MKCustomMapPin>(pin));

            RaiseCommand(PinSelectedCommand, pin);
        }
        /// <summary>
        /// Raises <see cref="PinDragEnd"/>
        /// </summary>
        /// <param name="pin">The dragged pin</param>
        protected void OnPinDragEnd(MKCustomMapPin pin)
        {
            PinDragEnd?.Invoke(this, new GenericEventArgs<MKCustomMapPin>(pin));

            RaiseCommand(PinDragEndCommand, pin);
        }
        /// <summary>
        /// Raises <see cref="MapClicked"/>
        /// </summary>
        /// <param name="position">The position on the map</param>
        protected void OnMapClicked(Position position)
        {
            MapClicked?.Invoke(this, new GenericEventArgs<Position>(position));

            RaiseCommand(MapClickedCommand, position);
        }
        /// <summary>
        /// Raises <see cref="MapLongPress"/>
        /// </summary>
        /// <param name="position">The position on the map</param>
        protected void OnMapLongPress(Position position)
        {
            MapLongPress?.Invoke(this, new GenericEventArgs<Position>(position));

            RaiseCommand(MapLongPressCommand, position);
        }
        /// <summary>
        /// Raises <see cref="RouteClicked"/>
        /// </summary>
        /// <param name="route">The tapped route</param>
        protected void OnRouteClicked(MKRoute route)
        {
            RouteClicked?.Invoke(this, new GenericEventArgs<MKRoute>(route));

            RaiseCommand(RouteClickedCommand, route);
        }
        /// <summary>
        /// Raises <see cref="RouteCalculationFinished"/>
        /// </summary>
        /// <param name="route">The route</param>
        protected void OnRouteCalculationFinished(MKRoute route)
        {
            RouteCalculationFinished?.Invoke(this, new GenericEventArgs<MKRoute>(route));

            RaiseCommand(RouteCalculationFinishedCommand, route);
        }
        /// <summary>
        /// Raises <see cref="RouteCalculationFailed"/>
        /// </summary>
        /// <param name="error">The error</param>
        protected void OnRouteCalculationFailed(RouteCalculationError error)
        {
            RouteCalculationFailed?.Invoke(this, new GenericEventArgs<RouteCalculationError>(error));

            RaiseCommand(RouteCalculationFailedCommand, error);
        }
        /// <summary>
        /// Raises <see cref="UserLocationChanged"/>
        /// </summary>
        /// <param name="position">The position of the user</param>
        protected void OnUserLocationChanged(Position position)
        {
            UserLocationChanged?.Invoke(this, new GenericEventArgs<Position>(position));

            RaiseCommand(UserLocationChangedCommand, position);
        }
        /// <summary>
        /// Raises <see cref="PinsReady"/>
        /// </summary>
        protected void OnPinsReady()
        {
            PinsReady?.Invoke(this, new EventArgs());

            RaiseCommand(PinsReadyCommand, null);
        }
        /// <summary>
        /// Raises <see cref="CalloutClicked"/>
        /// </summary>
        protected void OnCalloutClicked(MKCustomMapPin pin)
        {
            CalloutClicked?.Invoke(this, new GenericEventArgs<MKCustomMapPin>(pin));

            RaiseCommand(CalloutClickedCommand, pin);
        }
        /// <summary>
        /// Raises <see cref="MapReady"/>
        /// </summary>
        protected void OnMapReady()
        {
            MapReady?.Invoke(this, EventArgs.Empty);
            RaiseCommand(MapReadyCommand, null);
        }
        /// <summary>
        /// Raises a specific command
        /// </summary>
        /// <param name="command">The command to raise</param>
        /// <param name="parameter">Addition command parameter</param>
         void RaiseCommand(ICommand command, object parameter)
        {
            if(command != null && command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        void IMapFunctions.SetRenderer(IRendererFunctions renderer) => MapFunctions = renderer;
        /// <inheritdoc/>
        void IMapFunctions.RaisePinSelected(MKCustomMapPin pin) => OnPinSelected(pin);
        /// <inheritdoc/>
        void IMapFunctions.RaisePinDragEnd(MKCustomMapPin pin) => OnPinDragEnd(pin);
        /// <inheritdoc/>
        void IMapFunctions.RaiseMapClicked(Position position) => OnMapClicked(position);
        /// <inheritdoc/>
        void IMapFunctions.RaiseMapLongPress(Position position) => OnMapLongPress(position);
        /// <inheritdoc/>
        void IMapFunctions.RaiseUserLocationChanged(Position position) => OnUserLocationChanged(position);
        /// <inheritdoc/>
        void IMapFunctions.RaiseRouteClicked(MKRoute route) => OnRouteClicked(route);
        /// <inheritdoc/>
        void IMapFunctions.RaiseRouteCalculationFinished(MKRoute route) => OnRouteCalculationFinished(route);
        /// <inheritdoc/>
        void IMapFunctions.RaiseRouteCalculationFailed(RouteCalculationError route) => OnRouteCalculationFailed(route);
        /// <inheritdoc/>
        void IMapFunctions.RaisePinsReady() => OnPinsReady();
        /// <inheritdoc/>
        void IMapFunctions.RaiseCalloutClicked(MKCustomMapPin pin) => OnCalloutClicked(pin);
        /// <inheritdoc/>
        void IMapFunctions.RaiseMapReady() => OnMapReady();
    }
}
