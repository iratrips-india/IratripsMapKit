using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Iratrips.Mapkit.Interfaces;
using Iratrips.Mapkit.Models;
using Iratrips.Mapkit.Overlays;

namespace Iratrips.Mapkit
{
    public partial class TKCustomMap
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
        /// Gets/Sets the command which is raised when the camera is ideal
        /// </summary>
        public ICommand CameraIdealCommand
        {
            get => (ICommand)GetValueFromPlatform(nameof(CameraIdealCommand));
            set => SetValueFromPlatform(nameof(CameraIdealCommand), value);
        }

        /// <summary>
        /// Gets/Sets the command which is raised when the camera move started
        /// </summary>
        public ICommand CameraMoveStartedCommand
        {
            get => (ICommand)GetValueFromPlatform(nameof(CameraMoveStartedCommand));
            set => SetValueFromPlatform(nameof(CameraMoveStartedCommand), value);
        }

        /// <summary>
        /// Gets/Sets the command which is raised when the map is ready
        /// </summary>
        public ICommand MapReadyCommand
        {
            get => (ICommand)GetValueFromPlatform(nameof(MapReadyCommand));
            set => SetValueFromPlatform(nameof(MapReadyCommand), value);
        }
        /// <summary>
        /// Gets/Sets the current <see cref="MapType"/>
        /// </summary>
        public MapType MapType
        {
            get => (MapType)GetValueFromPlatform(nameof(MapType));
            set => SetValueFromPlatform(nameof(MapType), value);
        }
        /// <summary>
        /// Gets/Sets if the user should be displayed on the map
        /// </summary>
        public bool IsShowingUser
        {
            get => (bool)GetValueFromPlatform(nameof(IsShowingUser));
            set => SetValueFromPlatform(nameof(IsShowingUser), value);
        }
        /// <summary>
        /// Gets/Sets whether scrolling is enabled or not
        /// </summary>
        public bool HasScrollEnabled
        {
            get => (bool)GetValueFromPlatform(nameof(HasScrollEnabled));
            set => SetValueFromPlatform(nameof(HasScrollEnabled), value);
        }
        /// <summary>
        /// Gets/Sets whether zooming is enabled or not
        /// </summary>
        public bool HasZoomEnabled
        {
            get => (bool)GetValueFromPlatform(nameof(HasZoomEnabled));
            set => SetValueFromPlatform(nameof(HasZoomEnabled), value);
        }
        /// <summary>
        /// Gets/Sets the custom pins of the Map
        /// </summary>
        public IEnumerable<TKCustomMapPin> Pins
        {
            get => (IEnumerable<TKCustomMapPin>)GetValueFromPlatform(nameof(Pins));
            set => SetValueFromPlatform(nameof(Pins), value);
        }
        /// <summary>
        /// Gets/Sets the currently selected pin on the map
        /// </summary>
        public TKCustomMapPin SelectedPin
        {
            get => (TKCustomMapPin)GetValueFromPlatform(nameof(SelectedPin));
            set => SetValueFromPlatform(nameof(SelectedPin), value);
        }
        /// <summary>
        /// Gets/Sets the command when the map was clicked/tapped
        /// </summary>
        public ICommand MapClickedCommand
        {
            get => (ICommand)GetValueFromPlatform(nameof(MapClickedCommand));
            set => SetValueFromPlatform(nameof(MapClickedCommand), value);
        }
        /// <summary>
        /// Gets/Sets the command when a long press was performed on the map
        /// </summary>
        public ICommand MapLongPressCommand
        {
            get => (ICommand)GetValueFromPlatform(nameof(MapLongPressCommand));
            set => SetValueFromPlatform(nameof(MapLongPressCommand), value);
        }
        /// <summary>
        /// Gets/Sets the command when a pin drag ended. The pin already has the updated position set
        /// </summary>
        public ICommand PinDragEndCommand
        {
            get => (ICommand)GetValueFromPlatform(nameof(PinDragEndCommand));
            set => SetValueFromPlatform(nameof(PinDragEndCommand), value);
        }
        /// <summary>
        /// Gets/Sets the command when a pin got selected
        /// </summary>
        public ICommand PinSelectedCommand
        {
            get => (ICommand)GetValueFromPlatform(nameof(PinSelectedCommand));
            set => SetValueFromPlatform(nameof(PinSelectedCommand), value);
        }
        /// <summary>
        /// Gets/Sets the command when the pins are ready
        /// </summary>
        public ICommand PinsReadyCommand
        {
            get => (ICommand)GetValueFromPlatform(nameof(PinsReadyCommand));
            set => SetValueFromPlatform(nameof(PinsReadyCommand), value);
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
            get => (bool)GetValueFromPlatform(nameof(IsRegionChangeAnimated));
            set => SetValueFromPlatform(nameof(IsRegionChangeAnimated), value);
        }
        /// <summary>
        /// Gets/Sets the lines to display on the map
        /// </summary>
        public IEnumerable<TKPolyline> Polylines
        {
            get => (IEnumerable<TKPolyline>)GetValueFromPlatform(nameof(Polylines));
            set => SetValueFromPlatform(nameof(Polylines), value);
        }
        /// <summary>
        /// Gets/Sets the circles to display on the map
        /// </summary>
        public IEnumerable<TKCircle> Circles
        {
            get => (IEnumerable<TKCircle>)GetValueFromPlatform(nameof(Circles));
            set => SetValueFromPlatform(nameof(Circles), value);
        }
        /// <summary>
        /// Gets/Sets the command when a callout gets clicked. When this is set, there will be an accessory button visible inside the callout on iOS.
        /// Android will simply raise the command by clicking anywhere inside the callout, since Android simply renders a bitmap
        /// </summary>
        public ICommand CalloutClickedCommand
        {
            get => (ICommand)GetValueFromPlatform(nameof(CalloutClickedCommand));
            set => SetValueFromPlatform(nameof(CalloutClickedCommand), value);
        }
        /// <summary>
        /// Gets/Sets the rectangles to display on the map
        /// </summary>
        public IEnumerable<TKPolygon> Polygons
        {
            get => (IEnumerable<TKPolygon>)GetValueFromPlatform(nameof(Polygons));
            set => SetValueFromPlatform(nameof(Polygons), value);
        }
        /// <summary>
        /// Gets/Sets the visible map region
        /// </summary>
        public MapSpan MapRegion
        {
            get => (MapSpan)GetValueFromPlatform(nameof(MapRegion));
            set => SetValueFromPlatform(nameof(MapRegion), value);
        }
        /// <summary>
        /// Gets/Sets the routes to calculate and display on the map
        /// </summary>
        public IEnumerable<TKRoute> Routes
        {
            get => (IEnumerable<TKRoute>)GetValueFromPlatform(nameof(Routes));
            set => SetValueFromPlatform(nameof(Routes), value);
        }
        /// <summary>
        /// Gets/Sets the command when a route gets tapped
        /// </summary>
        public ICommand RouteClickedCommand
        {
            get => (ICommand)GetValueFromPlatform(nameof(RouteClickedCommand));
            set => SetValueFromPlatform(nameof(RouteClickedCommand), value);
        }
        /// <summary>
        /// Gets/Sets the command when a route calculation finished successfully
        /// </summary>
        public ICommand RouteCalculationFinishedCommand
        {
            get => (ICommand)GetValueFromPlatform(nameof(RouteCalculationFinishedCommand));
            set => SetValueFromPlatform(nameof(RouteCalculationFinishedCommand), value);
        }
        /// <summary>
        /// Gets/Sets the command when a route calculation failed
        /// </summary>
        public ICommand RouteCalculationFailedCommand
        {
            get => (ICommand)GetValueFromPlatform(nameof(RouteCalculationFailedCommand));
            set => SetValueFromPlatform(nameof(RouteCalculationFailedCommand), value);
        }
        /// <summary>
        /// Gets/Sets the options for displaying custom tiles via an url
        /// </summary>
        public TKTileUrlOptions TilesUrlOptions
        {
            get => (TKTileUrlOptions)GetValueFromPlatform(nameof(TilesUrlOptions));
            set => SetValueFromPlatform(nameof(TilesUrlOptions), value);
        }
        /// <summary>
        /// Gets/Sets the command when the user location changed
        /// </summary>
        public ICommand UserLocationChangedCommand
        {
            get => (ICommand)GetValueFromPlatform(nameof(UserLocationChangedCommand));
            set => SetValueFromPlatform(nameof(UserLocationChangedCommand), value);
        }
        /// <summary>
        /// Gets/Sets the avaiable functions on the map/renderer
        /// </summary>
        public IRendererFunctions MapFunctions
        {
            get => (IRendererFunctions)GetValueFromPlatform(nameof(MapFunctions));
            set => SetValueFromPlatform(nameof(MapFunctions), value);
        }
        /// <summary>
        /// Gets/Sets if traffic information should be displayed
        /// </summary>
        public bool ShowTraffic
        {
            get => (bool)GetValueFromPlatform(nameof(ShowTraffic));
            set => SetValueFromPlatform(nameof(ShowTraffic), value);
        }
        /// <summary>
        /// Gets/Sets function to retrieve a callout view. 
        /// </summary>
        public Func<TKCustomMapPin, ICalloutView> GetCalloutView
        {
            get => (Func<TKCustomMapPin, ICalloutView>)GetValueFromPlatform(nameof(GetCalloutView));
            set => SetValueFromPlatform(nameof(GetCalloutView), value);
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


        #region Platform Specific
        public partial object GetValueFromPlatform(string propertyName);
        public partial void SetValueFromPlatform(string propertyName, object value);
        #endregion
    }
}
