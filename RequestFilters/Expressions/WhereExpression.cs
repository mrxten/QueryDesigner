namespace RequestFilters.Expressions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal static class WhereExpression
    {
        /// <summary>
        /// String type.
        /// </summary>
        private static Type _stringType = typeof(string);

        /// <summary>
        /// Info about "Contains" method.
        /// </summary>
        private static MethodInfo _containsMethod = _stringType.GetMethod("Contains");

        /// <summary>
        /// Info about "StartsWith" method.
        /// </summary>
        private static MethodInfo _startsMethod = _stringType.GetMethod("StartsWith", new[] { typeof(string) });

        /// <summary>
        /// Info about "Contains" method for collection.
        /// </summary>
        private static MethodInfo _collectionContains = typeof(Enumerable).GetMethods().Single(
                method => method.Name == "Contains" && method.IsStatic &&
                method.GetParameters().Length == 2);

        /// <summary>
        /// Info about "Any" method with one parameter for collection.
        /// </summary>
        private static MethodInfo _collectionAny = typeof(Enumerable).GetMethods().Single(
                method => method.Name == "Any" && method.IsStatic &&
                method.GetParameters().Length == 1);

        /// <summary>
        /// Info about "Any" method with two parameter for collection.
        /// </summary>
        private static MethodInfo _collectionAny2 = typeof(Enumerable).GetMethods().Single(
                method => method.Name == "Any" && method.IsStatic &&
                method.GetParameters().Length == 2);

        /// <summary>
        /// Info about method of constructing expressions.
        /// </summary>
        private static MethodInfo _expressionMethod = typeof(WhereExpression).GetMethod("GetExpression");

        /// <summary>
        /// Available types for conversion.
        /// </summary>
        private static Type[] _availableCastTypes =
        {
            typeof(DateTime),
            typeof(bool),
            typeof(long),
            typeof(int),
            typeof(short),
            typeof(Guid)
        };

        /// <summary>
        /// Construction of expression between different expression and a filter.
        /// </summary>
        /// <param name="prop">Different expression.</param>
        /// <param name="field">Filter for database query.</param>
        /// <returns>Expression with filter.</returns>
        private static Expression ConstructExpressionOneFilter(Expression prop, WhereFilter field)
        {
            switch (field.FilterType)
            {
                case WhereFilterType.Equal:
                    return Expression.Equal(
                        prop,
                        Expression.Constant(TryCastFieldValueType(field.Value, prop.Type)));

                case WhereFilterType.NotEqual:
                    return Expression.NotEqual(
                        prop,
                        Expression.Constant(TryCastFieldValueType(field.Value, prop.Type)));

                case WhereFilterType.LessThan:
                    return Expression.LessThan(
                        prop,
                        Expression.Constant(TryCastFieldValueType(field.Value, prop.Type)));

                case WhereFilterType.GreaterThan:
                    return Expression.GreaterThan(
                        prop,
                        Expression.Constant(TryCastFieldValueType(field.Value, prop.Type)));

                case WhereFilterType.LessThanOrEqual:
                    return Expression.LessThanOrEqual(
                        prop,
                        Expression.Constant(TryCastFieldValueType(field.Value, prop.Type)));

                case WhereFilterType.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(
                        prop,
                        Expression.Constant(TryCastFieldValueType(field.Value, prop.Type)));

                case WhereFilterType.Contains:
                    return Expression.Call(prop, _containsMethod, Expression.Constant(field.Value, _stringType));

                case WhereFilterType.NotContains:
                    return Expression.Not(
                        Expression.Call(prop, _containsMethod, Expression.Constant(field.Value, _stringType)));

                case WhereFilterType.StartsWith:
                    return Expression.Call(prop, _startsMethod, Expression.Constant(field.Value, _stringType));

                case WhereFilterType.NotStartsWith:
                    return Expression.Not(
                        Expression.Call(prop, _startsMethod, Expression.Constant(field.Value, _stringType)));

                case WhereFilterType.InCollection:
                    var cc = _collectionContains.MakeGenericMethod(((MemberExpression)prop).Type);
                    return Expression.Call(cc, Expression.Constant(field.Value), prop);

                case WhereFilterType.NotInCollection:
                    var ncc = _collectionContains.MakeGenericMethod(((MemberExpression)prop).Type);
                    return Expression.Not(Expression.Call(ncc, Expression.Constant(field.Value), prop));

                case WhereFilterType.Any:
                    var ca = _collectionAny.MakeGenericMethod(
                        ((MemberExpression)prop).Type.GenericTypeArguments.First());
                    return Expression.Call(ca, prop);

                case WhereFilterType.NotAny:
                    var cna = _collectionAny.MakeGenericMethod(
                        ((MemberExpression)prop).Type.GenericTypeArguments.First());
                    return Expression.Not(Expression.Call(cna, prop));

                default:
                    throw new InvalidOperationException("Invalid field filter type.");
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
            if (value == null)
            {
                if (type.IsValueType)
                    throw new InvalidCastException("Null value may not be significant type.");
                return null;
            }

            var valueType = value.GetType();

            if (valueType == type)
                return value;

            if (type.BaseType == typeof(Enum))
                return Enum.Parse(type, Convert.ToString(value));

            if (!_availableCastTypes.Contains(type))
                return value;

            string s = Convert.ToString(value);
            object res = Activator.CreateInstance(type);
            var argTypes = new[] { _stringType, type.MakeByRefType() };
            object[] args = { s, res };
            var tryParse = type.GetMethod("TryParse", argTypes);

            if (!(bool)(tryParse?.Invoke(null, args) ?? false))
                throw new InvalidCastException($"Cannot convert value to type {type.Name}");

            return args[1];
        }
    }
}
