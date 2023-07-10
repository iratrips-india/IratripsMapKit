
using System.Text.Json.Serialization;

namespace Iratrips.Mapkit.Api.Google
{
    /// <summary>
    /// Prediction result of the Google Place API call
    /// </summary>
    public class GmsPlacePrediction : IPlaceResult
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("place_id")]
        public string PlaceId { get; set; }
        
        [JsonPropertyName("reference")]
        public string Reference { get; set; }
        
        ///<inheritdoc />
        public string Subtitle { get; set; }
    }
}
