using System;

namespace LuxAsp.Internals
{
    internal class RepositoryHolder<TEntity> : IDisposable, IRepositoryHolder
        where TEntity : class
    {
        public RepositoryHolder(Func<Repository> Ctor, IServiceProvider Services)
            => (Repository = Ctor()).Activate(Services);

        /// <summary>
        /// Get Repository.
        /// </summary>
        public Repository Repository { get; }

        /// <summary>
        /// Deactivate repository on dispose.
        /// </summary>
        public void Dispose() => Repository?.Deactivate();

    }
}
