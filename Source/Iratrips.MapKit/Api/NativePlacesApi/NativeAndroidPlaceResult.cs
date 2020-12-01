namespace Iratrips.MapKit.Api
{
    /// <summary>
    /// Android result set
    /// </summary>
    public class NativeAndroidPlaceResult : IPlaceResult
    {
        /// <summary>
        /// Gets/Sets the Place Id
        /// </summary>
        public string PlaceId { get; set; }
        ///<inheritdoc/>
        public string Description { get; set; }
        ///<inheritdoc />
        public string Subtitle { get; set; }
    }
}
