using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Authorizations.Internals
{
    internal class LuxUserAuthenticationTokenProvider<TTokenModel, TUserModel> : ILuxAuthenticationTokenProvider 
        where TTokenModel : LuxTokenModel, new() where TUserModel : LuxUserModel, new()
    {
        private static readonly Task<ILuxAuthenticatedMember> NULL_MEMBER = Task.FromResult(null as ILuxAuthenticatedMember);
        private static readonly Task<LuxAuthenticationGeneratedToken> EMPTY_TOKEN = Task.FromResult(LuxAuthenticationGeneratedToken.Empty);

        private Repository<TUserModel> m_Users;
        private LuxTokenRepository<TTokenModel> m_Tokens;

        /// <summary>
        /// Initialize the new Token Authentication which uses the User Model and the Token Model.
        /// </summary>
        /// <param name="Request"></param>
        public LuxUserAuthenticationTokenProvider(HttpRequest Request)
            => this.Request = Request;

        /// <summary>
        /// Http Request.
        /// </summary>
        public HttpRequest Request { get; }

        /// <summary>
        /// Get Token Repository.
        /// </summary>
        /// <returns></returns>
        private LuxTokenRepository<TTokenModel> GetTokenRepository()
        {
            if (m_Tokens is null)
            {
                var TokenRepo = Request.GetRequiredService<Repository<TTokenModel>>();
                if (!(TokenRepo is LuxTokenRepository<TTokenModel> _Tokens))
                    throw new InvalidProgramException("Token Repository should be inherited from LuxTokenRepository<> class.");

                m_Tokens = _Tokens;
            }

            return m_Tokens;
        }

        /// <summary>
        /// Get User Repository;
        /// </summary>
        /// <returns></returns>
        private Repository<TUserModel> GetUserRepository()
        {
            if (m_Users is null)
                m_Users = Request.GetRequiredService<Repository<TUserModel>>();

            return m_Users;
        }

        /// <summary>
        /// Create a new token from the member instance that.
        /// </summary>
        /// <param name="Member"></param>
        /// <param name="Cancellation"></param>
        /// <returns></returns>
        public async Task<LuxAuthenticationGeneratedToken> CreateAsync(ILuxAuthenticatedMember Member, CancellationToken Cancellation = default)
        {
            if (!(Member is ILuxAuthenticatedMember<TUserModel> _Member))
                return LuxAuthenticationGeneratedToken.Empty;

            var Tokens = GetTokenRepository();
            var TokenModel = await Tokens.CreateAsync(_Member.Model);

            return new LuxAuthenticationGeneratedToken
            {
                Token = new LuxAuthenticationToken
                {
                    Type = "bearer",
                    Value = TokenModel.Id.ToString(),
                    Expiration = TokenModel.ExpirationTime
                },

                RefreshToken = new LuxAuthenticationToken
                {
                    Type = "bearer",
                    Value = TokenModel.RefreshId.ToString(),
                    Expiration = TokenModel.ExpirationTime
                }
            };
        }

        /// <summary>
        /// Delete the token asynchronously.
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Cancellation"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(LuxAuthenticationToken Token, CancellationToken Cancellation = default)
        {
            if (!Token.IsValid || Guid.TryParse(Token.Value, out var TokenId))
                return false;

            var Tokens = GetTokenRepository();
            var TokenModel = Tokens.Load(X => X.Where(Y => Y.Id == TokenId)).FirstOrDefault();
            if (TokenModel is null)
                return false;

            return await TokenModel.DeleteAsync();
        }

        /// <summary>
        /// Refresh the token asynchronously.
        /// </summary>
        /// <param name="Member"></param>
        /// <param name="Token"></param>
        /// <param name="Cancellation"></param>
        /// <returns></returns>
        public Task<LuxAuthenticationGeneratedToken> RefreshAsync(ILuxAuthenticatedMember Member, LuxAuthenticationToken Token, CancellationToken Cancellation = default)
        {
            if (!(Member is ILuxAuthenticatedMember<TUserModel> _Member)) return EMPTY_TOKEN;
            if (!Token.IsValid || Guid.TryParse(Token.Value, out var RefreshId)) return EMPTY_TOKEN;

            var Tokens = GetTokenRepository();
            var UserId = Member.Guid;

            var TokenModel = Tokens
                .Load(X => X.Where(Y => Y.UserId == UserId).Where(Y => Y.RefreshId == RefreshId))
                .FirstOrDefault();

            if (TokenModel is null || TokenModel.IsExpired || !TokenModel.Refresh(RefreshId))
                return EMPTY_TOKEN;

            return Task.FromResult(new LuxAuthenticationGeneratedToken
            {
                Token = new LuxAuthenticationToken
                {
                    Type = "bearer",
                    Value = TokenModel.Id.ToString(),
                    Expiration = TokenModel.ExpirationTime
                },

                RefreshToken = new LuxAuthenticationToken
                {
                    Type = "bearer",
                    Value = TokenModel.RefreshId.ToString(),
                    Expiration = TokenModel.ExpirationTime
                }
            });
        }

        /// <summary>
        /// Resolve User by Token asynchronously.
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Cancellation"></param>
        /// <returns></returns>
        public Task<ILuxAuthenticatedMember> ResolveAsync(LuxAuthenticationToken Token, CancellationToken Cancellation = default)
        {
            if (!Token.IsValid || Guid.TryParse(Token.Value, out var TokenId))
                return NULL_MEMBER;

            var Tokens = GetTokenRepository();
            var TokenModel = Tokens.Load(X => X.Where(Y => Y.Id == TokenId)).FirstOrDefault();
            if (TokenModel is null || TokenModel.IsExpired) return NULL_MEMBER;

            var Users = GetUserRepository();
            var UserId = TokenModel.UserId;
            var UserModel = Users.Load(X => X.Where(Y => Y.Id == UserId)).FirstOrDefault();
            if (UserModel is null) return NULL_MEMBER;

            return Task.FromResult<ILuxAuthenticatedMember>(UserModel.ToMember());
        }
    }
}
