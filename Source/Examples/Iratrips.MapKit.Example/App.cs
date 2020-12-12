using Iratrips.MapKit.Api;
using Iratrips.MapKit.Api.Google;
using Iratrips.MapKit.Example.Pages;
using Xamarin.Forms;

namespace Iratrips.MapKit.Example
{
    public class App : Application
    {
        public App()
        {
            GmsPlace.Init("AIzaSyAeQUFd_NSAa7OrrNvVqz6XDTTXe2zrQe8");
            GmsDirection.Init("AIzaSyAeQUFd_NSAa7OrrNvVqz6XDTTXe2zrQe8");

            // The root page of your application
            var mainPage = new NavigationPage(new SamplePage());
            if (Device.RuntimePlatform == Device.iOS)
            {
                mainPage.BarBackgroundColor = Color.FromHex("#f1f1f1");
            }
            MainPage = mainPage;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            NativePlacesApi.Instance.DisconnectAndRelease();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}