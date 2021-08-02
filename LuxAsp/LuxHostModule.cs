using LuxAsp.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LuxAsp
{
    /// <summary>
    /// Base class for Lux Host Modules.
    /// </summary>
    public abstract partial class LuxHostModule
    {
        private static readonly object[] EMPTY_ARGS = new object[0];

        /// <summary>
        /// Invoke Configure Method internally.
        /// </summary>
        /// <param name="Builder"></param>
        internal void InvokeConfigure(ILuxHostBuilder Builder) => Configure(Builder);

        /// <summary>
        /// Called when this module installed for configuring the Lux Host.
        /// </summary>
        /// <param name="Builder"></param>
        protected abstract void Configure(ILuxHostBuilder Builder);

        /// <summary>
        /// Create Host Modules that configured at builder.
        /// </summary>
        /// <param name="Builder"></param>
        /// <returns></returns>
        internal static void Invoke(ILuxHostBuilder Builder)
        {
            var Props = Builder.Properties;

            var HandledAssemblies = new List<Assembly>();
            var HandledTypes = new List<Type>();

            var Automations = new List<LuxTypeAutomator>();
            var Assemblies = new Assembly[0];

            while (true)
            {
                if (Builder is LuxHostBuilder DefaultImpl)
                    Invoke(HandledAssemblies, DefaultImpl);

                if (Builder is LuxDryHostBuilder DummyImpl)
                    InvokeForDryRun(HandledAssemblies, DummyImpl);

                var Delta = HandledAssemblies.Where(X => !Assemblies.Contains(X)).ToArray();
                Assemblies = HandledAssemblies.ToArray();

                if (Delta.Length <= 0)
                    break;

                InvokeAutomationTypes(Builder, HandledTypes, Automations, Delta);
            }

            var Finalizations = new List<(int, Action<ILuxHostBuilder>)>();
            foreach (var Each in Automations)
                Each.Configure((Priority, Callback) => Finalizations.Add((Priority, Callback)));

            Automations.Clear();
            foreach (var Each in Finalizations.OrderBy(X => X.Item1).Select(X => X.Item2))
                Each?.Invoke(Builder);
        }

        /// <summary>
        /// Invoke Automation Types.
        /// </summary>
        /// <param name="Builder"></param>
        /// <param name="HandledTypes"></param>
        /// <param name="Automations"></param>
        /// <param name="Delta"></param>
        private static void InvokeAutomationTypes(ILuxHostBuilder Builder, List<Type> HandledTypes, List<LuxTypeAutomator> Automations, Assembly[] Delta)
        {
            foreach (var Each in Delta)
            {
                var Types = Each.GetTypes() as IEnumerable<Type>;
                var TargetTypes = Types
                    .Where(X => !X.IsAbstract && X.IsSubclassOf(typeof(LuxTypeAutomator)))
                    .ToArray();

                Types = Types.Where(X => !TargetTypes.Contains(X));
                foreach (var Auto in Automations) /* Invoke previously used automations. */
                    Auto.Invoke(Builder, Types.Where(X => X.IsSubclassOf(Auto.BaseType)));

                HandledTypes.AddRange(Types);
                foreach (var EachType in TargetTypes)
                {
                    var Auto = EachType
                        .GetConstructor(Type.EmptyTypes)
                        .Invoke(EMPTY_ARGS) as LuxTypeAutomator;

                    Automations.Add(Auto);

                    /* Then, Invoke new automations. */
                    Auto.Invoke(Builder, HandledTypes.Where(X => X.IsSubclassOf(Auto.BaseType)));
                }
            }
        }

        /// <summary>
        /// Load Host Modules for real-run environment.
        /// </summary>
        /// <param name="HandledAssemblies"></param>
        /// <param name="Builder"></param>
        private static void Invoke(List<Assembly> HandledAssemblies, LuxHostBuilder Builder)
        {
            while (true)
            {
                var Assemblies = Builder.GetCurrentApplicationParts()
                    .Append(typeof(LuxHostBuilder).Assembly)
                    .Append(Assembly.GetEntryAssembly())
                    .Where(X => !HandledAssemblies.Contains(X))
                    .ToArray();

                if (Assemblies.Length <= 0)
                    break;

                HandledAssemblies.AddRange(Assemblies);
                Builder.CommitApplicationParts();

                InvokeForAssemblies(Builder, Assemblies);
            }
        }

        /// <summary>
        /// Load Host Modules for dry-run environment.
        /// </summary>
        /// <param name="HandledAssemblies"></param>
        /// <param name="Builder"></param>
        private static void InvokeForDryRun(List<Assembly> HandledAssemblies, LuxDryHostBuilder Builder)
        {
            while (true)
            {
                var Assemblies = Builder.GetApplicationParts()
                    .Append(typeof(LuxHostBuilder).Assembly)
                    .Where(X => !HandledAssemblies.Contains(X))
                    .ToArray();

                if (Assemblies.Length <= 0)
                    break;

                HandledAssemblies.AddRange(Assemblies);
                InvokeForAssemblies(Builder, Assemblies);
            }
        }

        /// <summary>
        /// Create Host Modules from Assembly.
        /// </summary>
        /// <returns></returns>
        private static void InvokeForAssemblies(ILuxHostBuilder Builder, params Assembly[] Assemblies)
        {
            var Types = Assemblies.SelectMany(X => X.GetTypes())
                .Where(X => X.IsSubclassOf(typeof(LuxHostModule)) && !X.IsAbstract)
                .Select(X =>
                {
                    var Attr = X.GetCustomAttribute<LuxHostModuleAttribute>();
                    if (Attr is null || Builder.IsModuleUse(X))
                        return (Type: X, Priority: int.MaxValue);

                    return (Type: X, Priority: Attr.Priority);
                })
                .Where(X => X.Type != null).OrderBy(X => X.Priority)
                .Select(X => X.Type);

            foreach (var Each in Types)
                Builder.UseModule(Each);
        }
    }
}
