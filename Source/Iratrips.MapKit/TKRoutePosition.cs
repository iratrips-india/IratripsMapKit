using System;
using System.Collections.Generic;
using System.Text;

namespace Iratrips.Mapkit
{
    public class TkRoutePosition
    {
        public Position Position { get; private set; }
        public int CombineStepIndex { get; private set; }

        public TkRoutePosition(Position position, int combineStepIndex)
        {
            Position = position;
            CombineStepIndex = combineStepIndex;
        }
    }
}
