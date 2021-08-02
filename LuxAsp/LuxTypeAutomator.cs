using System;
using System.Collections;
using System.Collections.Generic;

namespace LuxAsp
{
    /// <summary>
    /// Base classes for Automation Scripts.
    /// </summary>
    public abstract class LuxTypeAutomator
    {
        /// <summary>
        /// Automation Target's Base-Type.
        /// </summary>
        public abstract Type BaseType { get; }

        /// <summary>
        /// Invoke Automation Script.
        /// </summary>
        /// <param name="Builder"></param>
        /// <param name="Types"></param>
        public abstract void Invoke(ILuxHostBuilder Builder, IEnumerable<Type> Types);

        /// <summary>
        /// Called after no more types scanned.
        /// Note: Register functor is for registering a callback that performs finalization.<br />
        /// When executing callbacks, new application part registrations are ignored.
        /// </summary>
        public virtual void Configure(Action<int, Action<ILuxHostBuilder>> Register) { }
    }
}
