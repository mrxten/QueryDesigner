using System;
using System.Collections.Generic;
using System.Linq;
using QueryFilters.Expressions;

namespace QueryFilters
{
    /// <summary>
    /// Override Quaryable functions.
    /// </summary>
    public static class QueryableExtend
    {
        /// <summary>
        /// FilterContainer expression.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="query">Integrable request.</param>
        /// <param name="request">Container for filters.</param>
        /// <returns>Performed query.</returns>
        public static IQueryable<T> Request<T>(this IQueryable<T> query, FilterContainer request)
        {
            return query
                .Where(request.Where)
                .OrderBy(request.OrderBy)
                .Skip(request.Skip)
                .Take(request.Take);
        } 

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
        /// <param name="filter">Sort filter.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidCastException" />
        /// <exception cref="InvalidOperationException" />
        /// <returns>Sorted query.</returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, OrderFilter filter)
        {
            return filter != null ? filter.GetOrderedQueryable(query) : (IOrderedQueryable<T>)query;
        }

        /// <summary>
        /// Sorting items based on a given OrderFilter.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="query">Integrable request.</param>
        /// <param name="filters">Enumeration of sort filters.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidCastException" />
        /// <exception cref="InvalidOperationException" />
        /// <returns>Sorted query.</returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, IEnumerable<OrderFilter> filters)
        {
            var res = (IOrderedQueryable<T>)query;
            var step = 0;
            return filters.Aggregate(res, (current, filter) => filter.GetOrderedQueryable(current, step++));
        }
    }
}
