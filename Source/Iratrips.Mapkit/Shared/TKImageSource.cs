using Microsoft.Maui.Controls;

namespace Iratrips.Mapkit
{
    public partial class TKImageSource
    {
        private ImageSource _platformSource = null;
        public ImageSource ToMaui()
        {
            if (_platformSource == null)
            {
                if (this.IsResource)
                    _platformSource = this.FilePath;
                else
                    _platformSource = ImageSource.FromFile(this.FilePath);
            }

            return _platformSource;
        }

    }
}
