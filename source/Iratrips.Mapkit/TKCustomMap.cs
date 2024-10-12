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


        public partial object GetValueFromPlatform(string propertyName)
        {
            if (propertyName == nameof(MapFunctions))
            {
                return GetValue(MapFunctionsProperty);
            }
            else if (propertyName == nameof(Pins))
            {
                return GetValue(PinsProperty);
            }
            else if (propertyName == nameof(SelectedPin))
            {
                return GetValue(SelectedPinProperty);
            }
            else if (propertyName == nameof(PinSelectedCommand))
            {
                return GetValue(PinSelectedCommandProperty);
            }
            else if (propertyName == nameof(MapClickedCommand))
            {
                return GetValue(MapClickedCommandProperty);
            }
            else if (propertyName == nameof(MapLongPressCommand))
            {
                return GetValue(MapLongPressCommandProperty);
            }
            else if (propertyName == nameof(PinDragEndCommand))
            {
                return GetValue(PinDragEndCommandProperty);
            }
            else if (propertyName == nameof(PinsReadyCommand))
            {
                return GetValue(PinsReadyCommandProperty);
            }
            else if (propertyName == nameof(IsRegionChangeAnimated))
            {
                return GetValue(IsRegionChangeAnimatedProperty);
            }
            else if (propertyName == nameof(ShowTraffic))
            {
                return GetValue(ShowTrafficProperty);
            }
            else if (propertyName == nameof(Polylines))
            {
                return GetValue(PolylinesProperty);
            }
            else if (propertyName == nameof(Circles))
            {
                return GetValue(CirclesProperty);
            }
            else if (propertyName == nameof(CalloutClickedCommand))
            {
                return GetValue(CalloutClickedCommandProperty);
            }
            else if (propertyName == nameof(Polygons))
            {
                return GetValue(PolygonsProperty);
            }
            else if (propertyName == nameof(MapRegion))
            {
                return GetValue(MapRegionProperty);
            }
            else if (propertyName == nameof(Routes))
            {
                return GetValue(RoutesProperty);
            }
            else if (propertyName == nameof(RouteClickedCommand))
            {
                return GetValue(RouteClickedCommandProperty);
            }
            else if (propertyName == nameof(RouteCalculationFinishedCommand))
            {
                return GetValue(RouteCalculationFinishedCommandProperty);
            }
            else if (propertyName == nameof(RouteCalculationFailedCommand))
            {
                return GetValue(RouteCalculationFailedCommandProperty);
            }
            else if (propertyName == nameof(TilesUrlOptions))
            {
                return GetValue(TilesUrlOptionsProperty);
            }
            else if (propertyName == nameof(UserLocationChangedCommand))
            {
                return GetValue(UserLocationChangedCommandProperty);
            }
            else if (propertyName == nameof(GetCalloutView))
            {
                return GetValue(GetCalloutViewProperty);
            }
            else if (propertyName == nameof(MapType))
            {
                return GetValue(MapTypeProperty);
            }
            else if (propertyName == nameof(IsShowingUser))
            {
                return GetValue(IsShowingUserProperty);
            }
            else if (propertyName == nameof(HasScrollEnabled))
            {
                return GetValue(HasScrollEnabledProperty);
            }
            else if (propertyName == nameof(HasZoomEnabled))
            {
                return GetValue(HasZoomEnabledProperty);
            }
            else if (propertyName == nameof(MapReadyCommand))
            {
                return GetValue(MapReadyCommandProperty);
            }
            else if (propertyName == nameof(CameraIdealCommand))
            {
                return GetValue(CameraIdealCommandProperty);
            }
            else if (propertyName == nameof(CameraMoveStartedCommand))
            {
                return GetValue(CameraMoveStartedCommandProperty);
            }
            else
                throw new ArgumentException("Not Implemented in partial class.", nameof(propertyName));
        }

        public partial void SetValueFromPlatform(string propertyName, object value)
        {
            if (propertyName == nameof(MapFunctions))
            {
                SetValue(MapTypeProperty, value);   
            }
            else if (propertyName == nameof(Pins))
            {
                SetValue(PinsProperty, value);
            }
            else if (propertyName == nameof(SelectedPin))
            {
                SetValue(SelectedPinProperty, value);
            }
            else if (propertyName == nameof(PinSelectedCommand))
            {
                SetValue(PinSelectedCommandProperty, value);
            }
            else if (propertyName == nameof(MapClickedCommand))
            {
                SetValue(MapClickedCommandProperty, value);
            }
            else if (propertyName == nameof(MapLongPressCommand))
            {
                SetValue(MapLongPressCommandProperty, value);
            }
            else if (propertyName == nameof(PinDragEndCommand))
            {
                SetValue(PinDragEndCommandProperty, value);
            }
            else if (propertyName == nameof(PinsReadyCommand))
            {
                SetValue(PinsReadyCommandProperty, value);
            }
            else if (propertyName == nameof(MapCenter))
            {
                SetValue(MapCenterProperty, value);
            }
            else if (propertyName == nameof(IsRegionChangeAnimated))
            {
                SetValue(IsRegionChangeAnimatedProperty, value);
            }
            else if (propertyName == nameof(ShowTraffic))
            {
                SetValue(ShowTrafficProperty, value);
            }
            else if (propertyName == nameof(Polylines))
            {
                SetValue(PolylinesProperty, value);
            }
            else if (propertyName == nameof(Circles))
            {
                SetValue(CirclesProperty, value);
            }
            else if (propertyName == nameof(CalloutClickedCommand))
            {
                SetValue(CalloutClickedCommandProperty, value);
            }
            else if (propertyName == nameof(Polygons))
            {
                SetValue(PolygonsProperty, value);
            }
            else if (propertyName == nameof(MapRegion))
            {
                SetValue(MapRegionProperty, value);
            }
            else if (propertyName == nameof(Routes))
            {
                SetValue(RoutesProperty, value);
            }
            else if (propertyName == nameof(RouteClickedCommand))
            {
                SetValue(RouteClickedCommandProperty, value);
            }
            else if (propertyName == nameof(RouteCalculationFinishedCommand))
            {
                SetValue(RouteCalculationFinishedCommandProperty, value);
            }
            else if (propertyName == nameof(RouteCalculationFailedCommand))
            {
                SetValue(RouteCalculationFailedCommandProperty, value);
            }
            else if (propertyName == nameof(TilesUrlOptions))
            {
                SetValue(TilesUrlOptionsProperty, value);
            }
            else if (propertyName == nameof(UserLocationChangedCommand))
            {
                SetValue(UserLocationChangedCommandProperty, value);
            }
            else if (propertyName == nameof(GetCalloutView))
            {
                SetValue(GetCalloutViewProperty, value);
            }
            else if (propertyName == nameof(MapType))
            {
                SetValue(MapTypeProperty, value);
            }
            else if (propertyName == nameof(IsShowingUser))
            {
                SetValue(IsShowingUserProperty, value);
            }
            else if (propertyName == nameof(HasScrollEnabled))
            {
                SetValue(HasScrollEnabledProperty, value);
            }
            else if (propertyName == nameof(HasZoomEnabled))
            {
                SetValue(HasZoomEnabledProperty, value);
            }
            else if (propertyName == nameof(MapReadyCommand))
            {
                SetValue(MapReadyCommandProperty, value);
            }
            else if (propertyName == nameof(CameraIdealCommand))
            {
                SetValue(CameraIdealCommandProperty, value);
            }
            else if (propertyName == nameof(CameraMoveStartedCommand))
            {
                SetValue(CameraMoveStartedCommandProperty, value);
            }
        }
    }
}
