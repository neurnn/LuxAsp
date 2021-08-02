using LuxAsp.Authorizations.Internals;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Authorizations
{
    /// <summary>
    /// Authentication Engine.
    /// </summary>
    public sealed class LuxAuthenticationBuilder : LuxHostConfiguration
    {
        protected override void Configure(ILuxHostBuilder Builder) { }

        private Func<HttpRequest, ILuxAuthenticationProvider> m_AuthenticationProvider;
        private Func<HttpRequest, ILuxAuthenticationTokenProvider> m_TokenProvider;
        private List<Func<HttpRequest, ILuxAuthenticationListener>> m_Listeners;

        public LuxAuthenticationBuilder()
        {
            m_Listeners = new List<Func<HttpRequest, ILuxAuthenticationListener>>();
        }

        /// <summary>
        /// Properties that used for building authentication driver.
        /// </summary>
        public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();

        /// <summary>
        /// Set Authentication Provider.
        /// </summary>
        /// <param name="Factory"></param>
        /// <returns></returns>
        public LuxAuthenticationBuilder SetAuthenticationProvider(Func<HttpRequest, ILuxAuthenticationProvider> Factory)
        {
            m_AuthenticationProvider = Factory;
            return this;
        }

        /// <summary>
        /// Set Token Authentication Provider.
        /// </summary>
        /// <param name="Factory"></param>
        /// <returns></returns>
        public LuxAuthenticationBuilder SetTokenProvider(Func<HttpRequest, ILuxAuthenticationTokenProvider> Factory)
        {
            m_TokenProvider = Factory;
            return this;
        }

        /// <summary>
        /// Adds an Authentication Event Listener.
        /// </summary>
        /// <param name="Factory"></param>
        /// <returns></returns>
        public LuxAuthenticationBuilder WhenAuthentication(Func<HttpRequest, ILuxAuthenticationListener> Factory)
        {
            m_Listeners.Add(Factory);
            return this;
        }

        /// <summary>
        /// Build the authentication provider.
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        internal ILuxAuthenticationProvider BuildProvider(HttpRequest Request)
        {
            if (m_AuthenticationProvider is null)
                return new LuxAuthenticationNullProvider(Request);

            return m_AuthenticationProvider(Request);
        }

        /// <summary>
        /// Build the token authorization provider.
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        internal ILuxAuthenticationTokenProvider BuildTokenProvider(HttpRequest Request)
        {
            if (m_TokenProvider is null)
                return new LuxAuthenticationNullTokenProvider(Request);

            return m_TokenProvider(Request);
        }

        private class CombinedListener : ILuxAuthenticationListener
        {
            private Queue<Func<HttpRequest, ILuxAuthenticationListener>> m_Factories;
            private List<ILuxAuthenticationListener> m_Listeners = new List<ILuxAuthenticationListener>();

            /// <summary>
            /// Combines the multiple authentication event listeners.
            /// </summary>
            /// <param name="Request"></param>
            /// <param name="Factories"></param>
            public CombinedListener(HttpRequest Request, IEnumerable<Func<HttpRequest, ILuxAuthenticationListener>> Factories)
            {
                m_Factories = new Queue<Func<HttpRequest, ILuxAuthenticationListener>>(Factories);
                this.Request = Request;
            }

            /// <summary>
            /// Request Instance.
            /// </summary>
            public HttpRequest Request { get; }

            /// <summary>
            /// Prepare the 
            /// </summary>
            /// <returns></returns>
            private CombinedListener PrepareListeners()
            {
                while (m_Factories.Count > 0)
                {
                    var Factory = m_Factories.Dequeue();
                    var Instance = Factory != null ? Factory(Request) : null;
                    if (Instance != null) m_Listeners.Add(Instance);
                }

                return this;
            }

            /// <summary>
            /// Called when the member authorized.
            /// </summary>
            /// <param name="Member"></param>
            /// <param name="Cancellation"></param>
            /// <returns></returns>
            public async Task OnAuthorized(ILuxAuthenticatedMember Member, CancellationToken Cancellation = default)
            {
                foreach (var Each in PrepareListeners().m_Listeners)
                    await Each.OnAuthorized(Member, Cancellation);
            }

            /// <summary>
            /// Called when the member unauthorized.
            /// </summary>
            /// <param name="Member"></param>
            /// <param name="Cancellation"></param>
            /// <returns></returns>
            public async Task OnUnauthorized(ILuxAuthenticatedMember Member, CancellationToken Cancellation = default)
            {
                foreach (var Each in PrepareListeners().m_Listeners)
                    await Each.OnUnauthorized(Member, Cancellation);
            }
        }

        /// <summary>
        /// Build the authorization event listener.
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        internal ILuxAuthenticationListener BuildListener(HttpRequest Request)
        {
            if (m_Listeners.Count <= 0) return new LuxAuthenticationNullListener(Request);
            if (m_Listeners.Count == 1) return m_Listeners.First().Invoke(Request);
            return new CombinedListener(Request, m_Listeners);
        }
    }

}
