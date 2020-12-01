using Iratrips.MapKit.Overlays;

namespace Iratrips.MapKit.Models
{
    /// <summary>
    /// Holds the <see cref="IMRoute"/> and the error message of the calculation failure
    /// </summary>
    public class RouteCalculationError
    {
        /// <summary>
        /// Gets the route
        /// </summary>
        public IMRoute Route { get;  set; }
        /// <summary>
        /// Gets the error message
        /// </summary>
        public string ErrorMessage { get;  set; }
        /// <summary>
        /// Creates a new instance of <see cref="RouteCalculationError"/>
        /// </summary>
        /// <param name="route">The route</param>
        /// <param name="errorMessage">The error message</param>
        public RouteCalculationError(IMRoute route, string errorMessage)
        {
            Route = route;
            ErrorMessage = errorMessage;
        }
    }
}
