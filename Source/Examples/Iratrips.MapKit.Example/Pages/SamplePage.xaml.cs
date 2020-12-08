﻿using Xamarin.Forms;

namespace Iratrips.MapKit.Example.Pages
{
    public partial class SamplePage : ContentPage
    {
        public SamplePage()
        {
            InitializeComponent();

            CreateView();
            BindingContext = new SampleViewModel();
        }

         async void CreateView()
        {
            var autoComplete = new PlacesAutoComplete { ApiToUse = PlacesAutoComplete.PlacesApi.Native };
            autoComplete.SetBinding(PlacesAutoComplete.PlaceSelectedCommandProperty, "PlaceSelectedCommand");

            var newYork = new Position(40.7142700, -74.0059700);
            var mapView = new MKCustomMap(MapSpan.FromCenterAndRadius(newYork, Distance.FromKilometers(2)));
            mapView.SetBinding(MKCustomMap.IsClusteringEnabledProperty, "IsClusteringEnabled");
            mapView.SetBinding(MKCustomMap.GetClusteredPinProperty, "GetClusteredPin");
            mapView.SetBinding(MKCustomMap.GetCalloutViewProperty, "GetCalloutView");
            mapView.SetBinding(MKCustomMap.PinsProperty, "Pins");
            mapView.SetBinding(MKCustomMap.MapClickedCommandProperty, "MapClickedCommand");
            mapView.SetBinding(MKCustomMap.MapLongPressCommandProperty, "MapLongPressCommand");
            
            mapView.SetBinding(MKCustomMap.PinSelectedCommandProperty, "PinSelectedCommand");
            mapView.SetBinding(MKCustomMap.SelectedPinProperty, "SelectedPin");
            mapView.SetBinding(MKCustomMap.RoutesProperty, "Routes");
            mapView.SetBinding(MKCustomMap.PinDragEndCommandProperty, "DragEndCommand");
            mapView.SetBinding(MKCustomMap.CirclesProperty, "Circles");
            mapView.SetBinding(MKCustomMap.CalloutClickedCommandProperty, "CalloutClickedCommand");
            mapView.SetBinding(MKCustomMap.PolylinesProperty, "Lines");
            mapView.SetBinding(MKCustomMap.PolygonsProperty, "Polygons");
            mapView.SetBinding(MKCustomMap.MapRegionProperty, "MapRegion");
            mapView.SetBinding(MKCustomMap.RouteClickedCommandProperty, "RouteClickedCommand");
            mapView.SetBinding(MKCustomMap.RouteCalculationFinishedCommandProperty, "RouteCalculationFinishedCommand");
            mapView.SetBinding(MKCustomMap.TilesUrlOptionsProperty, "TilesUrlOptions");
            mapView.SetBinding(MKCustomMap.MapFunctionsProperty, "MapFunctions");
            mapView.IsRegionChangeAnimated = true;
            mapView.IsShowingUser = true;

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
    }
}