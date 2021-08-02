using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace LuxAsp
{

    public abstract partial class Repository<TEntity> where TEntity : DataModel
    {
        internal class Bridge : IBridge
        {
            private static readonly Task<bool> ALWAYS_FALSE = Task.FromResult(false);
            private Repository<TEntity> m_Repository;

            /// <summary>
            /// Initialize the new Repository Bridge.
            /// </summary>
            /// <param name="Repository"></param>
            public Bridge(Repository<TEntity> Repository) => m_Repository = Repository;

            /// <summary>
            /// Test whether the bridge is alive or not.
            /// </summary>
            public bool IsValid => m_Repository.m_Scope != null;

            /// <summary>
            /// Gets the Service Provider.
            /// </summary>
            public IServiceProvider Services => IsValid 
                ? m_Repository.m_Scope.ServiceProvider : null;

            /// <summary>
            /// Gets the Database Instance.
            /// </summary>
            public Database Database => IsValid ? m_Repository.m_Database.Instance : null;

            /// <summary>
            /// Test whether the model has changes or not.
            /// </summary>
            /// <param name="Model"></param>
            /// <returns></returns>
            public bool IsChanged(DataModel Model)
            {
                if (IsValid && Model is TEntity _Model)
                {
                    var Entry = m_Repository
                        .m_Database.Instance.Entry(_Model);

                    Entry.DetectChanges();
                    return Entry.State == EntityState.Modified;
                }

                return false;
            }

            /// <summary>
            /// Save the model to database.
            /// </summary>
            /// <param name="Model"></param>
            /// <param name="IsNew"></param>
            /// <returns></returns>
            public Task<bool> SaveAsync(DataModel Model)
            {
                if (IsValid && Model is TEntity _Model)
                    return m_Repository.SaveRequested(_Model);

                return ALWAYS_FALSE;
            }

            /// <summary>
            /// Reload the model from database.
            /// </summary>
            /// <param name="Model"></param>
            /// <returns></returns>
            public Task<bool> ReloadAsync(DataModel Model)
            {
                if (IsValid && Model is TEntity _Model)
                    return m_Repository.ReloadRequested(_Model);

                return ALWAYS_FALSE;
            }

            /// <summary>
            /// Delete the model from database.
            /// </summary>
            /// <param name="Model"></param>
            /// <returns></returns>
            public Task<bool> DeleteAsync(DataModel Model)
            {
                if (IsValid && Model is TEntity _Model)
                    return m_Repository.DeleteRequested(_Model);

                return ALWAYS_FALSE;
            }
        }
    }
}
