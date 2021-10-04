using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LuxAsp.Maps
{
    /// <summary>
    /// Maps Geoplace.
    /// </summary>
    public sealed class MapsGeoplace : IEnumerable<Type>
    {
        public MapsGeoplace() { }

        /// <summary>
        /// Meta Data.
        /// </summary>
        public IDictionary<Type, object> MetaData { get; } = new Dictionary<Type, object>();

        /// <summary>
        /// Gets the enumerator that iterates the meta-data's type informations.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Type> GetEnumerator() => MetaData.Keys.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => MetaData.Keys.GetEnumerator();
    }
}
