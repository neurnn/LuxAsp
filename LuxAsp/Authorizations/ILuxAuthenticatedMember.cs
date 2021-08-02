using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LuxAsp.Authorizations
{
    /// <summary>
    /// Authenticated Member interface.
    /// </summary>
    public interface ILuxAuthenticatedMember
    {
        /// <summary>
        /// Guid that represents the member's unique identifier.
        /// </summary>
        Guid Guid { get; }

        /// <summary>
        /// Member Model.
        /// </summary>
        object Model { get; }

        /// <summary>
        /// Applied Authorizations for the member in current request. (Instant)
        /// This collection will be updated by LuxAuthorize filters.
        /// </summary>
        ILuxAuthorizationKindCollection Authorizations { get; }

        /// <summary>
        /// Applied Policies for the member in current request. (Instant)
        /// This collection will be updated by LuxPolicy filters.
        /// </summary>
        ILuxPolicyKindCollection Policies { get; }
    }

    /// <summary>
    /// Authenticated Member interface.
    /// </summary>
    public interface ILuxAuthenticatedMember<TModel> : ILuxAuthenticatedMember where TModel :class
    {
        /// <summary>
        /// Member Model.
        /// </summary>
        new TModel Model { get; }
    }
}
