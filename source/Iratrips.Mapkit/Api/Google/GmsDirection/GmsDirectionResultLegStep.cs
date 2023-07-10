
using System.Text.Json.Serialization;

namespace Iratrips.Mapkit.Api.Google
{
    /// <summary>
    /// A step inside a leg
    /// </summary>
    public class GmsDirectionResultLegStep
    {
        [JsonPropertyName("start_location")]
        public GmsLocation StartLocation { get; set; }
        [JsonPropertyName("end_location")]
        public GmsLocation EndLocation { get; set; }
        public GmsTextValue Distance { get; set; }
        public GmsTextValue Duration { get; set; }
        public GmsPolyline Polyline { get; set; }
        [JsonPropertyName("html_instructions")]
        public string HtmlInstructions { get; set; }
    }
}
