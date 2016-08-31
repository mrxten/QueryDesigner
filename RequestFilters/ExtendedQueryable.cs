namespace RequestFilters
{
    using System;
    using System.Linq;
    using RequestFilters.Expressions;
    /// <summary>
    /// Override Quaryable functions.
    /// </summary>
    public static class ExtendedQueryable
    {
        /// <summary>
        /// Filtration items based on a given WhereFilter.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="query">Integrable request.</param>
        /// <param name="filter">Filter for queryable expression.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidCastException" />
        /// <exception cref="InvalidOperationException" />
        /// <returns>Filtered query.</returns>
        public static IQueryable<T> Where<T>(this IQueryable<T> query, WhereFilter filter)
        {
            return filter != null ? query.Where(filter.GetExpression<T>()) : query;
        }

        /// <summary>
        /// Filtration items based on a given TreeFilter.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="query">Integrable request.</param>
        /// <param name="filter">Tree filter for queryable expression.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidCastException" />
        /// <exception cref="InvalidOperationException" />
        /// <returns>Filtered query.</returns>
        public static IQueryable<T> Where<T>(this IQueryable<T> query, TreeFilter filter)
        {
            return filter != null ? query.Where(filter.GetTreeExpression<T>()) : query;
        }

        /// <summary>
        /// Sorting items based on a given OrderFilter.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="query">Integrable request.</param>
        /// <param name="filter">Sorted query.</param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, OrderFilter filter)
        {
            throw new NotImplementedException();
        }
    }
}
