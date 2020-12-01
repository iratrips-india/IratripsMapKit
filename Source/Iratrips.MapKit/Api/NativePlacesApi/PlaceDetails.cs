

namespace Iratrips.MapKit.Api
{
    /// <summary>
    /// Details of a place
    /// </summary>
    public class PlaceDetails
    {
        /// <summary>
        /// The Address as formatted text
        /// </summary>
        public string FormattedAddress { get; set; }
        /// <summary>
        /// The international phone number
        /// </summary>
        public string InternationalPhoneNumber { get; set; }
        /// <summary>
        /// The website uri
        /// </summary>
        public string Website { get; set; }
        /// <summary>
        /// Gets/Sets the coordinate of the place
        /// </summary>
        public Position Coordinate { get; set; }
    }
}
