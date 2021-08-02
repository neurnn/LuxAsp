using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuxAsp.Authorizations
{
    public interface ILuxAuthorizationKind
    {
        /// <summary>
        /// Authorization properties that set by middlewares and filters.
        /// </summary>
        IDictionary<object, object> Properties { get; set; }
    }
}
