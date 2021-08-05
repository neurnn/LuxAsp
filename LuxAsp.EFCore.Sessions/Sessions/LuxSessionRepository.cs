using System;
using System.Linq;
using System.Threading.Tasks;

namespace LuxAsp.Sessions
{
    public class LuxSessionRepository : LuxSessionRepository<LuxSessionModel>
    {
    }

    /// <summary>
    /// Session Repository.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class LuxSessionRepository<TModel> : Repository<TModel> where TModel : LuxSessionModel, new()
    {
        /// <summary>
        /// Create a new Session using the Expiration's Age.
        /// </summary>
        /// <param name="Expiration"></param>
        /// <returns></returns>
        public async Task<TModel> CreateAsync(TimeSpan Expiration)
        {
            return await CreateAsync(() => new TModel
            {
                 ExpirationExtends = Expiration,
                 Bytes = new byte[0]
            });
        }

        /// <summary>
        /// Get Session by Id asynchronously.
        /// When no alive session found, this will return null.
        /// And this calls the Expiration callback if the session expired.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<TModel> GetAsync(Guid Id)
        {
            var Session = (await LoadAsync(Query(X => X.Id == Id))).FirstOrDefault();
            if (Session != null && Session.ExpirationTime <= DateTime.Now)
            {
                await Session.DeleteAsync();
                return null;
            }

            return Session;
        }

        /// <summary>
        /// Handle the expiration asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task ExpireAsync()
        {
            while (true)
            {
                var BaseTime = DateTime.Now; /* Perform Expiration in one thousand unit. */
                var Sessions = await LoadAsync(Query(X => X.ExpirationTime <= BaseTime).Take(1000));
                var Counter = 0;

                foreach (var Each in Sessions)
                {
                    if (await Each.DeleteAsync())
                        ++Counter;
                }

                if (Counter > 0)
                {
                    await Task.Delay(10);
                    continue;
                }

                break;
            }
        }

        /// <summary>
        /// Called when the Session is saving.
        /// </summary>
        /// <param name="Entity"></param>
        /// <param name="Next"></param>
        /// <returns></returns>
        protected override Task OnSaveRequest(TModel Entity, Func<Task<bool>> Next)
        {
            if (Entity.IsNew)
                Entity.CreationTime = DateTime.Now;

            if (Entity.IsNew || Entity.IsChanged)
            {
                Entity.LastWriteTime = DateTime.Now;
                Entity.ExpirationTime = Entity.LastWriteTime
                    .Add(Entity.ExpirationExtends);
            }

            return base.OnSaveRequest(Entity, Next);
        }
    }
}
