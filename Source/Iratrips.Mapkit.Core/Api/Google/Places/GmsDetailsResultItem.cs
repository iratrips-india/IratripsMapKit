
using System.Text.Json.Serialization;

namespace Iratrips.Mapkit.Api.Google
{
    /// <summary>
    /// Result item of the Google Places details api call
    /// </summary>
    public class GmsDetailsResultItem
    {
        /// <summary>
        /// All address components
        /// </summary>
        [JsonPropertyName("address_components")]
        public AddressComponents[] AddressComponents { get; set; }
        /// <summary>
        /// The Address as formatted text
        /// </summary>
        [JsonPropertyName("formatted_address")]
        public string FormattedAddress { get; set; }
        /// <summary>
        /// The phone number as formatted text
        /// </summary>
        [JsonPropertyName("formatted_phone_number")]
        public string FormattedPhoneNumer { get; set; }
        /// <summary>
        /// Geometry data of the place
        /// </summary>
        [JsonPropertyName("geometry")]
        public GmsGeometry Geometry { get; set; }
        /// <summary>
        /// Url to a place specific icon
        /// </summary>
        [JsonPropertyName("icon")]
        public string Icon { get; set; }
        /// <summary>
        /// Id 
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }
        /// <summary>
        /// The international phone number
        /// </summary>
        [JsonPropertyName("international_phone_number")]
        public string InternationalPhoneNumber { get; set; }
        /// <summary>
        /// Name of the place
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        /// <summary>
        /// Place Id
        /// </summary>
        [JsonPropertyName("place_id")]
        public string PlaceId { get; set; }

        // TODO: Add rest of the properties https://developers.google.com/places/web-service/details
    }
    /// <summary>
    /// Holds the data of an address component
    /// </summary>
    public struct AddressComponents
    {
        /// <summary>
        /// Long Name
        /// </summary>
        [JsonPropertyName("long_name")]
        public string LongName { get; set; }
        /// <summary>
        /// Short Name
        /// </summary>
        [JsonPropertyName("short_name")]
        public string ShortName { get; set; }
        /// <summary>
        /// Address Types e.g. "street_number" 
        /// </summary>
        public string[] Types { get; set; }
    }
    /// <summary>
    /// Google Geometry data
    /// </summary>
    public class GmsGeometry
    {
        /// <summary>
        /// Location of the place
        /// </summary>
        [JsonPropertyName("location")]
        public GmsLocation Location { get; set; }
    }
}
