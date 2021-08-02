using LuxAsp.Sessions;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Authorizations.Internals
{
    internal class LuxAuthentication : ILuxAuthentication
    {
        private static readonly string KEY_SESSION = "!!LAMS_MEMBER__GUID!!";
        private static readonly char[] SEPARATOR = new char[] { ' ', '\t' };

        private HttpContext m_Context;
        private LuxAuthenticationToken m_Token;

        private ILuxAuthenticationProvider m_Provider;
        private ILuxAuthenticationTokenProvider m_TokenProvider;
        private ILuxAuthenticationListener m_Listener;

        private ILuxAuthenticatedMember m_Member;
        private ILuxSession m_Session;

        private bool m_IsStateless = false;
        private bool m_Initiated = false;
        private bool m_Loaded = false;

        /// <summary>
        /// Initialize a new Authentication interface.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="Provider"></param>
        /// <param name="TokenProvider"></param>
        /// <param name="Listener"></param>
        public LuxAuthentication(
            HttpContext Context,
            ILuxAuthenticationProvider Provider,
            ILuxAuthenticationTokenProvider TokenProvider,
            ILuxAuthenticationListener Listener)
        {
            m_Context = Context;
            m_Provider = Provider;
            m_TokenProvider = TokenProvider;
            m_Listener = Listener;
        }

        /// <summary>
        /// Gets the HttpContext as Infrastructure.
        /// </summary>
        HttpContext ILuxInfrastructure<HttpContext>.Instance => m_Context;

        /// <summary>
        /// Gets the IServiceProvider as Infrastructure.
        /// </summary>
        IServiceProvider ILuxInfrastructure<IServiceProvider>.Instance => m_Context.RequestServices;

        /// <summary>
        /// Parse the header into token structure.
        /// </summary>
        /// <param name="Header"></param>
        /// <returns></returns>
        private static LuxAuthenticationToken ParseHeader(string Header)
        {
            if (Header is null || string.IsNullOrWhiteSpace(Header))
                return LuxAuthenticationToken.Empty;

            var Values = Header
                .Split(SEPARATOR, 2)
                .Where(X => !string.IsNullOrWhiteSpace((X ?? "").Trim()))
                .ToArray();

            return new LuxAuthenticationToken
            {
                Type = Values[0],
                Value = Values[1]
            };
        }

        /// <summary>
        /// Load the authorization informations.
        /// </summary>
        /// <returns></returns>
        private LuxAuthentication Load()
        {
            if (m_Loaded)
                return this;

            m_Loaded = true;

            if (m_Context.Request.Headers.TryGetValue("Authorization", out var Auth))
            {
                m_Token = ParseHeader(Auth.FirstOrDefault(X => !string.IsNullOrWhiteSpace(X)));
                m_Member = m_TokenProvider.ResolveAsync(m_Token).Result;
                m_IsStateless = m_Initiated = true;
                return this;
            }

            var Options = m_Context
                .GetRequiredService<LuxSessionOptions>();

            m_IsStateless = m_Initiated = false;
            m_Member = null;

            if (Options is null)
                return this;

            var Cookie = new CookieBuilder();
            Options.Cookies(Cookie);

            if (m_Context.Request.Cookies.TryGetValue(Cookie.Name, out var Value))
            {
                m_Session = m_Context.GetRequiredService<ILuxSession>();
                m_Member = m_Provider.ResolveAsync(m_Session.GetGuid(KEY_SESSION, Guid.Empty)).Result;
                m_Initiated = true;
            }

            return this;
        }

        /// <summary>
        /// Determines whether the authentication method is based on stateless or not.
        /// </summary>
        public bool IsStateless
        {
            get
            {
                Load();
                return m_IsStateless;
            }
        }

        /// <summary>
        /// Gets the Token if its stateless.
        /// </summary>
        public LuxAuthenticationToken? Token
        {
            get
            {
                Load();

                if (m_IsStateless)
                    return m_Token;

                return null;
            }
        }

        /// <summary>
        /// Authenticated Member.
        /// </summary>
        public ILuxAuthenticatedMember Member
        {
            get => m_Member;
            set
            {
                Initiate();

                if (IsStateless)
                    throw new NotSupportedException("Stateless authentication cannot be altered.");

                AuthorizeAsync(value).Wait();
            }
        }

        /// <summary>
        /// Abandon the authentication asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AbandonAsync(CancellationToken Token = default)
        {
            await InitiateAsync();

            if (m_Member is null) return false;
            if (IsStateless)
            {
                if (m_Token.IsValid)
                    await m_TokenProvider.DeleteAsync(m_Token, Token);

                await m_Listener.OnUnauthorized(m_Member, Token);

                m_Member = null;
                m_Token = LuxAuthenticationToken.Empty;
                return true;
            }

            await UnauthorizeAsync();
            return true;
        }

        /// <summary>
        /// Initiate the authentication system.
        /// </summary>
        private void Initiate()
        {
            if (m_Initiated || IsStateless || InitiateAsync().Result) return;
            throw new NotSupportedException("This request doesn't support the authentication system.");
        }

        /// <summary>
        /// Initiate the Session based Authentication Service.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> InitiateAsync()
        {
            if (m_Initiated || IsStateless)
                return true;

            try
            {
                m_Session = m_Context.GetRequiredService<ILuxSession>();
                m_Member = await m_Provider.ResolveAsync(m_Session.GetGuid(KEY_SESSION, Guid.Empty));
                m_Initiated = true;
            }

            catch { return false; }
            return true;
        }

        /// <summary>
        /// Authorize the Session.
        /// </summary>
        /// <param name="Member"></param>
        /// <returns></returns>
        private async Task AuthorizeAsync(ILuxAuthenticatedMember Member)
        {
            var NGuid = Member != null ? Member.Guid : Guid.Empty;
            var PGuid = m_Member != null ? m_Member.Guid : Guid.Empty;

            if (NGuid != PGuid)
            {
                await UnauthorizeAsync();

                if (Member is null)
                    m_Session.Unset(KEY_SESSION);

                else
                {
                    m_Session.SetGuid(KEY_SESSION, NGuid);
                    await m_Listener.OnAuthorized(m_Member = Member);
                }
            }
        }

        /// <summary>
        /// Unauthorize the Session.
        /// </summary>
        private async Task UnauthorizeAsync()
        {
            if (m_Member != null)
            {
                await m_Listener.OnUnauthorized(m_Member);
                m_Session.Unset(KEY_SESSION);
            }

            m_Member = null;
        }

        /// <summary>
        /// Create the Authentication Token based on currently authorized member.
        /// When not authenticated, this returns invalid token.
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        public async Task<LuxAuthenticationGeneratedToken> CreateAsync(CancellationToken Token = default)
        {
            await InitiateAsync();

            if (m_Member is null)
                return LuxAuthenticationGeneratedToken.Empty;

            await m_TokenProvider.CreateAsync(m_Member, Token);

            var OutToken = await m_TokenProvider.CreateAsync(m_Member, Token);
            if (OutToken.IsValid) await m_Listener.OnAuthorized(m_Member);
            return OutToken;
        }

        /// <summary>
        /// Refresh the Authorization Token from Member instance asynchronously.
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Member"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        public async Task<LuxAuthenticationGeneratedToken> RefreshAsync(LuxAuthenticationToken Refresh, CancellationToken Token = default)
        {
            await InitiateAsync();

            if (m_Member is null || Refresh.IsValid)
                return LuxAuthenticationGeneratedToken.Empty;

            return await m_TokenProvider.RefreshAsync(Member, Refresh, Token);
        }

    }
}
