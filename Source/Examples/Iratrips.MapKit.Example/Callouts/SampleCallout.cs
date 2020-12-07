using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Iratrips.MapKit.Example.Callouts
{
    public class SampleCallout : ContentView
    {
        public SampleCallout(MKCallout callout)
        {
            Content = new StackLayout
            {
                BackgroundColor = Color.DarkKhaki,
                Children = {
                    new Label { Text = callout.Title, TextColor = Color.FromHex("#333"), FontSize = 18 },
                    new Label { Text = callout.Subtitle, TextColor = Color.FromHex("#666"), FontSize = 14 },
                    new StackLayout {
                        Orientation = StackOrientation.Horizontal,
                        Spacing = 16,
                        Children =
                        {
                            new Label { Text = "Restaurant", TextColor = Color.FromHex("#666"), FontSize = 14 },
                            new Label { Text = " | ", TextColor = Color.FromHex("#666"), FontSize = 14 },
                            new Label { Text = "Kolhapuri", TextColor = Color.FromHex("#666"), FontSize = 14 },
                        }
                    }
                },
            };
        }
    }
}