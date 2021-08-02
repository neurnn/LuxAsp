using LuxAsp.Sessions.Internals;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace LuxAsp.Sessions
{
    public sealed class LuxSessionOptions
    {
        private Type m_SessionStoreType;
        private Func<IServiceProvider, ILuxSessionStore> m_SessionStoreFactory;

        /// <summary>
        /// Use Session Store.
        /// </summary>
        /// <typeparam name="TSessionStore"></typeparam>
        /// <returns></returns>
        public LuxSessionOptions Use<TSessionStore>(Func<IServiceProvider, TSessionStore> Factory = null)
            where TSessionStore : ILuxSessionStore
        {
            m_SessionStoreType = typeof(TSessionStore);

            if (Factory != null)
                m_SessionStoreFactory = Services => Factory(Services);

            else m_SessionStoreFactory = null;
            return this;
        }

        /// <summary>
        /// Properties.
        /// </summary>
        public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Timeout when loads Session.
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Expiration of the Session. (from last acts)
        /// </summary>
        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(15);

        /// <summary>
        /// Idle Timeout of the Session. (from last acts)
        /// </summary>
        public TimeSpan IdleTimeout { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Garbage Collection Timer.
        /// </summary>
        public TimeSpan GCTimer { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Cookie Options.
        /// </summary>
        public Action<CookieBuilder> Cookies { get; set; } = X =>
        {
            X.Name = "LUXSID";
            X.HttpOnly = true;
            X.IsEssential = true;
            X.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        };

        // ---------------------
        private static readonly object ITEM_KEY = new object();
        internal static LuxSessionOptions GetOption(IDictionary<object, object> App)
        {
            if (!App.ContainsKey(ITEM_KEY))
                 App[ITEM_KEY] = new LuxSessionOptions();

            return App[ITEM_KEY] as LuxSessionOptions;
        }

        /// <summary>
        /// Add Service to Service Collection
        /// </summary>
        /// <param name="Services"></param>
        internal LuxSessionOptions AddService(IServiceCollection Services)
        {
            if (m_SessionStoreFactory != null)
                Services.AddSingleton(m_SessionStoreFactory);

            else if (m_SessionStoreType != null)
                Services.AddSingleton(typeof(ILuxSessionStore), m_SessionStoreType);

            else Services.AddSingleton<ILuxSessionStore, LuxFileSessionStore>();
            return this;
        }
    }
}
