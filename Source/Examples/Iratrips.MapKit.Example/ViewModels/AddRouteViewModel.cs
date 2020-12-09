using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iratrips.MapKit;
using Iratrips.MapKit.Api;
using Iratrips.MapKit.Api.Google;
using Iratrips.MapKit.Overlays;
using Xamarin.Forms;

namespace Iratrips.MapKit.Example
{
    public class AddRouteViewModel
    {
        IPlaceResult _fromPlace, _toPlace;
        Position _from, _to;

        public ObservableCollection<MKCustomMapPin> Pins { get; set; }
        public ObservableCollection<MKRoute> Routes { get; set; }
        public MapSpan Bounds { get; set; }

        public Command<IPlaceResult> FromSelectedCommand
        {
            get
            {
                return new Command<IPlaceResult>(async (p) =>
                {
                    GmsPlacePrediction placeResult = (GmsPlacePrediction)p;
                    _fromPlace = placeResult;
                    var details = await GmsPlace.Instance.GetDetails(placeResult.PlaceId);
                    _from = details.Item.Geometry.Location.ToPosition();
                });
            }
        }
        public Command<IPlaceResult> ToSelectedCommand
        {
            get
            {
                return new Command<IPlaceResult>(async (p) =>
                {
                    GmsPlacePrediction placeResult = (GmsPlacePrediction)p;
                    _toPlace = placeResult;
                    var details = await GmsPlace.Instance.GetDetails(placeResult.PlaceId);
                    _to = details.Item.Geometry.Location.ToPosition(); 
                });
            }
        }

        public Command AddRouteCommand
        {
            get
            {
                return new Command(() =>
                {
                    if (_toPlace == null || _fromPlace == null) return;

                    var route = new MKRoute
                    {
                        TravelMode = MKRouteTravelMode.Driving,
                        Source = _from,
                        Destination = _to,
                        Color = Color.Blue
                    };

                    Pins.Add(new RoutePin
                    {
                        Route = route,
                        IsSource = true,
                        IsDraggable = true,
                        Position = _from,
                        Callout = new MKCallout
                        {
                            Title = _fromPlace.Description,
                        },
                        DefaultPinColor = Color.Green
                    });
                    Pins.Add(new RoutePin
                    {
                        Route = route,
                        IsSource = false,
                        IsDraggable = true,
                        Position = _to,
                        Callout = new MKCallout
                        {
                            Title = _toPlace.Description,
                        },
                        DefaultPinColor = Color.Red
                    });

                    Routes.Add(route);

                    Application.Current.MainPage.Navigation.PopAsync();
                });
            }
        }

        public AddRouteViewModel(ObservableCollection<MKRoute> routes, ObservableCollection<MKCustomMapPin> pins, MapSpan bounds)
        {
            Routes = routes;
            Pins = pins;
            Bounds = bounds;
        }
    }
}
