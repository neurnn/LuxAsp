using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuxAsp.Internals
{
    public struct InvokeOnDispose
    {
        private struct Async : IAsyncDisposable
        {
            public Async(Func<Task> Callback)
                => this.Callback = Callback;

            /// <summary>
            /// Callback to invoke.
            /// </summary>
            private Func<Task> Callback;

            /// <summary>
            /// Invoke on dispose.
            /// </summary>
            /// <returns></returns>
            public async ValueTask DisposeAsync()
            {
                if (Callback != null)
                    await Callback();

                Callback = null;
            }
        }

        private struct Sync : IDisposable
        {
            public Sync(Action Callback)
                => this.Callback = Callback;

            /// <summary>
            /// Callback to invoke.
            /// </summary>
            private Action Callback;

            /// <summary>
            /// Invoke on dispose.
            /// </summary>
            public void Dispose()
            {
                Callback?.Invoke();
                Callback = null;
            }
        }

        /// <summary>
        /// Create a new Disposable that invokes the callback.
        /// </summary>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public static IDisposable Create(Action Callback)
            => new Sync(Callback);

        /// <summary>
        /// Create a new Disposable that invokes the callback.
        /// </summary>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public static IAsyncDisposable Create(Func<Task> Callback)
            => new Async(Callback);

    }
}
