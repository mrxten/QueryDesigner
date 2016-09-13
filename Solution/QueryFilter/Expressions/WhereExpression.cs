using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryFilter.Expressions
{
    /// <summary>
    /// Constructor expressions for Where methods.
    /// </summary>
    internal static class WhereExpression
    {
        /// <summary>
        /// String type.
        /// </summary>
        private static readonly Type StringType = typeof(string);

        /// <summary>
        /// Expression type.
        /// </summary>
        private static readonly Type ExpType = typeof (Expression);

        /// <summary>
        /// Queryable type.
        /// </summary>
        private static readonly Type QueryableType = typeof(Queryable);

        /// <summary>
        /// Binary AndAlso method for expression.
        /// </summary>
        private static readonly MethodInfo AndExpMethod = ExpType.GetMethod("AndAlso", new[] { ExpType, ExpType });

        /// <summary>
        /// Binary OrElse method for expression.
        /// </summary>
        private static readonly MethodInfo OrExpMethod = ExpType.GetMethod("OrElse", new[] { ExpType, ExpType });

        /// <summary>
        /// Info about "Contains" method.
        /// </summary>
        private static readonly MethodInfo ContainsMethod = StringType.GetMethod("Contains");

        /// <summary>
        /// Info about "StartsWith" method.
        /// </summary>
        private static readonly MethodInfo StartsMethod = StringType.GetMethod("StartsWith", new[] { typeof(string) });

        /// <summary>
        /// Info about "Contains" method for collection.
        /// </summary>
        private static readonly MethodInfo CollectionContains = QueryableType.GetMethods().Single(
                method => method.Name == "Contains" && method.IsStatic &&
                method.GetParameters().Length == 2);

        /// <summary>
        /// Info about "Any" method with one parameter for collection.
        /// </summary>
        private static readonly MethodInfo CollectionAny = QueryableType.GetMethods().Single(
                method => method.Name == "Any" && method.IsStatic &&
                method.GetParameters().Length == 1);

        /// <summary>
        /// Info about "Any" method with two parameter for collection.
        /// </summary>
        private static readonly MethodInfo CollectionAny2 = typeof(Queryable).GetMethods().Single(
                method => method.Name == "Any" && method.IsStatic &&
                method.GetParameters().Length == 2);

        /// <summary>
        /// Info about method of constructing expressions.
        /// </summary>
        private static readonly MethodInfo ExpressionMethod = typeof(WhereExpression).GetMethod("GetExpression");

        /// <summary>
        /// Available types for conversion.
        /// </summary>
        private static readonly Type[] AvailableCastTypes =
        {
            typeof(DateTime),
            typeof(DateTime?),
            typeof(TimeSpan),
            typeof(TimeSpan?),
            typeof(bool),
            typeof(bool?),
            typeof(int),
            typeof(int?),
            typeof(uint),
            typeof(uint?),
            typeof(long),
            typeof(long?),
            typeof(ulong),
            typeof(ulong?),
            typeof(Guid),
            typeof(Guid?),
            typeof(double),
            typeof(double?),
            typeof(float),
            typeof(float?),
            typeof(decimal),
            typeof(decimal?),
            typeof(char),
            typeof(char?),
            typeof(string),
        };

        /// <summary>
        /// Get final expression for filter.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="filter">Filter for query.</param>
        /// <param name="suffix">Suffix vor variable.</param>
        /// <returns>Final expression.</returns>
        public static Expression<Func<T, bool>> GetExpression<T>(this WhereFilter filter, string suffix = "")
        {
            var e = Expression.Parameter(typeof(T), "e" + suffix);
            var exs = GetExpressionForField(e, filter, suffix + "0");
            return Expression.Lambda<Func<T, bool>>(exs, e);
        }

        /// <summary>
        /// Get final expression for tree filter.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="filter">Tree filter for query.</param>
        /// <param name="suffix">Suffix vor variable.</param>
        /// <returns>Final expression.</returns>
        public static Expression<Func<T, bool>> GetTreeExpression<T>(this TreeFilter filter, string suffix = "")
        {
            var e = Expression.Parameter(typeof(T), "e" + suffix);
            return Expression.Lambda<Func<T, bool>>(GetExpressionForTreeField(e, filter, suffix), e);
        }

        /// <summary>
        /// Construct expressions chain for tree filter.
        /// </summary>
        /// <param name="e">Parameter expression.</param>
        /// <param name="filter">Tree filter for query.</param>
        /// <param name="suffix">Suffix vor variable.</param>
        /// <returns>Expression chain.</returns>
        private static Expression GetExpressionForTreeField(ParameterExpression e, TreeFilter filter, string suffix)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            if (filter.OperatorType == TreeFilterType.None)
                return GetExpressionForField(e, filter, suffix + "0");

            if (!(filter.Operands?.Any() ?? false))
                throw new ArgumentException("Filter operands with operator type different from TreeFilterType.None cannot be empty.");

            var i = 0;
            var exp = GetExpressionForTreeField(e, filter.Operands[i], suffix + i);
            var mi = filter.OperatorType == TreeFilterType.And ? AndExpMethod : OrExpMethod;
            for (i = 1; i < filter.Operands.Count; i++)
            {
                var args = new object[] { exp, GetExpressionForTreeField(e, filter.Operands[i], suffix + i) };
                exp = (BinaryExpression)mi.Invoke(null, args);
            }
            return exp;
        }

        /// <summary>
        /// Construct expressions chain between WhereFilters.
        /// </summary>
        /// <param name="e">Parameter expression.</param>
        /// <param name="filter">Tree filter for query.</param>
        /// <param name="suffix">Suffix vor variable.</param>
        /// <returns>Expression chain.</returns>
        private static Expression GetExpressionForField(ParameterExpression e, WhereFilter filter, string suffix)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            if (filter.FilterType == WhereFilterType.None || string.IsNullOrWhiteSpace(filter.Field))
                throw new ArgumentException("Filter type cannot be None for single filter.");
            var s = filter.Field.Split('.');
            Expression prop = e;
            foreach (var t in s)
            {
                if (prop.Type.GetInterface("IEnumerable") != null)
                {
                    var generic = ExpressionMethod.MakeGenericMethod(
                        prop.Type.GenericTypeArguments.Single());
                    object[] pars = {
                        new WhereFilter
                        {
                            Field = t,
                            FilterType = filter.FilterType,
                            Value = filter.Value
                        },
                        suffix
                    };
                    var expr = (Expression)generic.Invoke(null, pars);
                    return Expression.Call(
                        CollectionAny2.MakeGenericMethod(
                            ((MemberExpression)prop).Type.GenericTypeArguments.First()),
                        prop,
                        expr);
                }
                prop = Expression.Property(prop, t);
            }
            var exp = GenerateExpressionOneField(prop, filter);
            return exp;
        }

        /// <summary>
        /// Construct bool-expression between different expression and a filter.
        /// </summary>
        /// <param name="prop">Different expression.</param>
        /// <param name="filter">Filter for query.</param>
        /// <returns>Expression with filter.</returns>
        private static Expression GenerateExpressionOneField(Expression prop, WhereFilter filter)
        {
            switch (filter.FilterType)
            {
                case WhereFilterType.Equal:
                    return Expression.Equal(
                        prop,
                        Expression.Constant(TryCastFieldValueType(filter.Value, prop.Type)));

                case WhereFilterType.NotEqual:
                    return Expression.NotEqual(
                        prop,
                        Expression.Constant(TryCastFieldValueType(filter.Value, prop.Type)));

                case WhereFilterType.LessThan:
                    return Expression.LessThan(
                        prop,
                        Expression.Constant(TryCastFieldValueType(filter.Value, prop.Type)));

                case WhereFilterType.GreaterThan:
                    return Expression.GreaterThan(
                        prop,
                        Expression.Constant(TryCastFieldValueType(filter.Value, prop.Type)));

                case WhereFilterType.LessThanOrEqual:
                    return Expression.LessThanOrEqual(
                        prop,
                        Expression.Constant(TryCastFieldValueType(filter.Value, prop.Type)));

                case WhereFilterType.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(
                        prop,
                        Expression.Constant(TryCastFieldValueType(filter.Value, prop.Type)));

                case WhereFilterType.Contains:
                    return Expression.Call(prop, ContainsMethod, Expression.Constant(filter.Value, StringType));

                case WhereFilterType.NotContains:
                    return Expression.Not(
                        Expression.Call(prop, ContainsMethod, Expression.Constant(filter.Value, StringType)));

                case WhereFilterType.StartsWith:
                    return Expression.Call(prop, StartsMethod, Expression.Constant(filter.Value, StringType));

                case WhereFilterType.NotStartsWith:
                    return Expression.Not(
                        Expression.Call(prop, StartsMethod, Expression.Constant(filter.Value, StringType)));

                case WhereFilterType.InCollection:
                    var cc = CollectionContains.MakeGenericMethod(((MemberExpression)prop).Type);
                    return Expression.Call(cc, Expression.Constant(filter.Value), prop);

                case WhereFilterType.NotInCollection:
                    var ncc = CollectionContains.MakeGenericMethod(((MemberExpression)prop).Type);
                    return Expression.Not(Expression.Call(ncc, Expression.Constant(filter.Value), prop));

                case WhereFilterType.Any:
                    var ca = CollectionAny.MakeGenericMethod(
                        ((MemberExpression)prop).Type.GenericTypeArguments.First());
                    return Expression.Call(ca, prop);

                case WhereFilterType.NotAny:
                    var cna = CollectionAny.MakeGenericMethod(
                        ((MemberExpression)prop).Type.GenericTypeArguments.First());
                    return Expression.Not(Expression.Call(cna, prop));

                default:
                    return prop;
            }
        }

        /// <summary>
        /// Value type filter field conversion.
        /// </summary>
        /// <param name="value">Filter value.</param>
        /// <param name="type">Conversion to type.</param>
        /// <returns>Converted value.</returns>
        private static object TryCastFieldValueType(object value, Type type)
        {
            if (value == null || !AvailableCastTypes.Contains(type))
                throw new InvalidCastException($"Cannot convert value to type {type.Name}.");

            var valueType = value.GetType();

            if (valueType == type)
                return value;

            if (type.BaseType == typeof(Enum))
                return Enum.Parse(type, Convert.ToString(value));


            var s = Convert.ToString(value);
            var res = Activator.CreateInstance(type);
            var argTypes = new[] { StringType, type.MakeByRefType() };
            object[] args = { s, res };
            var tryParse = type.GetMethod("TryParse", argTypes);

            if (!(bool)(tryParse?.Invoke(null, args) ?? false))
                throw new InvalidCastException($"Cannot convert value to type {type.Name}.");

            return args[1];
        }
    }
}
