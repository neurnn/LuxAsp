using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxAsp.Maps.Internals
{
    internal class MapsNullGeoplaceService : IMapsGeoplaceService
    {
        private static readonly Task<IEnumerable<MapsGeoplace>> EMPTY = Task.FromResult(null as IEnumerable<MapsGeoplace>);

        /// <summary>
        /// Geopoint Kind.
        /// </summary>
        public MapsGeopointKind GeopointKind => MapsGeopointKind.Unspec;

        /// <summary>
        /// Returns empty enumerable.
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public Task<IEnumerable<MapsGeoplace>> QueryAsync(string Address) => EMPTY;
        public Task<IEnumerable<MapsGeoplace>> QueryAsync(MapsGeopoint Geopoint) => EMPTY;
    }
}
