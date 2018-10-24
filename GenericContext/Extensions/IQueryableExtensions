using System;
using System.Linq;
using System.Linq.Expressions;

namespace GenericContext.Extensoes
{
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Allows to apply a null predicate in a Where condition without throwing an exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">IQueryable list of entities.</param>
        /// <param name="predicate">Predicate (allows null).</param>
        /// <returns></returns>
        public static IQueryable<T> WhereNullSafe<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return predicate == null ? source : source.Where(predicate);
        }
    }
}
