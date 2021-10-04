using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxAsp.Maps
{
    /// <summary>
    /// Maps Geolocation Service,
    /// that searches the address by [lat, lng] pair or reverse, and
    /// futhermore, this search places by address or [lat, lng].
    /// </summary>
    public interface IMapsGeoplaceService
    {
        /// <summary>
        /// Geopoint Kind that this service generates.
        /// </summary>
        MapsGeopointKind GeopointKind { get; }

        /// <summary>
        /// Query Places by Address.
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        Task<IEnumerable<MapsGeoplace>> QueryAsync(string Address);

        /// <summary>
        /// Query Places by Geopoint.
        /// </summary>
        /// <param name="Geopoint"></param>
        /// <returns></returns>
        Task<IEnumerable<MapsGeoplace>> QueryAsync(MapsGeopoint Geopoint);
    }
}
