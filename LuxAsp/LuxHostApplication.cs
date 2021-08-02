using LuxAsp.Internals;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LuxAsp
{
    public abstract class LuxHostApplication
    {
        private ILuxHostBuilder m_Builder;

        /// <summary>
        /// Run the application asynchronously.
        /// </summary>
        /// <typeparam name="App"></typeparam>
        /// <param name="Arguments"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static Task RunAsync<App>(params string[] Arguments) where App : LuxHostApplication, new() => (new App()).RunAsync(Arguments);

        /// <summary>
        /// Run the application.
        /// </summary>
        /// <typeparam name="App"></typeparam>
        /// <param name="Arguments"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static void Run<App>(params string[] Arguments) where App : LuxHostApplication, new() => (new App()).Run(Arguments);

        /// <summary>
        /// Dry Run the application and returns ILuxHostBuilder.
        /// </summary>
        /// <typeparam name="App"></typeparam>
        /// <returns></returns>
        [DebuggerHidden]
        public static ILuxHostBuilder DryRun<App>(params string[] Arguments) where App : LuxHostApplication, new() => (new App()).InternalDryRun<App>(Arguments);

        /// <summary>
        /// Run the application.
        /// </summary>
        /// <param name="Arguments"></param>
        /// <returns></returns>
        [DebuggerHidden]
        internal void Run(string[] Arguments)
        {
            var Builder = CreateHostBuilder(Arguments)
                .UseLuxAspCore(X => Configure(m_Builder = X, Arguments))
                .Build();

            Builder.Run();
        }

        /// <summary>
        /// Run the application.
        /// </summary>
        /// <param name="Arguments"></param>
        /// <returns></returns>
        [DebuggerHidden]
        internal async Task RunAsync(string[] Arguments)
        {
            var Builder = CreateHostBuilder(Arguments)
                .UseLuxAspCore(X => Configure(m_Builder = X, Arguments))
                .Build();

            await Builder.RunAsync();
        }

        /// <summary>
        /// Execute the configure method for getting configurations.
        /// </summary>
        /// <returns></returns>
        internal LuxDryHostBuilder InternalDryRun<TApp>(string[] Arguments)
        {
            var Builder = new LuxDryHostBuilder(typeof(TApp).Assembly);
            Configure(m_Builder = Builder, Arguments);

            /* Invoke all host modules. */
            LuxHostModule.Invoke(Builder);
            return Builder;
        }

        /// <summary>
        /// Get Application Directory.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public DirectoryInfo AppDir(string Name = null, bool Create = true)
        {
            var Dir = new DirectoryInfo(Path.Combine(m_Builder.ApplicationDirectory, (Name ?? "").TrimStart('/', '\\')));
            if (Create && !Dir.Exists) Directory.CreateDirectory(Dir.FullName);
            return Dir;
        }

        /// <summary>
        /// Creates a new Host Builder.
        /// </summary>
        /// <returns></returns>
        protected virtual IHostBuilder CreateHostBuilder(string[] Arguments)
            => Host.CreateDefaultBuilder(Arguments);

        /// <summary>
        /// Configure the Application.
        /// </summary>
        /// <param name="Builder"></param>
        protected abstract void Configure(ILuxHostBuilder Builder, string[] Arguments);
    }
}
