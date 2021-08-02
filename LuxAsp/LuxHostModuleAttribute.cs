using System;

namespace LuxAsp
{
    /// <summary>
    /// Specifies the Module should be loaded automatically by scanner.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class LuxHostModuleAttribute : Attribute
    {
        public LuxHostModuleAttribute(int Priority = 0) 
            => this.Priority = Priority;

        /// <summary>
        /// Priority of this module.
        /// </summary>
        public int Priority { get; }
    }
}
