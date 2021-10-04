using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Sessions.Internals
{
    internal partial class LuxMemorySessionStore
    {
        /// <summary>
        /// Memory Session.
        /// </summary>
        internal class MemorySession : LuxSessionBase
        {
            public override string Id
            {
                get => Guid.ToString();
                protected set => Guid = new Guid(value);
            }

            /// <summary>
            /// Lockable.
            /// </summary>
            public override ILuxSessionLockable Lockable { get; } = new AcquireAndRelease();

            /// <summary>
            /// Creation Time.
            /// </summary>
            public virtual DateTime LastAccessTime { get; set; } = DateTime.Now;

            /// <summary>
            /// Session Guid.
            /// </summary>
            public Guid Guid { get; set; }

            /// <summary>
            /// Do nothing.
            /// </summary>
            /// <returns></returns>
            public override Task FlushAsync() => Task.CompletedTask;
        }

    }
}
