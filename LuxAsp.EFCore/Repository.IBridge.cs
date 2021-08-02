using System;
using System.Threading.Tasks;

namespace LuxAsp
{
    public abstract partial class Repository
    {
        internal interface IBridge
        {
            /// <summary>
            /// Service Provider.
            /// </summary>
            IServiceProvider Services { get; }

            /// <summary>
            /// Get Database Instance.
            /// </summary>
            Database Database { get; }

            /// <summary>
            /// Test whether the model is changed or not.
            /// </summary>
            /// <param name="Model"></param>
            /// <returns></returns>
            bool IsChanged(DataModel Model);

            /// <summary>
            /// Save the model to database.
            /// </summary>
            /// <param name="Model"></param>
            Task<bool> SaveAsync(DataModel Model);

            /// <summary>
            /// Load the model from database.
            /// </summary>
            /// <param name="Model"></param>
            Task<bool> ReloadAsync(DataModel Model);

            /// <summary>
            /// Delete the model from database.
            /// </summary>
            /// <param name="Model"></param>
            /// <returns></returns>
            Task<bool> DeleteAsync(DataModel Model);
        }
    }
}
