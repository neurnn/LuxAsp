namespace LuxAsp.Internals
{
    internal interface IRepositoryHolder
    {
        /// <summary>
        /// Get Generic Repository.
        /// </summary>
        /// <returns></returns>
        Repository Repository { get; }
    }
}
