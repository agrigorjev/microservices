using EntityFramework.Extensions;
using EntityFramework.Future;
using System;
using System.Linq;
using System.Reflection;

namespace Mandara.Business.Data
{
    public static class MandaraFutureExtensions
    {
        public static FutureValue<TEntity> FutureFirstOrDefault<TEntity>(this IQueryable<TEntity> query)
            where TEntity : class
        {
            return (query.ToObjectQuery() == null)
                ? new FutureValue<TEntity>(query.FirstOrDefault())
                : FutureExtensions.FutureFirstOrDefault(query);
        }

        public static FutureQuery<TEntity> Future<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            if (query.ToObjectQuery() != null)
            {
                return FutureExtensions.Future(query);
            }

            object[] args = { query, null };

            return (FutureQuery<TEntity>)Activator.CreateInstance(
                typeof(FutureQuery<TEntity>),
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                args,
                null);
        }

        public static FutureCount FutureCount<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            return (query.ToObjectQuery() == null)
                ? new FutureCount(query.Count())
                : FutureExtensions.FutureCount(query);
        }
    }
}
