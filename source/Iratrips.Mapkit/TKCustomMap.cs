using Iratrips.Mapkit.Interfaces;
using Iratrips.Mapkit.Models;
using Iratrips.Mapkit.Overlays;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Iratrips.Mapkit
{
    public partial class TKCustomMap : View, IMapFunctions
    {
        /// <summary>
        /// Event raised when a pin gets selected
        /// </summary>
        public event EventHandler<GenericEventArgs<TKCustomMapPin>> PinSelected;
        /// <summary>
        /// Event raised when a drag of a pin ended
        /// </summary>
        public event EventHandler<GenericEventArgs<TKCustomMapPin>> PinDragEnd;
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
        public event EventHandler<GenericEventArgs<TKRoute>> RouteClicked;
        /// <summary>
        /// Event raised when a route gets tapped
        /// </summary>
        public event EventHandler<GenericEventArgs<TKPolyline>> PolylineClicked;
        /// <summary>
        /// Event raised when a route calculation finished successfully
        /// </summary>
        public event EventHandler<GenericEventArgs<TKRoute>> RouteCalculationFinished;
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
        public event EventHandler<GenericEventArgs<TKCustomMapPin>> CalloutClicked;
        /// <summary>
        /// Event raised when map is ready
        /// </summary>
        public event EventHandler MapReady;
        /// <summary>
        /// Event raised when camera movement has ended, there are no pending animations and the user has stopped interacting with the map.
        /// </summary>
        public event EventHandler CameraIdeal;

        /// <summary>
        /// Event raised when the camera starts moving after it has been idle or when the reason for camera motion has changed.
        /// </summary>
        public event EventHandler CameraMoveStarted;

        /// <summary>
        /// Property Key for the read-only bindable Property <see cref="MapFunctions"/>
        /// </summary>
        static readonly BindablePropertyKey MapFunctionsPropertyKey = BindableProperty.CreateReadOnly(
            nameof(MapFunctions),
            typeof(IRendererFunctions),
            typeof(TKCustomMap),
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
            typeof(IEnumerable<TKCustomMapPin>),
            typeof(TKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="SelectedPin" />
        /// </summary>
        public static readonly BindableProperty SelectedPinProperty = BindableProperty.Create(
            nameof(SelectedPin),
            typeof(TKCustomMapPin),
            typeof(TKCustomMap),
            defaultBindingMode: BindingMode.TwoWay);
        /// <summary>
        /// Bindable Property of <see cref="PinSelectedCommand" />
        /// </summary>
        public static readonly BindableProperty PinSelectedCommandProperty = BindableProperty.Create(
            nameof(PinSelectedCommand),
            typeof(ICommand),
            typeof(TKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="MapClickedCommand"/>
        /// </summary>
        public static readonly BindableProperty MapClickedCommandProperty = BindableProperty.Create(
            nameof(MapClickedCommand),
            typeof(ICommand),
            typeof(TKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="MapLongPressCommand"/>
        /// </summary>
        public static readonly BindableProperty MapLongPressCommandProperty = BindableProperty.Create(
            nameof(MapLongPressCommand),
            typeof(ICommand),
            typeof(TKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="PinDragEndCommand"/>
        /// </summary>
        public static readonly BindableProperty PinDragEndCommandProperty = BindableProperty.Create(
            nameof(PinDragEndCommand),
            typeof(Command<TKCustomMapPin>),
            typeof(TKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="PinsReadyCommand"/>
        /// </summary>
        public static readonly BindableProperty PinsReadyCommandProperty = BindableProperty.Create(
            nameof(PinsReadyCommand),
            typeof(ICommand),
            typeof(TKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="MapCenter"/>
        /// </summary>
        public static readonly BindablePropertyKey MapCenterProperty = BindableProperty.CreateReadOnly(
            nameof(MapCenter),
            typeof(Position),
            typeof(TKCustomMap),
            default(Position));
        /// <summary>
        /// Bindable Property of <see cref="IsRegionChangeAnimated"/>
        /// </summary>
        public static readonly BindableProperty IsRegionChangeAnimatedProperty = BindableProperty.Create(
            nameof(IsRegionChangeAnimated),
            typeof(bool),
            typeof(TKCustomMap),
            default(bool));
        /// <summary>
        /// Bindable Property of <see cref="ShowTraffic"/>
        /// </summary>
        public static readonly BindableProperty ShowTrafficProperty = BindableProperty.Create(
            nameof(ShowTraffic),
            typeof(bool),
            typeof(TKCustomMap),
            default(bool));
        /// <summary>
        /// Bindable Property of <see cref="Routes"/>
        /// </summary>
        public static readonly BindableProperty PolylinesProperty = BindableProperty.Create(
            nameof(Polylines),
            typeof(IEnumerable<TKPolyline>),
            typeof(TKCustomMap),
            null);
        /// <summary>
        /// Bindable Property of <see cref="Circles"/>
        /// </summary>
        public static readonly BindableProperty CirclesProperty = BindableProperty.Create(
            nameof(Circles),
            typeof(IEnumerable<TKCircle>),
            typeof(TKCustomMap),
            null);
        /// <summary>
        /// Bindable Property of <see cref="CalloutClickedCommand"/>
        /// </summary>
        public static readonly BindableProperty CalloutClickedCommandProperty = BindableProperty.Create(
            nameof(CalloutClickedCommand),
            typeof(ICommand),
            typeof(TKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="Polygons"/>
        /// </summary>
        public static readonly BindableProperty PolygonsProperty = BindableProperty.Create(
            nameof(Polygons),
            typeof(IEnumerable<TKPolygon>),
            typeof(TKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="MapRegion"/>
        /// </summary>
        public static readonly BindableProperty MapRegionProperty = BindableProperty.Create(
            nameof(MapRegion),
            typeof(MapSpan),
            typeof(TKCustomMap),
            defaultBindingMode: BindingMode.TwoWay);


        /// <summary>
        /// Dont use this property.
        /// </summary>

        public static readonly BindableProperty RoutesProperty = BindableProperty.Create(
            nameof(Routes),
            typeof(IEnumerable<TKRoute>),
            typeof(TKCustomMap));

        /// <summary>
        /// Bindable Property of <see cref="RouteClickedCommand"/>
        /// </summary>
        public static readonly BindableProperty RouteClickedCommandProperty = BindableProperty.Create(
            nameof(RouteClickedCommand),
            typeof(ICommand),
            typeof(TKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="RouteCalculationFinishedCommand"/>
        /// </summary>
        public static readonly BindableProperty RouteCalculationFinishedCommandProperty = BindableProperty.Create(
            nameof(RouteCalculationFinishedCommand),
            typeof(ICommand),
            typeof(TKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="RouteCalculationFailedCommand"/>
        /// </summary>
        public static readonly BindableProperty RouteCalculationFailedCommandProperty = BindableProperty.Create(
            nameof(RouteCalculationFailedCommand),
            typeof(ICommand),
            typeof(TKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="TilesUrlOptions"/>
        /// </summary>
        public static readonly BindableProperty TilesUrlOptionsProperty = BindableProperty.Create(
            nameof(TilesUrlOptions),
            typeof(TKTileUrlOptions),
            typeof(TKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="UserLocationChangedCommand"/>
        /// </summary>
        public static readonly BindableProperty UserLocationChangedCommandProperty = BindableProperty.Create(
            nameof(UserLocationChangedCommand),
            typeof(ICommand),
            typeof(TKCustomMap));
        /// <summary>
        /// Bindable Property of <see cref="GetCalloutView"/>
        /// </summary>
        public static readonly BindableProperty GetCalloutViewProperty = BindableProperty.Create(
            nameof(GetCalloutView),
            typeof(Func<TKCustomMapPin, View>),
            typeof(TKCustomMap));

        /// <summary>
        /// Binadble property of <see cref="MapType"/>
        /// </summary>
        public static readonly BindableProperty MapTypeProperty = BindableProperty.Create(
            nameof(MapType),
            typeof(MapType),
            typeof(TKCustomMap),
            default(MapType));
        /// <summary>
        /// Binadble property of <see cref="IsShowingUser"/>
        /// </summary>
        public static readonly BindableProperty IsShowingUserProperty = BindableProperty.Create(
            nameof(IsShowingUser),
            typeof(bool),
            typeof(TKCustomMap),
            default(bool));
        /// <summary>
        /// Binadble property of <see cref="HasScrollEnabled"/>
        /// </summary>
        public static readonly BindableProperty HasScrollEnabledProperty = BindableProperty.Create(
            nameof(HasScrollEnabled),
            typeof(bool),
            typeof(TKCustomMap),
            true);
        /// <summary>
        /// Binadble property of <see cref="HasZoomEnabled"/>
        /// </summary>
        public static readonly BindableProperty HasZoomEnabledProperty = BindableProperty.Create(
            nameof(HasZoomEnabled),
            typeof(bool),
            typeof(TKCustomMap),
            true);
        /// <summary>
        /// Binadble property of <see cref="MapReadyCommand"/>
        /// </summary>
        public static readonly BindableProperty MapReadyCommandProperty = BindableProperty.Create(
            nameof(MapReadyCommand),
            typeof(ICommand),
            typeof(TKCustomMap),
            default(ICommand));
        /// <summary>
        /// Binadble property of <see cref="CameraIdealCommand"/>
        /// </summary>
        public static readonly BindableProperty CameraIdealCommandProperty = BindableProperty.Create(
            nameof(CameraIdealCommand),
            typeof(ICommand),
            typeof(TKCustomMap),
            default(ICommand));

        /// <summary>
        /// Binadble property of <see cref="CameraMoveStartedCommand"/>
        /// </summary>
        public static readonly BindableProperty CameraMoveStartedCommandProperty = BindableProperty.Create(
            nameof(CameraMoveStartedCommand),
            typeof(ICommand),
            typeof(TKCustomMap),
            default(ICommand));

        /// <summary>
        /// Gets/Sets the command which is raised when the camera is ideal
        /// </summary>
        public ICommand CameraIdealCommand
        {
            get => (ICommand)GetValue(CameraIdealCommandProperty);
            set => SetValue(CameraIdealCommandProperty, value);
        }

        /// <summary>
        /// Gets/Sets the command which is raised when the camera move started
        /// </summary>
        public ICommand CameraMoveStartedCommand
        {
            get => (ICommand)GetValue(CameraMoveStartedCommandProperty);
            set => SetValue(CameraMoveStartedCommandProperty, value);
        }

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
        public IEnumerable<TKCustomMapPin> Pins
        {
            get => (IEnumerable<TKCustomMapPin>)GetValue(PinsProperty);
            set => SetValue(PinsProperty, value);
        }
        /// <summary>
        /// Gets/Sets the currently selected pin on the map
        /// </summary>
        public TKCustomMapPin SelectedPin
        {
            get => (TKCustomMapPin)GetValue(SelectedPinProperty);
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
        public IEnumerable<TKPolyline> Polylines
        {
            get => (IEnumerable<TKPolyline>)GetValue(PolylinesProperty);
            set => SetValue(PolylinesProperty, value);
        }
        /// <summary>
        /// Gets/Sets the circles to display on the map
        /// </summary>
        public IEnumerable<TKCircle> Circles
        {
            get => (IEnumerable<TKCircle>)GetValue(CirclesProperty);
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
        public IEnumerable<TKPolygon> Polygons
        {
            get => (IEnumerable<TKPolygon>)GetValue(PolygonsProperty);
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
        public IEnumerable<TKRoute> Routes
        {
            get => (IEnumerable<TKRoute>)GetValue(RoutesProperty);
            set => SetValue(RoutesProperty, value);
        }
        /// <summary>
        /// Gets/Sets the command when a route gets tapped
        /// </summary>
        public ICommand RouteClickedCommand
        {
            get => (Command<TKRoute>)GetValue(RouteClickedCommandProperty);
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
        public TKTileUrlOptions TilesUrlOptions
        {
            get => (TKTileUrlOptions)GetValue(TilesUrlOptionsProperty);
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
        public Func<TKCustomMapPin, View> GetCalloutView
        {
            get => (Func<TKCustomMapPin, View>)GetValue(GetCalloutViewProperty);
            set => SetValue(GetCalloutViewProperty, value);
        }

        /// <summary>
        /// Creates a new instance of <c>MKCustomMap</c>
        /// </summary>
        public TKCustomMap()
            : base()
        {
            MapRegion = MapSpan.FromCenterAndRadius(new Position(40.7142700, -74.0059700), Distance.FromKilometers(2));
        }
        /// <summary>
        /// Creates a new instance of <c>MKCustomMap</c>
        /// </summary>
        /// <param name="region">The initial region of the map</param>
        public TKCustomMap(MapSpan region)
        {
            MapRegion = region;
        }
        /// <summary>
        /// Creates a new instance of <see cref="TKCustomMap"/>
        /// </summary>
        /// <param name="initialLatitude">The initial latitude value</param>
        /// <param name="initialLongitude">The initial longitude value</param>
        /// <param name="distanceInKilometers">The initial zoom distance in kilometers</param>
        public TKCustomMap(double initialLatitude, double initialLongitude, double distanceInKilometers) :
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
        /// Rotate the map with the computed bearing for directions
        /// </summary>
        /// <param name="current">New location of the device</param>
        /// <param name="bearing">Bearing value computed using GetBearing function.</param>
        public void MoveToCurrentForDriving(Position current, double bearing) => MapFunctions.MoveToCurrentForDriving(current, bearing);

        /// <summary>
        /// Returns the snap position which can be use to show current location on the road.
        /// </summary>
        /// <param name="nearestPointOnRoute">Last location point on the poly line.</param>
        /// <param name="nextPointOnRoute">Next location point on the poly line.</param>
        /// <param name="currentPosition">Current device location</param>
        public Position GetSnapPosition(Position nearestPointOnRoute, Position nextPointOnRoute,
            Position currentPosition) =>
            MapFunctions.GetSnapPosition(nearestPointOnRoute, nextPointOnRoute, currentPosition);

        /// <summary>
        /// Returns current bearing.
        /// </summary>
        public float? GetCurrentBearing() => MapFunctions.GetCurrentBearing();

        /// <summary>
        /// Returns true if the marker is on screen.
        /// </summary>
        /// <param name="current">Current device location</param>
        public bool IsOnScreen(Position current) => MapFunctions.IsOnScreen(current);

        /// <summary>
        /// Animate marker position at given speed.
        /// </summary>
        /// <param name="pin">Pin which needs to be animated</param>
        /// <param name="newPosition">New position</param>
        public void AnimateMarkerPosition(TKCustomMapPin pin, Position newPosition) => MapFunctions.AnimateMarkerPosition(pin, newPosition);

        /// <summary>
        /// Animate marker position at given speed.
        /// </summary>
        /// <param name="pin">Pin which needs to be animated</param>
        /// <param name="speed">Current speed in meters / second</param>
        /// <param name="currentPosition">Current position</param>
        /// <param name="nextPositions">Next few positions</param>
        public void AnimateMarkerPosition(TKCustomMapPin pin, double speed, Position currentPosition, List<Position> nextPositions) => MapFunctions.AnimateMarkerPosition(pin, speed, currentPosition, nextPositions);

        /// <summary>
        /// Stop marker position animation.
        /// </summary>
        /// <param name="pin">Pin which animation needs to be stop</param>
        public void StopAnimateMarkerPosition(TKCustomMapPin pin) => MapFunctions.StopAnimateMarkerPosition(pin);

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
        public IEnumerable<Position> ScreenLocationsToGeocoordinates(params TKPoint[] screenLocations) => MapFunctions.ScreenLocationsToGeocoordinates(screenLocations);

        /// <summary>
        /// Manually cleanup the platform renderer.
        /// </summary>
        public void CleanUp() => MapFunctions?.CleanUp();

        /// <summary>
        /// Raises <see cref="PinSelected"/>
        /// </summary>
        /// <param name="pin">The selected pin</param>
        protected void OnPinSelected(TKCustomMapPin pin)
        {
            PinSelected?.Invoke(this, new GenericEventArgs<TKCustomMapPin>(pin));

            RaiseCommand(PinSelectedCommand, pin);
        }
        /// <summary>
        /// Raises <see cref="PinDragEnd"/>
        /// </summary>
        /// <param name="pin">The dragged pin</param>
        protected void OnPinDragEnd(TKCustomMapPin pin)
        {
            PinDragEnd?.Invoke(this, new GenericEventArgs<TKCustomMapPin>(pin));

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
        protected void OnRouteClicked(TKRoute route)
        {
            RouteClicked?.Invoke(this, new GenericEventArgs<TKRoute>(route));

            RaiseCommand(RouteClickedCommand, route);
        }

        /// <summary>
        /// Raises <see cref="RouteClicked"/>
        /// </summary>
        /// <param name="route">The tapped route</param>
        protected void OnPolylineClicked(TKPolyline poly)
        {
            PolylineClicked?.Invoke(this, new GenericEventArgs<TKPolyline>(poly));

            RaiseCommand(RouteClickedCommand, poly);
        }

        /// <summary>
        /// Raises <see cref="RouteCalculationFinished"/>
        /// </summary>
        /// <param name="route">The route</param>
        protected void OnRouteCalculationFinished(TKRoute route)
        {
            RouteCalculationFinished?.Invoke(this, new GenericEventArgs<TKRoute>(route));

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
        protected void OnCalloutClicked(TKCustomMapPin pin)
        {
            CalloutClicked?.Invoke(this, new GenericEventArgs<TKCustomMapPin>(pin));

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
        /// Raises <see cref="CameraIdeal"/>
        /// </summary>
        protected void OnCameraIdeal()
        {
            CameraIdeal?.Invoke(this, EventArgs.Empty);
            RaiseCommand(CameraIdealCommand, null);
        }

        /// <summary>
        /// Raises <see cref="CameraMoveStarted"/>
        /// </summary>
        protected void OnCameraMoveStarted()
        {
            CameraMoveStarted?.Invoke(this, EventArgs.Empty);
            RaiseCommand(CameraMoveStartedCommand, null);
        }

        /// <summary>
        /// Raises a specific command
        /// </summary>
        /// <param name="command">The command to raise</param>
        /// <param name="parameter">Addition command parameter</param>
        void RaiseCommand(ICommand command, object parameter)
        {
            if (command != null && command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        void IMapFunctions.SetRenderer(IRendererFunctions renderer) => MapFunctions = renderer;
        /// <inheritdoc/>
        void IMapFunctions.RaisePinSelected(TKCustomMapPin pin) => OnPinSelected(pin);
        /// <inheritdoc/>
        void IMapFunctions.RaisePinDragEnd(TKCustomMapPin pin) => OnPinDragEnd(pin);
        /// <inheritdoc/>
        void IMapFunctions.RaiseMapClicked(Position position) => OnMapClicked(position);
        /// <inheritdoc/>
        void IMapFunctions.RaiseMapLongPress(Position position) => OnMapLongPress(position);
        /// <inheritdoc/>
        void IMapFunctions.RaiseUserLocationChanged(Position position) => OnUserLocationChanged(position);
        /// <inheritdoc/>
        void IMapFunctions.RaiseRouteClicked(TKRoute route) => OnRouteClicked(route);
        /// <inheritdoc/>
        void IMapFunctions.RaisePolylineClicked(TKPolyline poly) => OnPolylineClicked(poly);

        /// <inheritdoc/>
        void IMapFunctions.RaiseRouteCalculationFinished(TKRoute route) => OnRouteCalculationFinished(route);
        /// <inheritdoc/>
        void IMapFunctions.RaiseRouteCalculationFailed(RouteCalculationError route) => OnRouteCalculationFailed(route);
        /// <inheritdoc/>
        void IMapFunctions.RaisePinsReady() => OnPinsReady();
        /// <inheritdoc/>
        void IMapFunctions.RaiseCalloutClicked(TKCustomMapPin pin) => OnCalloutClicked(pin);
        /// <inheritdoc/>
        void IMapFunctions.RaiseMapReady() => OnMapReady();

        /// <inheritdoc/>
        void IMapFunctions.RaiseCameraIdeal() => OnCameraIdeal();

        /// <inheritdoc/>
        void IMapFunctions.RaiseCameraMoveStarted() => OnCameraMoveStarted();
    }
}
