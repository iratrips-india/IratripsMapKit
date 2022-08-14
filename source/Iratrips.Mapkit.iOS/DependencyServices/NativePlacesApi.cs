using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using MapKit;
using Iratrips.Mapkit.Api;
using Iratrips.Mapkit.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(TKNativePlacesApi))]

namespace Iratrips.Mapkit.iOS
{
    /// <summary>
    /// iOS implementation of the <see cref="INativePlacesApi"/>
    /// </summary>
    public class TKNativePlacesApi : INativePlacesApi
    {
        /// <summary>
        /// Just to avoid linking
        /// </summary>
        [Preserve]
        public static void Init() { }
        ///<inheritdoc/>
        public void Connect()
        {
            // Nothing to do on iOS
        }
        ///<inheritdoc/>
        public void DisconnectAndRelease()
        {
            // Nothing to do on iOS
        }
        ///<inheritdoc/>
        public async Task<IEnumerable<IPlaceResult>> GetPredictions(string query, MapSpan bounds)
        {
            List<IPlaceResult> result = new List<IPlaceResult>();

            var region = new MKCoordinateRegion(bounds.Center.ToLocationCoordinate(), new MKCoordinateSpan(0.25, 0.25));

            var request = new MKLocalSearchRequest 
            {
                NaturalLanguageQuery = query,
                Region = region
            };

            MKLocalSearch search = new MKLocalSearch(request);
            var nativeResult = await search.StartAsync();

            if (nativeResult != null && nativeResult.MapItems != null)
            {
                result.AddRange(nativeResult.MapItems.Select(i =>
                    new NativeiOSPlaceResult
                    {
                        Description = string.Format("{0}, {1} {2}", i.Placemark.Title, i.Placemark.AdministrativeArea, i.Placemark.SubAdministrativeArea),
                        Details = new PlaceDetails
                        {
                            Coordinate = i.Placemark.Coordinate.ToPosition(),
                            FormattedAddress = i.Placemark.Title,
                            InternationalPhoneNumber = i.PhoneNumber.ToString(),
                            Website = i.Url?.ToString()
                           
                        }
                    }));
                return result;
            }
            return null;
        }
        ///<inheritdoc/>
        public Task<PlaceDetails> GetDetails(string id)
        {
            throw new NotImplementedException("Not neccessary on iOS. Details already returned inside the prediction result set");
        }
    }
}
