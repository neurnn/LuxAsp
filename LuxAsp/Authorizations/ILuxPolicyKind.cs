using System;
using System.Collections.Generic;

namespace LuxAsp.Authorizations
{
    public interface ILuxPolicyKind
    {
        /// <summary>
        /// Policy properties that set by middlewares and filters.
        /// </summary>
        IDictionary<object, object> Properties { get; set; }

        /// <summary>
        /// Expiration of this policy.
        /// Note: when this value is null, it means that the policy is permanent.
        /// </summary>
        DateTime? Expiration { get; }
    }
}
