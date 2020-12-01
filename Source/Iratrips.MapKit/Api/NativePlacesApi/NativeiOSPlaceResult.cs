namespace Iratrips.MapKit.Api
{
    /// <summary>
    /// iOS Result set
    /// </summary>
    public class NativeiOSPlaceResult : IPlaceResult
    {
        ///<inheritdoc/>
        public string Description { get; set; }
        /// <summary>
        /// Gets/Sets the details of the place
        /// </summary>
        public PlaceDetails Details { get; set; }
        ///<inheritdoc />
        public string Subtitle { get; set; }
    }
}
