using LuxAsp.Authorizations;
using LuxAsp.Authorizations.Internals;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace LuxAsp
{
    /// <summary>
    /// Lux Authentication Builder Extensions.
    /// </summary>
    public static class LuxEFCoreAuthorizationExtensions
    {
        /// <summary>
        /// Configure User Model to database options.
        /// </summary>
        /// <typeparam name="TUserModel"></typeparam>
        /// <typeparam name="TUserRepository"></typeparam>
        /// <param name="This"></param>
        /// <returns></returns>
        public static IDatabaseOptions WithUserModel<TUserModel, TUserRepository>(this IDatabaseOptions This)
            where TUserModel : LuxUserModel, new() where TUserRepository : LuxUserRepository<TUserModel>, new()
        {
            return This.With<TUserModel, TUserRepository>();
        }

        /// <summary>
        /// Configure Token Model to database options.
        /// </summary>
        /// <typeparam name="TTokenModel"></typeparam>
        /// <typeparam name="TTokenRepository"></typeparam>
        /// <param name="This"></param>
        /// <returns></returns>
        public static IDatabaseOptions WithTokenModel<TTokenModel, TTokenRepository>(this IDatabaseOptions This)
            where TTokenModel : LuxTokenModel, new() where TTokenRepository: LuxTokenRepository<TTokenModel>, new()
        {
            return This.With<TTokenModel, TTokenRepository>();
        }

        /// <summary>
        /// Configure User Model with default user repository to database options.
        /// </summary>
        /// <typeparam name="TUserModel"></typeparam>
        /// <param name="This"></param>
        /// <returns></returns>
        public static IDatabaseOptions WithUserModel<TUserModel>(this IDatabaseOptions This)
            where TUserModel : LuxUserModel, new() => This.WithUserModel<TUserModel, LuxUserRepository<TUserModel>>();

        /// <summary>
        /// Configure Token Model with default token repository to database options.
        /// </summary>
        /// <typeparam name="TUserModel"></typeparam>
        /// <param name="This"></param>
        /// <returns></returns>
        public static IDatabaseOptions WithTokenModel<TTokenModel>(this IDatabaseOptions This)
            where TTokenModel : LuxTokenModel, new() => This.WithTokenModel<TTokenModel, LuxTokenRepository<TTokenModel>>();

        /// <summary>
        /// Configure Default User Model and Default User Repository.
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static IDatabaseOptions WithUserModel(this IDatabaseOptions This) => This.WithUserModel<LuxUserModel, LuxUserRepository>();

        /// <summary>
        /// Configure Default Token Model and Default Token Repository.
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static IDatabaseOptions WithTokenModel(this IDatabaseOptions This) => This.WithTokenModel<LuxTokenModel, LuxTokenRepository>();

        /// <summary>
        /// Use User-Repository based Authentication Provider.
        /// </summary>
        /// <typeparam name="TUserModel"></typeparam>
        /// <param name="This"></param>
        /// <returns></returns>
        public static LuxAuthenticationBuilder UseUserAuthentication<TUserModel>(this LuxAuthenticationBuilder This) where TUserModel : LuxUserModel, new() 
            => This.SetAuthenticationProvider(Request => new LuxUserAuthenticationProvider<TUserModel>(Request));

        /// <summary>
        /// Use User-Repository and Token-Repository based Authentication Providers.
        /// This configures Authentication Provider and Token Provider instances.
        /// </summary>
        /// <typeparam name="TUserModel"></typeparam>
        /// <typeparam name="TTokenModel"></typeparam>
        /// <param name="This"></param>
        /// <returns></returns>
        public static LuxAuthenticationBuilder UseUserAuthentication<TUserModel, TTokenModel>(this LuxAuthenticationBuilder This)
             where TUserModel : LuxUserModel, new() where TTokenModel : LuxTokenModel, new()
        {
            This.SetAuthenticationProvider(Request => new LuxUserAuthenticationProvider<TUserModel>(Request));
            This.SetTokenProvider(Request => new LuxUserAuthenticationTokenProvider<TTokenModel, TUserModel>(Request));
            return This;
        }

        /// <summary>
        /// Get User Model from the Authentication interface.
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static LuxUserModel GetUserModel(this ILuxAuthentication This)
        {
            if (This.Member != null && This.Member.Model is LuxUserModel UserModel)
                return UserModel;

            return null;
        }

        /// <summary>
        /// Get Token Model from the Authentication interface.
        /// Note that, when the token model mismatch, this always returns null.
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static TTokenModel GetTokenModel<TTokenModel>(this ILuxAuthentication This)
            where TTokenModel : LuxTokenModel, new()
        {
            if (!This.Token.HasValue || !This.Token.Value.IsValid ||
                !Guid.TryParse(This.Token.Value.Value, out var TokenId))
                return null;

            if (This is ILuxInfrastructure<IServiceProvider> _SPI)
            {
                var Tokens = _SPI.Instance.GetRequiredService<LuxTokenRepository<TTokenModel>>();
                return Tokens.Load(X => X.Where(Y => Y.Id == TokenId)).FirstOrDefault();
            }

            return null;
        }

        /// <summary>
        /// Get Token Model from the Authentication interface.
        /// Note that, when the token model isn't default token model implementation, this always returns null.
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static LuxTokenModel GetTokenModel(this ILuxAuthentication This) => This.GetTokenModel<LuxTokenModel>();

        /// <summary>
        /// Get User Model from the Authentication interface.
        /// </summary>
        /// <typeparam name="TUserModel"></typeparam>
        /// <param name="This"></param>
        /// <returns></returns>
        public static TUserModel GetUserModel<TUserModel>(this ILuxAuthentication This) where TUserModel : LuxUserModel
        {
            if (This.Member != null && This.Member is ILuxAuthenticatedMember<TUserModel> Member)
                return Member.Model;

            return null;
        }

        /// <summary>
        /// Set User Model to the Authentication interface.
        /// </summary>
        /// <typeparam name="TUserModel"></typeparam>
        /// <param name="This"></param>
        /// <param name="UserModel"></param>
        /// <returns></returns>
        public static ILuxAuthentication SetUserModel<TUserModel>(this ILuxAuthentication This, TUserModel UserModel) where TUserModel : LuxUserModel
        {
            if (UserModel is null)
            {
                This.Member = null;
                return This;
            }

            This.Member = UserModel.ToMember();
            return This;
        }
    }
}
