using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace LuxAsp.Authorizations
{
    public struct LuxAuthenticationToken
    {
        public static readonly LuxAuthenticationToken Empty = new LuxAuthenticationToken();

        /// <summary>
        /// Authorization Type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Authorization Value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Expiration of the authorization.
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Test whether the generated token is valid or not.
        /// </summary>
        public bool IsValid => !string.IsNullOrWhiteSpace(Type) && !string.IsNullOrWhiteSpace(Value);
    }
}
