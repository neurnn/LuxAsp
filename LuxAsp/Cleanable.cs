using System;
using System.Collections.Generic;

namespace LuxAsp
{
    public sealed class Cleanable<TObject> : IDisposable where TObject : class
    {
        private Queue<Action<TObject>> m_Callbacks = new Queue<Action<TObject>>();

        /// <summary>
        /// Wraps an instance.
        /// </summary>
        /// <param name="Instance"></param>
        public Cleanable(TObject Instance) => this.Instance = Instance;

        /// <summary>
        /// Instance.
        /// </summary>
        public TObject Instance { get; }

        /// <summary>
        /// Add a clean-up callback.
        /// </summary>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public Cleanable<TObject> Configure(Action<TObject> Callback)
        {
            lock (m_Callbacks)
                  m_Callbacks.Enqueue(Callback);

            return this;
        }

        /// <summary>
        /// Dispose the Instance.
        /// </summary>
        public void Dispose()
        {
            while(true)
            {
                Action<TObject> Callback;

                lock (m_Callbacks)
                {
                    if (m_Callbacks.Count <= 0)
                        break;

                    Callback = m_Callbacks.Dequeue();
                }

                Callback?.Invoke(Instance);
            }

            if (Instance is IDisposable _Last)
                _Last.Dispose();
        }
    }
}
