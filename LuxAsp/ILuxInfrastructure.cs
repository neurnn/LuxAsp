namespace LuxAsp
{
    public interface ILuxInfrastructure<TInstance>
    {
        /// <summary>
        /// Gets the Infrastructure Instance.
        /// </summary>
        TInstance Instance { get; }
    }
}
