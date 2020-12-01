using Xamarin.Forms;

namespace Iratrips.MapKit.Api
{
    /// <summary>
    /// Manages instance of <see cref="INativePlacesApi"/>
    /// </summary>
    public static class NativePlacesApi
    {
         static INativePlacesApi _instance;

        /// <summary>
        /// Gets an instance of <see cref="INativePlacesApi"/>
        /// </summary>
        public static INativePlacesApi Instance
        {
            get
            {
                return _instance ?? (_instance = DependencyService.Get<INativePlacesApi>());
            }
        }
    }
}
