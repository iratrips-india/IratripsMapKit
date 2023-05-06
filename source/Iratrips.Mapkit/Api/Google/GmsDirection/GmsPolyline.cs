using System.Collections.Generic;
using Newtonsoft.Json;

namespace Iratrips.Mapkit.Api.Google
{
    /// <summary>
    /// Google Polyline class
    /// </summary>
    public class GmsPolyline
    {
        /// <summary>
        /// Gets the points as string
        /// </summary>

        private string _points = null;
        public string Points
        {
            get => _points;
            set
            {
                _points = value;
                _positions = null;
            }
        }


        private IEnumerable<Position> _positions = null;

        /// <summary>
        /// Gets the converted positions
        /// </summary>
        [JsonIgnore]
        public IEnumerable<Position> Positions
        {
            get
            {
                if (_positions == null)
                    _positions = GooglePoints.Decode(Points);

                return _positions;
            }
        }

        public GmsPolyline()
        {

        }

        public GmsPolyline(string points, IEnumerable<Position> positions)
        {
            this.Points = points;
            _positions = positions;
        }
    }
}
