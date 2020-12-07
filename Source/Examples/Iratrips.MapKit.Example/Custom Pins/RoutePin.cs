using Iratrips.MapKit;
using Iratrips.MapKit.Overlays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TK.CustomMap.Sample
{
    /// <summary>
    /// Pin which is either source or destination of a route
    /// </summary>
    public class RoutePin : MKCustomMapPin
    {
        /// <summary>
        /// Gets/Sets if the pin is the source of a route. If <value>false</value> pin is destination
        /// </summary>
        public bool IsSource { get; set; }
        /// <summary>
        /// Gets/Sets reference to the route
        /// </summary>
        public MKRoute Route { get; set; }
    }
}
