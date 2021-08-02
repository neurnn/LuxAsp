using System;
using System.Collections.Generic;

namespace LuxAsp.Sessions
{
    /// <summary>
    /// Session interface.
    /// </summary>
    public interface ILuxSession
    {
        /// <summary>
        /// Session ID.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Lockable. (return null If locking not supported)
        /// </summary>
        ILuxSessionLockable Lockable { get; }

        /// <summary>
        /// Get all keys which is used for this session.
        /// </summary>
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// Get Session Value by its Key.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        byte[] Get(string Key, Func<byte[]> Default = null);

        /// <summary>
        /// Set Session Value by its key.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        ILuxSession Set(string Key, byte[] Value);

        /// <summary>
        /// Unset the Session Value by its key.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        bool Unset(string Key);

        /// <summary>
        /// Destroy this session when the session commited.
        /// </summary>
        /// <returns></returns>
        bool Abandon();
    }
}
