using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Iratrips.MapKit.Example.Callouts
{
    public class SampleCallout : ContentView
    {
        public SampleCallout(MKCustomMapPin pin)
        {
            Content = new StackLayout
            {
                BackgroundColor = Color.DarkKhaki,
                Children = {
                    new Label { Text = pin.Callout.Title, TextColor = Color.FromHex("#333"), FontSize = 18 },
                    new Label { Text = pin.Callout.Subtitle, TextColor = Color.FromHex("#666"), FontSize = 14 },
                    new StackLayout {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            new Label { Text = "Restaurant", TextColor = Color.FromHex("#666"), FontSize = 14 },
                            new Label { Text = " | ", TextColor = Color.FromHex("#666"), FontSize = 14 },
                            new Label { Text = "Indian", TextColor = Color.FromHex("#666"), FontSize = 14 },
                        }
                    }
                },
            };
        }
    }
}