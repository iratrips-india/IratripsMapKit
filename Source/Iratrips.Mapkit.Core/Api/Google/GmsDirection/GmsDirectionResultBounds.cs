
using System.Text.Json.Serialization;

namespace Iratrips.Mapkit.Api.Google
{
    /// <summary>
    /// Bounds of the Direction API call result
    /// </summary>
    public class GmsDirectionResultBounds
    {
        /// <summary>
        /// Gets the north-east boundary
        /// </summary>
        [JsonPropertyName("northeast")]
        public GmsLocation NorthEast { get; set; }
        /// <summary>
        /// Gets the south-west boundary
        /// </summary>
        [JsonPropertyName("southwest")]
        public GmsLocation SouthWest { get; set; }
    }
}
