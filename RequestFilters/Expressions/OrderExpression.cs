using System.Collections.Generic;

namespace RequestFilters.Expressions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal static class OrderExpression
    {
        /// <summary>
        /// Queryable type.
        /// </summary>
        private static readonly Type QueryableType = typeof(Queryable);

        /// <summary>
        /// OrderBy method.
        /// </summary>
        private static readonly MethodInfo QueryableOrderBy = QueryableType.GetMethods().Single(
                method => method.Name == "OrderBy"
                        && method.IsGenericMethodDefinition
                        && method.GetGenericArguments().Length == 2
                        && method.GetParameters().Length == 2);

        /// <summary>
        /// OrderBy desc method.
        /// </summary>
        private static readonly MethodInfo QueryableOrderByDescending = QueryableType.GetMethods().Single(
                method => method.Name == "OrderByDescending"
                        && method.IsGenericMethodDefinition
                        && method.GetGenericArguments().Length == 2
                        && method.GetParameters().Length == 2);

        /// <summary>
        /// ThenBy method.
        /// </summary>
        private static readonly MethodInfo QueryableThenBy = QueryableType.GetMethods().Single(
                method => method.Name == "ThenBy"
                        && method.IsGenericMethodDefinition
                        && method.GetGenericArguments().Length == 2
                        && method.GetParameters().Length == 2);

        /// <summary>
        /// ThenBy desc method.
        /// </summary>
        private static readonly MethodInfo QueryableThenByDescending = QueryableType.GetMethods().Single(
                method => method.Name == "ThenByDescending"
                        && method.IsGenericMethodDefinition
                        && method.GetGenericArguments().Length == 2
                        && method.GetParameters().Length == 2);

        /// <summary>
        /// Perform data sorting.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="filter">.</param>
        /// <param name="data">Data for sort.</param>
        /// <param name="step">Step number.</param>
        /// <returns>Sorted query.</returns>
        public static IOrderedQueryable<T> GetOrderedQueryable<T>(this OrderFilter filter, IQueryable<T> data, int step = 0)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            Type type = typeof(T);

            ParameterExpression arg = Expression.Parameter(type, "x" + step);
            Expression expr = arg;
            string[] spl = filter.Field.Split('.');
            for (int i = 0; i < spl.Length; i++)
            {
                PropertyInfo pi = type.GetProperty(spl[i]);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }

            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            Expression lambda = Expression.Lambda(delegateType, expr, arg);
            MethodInfo m = step == 0
                ? filter.Order == OrderFilterType.Asc ? QueryableOrderBy : QueryableOrderByDescending
                : filter.Order == OrderFilterType.Asc ? QueryableThenBy : QueryableThenByDescending;
            
            return ((IOrderedQueryable<T>)m.MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] { data, lambda }));
        }
    }
}
