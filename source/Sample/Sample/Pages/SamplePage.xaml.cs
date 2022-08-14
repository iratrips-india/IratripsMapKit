using System;
using Iratrips.Mapkit;
using Xamarin.Forms;

namespace Sample
{
    public partial class SamplePage : ContentPage
    {
        public SamplePage()
        {
            InitializeComponent();
            CreateView();
            BindingContext = new SampleViewModel();
        }

        void CreateView()
        {
            var autoComplete = new PlacesAutoComplete { ApiToUse = PlacesAutoComplete.PlacesApi.Native };
            autoComplete.SetBinding(PlacesAutoComplete.PlaceSelectedCommandProperty, "PlaceSelectedCommand");

            var newYork = new Position(40.7142700, -74.0059700);
            var mapView = new TKCustomMap(MapSpan.FromCenterAndRadius(newYork, Distance.FromKilometers(2)));
            mapView.SetBinding(TKCustomMap.IsClusteringEnabledProperty, "IsClusteringEnabled");
            mapView.SetBinding(TKCustomMap.GetClusteredPinProperty, "GetClusteredPin");
            mapView.SetBinding(TKCustomMap.PinsProperty, "Pins");
            mapView.SetBinding(TKCustomMap.MapClickedCommandProperty, "MapClickedCommand");
            mapView.SetBinding(TKCustomMap.MapLongPressCommandProperty, "MapLongPressCommand");
            mapView.SetBinding(TKCustomMap.CameraIdealCommandProperty, "CameraIdealCommand");
            mapView.SetBinding(TKCustomMap.CameraMoveStartedCommandProperty, "CameraMoveStartedCommand");

            mapView.SetBinding(TKCustomMap.PinSelectedCommandProperty, "PinSelectedCommand");
            mapView.SetBinding(TKCustomMap.SelectedPinProperty, "SelectedPin");
            mapView.SetBinding(TKCustomMap.RoutesProperty, "Routes");
            mapView.SetBinding(TKCustomMap.PinDragEndCommandProperty, "DragEndCommand");
            mapView.SetBinding(TKCustomMap.CirclesProperty, "Circles");
            mapView.SetBinding(TKCustomMap.CalloutClickedCommandProperty, "CalloutClickedCommand");
            mapView.SetBinding(TKCustomMap.PolylinesProperty, "Lines");
            mapView.SetBinding(TKCustomMap.PolygonsProperty, "Polygons");
            mapView.SetBinding(TKCustomMap.MapRegionProperty, "MapRegion");
            mapView.SetBinding(TKCustomMap.RouteClickedCommandProperty, "RouteClickedCommand");
            mapView.SetBinding(TKCustomMap.RouteCalculationFinishedCommandProperty, "RouteCalculationFinishedCommand");
            mapView.SetBinding(TKCustomMap.TilesUrlOptionsProperty, "TilesUrlOptions");
            mapView.SetBinding(TKCustomMap.MapFunctionsProperty, "MapFunctions");
            mapView.IsRegionChangeAnimated = true;
            mapView.IsShowingUser = true;

            mapView.GetCalloutView = this.OnGetCalloutView;

            autoComplete.SetBinding(PlacesAutoComplete.BoundsProperty, "MapRegion");

            Content = mapView;
            //_baseLayout.Children.Add(
            //    mapView,
            //    Constraint.RelativeToView(autoComplete, (r, v) => v.X),
            //    Constraint.RelativeToView(autoComplete, (r, v) => autoComplete.HeightOfSearchBar),
            //    heightConstraint: Constraint.RelativeToParent((r) => r.Height - autoComplete.HeightOfSearchBar),
            //    widthConstraint: Constraint.RelativeToView(autoComplete, (r, v) => v.Width));

            //_baseLayout.Children.Add(
            //    autoComplete,
            //    Constraint.Constant(0),
            //    Constraint.Constant(0));
        }

        private View OnGetCalloutView(TKCustomMapPin pin)
        {
            var stackLayout = new StackLayout
            {
                WidthRequest = 200,
                HeightRequest = 100,
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Color.Red
            };

            var titleLabel = new Label
            {
                Text = pin.Callout.Title,
                FontSize = 16,
                TextColor = Color.Brown
            };

            var subtitleLabel = new Label
            {
                Text = pin.Callout.Subtitle,
                FontSize = 12,
                TextColor = Color.AliceBlue
            };

            stackLayout.Children.Add(titleLabel);
            stackLayout.Children.Add(subtitleLabel);

            return stackLayout;
        }
    }
}