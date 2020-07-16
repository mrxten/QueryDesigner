using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryDesignerCore.Expressions
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
        private static readonly Type ExpType = typeof(Expression);

        /// <summary>
        /// Queryable type.
        /// </summary>
        private static readonly Type QueryableType = typeof(Queryable);


        /// <summary>
        /// Binary AndAlso method for expression.
        /// </summary>
        private static readonly MethodInfo AndExpMethod = ExpType.GetRuntimeMethod("AndAlso", new[] { ExpType, ExpType });

        /// <summary>
        /// Binary OrElse method for expression.
        /// </summary>
        private static readonly MethodInfo OrExpMethod = ExpType.GetRuntimeMethod("OrElse", new[] { ExpType, ExpType });

        /// <summary>
        /// Info about "StartsWith" method.
        /// </summary>
        private static readonly MethodInfo StartsMethod = StringType.GetRuntimeMethod("StartsWith", new[] { StringType });

        /// <summary>
        /// Info about "Contains" method.
        /// </summary>
        private static readonly MethodInfo ContainsMethod = StringType.GetRuntimeMethod("Contains", new[] { StringType });

        /// <summary>
        /// Info about "EndsWith" method.
        /// </summary>
        private static readonly MethodInfo EndsMethod = StringType.GetRuntimeMethod("EndsWith", new[] { StringType });

        /// <summary>
        /// Info about AsQueryableMethod.
        /// </summary>
        private static readonly MethodInfo AsQueryableMethod = QueryableType.GetRuntimeMethods().FirstOrDefault(
                method => method.Name == "AsQueryable" && method.IsStatic);

        /// <summary>
        /// Info about "Any" method with one parameter for collection.
        /// </summary>
        private static readonly MethodInfo CollectionAny = QueryableType.GetRuntimeMethods().Single(
                method => method.Name == "Any" && method.IsStatic &&
                method.GetParameters().Length == 1);

        /// <summary>
        /// Info about "Any" method with two parameter for collection.
        /// </summary>
        private static readonly MethodInfo CollectionAny2 = typeof(Queryable).GetRuntimeMethods().Single(
                method => method.Name == "Any" && method.IsStatic &&
                method.GetParameters().Length == 2);

        /// <summary>
        /// Info about method of constructing expressions.
        /// </summary>
        private static readonly MethodInfo ExpressionMethod = typeof(WhereExpression).GetRuntimeMethods().FirstOrDefault(m => m.Name == "GetExpression");

        /// <summary>
        /// Available types for conversion.
        /// </summary>
        private static readonly Type[] AvailableCastTypes =
        {
            typeof(DateTime),
            typeof(DateTime?),
            typeof(DateTimeOffset),
            typeof(DateTimeOffset?),
            typeof(TimeSpan),
            typeof(TimeSpan?),
            typeof(bool),
            typeof(bool?),
            typeof(byte?),
            typeof(sbyte?),
            typeof(short),
            typeof(short?),
            typeof(ushort),
            typeof(ushort?),
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
            typeof(string)
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
        private static Expression GetExpressionForTreeField(Expression e, TreeFilter filter, string suffix)
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
        private static Expression GetExpressionForField(Expression e, WhereFilter filter, string suffix)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            if (filter.FilterType == WhereFilterType.None || string.IsNullOrWhiteSpace(filter.Field))
                throw new ArgumentException("Filter type cannot be None for single filter.");
            var s = filter.Field.Split('.');
            Expression prop = e;

            foreach (var t in s)
            {
                if (IsEnumerable(prop))
                {
                    prop = AsQueryable(prop);

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
                        prop.Type.GenericTypeArguments.First()),
                        prop,
                        expr);
                }
                prop = Expression.Property(prop, GetDeclaringProperty(prop, t));
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
                        ToStaticParameterExpressionOfType(TryCastFieldValueType(filter.Value, prop.Type), prop.Type));

                case WhereFilterType.NotEqual:
                    return Expression.NotEqual(
                        prop,
                        ToStaticParameterExpressionOfType(TryCastFieldValueType(filter.Value, prop.Type), prop.Type));

                case WhereFilterType.LessThan:
                    return Expression.LessThan(
                        prop,
                        ToStaticParameterExpressionOfType(TryCastFieldValueType(filter.Value, prop.Type), prop.Type));

                case WhereFilterType.GreaterThan:
                    return Expression.GreaterThan(
                        prop,
                        ToStaticParameterExpressionOfType(TryCastFieldValueType(filter.Value, prop.Type), prop.Type));

                case WhereFilterType.LessThanOrEqual:
                    return Expression.LessThanOrEqual(
                        prop,
                        ToStaticParameterExpressionOfType(TryCastFieldValueType(filter.Value, prop.Type), prop.Type));

                case WhereFilterType.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(
                        prop,
                        ToStaticParameterExpressionOfType(TryCastFieldValueType(filter.Value, prop.Type), prop.Type));

                case WhereFilterType.StartsWith:
                    return Expression.Call(prop, StartsMethod, Expression.Constant(filter.Value, StringType));

                case WhereFilterType.NotStartsWith:
                    return Expression.Not(
                        Expression.Call(prop, StartsMethod, Expression.Constant(filter.Value, StringType)));

                case WhereFilterType.Contains:
                    return Expression.Call(prop, ContainsMethod, Expression.Constant(filter.Value, StringType));

                case WhereFilterType.NotContains:
                    return Expression.Not(
                        Expression.Call(prop, ContainsMethod, Expression.Constant(filter.Value, StringType)));

                case WhereFilterType.EndsWith:
                    return Expression.Call(prop, EndsMethod, Expression.Constant(filter.Value, StringType));

                case WhereFilterType.NotEndsWith:
                    return Expression.Not(
                        Expression.Call(prop, EndsMethod, Expression.Constant(filter.Value, StringType)));

                case WhereFilterType.Any:
                    if (IsEnumerable(prop))
                        prop = AsQueryable(prop);
                    var ca = CollectionAny.MakeGenericMethod(
                        prop.Type.GenericTypeArguments.First());
                    return Expression.Call(ca, prop);

                case WhereFilterType.NotAny:
                    if (IsEnumerable(prop))
                        prop = AsQueryable(prop);
                    var cna = CollectionAny.MakeGenericMethod(
                        prop.Type.GenericTypeArguments.First());
                    return Expression.Not(Expression.Call(cna, prop));

                case WhereFilterType.IsNull:
                    return Expression.Equal(prop, ToStaticParameterExpressionOfType(null, prop.Type));

                case WhereFilterType.IsNotNull:
                    return Expression.Not(
                        Expression.Equal(prop, ToStaticParameterExpressionOfType(null, prop.Type)));

                case WhereFilterType.IsEmpty:
                    if (prop.Type != typeof(string))
                        throw new InvalidCastException($"{filter.FilterType} can be applied to String type only");
                    return Expression.Equal(prop, ToStaticParameterExpressionOfType(string.Empty, prop.Type));

                case WhereFilterType.IsNotEmpty:
                    if (prop.Type != typeof(string))
                        throw new InvalidCastException($"{filter.FilterType} can be applied to String type only");
                    return Expression.Not(
                        Expression.Equal(prop, ToStaticParameterExpressionOfType(string.Empty, prop.Type)));

                case WhereFilterType.IsNullOrEmpty:
                    if (prop.Type != typeof(string))
                        throw new InvalidCastException($"{filter.FilterType} can be applied to String type only");
                    return Expression.OrElse(
                        Expression.Equal(prop, ToStaticParameterExpressionOfType(null, prop.Type)),
                        Expression.Equal(prop, ToStaticParameterExpressionOfType(string.Empty, prop.Type)));

                case WhereFilterType.IsNotNullOrEmpty:
                    if (prop.Type != typeof(string))
                        throw new InvalidCastException($"{filter.FilterType} can be applied to String type only");
                    return Expression.Not(
                        Expression.OrElse(
                            Expression.Equal(prop, ToStaticParameterExpressionOfType(null, prop.Type)),
                            Expression.Equal(prop, ToStaticParameterExpressionOfType(string.Empty, prop.Type))));

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
            if (value == null || (!AvailableCastTypes.Contains(type) && !type.GetTypeInfo().IsEnum))
                throw new InvalidCastException($"Cannot convert value to type {type.Name}.");

            var valueType = value.GetType();

            if (valueType == type)
                return value;

            if (type.GetTypeInfo().BaseType == typeof(Enum))
                return Enum.Parse(type, Convert.ToString(value));


            var s = Convert.ToString(value);
            object res;

            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GenericTypeArguments[0];
                res = Activator.CreateInstance(typeof(Nullable<>).MakeGenericType(type));
            }
            else
            {
                res = Activator.CreateInstance(type);
            }

            var argTypes = new[] { StringType, type.MakeByRefType() };
            object[] args = { s, res };
            var tryParse = type.GetRuntimeMethod("TryParse", argTypes);

            if (!(bool)(tryParse?.Invoke(null, args) ?? false))
                throw new InvalidCastException($"Cannot convert value to type {type.Name}.");

            return args[1];
        }

        /// <summary>
        /// Creates parameters expression of static value.
        /// </summary>
        /// <returns>The static parameter expression of type.</returns>
        /// <param name="obj">Filter value.</param>
        /// <param name="type">Type of value.</param>
        private static Expression ToStaticParameterExpressionOfType(object obj, Type type)
            => Expression.Convert(
                Expression.Property(
                    Expression.Constant(new { obj }), 
                    "obj"),
                type);

        /// <summary>
        /// Cast IEnumerable to IQueryable.
        /// </summary>
        /// <param name="prop">IEnumerable expression</param>
        /// <returns>IQueryable expression.</returns>
        private static Expression AsQueryable(Expression prop)
        {
            return Expression.Call(
                        AsQueryableMethod.MakeGenericMethod(prop.Type.GenericTypeArguments.Single()),
                        prop);
        }


        /// <summary>
        /// Expression type is IEnumerable.
        /// </summary>
        /// <param name="prop">Verifiable expression.</param>
        /// <returns>Result of checking.</returns>
        private static bool IsEnumerable(Expression prop)
        {
            return prop.Type.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(x => x.Name == "IEnumerable") != null;
        }

        /// <summary>
        /// Get property from class in which it is declared.
        /// </summary>
        /// <param name="e">Parameter expression.</param>
        /// <param name="name">Name of property.</param>
        /// <returns>Property info.</returns>
        private static PropertyInfo GetDeclaringProperty(Expression e, string name)
        {
            var t = e.Type;
            var p = t.GetRuntimeProperties().SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (p == null)
            {
                throw new InvalidOperationException(string.Format("Property '{0}' not found on type '{1}'", name, t));
            }

            if (t != p.DeclaringType)
            {
                p = p.DeclaringType.GetRuntimeProperties().SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
            return p;
        }
    }
}
