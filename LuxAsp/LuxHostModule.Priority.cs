namespace LuxAsp
{
    public abstract partial class LuxHostModule
    {
        public sealed class Priority
        {
            /// <summary>
            /// Priority Minimum Value.
            /// </summary>
            public const int Min = 0;

            /// <summary>
            /// Priority Maximum Value.
            /// </summary>
            public const int Max = 0x0fffff;

            /// <summary>
            /// Database Priority Value.
            /// </summary>
            public const int Database = 0x03ffff;

            /// <summary>
            /// Session Priority Value.
            /// </summary>
            public const int Session = 0x04ffff;

            /// <summary>
            /// Early Default Priority Value.
            /// </summary>
            public const int EarlyDefault = 0x5ffff;

            /// <summary>
            /// Static Content Priority Value.
            /// </summary>
            public const int Statics = 0x6ffff;

            /// <summary>
            /// Default Priority Value.
            /// </summary>
            public const int Default = 0x07ffff;

            /// <summary>
            /// Authentication Priority Value.
            /// </summary>
            public const int Authentication = 0x0cffff;

            /// <summary>
            /// Authorization Priority Value.
            /// </summary>
            public const int Authorization = 0x0dffff;

            /// <summary>
            /// Late Default Priority Value.
            /// </summary>
            public const int LateDefault = 0x0effff;

            internal const int Between_Default_Late = (Default + LateDefault) / 2;
            

            /// <summary>
            /// Make Priority that called after the priority.
            /// </summary>
            /// <param name="Priority"></param>
            /// <returns></returns>
            public static int After(int Priority, int N = 1) => Priority + N;

            /// <summary>
            /// Make Priority that called before the priority.
            /// </summary>
            /// <param name="Priority"></param>
            /// <returns></returns>
            public static int Before(int Priority, int N = 1) => Priority - N;

            /// <summary>
            /// Make Between Priority.
            /// </summary>
            /// <param name="PriorityA"></param>
            /// <param name="PriorityB"></param>
            /// <returns></returns>
            internal static int Between(int PriorityA, int PriorityB)
                => (PriorityA + PriorityB) / 2;
        }
    }
}
