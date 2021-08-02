using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxAsp.Internals
{
    internal class DefaultRepository<TEntity> : Repository<TEntity> where TEntity : DataModel
    {
    }
}
