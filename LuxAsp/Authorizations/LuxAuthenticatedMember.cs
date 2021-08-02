using System;
using System.Collections.Generic;

namespace LuxAsp.Authorizations
{
    /// <summary>
    /// Member Model Wrapper.
    /// </summary>
    public class LuxAuthenticatedMember<TModel> : ILuxAuthenticatedMember<TModel> where TModel : class
    {
        protected LuxAuthenticatedMember(Guid Guid, TModel Model)
        {
            this.Model = Model;
            this.Guid = Guid;
        }

        /// <summary>
        /// Create ILuxAuthenticatedMember instance from Guid and Model.
        /// </summary>
        /// <param name="Guid"></param>
        /// <param name="Model"></param>
        /// <returns></returns>
        public static LuxAuthenticatedMember<TModel> FromModel(Guid Guid, TModel Model)
               => new LuxAuthenticatedMember<TModel>(Guid, Model);

        /// <summary>
        /// Guid that represents the member's unique identifier.
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// Member Model.
        /// </summary>
        public TModel Model { get; }

        /// <summary>
        /// Authorization Collection.
        /// This collection will be updated by LuxAuthorize filters.
        /// </summary>
        public ILuxAuthorizationKindCollection Authorizations { get; } = new LuxAuthorizationKindCollection();

        /// <summary>
        /// Applied Policies for the member in current request. (Instant)
        /// This collection will be updated by LuxPolicy filters.
        /// </summary>
        public ILuxPolicyKindCollection Policies { get; } = new LuxPolicyKindCollection();

        /// <summary>
        /// Member Model (ILuxAuthenticatedMember).
        /// </summary>
        object ILuxAuthenticatedMember.Model => Model;
    }
}
