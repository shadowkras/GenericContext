using GenericContext.Context;
using GenericContext.Extensions;
using GenericContext.Interfaces;
using GenericContext.Resources;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GenericContext.Repository
{
    /// <summary>
    /// Generic repository class with standard CRUD methods.
    /// </summary>
    /// <typeparam name="TEntity">Database entity.</typeparam>
    /// <typeparam name="TContext">Database context. Class must inherit GenericContext.</typeparam>
    public abstract class GenericRepository<TEntity, TContext> : IGenericRepository<TEntity>, IDisposable where TEntity : class
    {
        #region Fields

        protected readonly GenericContext<TContext> _dbContext;
        public DbSet<TEntity> DbSet;

        #endregion

        #region Construtor

        /// <summary>
        /// Generic construtor based on a specific DbContext.
        /// </summary>
        /// <param name="dbContext"></param>
        public GenericRepository(GenericContext<TContext> dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("dbContext");
            _dbContext.SetTrackingBehavior();

            try
            {
                DbSet = dbContext.Set<TEntity>();
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception(string.Format(Errors.Possible_0, $"map the entity: {this.GetType().Name}"), ex);
            }
        }

        #endregion

        #region Insert

        /// <summary>
        /// Insert a new entity to our repository.
        /// <para>Examples:</para>
        /// <para>_repository.Insert(newEntity);</para>
        /// </summary>
        /// <param name="entity">Entity instance to be saved to our repository.</param>
        public virtual void Insert(TEntity entity)
        {
            DbSet.Add(entity);
        }

        /// <summary>
        /// Method to insert a list of entities to our repository.
        /// <para>Examples:</para>
        /// <para>_repository.Insert(entityList);</para>
        /// </summary>
        /// <param name="entities">List of entities to be saved to our repository.</param>
        public virtual void Insert(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
        }

        #endregion

        #region Update

        /// <summary>
        /// Method to update a single entity.
        /// <para>Examples:</para>
        /// <para>_repository.Update(entity);</para>
        /// </summary>
        /// <param name="entity">Entity instance to be saved to our repository.</param>
        public virtual void Update(TEntity entity)
        {
            DbSet.Update(entity);
        }

        /// <summary>
        /// Method to update our repository using a list of entities.
        /// <para>Examples:</para>
        /// <para>_repository.Update(entityList);</para>
        /// </summary>
        /// <param name="entities">List of entities to be saved to our repository.</param>
        public virtual void Update(IEnumerable<TEntity> entities)
        {
            DbSet.UpdateRange(entities);
        }

        /// <summary>
        /// Method to update specific properties of an entity.
        /// <para>Examples:</para>
        /// <para>_repository.Update(user, p => p.FirstName, p => p.LastName);</para>
        /// <para>_repository.Update(user, p => p.Password);</para>
        /// </summary>
        /// <param name="entity">Entity instance to be saved to our repository.</param>
        /// <param name="propriedades">Array of expressions with the properties that will be changed.</param>
        public void Update(TEntity entity, params Expression<Func<TEntity, object>>[] propriedades)
        {
            _dbContext.Attach(entity);

            foreach (var item in propriedades.AsParallel())
            {
                _dbContext.Entry(entity).Property(item).IsModified = true;
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete an entity from our repository.
        /// <para>Examples:</para>
        /// <para>_repository.Delete(entity);</para>
        /// </summary>
        /// <param name="entity">Entity instance to be deleted to our repository.</param>
        public virtual void Delete(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        /// <summary>
        /// Delete an entity from our repository.
        /// <para>Examples:</para>
        /// <para>_repository.Delete(entityList);</para>
        /// </summary>
        /// <param name="entities">List of entities to be deleted to our repository.</param>
        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
        }

        /// <summary>
        /// Delete an entity from our repository.
        /// <para>Examples:</para>
        /// <para>_repository.Delete(p=> p.UserId == userId);</para>
        /// </summary>
        /// <param name="predicate">Filter applied to our search.</param>
        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            DbSet.AsQueryable()
                 .Where(predicate)
                 .ToList()
                 .ForEach(entity => DbSet.Remove(entity));
        }

        #endregion

        #region Select

        /// <summary>
        /// Select an entity using it's primary keys as search criteria.
        /// <para>Examples:</para>
        /// <para>_repository.SelectByKey(userId);</para>
        /// <para>_repository.SelectByKey(companyId, userId);</para>
        /// </summary>
        /// <param name="primaryKeys">Primary key properties of our entity.</param>
        /// <returns>Returns an entity from our repository.</returns>
        public TEntity SelectByKey(params object[] primaryKeys)
        {
            return DbSet.Find(primaryKeys)
                        .Detach(_dbContext);
        }

        /// <summary>
        /// Select all entities from our repository
        /// <para>Examples:</para>
        /// <para>_repository.SelectAll();</para>
        /// </summary>
        /// <returns>Returns all entities from our repository.</returns>
        public virtual IList<TEntity> SelectAll()
        {
            return DbSet.ToList();
        }

        /// <summary>
        /// Select entities using pagination (take N).
        /// <para>Examples:</para>
        /// <para>_repository.SelectAllByPage(1, 20);</para>
        /// <para>_repository.SelectAllByPage(pageNumber, quantityPerPage);</para>
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="quantity">Number of entities to select per page.</param>
        /// <returns>Returns entities from our repository.</returns>
        public virtual IList<TEntity> SelectAllByPage(int pageNumber, int quantity)
        {
            return DbSet.Skip(Math.Max(pageNumber - 1, 0) * quantity)
                        .Take(quantity)
                        .ToList();
        }

        /// <summary>
        /// Select an entity from our repository using a filter.
        /// <para>Examples:</para>
        /// <para>_repository.Select(p=> p.UserId == 1);</para>
        /// <para>_repository.Select(p=> p.UserName.Contains("John") == true);</para>
        /// </summary>
        /// <param name="predicate">Filter applied to our search.</param>
        /// <returns>Returns an entity from our repository.</returns>
        public virtual TEntity Select(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.WhereNullSafe(predicate)
                        .FirstOrDefault();
        }

        /// <summary>
        /// Select specific properties of an entity from our repository.
        /// <para>Examples:</para>
        /// <para>_repository.Select(p=> p.UserId == userId, p=> p.LastName);</para>
        /// </summary>
        /// <typeparam name="TResult">Entity returned from our search.</typeparam>
        /// <param name="predicate">Filter applied to our search.</param>
        /// <param name="properties">Fields that will be selected and populated in our result.</param>
        /// <returns>Returns an entity from our repository.</returns>
        public TResult Select<TResult>(Expression<Func<TEntity, bool>> predicate,
                                       Expression<Func<TEntity, TResult>> properties)
        {
            return DbSet.WhereNullSafe(predicate)
                        .Select(properties)
                        .FirstOrDefault();
        }

        /// <summary>
        /// Select a list of entities from our repository using a filter.
        /// <para>Examples:</para>
        /// <para>_repository.SelectList(p => p.LastName.Contains("Doe"));</para>
        /// <para>_repository.SelectList(null);</para>
        /// </summary>
        /// <param name="predicate">Filter applied to our search.</param>
        /// <returns>Returns a list of entities from our repository.</returns>
        public IList<TEntity> SelectList(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.WhereNullSafe(predicate)
                        .ToList();
        }

        /// <summary>
        /// Select specific properties in list of entities from our repository using a filter.
        /// <para>Examples:</para>
        /// <para>_repository.SelectList(p => p.UserId, p => p.LastName.Contains("Doe"));</para>
        /// <para>_repository.SelectList(p => p.UserId, p => p.IsAdmin == true);</para>
        /// </summary>
        /// <typeparam name="TResult">Entity returned from our search.</typeparam>
        /// <param name="properties">Fields that will be selected and populated.</param>
        /// <param name="predicate">Filter applied to our search.</param>
        /// <returns>Returns a list of entities from our repository.</returns>
        public IList<TResult> SelectList<TResult>(Expression<Func<TEntity, bool>> predicate,
                                                  Expression<Func<TEntity, TResult>> properties)
        {
            return DbSet.WhereNullSafe(predicate)
                        .Select(properties)
                        .ToList();
        }

        /// <summary>
        /// Method to verify if there are any entries in our repository using a filter.
        /// <para>Examples:</para>
        /// <para>_repository.Exists(p => p.UserId == user.Id)</para>
        /// <para>_repository.Exists(p => p.UserId == id &amp;&amp; p.IsAdmin == false);</para>
        /// </summary>
        /// <param name="predicate">Filter applied to our search.</param>
        /// <returns>Returns if any entity was found using the search criteria.</returns>
        public virtual bool Exists(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Any(predicate);
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Dispose method.
        /// </summary>
        public void Dispose()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}