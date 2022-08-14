using System;
using Iratrips.Mapkit;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Iratrips.Mapkit.Api.Google;

namespace Sample
{
    public partial class App : Application
    {
        public App ()
        {
            InitializeComponent();

            GmsPlace.Init("AIzaSyBHrpNgIDjAgugBye4fciAcDNaj8EFxh1M");
            GmsDirection.Init("AIzaSyBHrpNgIDjAgugBye4fciAcDNaj8EFxh1M");

            // The root page of your application
            var mainPage = new NavigationPage(new SamplePage());
            if (Device.OS == TargetPlatform.iOS)
            {
                mainPage.BarBackgroundColor = Color.FromHex("#000000");
            }
            MainPage = mainPage;
        }

        protected override void OnStart ()
        {
        }

        protected override void OnSleep ()
        {
        }

        protected override void OnResume ()
        {
        }
    }
}

