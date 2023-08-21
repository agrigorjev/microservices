using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;

namespace Mandara.Entities.Extensions
{
    public enum EntityUpdateMethod
    {
        Update,
        Recreate
    }

    /// <summary>
    /// Extensions helps entities saving
    /// </summary>
    public static class EntitiesExtensions
    {
        public static MandaraEntities Save<TEntity>(this MandaraEntities cxt, DbSet<TEntity> dbSet, TEntity entity, Func<TEntity, int> entityIdFunc, out Func<int> newIdFunc)
            where TEntity : class
        {
            TEntity existEntity = dbSet.Find(entityIdFunc(entity));
            if (existEntity != null)
            {
                cxt.Entry(existEntity).CurrentValues.SetValues(entity);
                newIdFunc = () => entityIdFunc(existEntity);
            }
            else
            {
                // We don't use the dbSet.Add method because this method adds the detached entities in the references.
                TEntity newEntity = dbSet.Create<TEntity>();
                dbSet.Add(newEntity);
                cxt.Entry(newEntity).CurrentValues.SetValues(entity);
                newIdFunc = () => entityIdFunc(newEntity);
            }
            return cxt;
        }

        public static MandaraEntities Save<TEntity>(this MandaraEntities cxt, DbSet<TEntity> dbSet, ICollection<TEntity> entities, Func<TEntity, int> entityIdFunc, Expression<Func<TEntity, bool>> list_predicate)
            where TEntity : class
        {
            List<TEntity> deletedEntities;
            return Save(cxt, dbSet, entities, entityIdFunc, list_predicate, true, out deletedEntities);
        }

        public static MandaraEntities Save<TEntity>(this MandaraEntities cxt, DbSet<TEntity> dbSet, ICollection<TEntity> entities, Func<TEntity, TEntity, bool> entityIDComparer, Expression<Func<TEntity, bool>> list_predicate)
            where TEntity : class
        {
            List<TEntity> deletedEntities;
            return Save(cxt, dbSet, entities, entityIDComparer, list_predicate, true, out deletedEntities);
        }

        public static MandaraEntities Save<TEntity>(this MandaraEntities cxt, DbSet<TEntity> dbSet, ICollection<TEntity> entities, Func<TEntity, int> entityIdFunc, Expression<Func<TEntity, bool>> list_predicate, EntityUpdateMethod updateMethod)
          where TEntity : class
        {
            List<TEntity> deletedEntities;
            return Save(cxt, dbSet, entities, entityIdFunc, list_predicate, true, updateMethod, out deletedEntities);
        }

        public static MandaraEntities Save<TEntity>(this MandaraEntities cxt, DbSet<TEntity> dbSet, ICollection<TEntity> entities, Func<TEntity, TEntity, bool> entityIDComparer, Expression<Func<TEntity, bool>> list_predicate, EntityUpdateMethod updateMethod)
            where TEntity : class
        {
            List<TEntity> deletedEntities;
            return Save(cxt, dbSet, entities, entityIDComparer, list_predicate, true, updateMethod, out deletedEntities);
        }

        /// <summary>
        /// Saves the Collection of entities with removing of deleted entities
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="cxt"></param>
        /// <param name="dbSet"></param>
        /// <param name="entities"></param>
        /// <param name="entityIdFunc"></param>
        /// <param name="list_predicate"></param>
        /// <param name="remove_deleted"></param>
        /// <param name="deletedEntities"></param>
        /// <returns></returns>
        public static MandaraEntities Save<TEntity>(this MandaraEntities cxt, DbSet<TEntity> dbSet, ICollection<TEntity> entities, Func<TEntity, int> entityIdFunc, Expression<Func<TEntity, bool>> list_predicate, bool remove_deleted, out List<TEntity> deletedEntities)
            where TEntity : class
        {
            return Save(cxt, dbSet, entities, entityIdFunc, list_predicate, remove_deleted, EntityUpdateMethod.Update, out deletedEntities);
        }


        /// <summary>
        /// Saves the Collection of entities with removing of deleted entities
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="cxt"></param>
        /// <param name="dbSet"></param>
        /// <param name="entities"></param>
        /// <param name="entityIdFunc"></param>
        /// <param name="list_predicate"></param>
        /// <param name="remove_deleted"></param>
        /// <param name="updateMethod"></param>
        /// <param name="deletedEntities"></param>
        /// <returns></returns>
        public static MandaraEntities Save<TEntity>(this MandaraEntities cxt, DbSet<TEntity> dbSet, ICollection<TEntity> entities, Func<TEntity, int> entityIdFunc, Expression<Func<TEntity, bool>> list_predicate, bool remove_deleted, EntityUpdateMethod updateMethod, out List<TEntity> deletedEntities)
            where TEntity : class
        {
            List<TEntity> entitiesList = new List<TEntity>(entities);

            // get the original set
            List<TEntity> existList = dbSet.Where(list_predicate).ToList();

            foreach (TEntity entity in entitiesList)
            {
                TEntity existEntity = existList.FirstOrDefault(e => entityIdFunc(e) == entityIdFunc(entity));
                if (existEntity != null)
                {
                    if (updateMethod == EntityUpdateMethod.Update)
                    {
                        cxt.Entry(existEntity).CurrentValues.SetValues(entity);
                    }
                    else if (updateMethod == EntityUpdateMethod.Recreate)
                    {
                        dbSet.Remove(existEntity);
                        TEntity newEntity = dbSet.Create<TEntity>();
                        dbSet.Add(newEntity);
                        cxt.Entry(newEntity).CurrentValues.SetValues(entity);
                    }
                }
                else
                {
                    // We don't use the dbSet.Add method because this method adds the detached entities in the references.
                    TEntity newEntity = dbSet.Create<TEntity>();
                    dbSet.Add(newEntity);
                    cxt.Entry(newEntity).CurrentValues.SetValues(entity);
                }
            }

            deletedEntities = existList.Where(e => entitiesList.All(s => entityIdFunc(s) != entityIdFunc(e))).ToList();

            if (remove_deleted && deletedEntities.Count > 0)
                dbSet.RemoveRange(deletedEntities);

            return cxt;
        }

        public static MandaraEntities SaveMany<TEntity>(this MandaraEntities cxt, DbSet<TEntity> dbSet, ICollection<TEntity> entities, Func<TEntity, int> entityIdFunc, Expression<Func<TEntity, bool>> list_predicate, bool remove_deleted, EntityUpdateMethod updateMethod, out List<TEntity> deletedEntities)
      where TEntity : class
        {
            List<TEntity> entitiesList = new List<TEntity>(entities);

            // get the original set
            List<TEntity> existList = dbSet.Where(list_predicate).ToList();

            foreach (TEntity entity in entitiesList)
            {
                TEntity existEntity = existList.FirstOrDefault(e => entityIdFunc(e) == entityIdFunc(entity));
                if (existEntity != null)
                {
                    if (updateMethod == EntityUpdateMethod.Update)
                    {
                        cxt.Entry(existEntity).CurrentValues.SetValues(entity);
                    }
                    else if (updateMethod == EntityUpdateMethod.Recreate)
                    {
                        dbSet.Remove(existEntity);
                        TEntity newEntity = dbSet.Create<TEntity>();
                        dbSet.Add(newEntity);
                        cxt.Entry(newEntity).CurrentValues.SetValues(entity);
                    }
                }
                else
                {
                    // We don't use the dbSet.Add method because this method adds the detached entities in the references.
                    TEntity newEntity = dbSet.Create<TEntity>();
                    dbSet.Add(newEntity);
                    cxt.Entry(newEntity).CurrentValues.SetValues(entity);
                }
            }

            deletedEntities = existList.Where(e => entitiesList.All(s => entityIdFunc(s) != entityIdFunc(e))).ToList();

            if (remove_deleted && deletedEntities.Count > 0)
                dbSet.RemoveRange(deletedEntities);

            return cxt;
        }


        public static MandaraEntities Save<TEntity>(this MandaraEntities cxt, DbSet<TEntity> dbSet, ICollection<TEntity> entities, Func<TEntity, TEntity, bool> entityIDComparer, Expression<Func<TEntity, bool>> list_predicate, bool remove_deleted, out List<TEntity> deletedEntities)
            where TEntity : class
        {
            return Save(cxt, dbSet, entities, entityIDComparer, list_predicate, remove_deleted, EntityUpdateMethod.Update, out deletedEntities);
        }

        /// <summary>
        /// Saves the Collection of entities with removing of deleted entities
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="cxt"></param>
        /// <param name="dbSet"></param>
        /// <param name="entities"></param>
        /// <param name="entityIDComparer"></param>
        /// <param name="list_predicate"></param>
        /// <param name="remove_deleted"></param>
        /// <param name="updateMethod"></param>
        /// <param name="deletedEntities"></param>
        /// <returns></returns>
        public static MandaraEntities Save<TEntity>(this MandaraEntities cxt, DbSet<TEntity> dbSet, ICollection<TEntity> entities, Func<TEntity, TEntity, bool> entityIDComparer, Expression<Func<TEntity, bool>> list_predicate, bool remove_deleted, EntityUpdateMethod updateMethod, out List<TEntity> deletedEntities)
            where TEntity : class
        {

            List<TEntity> entitiesList = new List<TEntity>(entities);

            // get the original set
            List<TEntity> existList = dbSet.Where(list_predicate).ToList();

            foreach (TEntity entity in entitiesList)
            {
                TEntity existEntity = existList.FirstOrDefault(e => entityIDComparer(entity, e));
                if (existEntity != null)
                {
                    if (updateMethod == EntityUpdateMethod.Update)
                    {
                        cxt.Entry(existEntity).CurrentValues.SetValues(entity);
                    }
                    else if (updateMethod == EntityUpdateMethod.Recreate)
                    {
                        dbSet.Remove(existEntity);
                        TEntity newEntity = dbSet.Create<TEntity>();
                        dbSet.Add(newEntity);
                        cxt.Entry(newEntity).CurrentValues.SetValues(entity);
                    }
                }
                else
                {
                    // We don't use the dbSet.Add method because this method adds the detached entities in the references.
                    TEntity newEntity = dbSet.Create<TEntity>();
                    dbSet.Add(newEntity);
                    cxt.Entry(newEntity).CurrentValues.SetValues(entity);
                }
            }

            deletedEntities = existList.Where(e => entitiesList.All(s => !entityIDComparer(s, e))).ToList();

            if (remove_deleted && deletedEntities.Count > 0)
                dbSet.RemoveRange(deletedEntities);

            return cxt;
        }

   
        public static void Remove<TEntity>(this DbSet<TEntity> dbSet, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            dbSet.RemoveRange(dbSet.Where(predicate));
        }
    }
}
