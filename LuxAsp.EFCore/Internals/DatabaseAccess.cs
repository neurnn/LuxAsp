namespace LuxAsp.Internals
{
    /// <summary>
    /// This makes the instance shouldn't be disposed twice.
    /// </summary>
    internal sealed class DatabaseAccess
    {
        public DatabaseAccess(Database Instance)
            => this.Instance = Instance;

        /// <summary>
        /// Get Instance.
        /// </summary>
        public Database Instance { get; }
    }
}
