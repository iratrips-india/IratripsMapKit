using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Maps;
using Android.OS;
using System;

namespace Iratrips.Mapkit.Droid
{
    public static class TKGoogleMaps
    {
        public static bool IsInitialized { get;  set; }

        public static Context Context { get;  set; }

        public static void Init(Activity activity, Bundle bundle)
        {
            if (IsInitialized)
                return;

            Context = activity;

            TKCustomMapHandler.Bundle = bundle;

#pragma warning disable 618
            if (GooglePlayServicesUtil.IsGooglePlayServicesAvailable(Context) == ConnectionResult.Success)
#pragma warning restore 618
            {
                try
                {
                    MapsInitializer.Initialize(Context);
                    IsInitialized = true;
                }
                catch (Exception e)
                {
                    Android.Util.Log.Error("Iratrips", "Google Play Services Not Found");
                    Android.Util.Log.Error("Iratrips", $"Exception: {e}");
                }
            }
            else
                Android.Util.Log.Info("Iratrips", "Google Play Services Not available");
        }
    }
}
