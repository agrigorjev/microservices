using Mandara.Extensions.Option;
using System.Collections.Generic;

namespace Mandara.Business.DataInterface
{
    /// <summary>
    /// A basic cache of entities of arbitrary type given by the type parameter T.  The cache only allows items to be
    /// added or updated at present.
    /// </summary>
    public interface IEntityCache<T, U>
    {
        /// <summary>
        /// From a collection of entities, add those that are not already in the cache and update those that are.
        /// </summary>
        /// <param name="entities"></param>
        void AddOrUpdateEntities(IEnumerable<T> entities);
        void AddOrUpdateEntity(T entity);

        TryGetResult<T> TryGetById(U identifier);
    }
}