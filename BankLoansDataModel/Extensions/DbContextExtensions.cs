using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BankLoansDataModel.Extensions
{
    public static class DbContextExtensions
    {
        public static async Task ReloadEntityAsync<TEntity>(this DbContext context, TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
        {
            await context.Entry(entity).ReloadAsync(cancellationToken);
        }

        public static async Task ReloadAllEntitiesAsync<TEntity>(this DbContext dbcontext, IEnumerable<TEntity> entities) where TEntity : class
        {
            foreach (var e in entities)
            {
                await dbcontext.ReloadEntityAsync(e);
            }
        }
    }
}
