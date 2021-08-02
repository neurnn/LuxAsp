using LuxAsp.Internals;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LuxAsp
{
    /// <summary>
    /// Base class of Entity Repository.
    /// </summary>
    public abstract partial class Repository
    {
        /// <summary>
        /// Prevent the default constructor from getting called by user implementations.
        /// </summary>
        internal Repository()
        {
        }

        /// <summary>
        /// Activate the Repository.
        /// </summary>
        /// <param name="Services"></param>
        internal abstract void Activate(IServiceProvider Services);

        /// <summary>
        /// Deactivate the Repository.
        /// </summary>
        internal abstract void Deactivate();
    }

    /// <summary>
    /// Base class of Entity Repository.
    /// </summary>
    public abstract partial class Repository<TEntity> : Repository where TEntity : DataModel
    {
        private IServiceScope m_Scope;
        private DatabaseAccess m_Database;
        private Bridge m_Bridge;

        /// <summary>
        /// Initialize the Repository.
        /// </summary>
        public Repository() : base() 
            => m_Bridge = new Bridge(this);

        /// <summary>
        /// Schema Qualified Table Name.
        /// </summary>
        public string QualifiedTableName { get; private set; }

        /// <summary>
        /// Application Services.
        /// </summary>
        public IServiceProvider Services
        {
            get
            {
                lock (this)
                {
                    return (m_Scope ?? throw new RepositoryException("This repository didn't activated."))
                        .ServiceProvider;
                }
            }
        }

        /// <summary>
        /// DbSet instance.
        /// </summary>
        public DbSet<TEntity> DbSet
        {
            get
            {
                lock (this)
                {
                    if (m_Database is null)
                        throw new RepositoryException("This repository didn't activated.");

                    return m_Database.Instance.Set<TEntity>();
                }
            }
        }

        /// <summary>
        /// Activates the Repository.
        /// </summary>
        /// <param name="Services"></param>
        internal override void Activate(IServiceProvider Services)
        {
            lock (this)
            {
                m_Scope = Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
                m_Database = m_Scope.ServiceProvider.GetRequiredService<DatabaseAccess>();
                QualifiedTableName = m_Database.Instance.GetQualifiedTableName<TEntity>();
                OnBegin();
            }
        }

        /// <summary>
        /// Deactivate the Repository.
        /// </summary>
        internal override void Deactivate()
        {
            lock (this)
            {
                if (m_Scope != null)
                {
                    OnEnd();

                    m_Scope.Dispose();
                    m_Database = null;
                    m_Scope = null;
                }
            }
        }

        /// <summary>
        /// Called when repository instance is required.
        /// But, DO NOT use this for migrating datas.
        /// </summary>
        protected virtual void OnBegin() { }

        /// <summary>
        /// Called when repository instance isn't required anymore.
        /// But, DO NOT use this for migrating datas.
        /// </summary>
        protected virtual void OnEnd() { }

        /// <summary>
        /// Create a new Entity but not saved yet.
        /// It will be saved when calls entity's Save() method.
        /// </summary>
        /// <param name="Configure"></param>
        /// <returns></returns>
        public virtual TEntity Create(Func<TEntity> Configure)
        {
            var New = Configure();
            New.SetBridge(m_Bridge);
            return New;
        }

        /// <summary>
        /// Create a new Entity and save it immediately.
        /// </summary>
        /// <param name="Configure"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> CreateAsync(Func<TEntity> Configure)
        {
            var New = Configure();
            New.SetBridge(m_Bridge);

            if (await New.SaveAsync())
                return New;

            return null;
        }

        /// <summary>
        /// Query All Entities.
        /// </summary>
        /// <param name="Selector"></param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> Query() => new Queryable(DbSet, m_Bridge);

        /// <summary>
        /// Query Entities by Selector.
        /// </summary>
        /// <param name="Selector"></param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> Selector) => new Queryable(DbSet.Where(Selector), m_Bridge);

        /// <summary>
        /// Load Entities by Selector.
        /// </summary>
        /// <param name="Selector"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> Load(Func<IQueryable<TEntity>, IEnumerable<TEntity>> Selector) => Selector(new Queryable(DbSet, m_Bridge));

        /// <summary>
        /// Called when the model is created on the database.
        /// </summary>
        /// <returns></returns>
        protected virtual Task OnCreated(TEntity Entity) => Task.CompletedTask;

        /// <summary>
        /// Called when the model is deleted from the database.
        /// </summary>
        /// <returns></returns>
        protected virtual Task OnDeleted(TEntity Entity) => Task.CompletedTask;

        /// <summary>
        /// Called when the entity should be saved.
        /// Note: to cancel save, do not call Next() delegate.
        /// </summary>
        /// <param name="Next"></param>
        /// <returns></returns>
        protected virtual Task OnSaveRequest(TEntity Entity, Func<Task<bool>> Next) => Next();

        /// <summary>
        /// Called when the entity should be saved.
        /// Note: to cancel delete, do not call Next() delegate.
        /// </summary>
        /// <param name="Entity"></param>
        /// <param name="Next"></param>
        /// <returns></returns>
        protected virtual Task OnDeleteRequest(TEntity Entity, Func<Task<bool>> Next) => Next();

        /// <summary>
        /// Save Changes asynchronously.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SaveChangesAsync()
        {
            try { await m_Database.Instance.SaveChangesAsync(); }
            catch (DbUpdateException)
            {
                if (Debugger.IsAttached)
                    throw;

                return false;
            }

            return true;
        }

        /// <summary>
        /// Try to save an entity to the database.
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        private async Task<bool> SaveRequested(TEntity Model)
        {
            if (m_Database is null)
                return false;
            
            bool Result = false;
            await OnSaveRequest(Model, async () =>
            {
                await Model.InvokeSaveRequest(async () 
                    => Result = await HandleSaveRequest(Model));
                return Result;
            });

            return Result;
        }

        /// <summary>
        /// Handle Save request asynchronously.
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        private async Task<bool> HandleSaveRequest(TEntity Model)
        {
            EntityState State;
            var Database = m_Database.Instance;

            if (Model.IsNew)
                 State = DbSet.Add(Model).State;
            else State = Database.Entry(Model).State;

            switch (State)
            {
                case EntityState.Added:
                case EntityState.Modified:
                    break;

                case EntityState.Unchanged:
                    return true;

                default: return false;
            }

            if (await SaveChangesAsync())
            {
                switch (State)
                {
                    case EntityState.Added:
                        await OnCreated(Model);
                        await Model.NotifyCreated();
                        Model.IsNew = false;
                        break;

                    case EntityState.Modified:
                        Model.IsNew = false;
                        break;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Try to reload entity asynchronously.
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        private async Task<bool> ReloadRequested(TEntity Model)
        {
            if (m_Database is null || Model.IsNew)
                return false;

            var Entry = m_Database.Instance.Entry(Model);
            if (Entry != null)
            {
                try { await Entry.ReloadAsync(); }
                catch { return false; }

                if (Entry.State != EntityState.Deleted)
                    return true;

                await Model.NotifyDeleted();
                await OnDeleted(Model);
            }

            return false;
        }

        /// <summary>
        /// Try to delete the model asynchronously.
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        private async Task<bool> DeleteRequested(TEntity Model)
        {
            if (m_Database is null)
                return false;

            if (Model.IsNew)
                return true;

            bool Result = false;
            await OnDeleteRequest(Model, async () =>
            {
                await Model.InvokeDeleteRequest(async ()
                    => Result = await HandleDeleteRequest(Model));
                return Result;
            });

            return Result;
        }

        /// <summary>
        /// Handle Delete request asynchronously.
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        private async Task<bool> HandleDeleteRequest(TEntity Model)
        {
            try { DbSet.Remove(Model); }
            catch { return false; }

            if (await SaveChangesAsync())
            {
                await Model.NotifyDeleted();
                await OnDeleted(Model);
                return true;
            }

            return false;
        }
    }
}
