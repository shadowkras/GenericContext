using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GenericContext.Interfaces
{
    public interface IGenericRepository<TEntity> : IDisposable where TEntity : class
    {
        /// <summary>
        /// Insert a new entity to our repository.
        /// <para>Examples:</para>
        /// <para>_repository.Insert(newEntity);</para>
        /// </summary>
        /// <param name="entity">Entity instance to be saved to our repository.</param>
        void Insert(TEntity entity);

        /// <summary>
        /// Method to insert a list of entities to our repository.
        /// <para>Examples:</para>
        /// <para>_repository.Insert(entityList);</para>
        /// </summary>
        /// <param name="entities">List of entities to be saved to our repository.</param>
        void Insert(IEnumerable<TEntity> entities);

        /// <summary>
        /// Method to update a single entity.
        /// <para>Examples:</para>
        /// <para>_repository.Update(entity);</para>
        /// </summary>
        /// <param name="entity">Entity instance to be saved to our repository.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Method to update our repository using a list of entities.
        /// <para>Examples:</para>
        /// <para>_repository.Update(entityList);</para>
        /// </summary>
        /// <param name="entities">List of entities to be saved to our repository.</param>
        void Update(IEnumerable<TEntity> entities);

        /// <summary>
        /// Method to update specific properties of an entity.
        /// <para>Examples:</para>
        /// <para>_repository.Update(user, p => p.FirstName, p => p.LastName);</para>
        /// <para>_repository.Update(user, p => p.Password);</para>
        /// </summary>
        /// <param name="entity">Entity instance to be saved to our repository.</param>
        /// <param name="propriedades">Array of expressions with the properties that will be changed.</param>
        void Update(TEntity entity, params Expression<Func<TEntity, object>>[] propriedades);

        /// <summary>
        /// Delete an entity from our repository.
        /// <para>Examples:</para>
        /// <para>_repository.Delete(entity);</para>
        /// </summary>
        /// <param name="entity">Entity instance to be deleted to our repository.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Delete an entity from our repository.
        /// <para>Examples:</para>
        /// <para>_repository.Delete(entityList);</para>
        /// </summary>
        /// <param name="entities">List of entities to be deleted to our repository.</param>
        void Delete(IEnumerable<TEntity> entities);

        /// <summary>
        /// Delete an entity from our repository.
        /// <para>Examples:</para>
        /// <para>_repository.Delete(p=> p.UserId == userId);</para>
        /// </summary>
        /// <param name="predicate">Filter applied to our search.</param>
        void Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Select an entity using it's primary keys as search criteria.
        /// <para>Examples:</para>
        /// <para>_repository.SelectByKey(userId);</para>
        /// <para>_repository.SelectByKey(companyId, userId);</para>
        /// </summary>
        /// <param name="primaryKeys">Primary key properties of our entity.</param>
        /// <returns>Returns an entity from our repository.</returns>
        TEntity SelectByKey(params object[] primaryKeys);

        /// <summary>
        /// Select all entities from our repository
        /// <para>Examples:</para>
        /// <para>_repository.SelectAll();</para>
        /// </summary>
        /// <returns>Returns all entities from our repository.</returns>
        IList<TEntity> SelectAll();

        /// <summary>
        /// Select entities using pagination (take N).
        /// <para>Examples:</para>
        /// <para>_repository.SelectAllByPage(1, 20);</para>
        /// <para>_repository.SelectAllByPage(pageNumber, quantityPerPage);</para>
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="quantity">Number of entities to select per page.</param>
        /// <returns>Returns entities from our repository.</returns>
        IList<TEntity> SelectAllByPage(int pageNumber, int quantity);

        /// <summary>
        /// Select an entity from our repository using a filter.
        /// <para>Examples:</para>
        /// <para>_repository.Select(p=> p.UserId == 1);</para>
        /// <para>_repository.Select(p=> p.UserName.Contains("John") == true);</para>
        /// </summary>
        /// <param name="predicate">Filter applied to our search.</param>
        /// <returns>Returns an entity from our repository.</returns>
        TEntity Select(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Select specific properties of an entity from our repository.
        /// <para>Examples:</para>
        /// <para>_repository.Select(p=> p.UserId == userId, p=> p.LastName);</para>
        /// </summary>
        /// <typeparam name="TResult">Entity returned from our search.</typeparam>
        /// <param name="predicate">Filter applied to our search.</param>
        /// <param name="properties">Fields that will be selected and populated in our result.</param>
        /// <returns>Returns an entity from our repository.</returns>
        TResult Select<TResult>(Expression<Func<TEntity, bool>> predicate,
                                Expression<Func<TEntity, TResult>> properties);
    }
}