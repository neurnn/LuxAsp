using System;
using System.Threading.Tasks;

namespace LuxAsp.Authorizations
{
    /// <summary>
    /// Basic Token Repository.
    /// </summary>
    public class LuxTokenRepository : LuxTokenRepository<LuxTokenModel>
    {
    }

    /// <summary>
    /// Token Repository.
    /// </summary>
    /// <typeparam name="TTokenModel"></typeparam>
    public class LuxTokenRepository<TTokenModel> 
        : Repository<TTokenModel> where TTokenModel : LuxTokenModel, new()
    {
        /// <summary>
        /// Default Expiration Due when the refresh token acts.
        /// </summary>
        public TimeSpan DefaultExpirationDue { get; protected set; } = TimeSpan.FromDays(7);

        /// <summary>
        /// Create a token for the user synchronously.
        /// </summary>
        /// <typeparam name="TUserModel"></typeparam>
        /// <param name="User"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public virtual TTokenModel Create<TUserModel>(TUserModel User, Action<TTokenModel> Callback = null)
            where TUserModel : LuxUserModel
        {
            return Create(() =>
            {
                var New = new TTokenModel();
                New.UserId = User.Id;

                Callback?.Invoke(New);
                return New;
            });
        }

        /// <summary>
        /// Create a token for the user asynchronously.
        /// </summary>
        /// <typeparam name="TUserModel"></typeparam>
        /// <param name="User"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public virtual async Task<TTokenModel> CreateAsync<TUserModel>(TUserModel User, Action<TTokenModel> Callback = null)
            where TUserModel : LuxUserModel
        {
            return await CreateAsync(() =>
            {
                var New = new TTokenModel();
                New.UserId = User.Id;

                Callback?.Invoke(New);
                return New;
            });
        }

        /// <summary>
        /// Configure the token properties when its saving first.
        /// </summary>
        /// <param name="Entity"></param>
        /// <param name="Next"></param>
        /// <returns></returns>
        protected override Task OnSaveRequest(TTokenModel Entity, Func<Task<bool>> Next)
        {
            if (Entity.IsNew)
            {
                Entity.CreationTime = DateTime.Now;
                Entity.RefreshId = Guid.NewGuid();
                Entity.RefreshExpiration = DefaultExpirationDue;
                Entity.ExpirationTime = DateTime.Now.Add(DefaultExpirationDue);
                Entity.LatestRefreshTime = DateTime.Now;
            }

            return base.OnSaveRequest(Entity, Next);
        }
    }
}
