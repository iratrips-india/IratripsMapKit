﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Iratrips.MapKit.Api;
using Iratrips.MapKit.Api.Google;
using Iratrips.MapKit.Api.OSM;
using Iratrips.MapKit.Example.Callouts;
using Iratrips.MapKit.Example.Pages;
using Iratrips.MapKit.Interfaces;
using Iratrips.MapKit.Overlays;
using Xamarin.Forms;

namespace Iratrips.MapKit.Example
{
    public class SampleViewModel : INotifyPropertyChanged
    {
        MKTileUrlOptions _tileUrlOptions;

        MapSpan _mapRegion = MapSpan.FromCenterAndRadius(new Position(40.7142700, -74.0059700), Distance.FromKilometers(2));
        Position _mapCenter;
        MKCustomMapPin _selectedPin;
        bool _isClusteringEnabled = true;
        ObservableCollection<MKCustomMapPin> _pins;
        ObservableCollection<MKRoute> _routes;
        ObservableCollection<MKCircle> _circles;
        ObservableCollection<MKPolyline> _lines;
        ObservableCollection<MKPolygon> _polygons;
        Random _random = new Random(1984);
        Func<MKCustomMapPin, SampleCallout> _generateCalloutViewFunc;

        public MKTileUrlOptions TilesUrlOptions
        {
            get
            {
                return _tileUrlOptions;
                //return new MKTileUrlOptions(
                //    "http://a.basemaps.cartocdn.com/dark_all/{2}/{0}/{1}.png", 256, 256, 0, 18);
                //return new MKTileUrlOptions(
                //    "http://a.tile.openstreetmap.org/{2}/{0}/{1}.png", 256, 256, 0, 18);
            }
            set
            {
                if (_tileUrlOptions != value)
                {
                    _tileUrlOptions = value;
                    OnPropertyChanged("TilesUrlOptions");
                }
            }
        }

        public IRendererFunctions MapFunctions { get; set; }

        public Command RunSimulationCommand
        {
            get
            {
                return new Command(async _ =>
                {
                    if (!(await Application.Current.MainPage.DisplayAlert("Start Test?", "Start simulation test?", "Yes", "No")))
                        return;

                    Pins.Clear();

                    #region Clustering Test

                    for (int i = 0; i < 20; i++)
                    {
                        var newPin = new MKCustomMapPin
                        {
                            Position = GetDummyPosition(),
                            Callout = new MKCallout
                            {
                                Title = "Cluster Test"
                            }
                        };
                        Pins.Add(newPin);
                    }
                    MapFunctions.FitMapRegionToPositions(Pins.Select(i => i.Position), false);
                    IsClusteringEnabled = true;
                    MapRegion = MapSpan.FromCenterAndRadius(MapRegion.Center, Distance.FromKilometers(100));
                    await Task.Delay(2000);
                    IsClusteringEnabled = false;
                    await Task.Delay(2000);
                    IsClusteringEnabled = true;
                    await Task.Delay(2000);
                    IsClusteringEnabled = false;
                    await Task.Delay(2000);

                    Pins.Clear();

                    #endregion Clustering Test

                    #region PinTest

                    var pin = new MKCustomMapPin
                    {
                        Position = new Position(40.718577, -74.083754),
                        Callout = new MKCallout
                        {
                            Title = "Simulation Test"
                        },
                    };

                    _pins.Add(pin);
                    await Task.Delay(1000);

                    SelectedPin = pin;
                    await Task.Delay(1000);

                    SelectedPin = null;
                    await Task.Delay(1000);

                    SelectedPin = pin;
                    await Task.Delay(1000);

                    SelectedPin = null;
                    await Task.Delay(1000);

                    pin.DefaultPinColor = Color.Purple;
                    await Task.Delay(1000);
                    pin.DefaultPinColor = Color.Green;
                    await Task.Delay(1000);

                    pin.Image = Device.OnPlatform("Icon-Small.png", "icon.png", "icon.png");
                    await Task.Delay(1000);
                    pin.Image = null;
                    await Task.Delay(1000);

                    _pins.Remove(pin);
                    await Task.Delay(1000);
                    _pins.Add(pin);
                    await Task.Delay(1000);
                    pin.Position = new Position(40.718281, -74.085179);
                    await Task.Delay(1000);
                    pin.Position = new Position(40.717476, -74.080915);
                    await Task.Delay(1000);
                    pin.Position = new Position(40.718577, -74.083754);
                    await Task.Delay(1000);
                    _pins.Clear();

                    #endregion PinTest

                    #region Circles Test

                    var circle = new MKCircle
                    {
                        Center = new Position(40.659743, -74.049422),
                        Color = Color.Red,
                        Radius = 1000
                    };
                    _circles.Add(circle);
                    await Task.Delay(1000);

                    circle.Color = Color.Green;
                    await Task.Delay(1000);
                    circle.Color = Color.Purple;
                    await Task.Delay(1000);

                    circle.Radius = 2000;
                    await Task.Delay(1000);
                    circle.Radius = 3000;
                    await Task.Delay(1000);

                    circle.Center = new Position(40.718577, -74.083754);
                    await Task.Delay(1000);

                    _circles.Remove(circle);
                    await Task.Delay(1000);
                    _circles.Add(circle);
                    await Task.Delay(1000);
                    _circles.Clear();

                    #endregion Circles Test

                    #region Lines Test

                    Lines = new ObservableCollection<MKPolyline>();

                    var line = new MKPolyline
                    {
                        Color = Color.Pink,
                        LineWidth = 2f,
                        LineCoordinates = new List<Position>(new Position[]
                        {
                            new Position(40.647241, -74.081007),
                            new Position(40.702873, -74.016162)
                        })
                    };

                    _lines.Add(line);
                    await Task.Delay(1000);

                    line.Color = Color.Red;
                    await Task.Delay(1000);
                    line.Color = Color.Green;
                    await Task.Delay(1000);

                    line.LineCoordinates = new List<Position>(new Position[]
                    {
                        new Position(40.647241, -74.081007),
                        new Position(40.702873, -74.016162),
                        new Position(40.690602, -74.017309)
                    });
                    await Task.Delay(1000);
                    _lines.Remove(line);
                    await Task.Delay(1000);
                    _lines.Add(line);
                    await Task.Delay(1000);
                    _lines.Clear();

                    #endregion Lines Test

                    #region Polygon Test

                    Polygons = new ObservableCollection<MKPolygon>();

                    var poly = new MKPolygon
                    {
                        StrokeColor = Color.Green,
                        StrokeWidth = 2f,
                        Color = Color.Red,
                        Coordinates = new List<Position>(new Position[]
                        {
                            new Position(40.716901, -74.055969),
                            new Position(40.699878, -73.986296),
                            new Position(40.636811, -74.076240)
                        })
                    };

                    _polygons.Add(poly);
                    await Task.Delay(1000);

                    poly.StrokeColor = Color.Purple;
                    await Task.Delay(1000);
                    poly.StrokeWidth = 5f;
                    await Task.Delay(1000);
                    poly.StrokeWidth = 0;
                    await Task.Delay(1000);
                    poly.StrokeWidth = 2f;
                    await Task.Delay(1000);

                    poly.Color = Color.Yellow;
                    await Task.Delay(1000);

                    _polygons.Remove(poly);
                    await Task.Delay(1000);
                    _polygons.Add(poly);
                    await Task.Delay(1000);
                    _polygons.Clear();

                    #endregion Polygon Test

                    #region Tiles Test

                    TilesUrlOptions = new MKTileUrlOptions(
                        "http://a.basemaps.cartocdn.com/dark_all/{2}/{0}/{1}.png", 256, 256, 0, 18);
                    await Task.Delay(5000);
                    TilesUrlOptions = null;
                    await Task.Delay(5000);
                    TilesUrlOptions = new MKTileUrlOptions(
                        "http://a.tile.openstreetmap.org/{2}/{0}/{1}.png", 256, 256, 0, 18);

                    #endregion Tiles Test
                });
            }
        }

        public bool IsClusteringEnabled
        {
            get => _isClusteringEnabled;
            set
            {
                _isClusteringEnabled = value;
                OnPropertyChanged(nameof(IsClusteringEnabled));
            }
        }

        public Command ShowListCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (_pins == null || !_pins.Any())
                    {
                        Application.Current.MainPage.DisplayAlert("Nothing there!", "No pins to show!", "OK");
                        return;
                    }
                    var listPage = new PinListPage(Pins);
                    listPage.PinSelected += async (o, e) =>
                    {
                        SelectedPin = e.Pin;
                        await Application.Current.MainPage.Navigation.PopAsync();
                    };
                    await Application.Current.MainPage.Navigation.PushAsync(listPage);
                });
            }
        }

        /// <summary>
        /// Map region bound to <see cref="MKCustomMap"/>
        /// </summary>
        public MapSpan MapRegion
        {
            get { return _mapRegion; }
            set
            {
                if (_mapRegion != value)
                {
                    _mapRegion = value;
                    OnPropertyChanged("MapRegion");
                }
            }
        }

        /// <summary>
        /// Pins bound to the <see cref="MKCustomMap"/>
        /// </summary>
        public ObservableCollection<MKCustomMapPin> Pins
        {
            get { return _pins; }
            set
            {
                if (_pins != value)
                {
                    _pins = value;
                    OnPropertyChanged("Pins");
                }
            }
        }

        /// <summary>
        /// Routes bound to the <see cref="MKCustomMap"/>
        /// </summary>
        public ObservableCollection<MKRoute> Routes
        {
            get { return _routes; }
            set
            {
                if (_routes != value)
                {
                    _routes = value;
                    OnPropertyChanged("Routes");
                }
            }
        }

        /// <summary>
        /// Circles bound to the <see cref="MKCustomMap"/>
        /// </summary>
        public ObservableCollection<MKCircle> Circles
        {
            get { return _circles; }
            set
            {
                if (_circles != value)
                {
                    _circles = value;
                    OnPropertyChanged("Circles");
                }
            }
        }

        /// <summary>
        /// Lines bound to the <see cref="MKCustomMap"/>
        /// </summary>
        public ObservableCollection<MKPolyline> Lines
        {
            get { return _lines; }
            set
            {
                if (_lines != value)
                {
                    _lines = value;
                    OnPropertyChanged("Lines");
                }
            }
        }

        /// <summary>
        /// Polygons bound to the <see cref="MKCustomMap"/>
        /// </summary>
        public ObservableCollection<MKPolygon> Polygons
        {
            get { return _polygons; }
            set
            {
                if (_polygons != value)
                {
                    _polygons = value;
                    OnPropertyChanged("Polygons");
                }
            }
        }

        /// <summary>
        /// Map center bound to the <see cref="MKCustomMap"/>
        /// </summary>
        public Position MapCenter
        {
            get { return _mapCenter; }
            set
            {
                if (_mapCenter != value)
                {
                    _mapCenter = value;
                    OnPropertyChanged("MapCenter");
                }
            }
        }

        /// <summary>
        /// Selected pin bound to the <see cref="MKCustomMap"/>
        /// </summary>
        public MKCustomMapPin SelectedPin
        {
            get { return _selectedPin; }
            set
            {
                if (_selectedPin != value)
                {
                    _selectedPin = value;
                    OnPropertyChanged("SelectedPin");
                }
            }
        }

        /// <summary>
        /// Map Long Press bound to the <see cref="MKCustomMap"/>
        /// </summary>
        public Command<Position> MapLongPressCommand
        {
            get
            {
                return new Command<Position>(async position =>
                {
                    var action = await Application.Current.MainPage.DisplayActionSheet(
                        "Long Press",
                        "Cancel",
                        null,
                        "Add Pin",
                        "Add Circle");

                    if (action == "Add Pin")
                    {
                        var pin = new MKCustomMapPin
                        {
                            Position = position,
                            Callout = new MKCallout
                            {
                                Title = string.Format("Pin {0}, {1}", position.Latitude, position.Longitude),
                                Subtitle = "Click for more info...",
                                HasCustomView = true,
                                IsClickable = false
                            },
                            Image = "ic_place_black_24dp",
                            IsDraggable = true
                        };
                        _pins.Add(pin);
                    }
                    else if (action == "Add Circle")
                    {
                        var circle = new MKCircle
                        {
                            Center = position,
                            Radius = 10000,
                            Color = Color.FromRgba(100, 0, 0, 80)
                        };
                        _circles.Add(circle);
                    }
                });
            }
        }

        /// <summary>
        /// Map Clicked bound to the <see cref="MKCustomMap"/>
        /// </summary>
        public Command<Position> MapClickedCommand
        {
            get
            {
                return new Command<Position>((positon) =>
                {
                    SelectedPin = null;

                    // Determine if a point was inside a circle
                    if ((from c in _circles let distanceInMeters = c.Center.DistanceTo(positon) * 1000 where distanceInMeters <= c.Radius select c).Any())
                    {
                        Application.Current.MainPage.DisplayAlert("Circle tap", "Circle was tapped", "OK");
                    }
                });
            }
        }

        /// <summary>
        /// Command when a place got selected
        /// </summary>
        public Command<IPlaceResult> PlaceSelectedCommand
        {
            get
            {
                return new Command<IPlaceResult>(async p =>
                {
                    var gmsResult = p as GmsPlacePrediction;
                    if (gmsResult != null)
                    {
                        var details = await GmsPlace.Instance.GetDetails(gmsResult.PlaceId);
                        MapCenter = new Position(details.Item.Geometry.Location.Latitude, details.Item.Geometry.Location.Longitude);
                        return;
                    }
                    var osmResult = p as OsmNominatimResult;
                    if (osmResult != null)
                    {
                        MapCenter = new Position(osmResult.Latitude, osmResult.Longitude);
                        return;
                    }

                    if (Device.RuntimePlatform == Device.Android)
                    {
                        var prediction = (NativeAndroidPlaceResult)p;

                        var details = await NativePlacesApi.Instance.GetDetails(prediction.PlaceId);

                        MapCenter = details.Coordinate;
                    }
                    else if (Device.RuntimePlatform == Device.iOS)
                    {
                        var prediction = (NativeiOSPlaceResult)p;

                        MapCenter = prediction.Details.Coordinate;
                    }
                });
            }
        }

        /// <summary>
        /// Pin Selected bound to the <see cref="MKCustomMap"/>
        /// </summary>
        public Command PinSelectedCommand
        {
            get
            {
                return new Command<MKCustomMapPin>((MKCustomMapPin pin) =>
                {
                    MapRegion = MapSpan.FromCenterAndRadius(SelectedPin.Position, Distance.FromKilometers(1));
                });
            }
        }

        /// <summary>
        /// Drag End bound to the <see cref="MKCustomMap"/>
        /// </summary>
        public Command<MKCustomMapPin> DragEndCommand
        {
            get
            {
                return new Command<MKCustomMapPin>(pin =>
                {
                    var routePin = pin as RoutePin;

                    if (routePin != null)
                    {
                        if (routePin.IsSource)
                        {
                            routePin.Route.Source = pin.Position;
                        }
                        else
                        {
                            routePin.Route.Destination = pin.Position;
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Route clicked bound to the <see cref="MKCustomMap"/>
        /// </summary>
        public Command<MKRoute> RouteClickedCommand
        {
            get
            {
                return new Command<MKRoute>(async r =>
                {
                    var action = await Application.Current.MainPage.DisplayActionSheet(
                        "Route tapped",
                        "Cancel",
                        null,
                        "Show Instructions");

                    if (action == "Show Instructions")
                    {
                        await Application.Current.MainPage.Navigation.PushAsync(new HtmlInstructionsPage(r));
                    }
                });
            }
        }

        /// <summary>
        /// Callout clicked bound to the <see cref="MKCustomMap"/>
        /// </summary>
        public Command CalloutClickedCommand
        {
            get
            {
                return new Command<MKCustomMapPin>(async (MKCustomMapPin pin) =>
                {
                    var action = await Application.Current.MainPage.DisplayActionSheet(
                        "Callout clicked",
                        "Cancel",
                        "Remove Pin");

                    if (action == "Remove Pin")
                    {
                        _pins.Remove(pin);
                    }
                });
            }
        }

        public Command ClearMapCommand
        {
            get
            {
                return new Command(() =>
                {
                    _pins.Clear();
                    _circles.Clear();
                    if (_routes != null)
                        _routes.Clear();
                });
            }
        }

        /// <summary>
        /// Navigate to a new page to get route source/destination
        /// </summary>
        public Command AddRouteCommand
        {
            get
            {
                return new Command(() =>
                {
                    if (Routes == null) Routes = new ObservableCollection<MKRoute>();

                    var addRoutePage = new AddRoutePage(Routes, Pins, MapRegion);
                    Application.Current.MainPage.Navigation.PushAsync(addRoutePage);
                });
            }
        }

        /// <summary>
        /// Command when a route calculation finished
        /// </summary>
        public Command<MKRoute> RouteCalculationFinishedCommand
        {
            get
            {
                return new Command<MKRoute>(r =>
                {
                    // move to the bounds of the route
                    MapRegion = r.Bounds;
                });
            }
        }

        public Func<MKCustomMapPin, Xamarin.Forms.View> GetCalloutView => (pin) =>
        {
            return new SampleCallout(pin);
        };

        public Func<string, IEnumerable<MKCustomMapPin>, MKCustomMapPin> GetClusteredPin => (group, clusteredPins) =>
        {
            return null;
            //return new MKCustomMapPin
            //{
            //    DefaultPinColor = Color.Blue,
            //    Title = clusteredPins.Count().ToString(),
            //    ShowCallout = true
            //};
        };

        public SampleViewModel()
        {
            _mapCenter = new Position(40.7142700, -74.0059700);

            _pins = new ObservableCollection<MKCustomMapPin>();
            _circles = new ObservableCollection<MKCircle>();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        Position GetDummyPosition()
        {
            return new Position(Random(51.6723432, 51.38494009999999), Random(0.148271, -0.3514683));
        }
        double Random(double min, double max)
        {
            return _random.NextDouble() * (max - min) + min;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}