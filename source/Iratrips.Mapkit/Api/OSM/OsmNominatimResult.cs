
using System.Text.Json.Serialization;

namespace Iratrips.Mapkit.Api.OSM
{
    /// <summary>
    /// Result class of the OSM Nominatim search API call
    /// </summary>
    public class OsmNominatimResult : IPlaceResult
    {
        /// <summary>
        /// Gets/Sets the id of the place
        /// </summary>
        [JsonPropertyName("place_id")]
        public string PlaceId { get; set; }
        /// <summary>
        /// Gets/Sets the OSM id
        /// </summary>
        [JsonPropertyName("osm_id")]
        public string OsmId { get; set; }
        /// <summary>
        /// Gets/Sets latitude
        /// </summary>
        [JsonPropertyName("lat")]
        public double Latitude { get; set; }
        /// <summary>
        /// Gets/Sets longitude
        /// </summary>
        [JsonPropertyName("lon")]
        public double Longitude { get; set; }
        /// <summary>
        /// Gets/Sets description
        /// </summary>
        [JsonPropertyName("display_name")]
        public string Description { get; set; }
        ///<inheritdoc />
        public string Subtitle { get; set; }
    }
}
