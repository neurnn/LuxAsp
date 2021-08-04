using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace LuxAsp
{
    public abstract class DataModel
    {
        private static readonly Task<bool> ALWAYS_FALSE = Task.FromResult(false);

        internal Task InvokeSaveRequest(Func<Task<bool>> Next) => OnSaveRequest(Next);
        internal Task InvokeDeleteRequest(Func<Task<bool>> Next) => OnDeleteRequest(Next);

        internal Task NotifyCreated() => OnCreated();
        internal Task NotifyLoaded() => OnLoaded();
        internal Task NotifyDeleted() => OnDeleted();


        [NotMapped][JsonIgnore]
        private Repository.IBridge m_Bridge;

        /// <summary>
        /// Put Repository Bridge.
        /// </summary>
        /// <param name="Services"></param>
        internal void SetBridge(Repository.IBridge Bridge) => m_Bridge = Bridge;

        /// <summary>
        /// Determines whether the model is new or not.
        /// </summary>
        [NotMapped][JsonIgnore]
        public bool IsNew { get; internal set; } = true;

        /// <summary>
        /// Test whether the model is valid or not.
        /// </summary>
        [NotMapped][JsonIgnore]
        public bool IsValid => m_Bridge != null;

        /// <summary>
        /// Test whether the model has changes or not.
        /// </summary>
        [NotMapped][JsonIgnore]
        public bool IsChanged => IsNew || (m_Bridge != null && m_Bridge.IsChanged(this));

        /// <summary>
        /// Gets the repository service if this model is valid.
        /// </summary>
        [NotMapped][JsonIgnore]
        public IServiceProvider Services => m_Bridge != null ? m_Bridge.Services : null;

        /// <summary>
        /// Gets the Database interface if this model is valid.
        /// </summary>
        [NotMapped][JsonIgnore]
        public Database Database => m_Bridge != null ? m_Bridge.Database : null;

        /// <summary>
        /// Save changes.
        /// </summary>
        /// <returns></returns>
        public bool Save() => SaveAsync().Result;

        /// <summary>
        /// Reload the model.
        /// </summary>
        /// <returns></returns>
        public bool Reload() => ReloadAsync().Result;

        /// <summary>
        /// Delete the model.
        /// </summary>
        /// <returns></returns>
        public bool Delete() => DeleteAsync().Result;

        /// <summary>
        /// Save asynchronously.
        /// </summary>
        /// <returns></returns>
        public Task<bool> SaveAsync()
        {
            if (m_Bridge is null) return ALWAYS_FALSE;
            return m_Bridge.SaveAsync(this);
        }

        /// <summary>
        /// Reload asynchronously.
        /// </summary>
        /// <returns></returns>
        public Task<bool> ReloadAsync()
        {
            if (m_Bridge is null) return ALWAYS_FALSE;
            return m_Bridge.ReloadAsync(this);
        }

        /// <summary>
        /// Delete asynchronously.
        /// </summary>
        /// <returns></returns>
        public Task<bool> DeleteAsync()
        {
            if (m_Bridge is null) return ALWAYS_FALSE;
            return m_Bridge.DeleteAsync(this);
        }

        /// <summary>
        /// Called when this model is created on the database.
        /// </summary>
        /// <returns></returns>
        protected virtual Task OnCreated() => Task.CompletedTask;

        /// <summary>
        /// Called when this model is loaded from the database.
        /// </summary>
        /// <returns></returns>
        protected virtual Task OnLoaded() => Task.CompletedTask;

        /// <summary>
        /// Called when this model is deleted from the database.
        /// </summary>
        /// <returns></returns>
        protected virtual Task OnDeleted() => Task.CompletedTask;

        /// <summary>
        /// Called when this model is saving to the database.
        /// Note: User can cancel the operation by skipping to call the next delegate.
        /// </summary>
        /// <param name="Next"></param>
        /// <returns></returns>
        protected virtual Task OnSaveRequest(Func<Task<bool>> Next) => Next();

        /// <summary>
        /// Called when this model is deleting from the database.
        /// Note: User can cancel the operation by skipping to call the next delegate.
        /// </summary>
        /// <param name="Next"></param>
        /// <returns></returns>
        protected virtual Task OnDeleteRequest(Func<Task<bool>> Next) => Next();

    }
}
