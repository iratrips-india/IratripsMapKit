using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iratrips.MapKit
{
    /// <summary>
    /// <see cref="EventArgs"/> providing a <see cref="Position"/>
    /// </summary>
    public class GenericEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Gets the value of the <see cref="TKGenericEventArgs"/>
        /// </summary>
        public T Value { get;  set; }
        /// <summary>
        /// Creates a new instance of <see cref="TKGenericEventArgs"/>
        /// </summary>
        /// <param name="value">The value</param>
        public GenericEventArgs(T value)
        {
            Value = value;
        }
    }
}
