using System;
using System.Collections.Generic;
using System.Text;

namespace Iratrips.Mapkit
{
    public partial class TKColor
    {
        public TKColor(string hex)
        {
            Hex = hex;
        }

        public string Hex { get; private set; }

    }
}
