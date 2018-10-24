using Microsoft.EntityFrameworkCore;
using System;

namespace GenericContext.Extensoes
{
    internal static class TEntityExtensions
    {
        #region Dettach do entity framework

        /// <summary>
        /// Detaches an entity from a DbContext, disabling lazy loading.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <param name="entity">Entity instance.</param>
        /// <param name="context">DbContext that controls the entity.</param>
        /// <returns></returns>
        internal static TEntity Detach<TEntity>(this TEntity entity, DbContext context) where TEntity : class
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            else if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            else
            {
                context.Entry(entity).State = EntityState.Detached;
            }

            return entity;
        }

        #endregion
    }
}
