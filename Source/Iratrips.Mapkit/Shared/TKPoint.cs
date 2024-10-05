using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Iratrips.Mapkit
{
    public partial class TKPoint
    {
        private Point? _platformPoint = null;
        public Point ToMaui()
        {
            if (_platformPoint == null)
                _platformPoint = new Point(X, Y);

            return _platformPoint.Value;
        }

    }
}
