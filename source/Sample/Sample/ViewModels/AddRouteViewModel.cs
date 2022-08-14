using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iratrips.Mapkit;
using Iratrips.Mapkit.Api;
using Iratrips.Mapkit.Overlays;
using Xamarin.Forms;

namespace Sample
{
    public class AddRouteViewModel
    {
         IPlaceResult _fromPlace, _toPlace;
         Position _from, _to;

        public ObservableCollection<TKCustomMapPin> Pins { get;  set; }
        public ObservableCollection<TKRoute> Routes { get;  set; }
        public MapSpan Bounds { get;  set; }

        public Command<IPlaceResult> FromSelectedCommand
        {
            get
            {
                return new Command<IPlaceResult>(async (p) => 
                {
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        NativeiOSPlaceResult placeResult = (NativeiOSPlaceResult)p;
                        _fromPlace = placeResult;
                        _from = placeResult.Details.Coordinate;
                    }
                    else
                    {
                        NativeAndroidPlaceResult placeResult = (NativeAndroidPlaceResult)p;
                        _fromPlace = placeResult;
                        var details = await NativePlacesApi.Instance.GetDetails(placeResult.PlaceId);

                        _from = details.Coordinate;
                    }
                });
            }
        }
        public Command<IPlaceResult> ToSelectedCommand
        {
            get
            {
                return new Command<IPlaceResult>(async (p) => 
                {
                    if(Device.RuntimePlatform == Device.iOS)
                    {
                        NativeiOSPlaceResult placeResult = (NativeiOSPlaceResult)p;
                        _toPlace = placeResult;
                        _to = placeResult.Details.Coordinate;
                    }
                    else
                    {
                        NativeAndroidPlaceResult placeResult = (NativeAndroidPlaceResult)p;
                        _toPlace = placeResult;
                        var details = await NativePlacesApi.Instance.GetDetails(placeResult.PlaceId);

                        _to = details.Coordinate;
                    }
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

                    var route = new TKRoute
                    {
                        TravelMode = TKRouteTravelMode.Driving,
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
                        DefaultPinColor = Color.Green,
                        Callout = new TKCallout
                        {
                            Title = _fromPlace.Description,
                            IsClickable = false
                        }
                    });

                    Pins.Add(new RoutePin
                    {
                        Route = route,
                        IsSource = false,
                        IsDraggable = true,
                        Position = _to,
                        DefaultPinColor = Color.Red,
                        Callout = new TKCallout
                        {
                            Title = _toPlace.Description,
                            IsClickable = false
                        }
                    });

                    Routes.Add(route);

                    Application.Current.MainPage.Navigation.PopAsync();
                });
            }
        }

        public AddRouteViewModel(ObservableCollection<TKRoute> routes, ObservableCollection<TKCustomMapPin> pins, MapSpan bounds)
        {
            Routes = routes;
            Pins = pins;
            Bounds = bounds;
        }
    }
}
