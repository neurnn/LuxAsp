using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Sessions.Internals
{

    internal class LuxEFCoreSessionStore<TModel, TRepository> : LuxSessionStoreBase
        where TRepository : LuxSessionRepository<TModel> where TModel : LuxSessionModel, new()
    {
        /// <summary>
        /// EFCore Session instance.
        /// </summary>
        internal class EFCoreSession : LuxSessionBase
        {
            public EFCoreSession(TModel Model) => this.Model = Model;

            /// <summary>
            /// Model Instance.
            /// </summary>
            public TModel Model { get; }

            /// <summary>
            /// Gets the Model Id.
            /// </summary>
            public override string Id
            {
                protected set => throw new NotSupportedException();
                get => Model.Id.ToString();
            }

            /// <summary>
            /// Note that, the Lockable is based on 
            /// </summary>
            public override ILuxSessionLockable Lockable { get; } = new AcquireAndRelease();

            /// <summary>
            /// Flush changes to database.
            /// </summary>
            /// <returns></returns>
            public override async Task FlushAsync() => await Model.SaveAsync();
        }

        /// <summary>
        /// Initialize a new Session Store.
        /// </summary>
        /// <param name="Options"></param>
        public LuxEFCoreSessionStore(IServiceProvider Services, LuxSessionOptions Options, ILuxSessionStoreWorker Worker)
            : base(Options, Worker) => this.Services = Services;

        public IServiceProvider Services { get; }

        /// <summary>
        /// Get Repository.
        /// </summary>
        /// <returns></returns>
        private TRepository GetRepository() => HttpContext.GetRequiredService<TRepository>();

        /// <summary>
        /// Create a new Session asynchronously.
        /// When the Session supports Lockable, this should return with its locked.
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        protected override async Task<ILuxSession> CreateAsync(TimeSpan Expiration, CancellationToken Token)
        {
            while (!Token.IsCancellationRequested)
            {
                var Model = await GetRepository().CreateAsync(Expiration);

                if (!await Model.SaveAsync()) 
                    continue;

                return new EFCoreSession(Model);
            }

            return null;
        }

        /// <summary>
        /// Get the Session asynchronously.
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        protected override async Task<ILuxSession> GetAsync(string Id, TimeSpan Expiration, CancellationToken Token)
        {
            if (Guid.TryParse(Id, out var SessionId))
            {
                var Model = await GetRepository().GetAsync(SessionId);
                if (Model != null)
                {
                    if (TryRestoreSession(Model, out var Session))
                        return Session;

                    await Model.DeleteAsync();
                }
            }

            return null;
        }

        /// <summary>
        /// Flush the Session asynchronously.
        /// </summary>
        /// <param name="Session"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        protected override async Task FlushAsync(ILuxSession Session, CancellationToken Token)
        {
            if (Session is EFCoreSession _EFSession)
            {
                if (!TryStoreSession(_EFSession.Model, _EFSession) ||
                    !await _EFSession.Model.SaveAsync())
                     await _EFSession.Model.DeleteAsync();
            }

            await base.FlushAsync(Session, Token);
        }

        /// <summary>
        /// Delete the Session asynchronously.
        /// </summary>
        /// <param name="Session"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        protected override async Task DeleteAsync(ILuxSession Session, CancellationToken Token)
        {
            if (Session is EFCoreSession _EFSession)
                await _EFSession.Model.DeleteAsync();
        }

        /// <summary>
        /// Collect garbage sessions
        /// </summary>
        /// <param name="Expiration"></param>
        /// <returns></returns>
        protected override async ValueTask CollectAsync(TimeSpan Expiration)
        {
            using var Scope = Services.CreateScope();
            var Repository = Scope.ServiceProvider.GetRequiredService<TRepository>();

            await Repository.ExpireAsync();
            await base.CollectAsync(Expiration);
        }

        /// <summary>
        /// Try to restore the Session.
        /// </summary>
        /// <returns></returns>
        private bool TryRestoreSession(TModel Model, out EFCoreSession Session)
        {
            using (var Reader = new BinaryReader(new MemoryStream(Model.Bytes), Encoding.UTF8, false))
            {
                if (!Deserialize(Reader, Session = new EFCoreSession(Model)))
                {
                    Session = null;
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Try to Store the Idle Session to repository.
        /// </summary>
        /// <returns></returns>
        private bool TryStoreSession(TModel Model, EFCoreSession Session)
        {
            using var Stream = new MemoryStream();
            using (var Writer = new BinaryWriter(Stream, Encoding.UTF8, true))
            {
                try
                {
                    Writer.Write((ushort)1);
                    Writer.Write(Session.Keys.Count());

                    foreach (var Each in Session.Keys)
                    {
                        var Bytes = Session.Get(Each);

                        Writer.Write(Bytes != null ? Bytes.Length : 0);
                        Writer.Write(Each);

                        if (Bytes.Length <= 0)
                            continue;

                        Writer.Write(Bytes);
                    }
                }

                catch
                {
                    Model.Bytes = null;
                    return false;
                }
            }

            Model.Bytes = Stream.ToArray();
            return true;
        }

        /// <summary>
        /// Deserialize the Session from Binary Reader.
        /// </summary>
        /// <param name="Reader"></param>
        /// <param name="Where"></param>
        /// <returns></returns>
        private bool Deserialize(BinaryReader Reader, EFCoreSession Where)
        {
            try
            {
                var Version = Reader.ReadInt16();
                if (Version != 1)
                    return false;

                var Count = Reader.ReadInt32();
                while (Count-- > 0)
                {
                    var Length = Reader.ReadInt32();
                    var Name = Reader.ReadString();

                    if (Length <= 0)
                        continue;

                    Where.Set(Name, Reader.ReadBytes(Length));
                }

                return true;
            }

            catch { }

            Where.Abandon();
            return false;
        }
    }
}
