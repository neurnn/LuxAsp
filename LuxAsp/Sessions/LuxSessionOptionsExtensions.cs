using LuxAsp.Sessions.Internals;
using System.IO;

namespace LuxAsp.Sessions
{
    public static class LuxSessionOptionsExtensions
    {
        /// <summary>
        /// Use File System for storing Sessions. (Default)
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Directory"></param>
        /// <returns></returns>
        public static LuxSessionOptions UseFileStore(this LuxSessionOptions This, DirectoryInfo Directory)
        {
            This.Properties["LUX_FSSESS_DIR"] = Directory;
            This.Use<LuxFileSessionStore>();
            return This;
        }

        /// <summary>
        /// Use Memory for storing Sessions.
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static LuxSessionOptions UseMemoryStore(this LuxSessionOptions This)
        {
            This.Use<LuxMemorySessionStore>();
            return This;
        }
    }
}
