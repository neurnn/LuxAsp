using System;
using System.Linq;
using System.Threading.Tasks;

namespace LuxAsp.Authorizations
{
    /// <summary>
    /// Basic User Repository.
    /// </summary>
    public class LuxUserRepository : LuxUserRepository<LuxUserModel>
    {

    }

    /// <summary>
    /// User Repository.
    /// </summary>
    /// <typeparam name="TUserModel"></typeparam>
    public class LuxUserRepository<TUserModel> : Repository<TUserModel> where TUserModel : LuxUserModel, new()
    {
        /// <summary>
        /// Create an user using its LoginName, Password and Name.
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="Password"></param>
        /// <param name="Name"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public virtual TUserModel Create(string LoginName,
            string Password, string Name, Action<TUserModel> Callback = null)
        {
            var Count = Query(X => X.LoginName == LoginName).Count();
            if (Count > 0) throw new ArgumentException(
                $"Login Name, `{LoginName}` has already taken for other user.");

            return Create(() =>
            {
                var New = new TUserModel();

                New.LoginName = LoginName;
                New.Name = Name;

                New.SetPassword(Password);
                return New;
            });
        }

        /// <summary>
        /// Create an user using its LoginName, Password and Name.
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="Password"></param>
        /// <param name="Name"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public virtual async Task<TUserModel> CreateAsync(string LoginName, 
            string Password, string Name, Action<TUserModel> Callback = null)
        {
            var Count = Query(X => X.LoginName == LoginName).Count();
            if (Count > 0) throw new ArgumentException(
                $"Login Name, `{LoginName}` has already taken for other user.");

            return await CreateAsync(() =>
            {
                var New = new TUserModel();

                New.LoginName = LoginName;
                New.Name = Name;

                New.SetPassword(Password);
                return New;
            });
        }

        /// <summary>
        /// Sets the CreationTime and LastWriteTime property before saving changes.
        /// </summary>
        /// <param name="Entity"></param>
        /// <param name="Next"></param>
        /// <returns></returns>
        protected override Task OnSaveRequest(TUserModel Entity, Func<Task<bool>> Next)
        {
            if (Entity.IsNew)
                Entity.CreationTime = DateTime.Now;

            if (Entity.IsNew || Entity.IsChanged)
                Entity.LastWriteTime = DateTime.Now;

            return base.OnSaveRequest(Entity, Next);
        }
    }
}
