namespace Iratrips.Mapkit
{
    public partial class TKImageSource
    {
        public TKImageSource(string path, bool resource)
        {
            this.FilePath = path;
            this.IsResource = resource;
        }

        public string FilePath { get; private set; }
        public bool IsResource { get; private set; }

        public static implicit operator TKImageSource(string value)
        {
            return new TKImageSource(value, true);
        }
    }
}
