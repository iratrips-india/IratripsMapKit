using Microsoft.Maui.Graphics;

namespace Iratrips.Mapkit
{
    public partial class TKColor
    {
        private Color _platformColor = null;
        public Color ToMaui()
        {
            if (_platformColor == null)
                _platformColor = Color.Parse(Hex);

            return _platformColor;
        }


    }
}
