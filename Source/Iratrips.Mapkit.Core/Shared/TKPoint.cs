using System;
using System.Collections.Generic;
using System.Text;

namespace Iratrips.Mapkit
{
    public partial class TKPoint
    {
        public TKPoint(double x, double y)
        {
            X = x; 
            Y = y;
        }

        public double X { get; private set; }
        public double Y { get; private set; }
    }
}
