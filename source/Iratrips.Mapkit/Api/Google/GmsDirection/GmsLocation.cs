
using System.Text.Json.Serialization;

namespace Iratrips.Mapkit.Api.Google
{
    /// <summary>
    /// Holds latitude longitude
    /// </summary>
    public struct GmsLocation
    {
        /// <summary>
        /// Latitude
        /// </summary>
        [JsonPropertyName("lat")]
        public double Latitude { get; set; }
        /// <summary>
        /// Longitude
        /// </summary>
        [JsonPropertyName("Lng")]
        public double Longitude { get; set; }
        /// <summary>
        /// Convert to position
        /// </summary>
        /// <returns><see cref="Position"/></returns>
        public Position ToPosition()
        {
            return new Position(Latitude, Longitude);
        }
    }
}
