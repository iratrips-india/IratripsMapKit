using Iratrips.Mapkit.Overlays;

namespace Iratrips.Mapkit.Models
{
    /// <summary>
    /// Holds the <see cref="TKRoute"/> and the error message of the calculation failure
    /// </summary>
    public class RouteCalculationError
    {
        /// <summary>
        /// Gets the route
        /// </summary>
        public TKRoute Route { get;  set; }
        /// <summary>
        /// Gets the error message
        /// </summary>
        public string ErrorMessage { get;  set; }
        /// <summary>
        /// Creates a new instance of <see cref="RouteCalculationError"/>
        /// </summary>
        /// <param name="route">The route</param>
        /// <param name="errorMessage">The error message</param>
        public RouteCalculationError(TKRoute route, string errorMessage)
        {
            Route = route;
            ErrorMessage = errorMessage;
        }
    }
}
