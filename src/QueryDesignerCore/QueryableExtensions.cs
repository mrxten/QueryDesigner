using System;
using System.Collections.Generic;
using System.Linq;
using QueryDesignerCore.Expressions;

namespace QueryDesignerCore
{
    /// <summary>
    /// Override Quaryable functions.
    /// </summary>
    public static class QueryableExtensions
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
            IQueryable<T> res = query
                .Where(request.Where)
                .OrderBy(request.OrderBy);
            if (request.Skip >= 0)
                res = res.Skip(request.Skip);
            if (request.Take > 0)
                res = res.Take(request.Take);
            return res;
        }

        /// <summary>
        /// FilterContainer expression.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="query">An enumerated request.</param>
        /// <param name="request">Container for filters.</param>
        /// <returns>Performed query.</returns>
        public static IQueryable<T> Request<T>(this IEnumerable<T> query, FilterContainer request)
        {
            return query.AsQueryable().Request(request);
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
        /// Filtration items based on a given WhereFilter.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="query">An enumerated request.</param>
        /// <param name="filter">Filter for queryable expression.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidCastException" />
        /// <exception cref="InvalidOperationException" />
        /// <returns>Filtered query.</returns>
        public static IQueryable<T> Where<T>(this IEnumerable<T> query, WhereFilter filter)
        {
            return filter != null ? query.AsQueryable().Where(filter.GetExpression<T>()) : query.AsQueryable();
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
        /// Filtration items based on a given TreeFilter.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="query">An enumerated request.</param>
        /// <param name="filter">Tree filter for queryable expression.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidCastException" />
        /// <exception cref="InvalidOperationException" />
        /// <returns>Filtered query.</returns>
        public static IQueryable<T> Where<T>(this IEnumerable<T> query, TreeFilter filter)
        {
            return filter != null ? query.AsQueryable().Where(filter.GetTreeExpression<T>()) : query.AsQueryable();
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
            return filter != null ? filter.GetOrderedQueryable(query, OrderStep.First) : (IOrderedQueryable<T>)query;
        }

        /// <summary>
        /// Sorting items based on a given OrderFilter.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="query">An enumerated request.</param>
        /// <param name="filter">Sort filter.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidCastException" />
        /// <exception cref="InvalidOperationException" />
        /// <returns>Sorted query.</returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IEnumerable<T> query, OrderFilter filter)
        {
            return filter != null ? filter.GetOrderedQueryable(query.AsQueryable(), OrderStep.First) : (IOrderedQueryable<T>)query.AsQueryable();
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
        public static IOrderedQueryable<T> ThenBy<T>(this IQueryable<T> query, OrderFilter filter)
        {
            return filter != null ? filter.GetOrderedQueryable(query, OrderStep.Next) : (IOrderedQueryable<T>)query;
        }

        /// <summary>
        /// Sorting items based on a given OrderFilter.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="query">An enumerated request.</param>
        /// <param name="filter">Sort filter.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidCastException" />
        /// <exception cref="InvalidOperationException" />
        /// <returns>Sorted query.</returns>
        public static IOrderedQueryable<T> ThenBy<T>(this IEnumerable<T> query, OrderFilter filter)
        {
            return filter != null ? filter.GetOrderedQueryable(query.AsQueryable(), OrderStep.Next) : (IOrderedQueryable<T>)query.AsQueryable();
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
            var step = OrderStep.First;
            if (filters != null)
                foreach (var filter in filters)
                {
                    res = filter.GetOrderedQueryable(res, step);
                    step = OrderStep.Next;
                }
            return res;
        }

        /// <summary>
        /// Sorting items based on a given OrderFilter.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="query">An enumerated request.</param>
        /// <param name="filters">Enumeration of sort filters.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidCastException" />
        /// <exception cref="InvalidOperationException" />
        /// <returns>Sorted query.</returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IEnumerable<T> query, IEnumerable<OrderFilter> filters)
        {
            return query.AsQueryable().OrderBy(filters);
        }
    }
}
