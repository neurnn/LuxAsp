using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace LuxAsp.Internals
{
    internal sealed class DatabaseExtension : IDbContextOptionsExtension
    {
        private DbContextOptionsExtensionInfo m_Info;

        public DatabaseExtension()
        {

        }

        public DbContextOptionsExtensionInfo Info => (m_Info ??= new MetaData(this));

        public List<Type> EntityTypes { get; } = new List<Type>();
        public List<Type> RepositoryTypes { get; } = new List<Type>();

        public void ApplyServices(IServiceCollection services) { }
        public void Validate(IDbContextOptions options) { }

        private class MetaData : DbContextOptionsExtensionInfo
        {
            public MetaData(IDbContextOptionsExtension extension) : base(extension)
            {
            }

            public override bool IsDatabaseProvider => false;

            public override string LogFragment => "";

            public override long GetServiceProviderHashCode() => 0;

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo) { }
        }
    }
}
