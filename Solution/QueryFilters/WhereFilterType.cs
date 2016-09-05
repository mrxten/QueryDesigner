namespace QueryFilters
{
    /// <summary>
    /// Type of elements comparison.
    /// </summary>
    public enum WhereFilterType
    {
        /// <summary>
        /// The field is not a filtered.
        /// </summary>
        None,

        /// <summary>
        /// The field is equal to the value.
        /// </summary>
        Equal,

        /// <summary>
        /// The field is not equal to the value.
        /// </summary>
        NotEqual,

        /// <summary>
        /// The field is less than the value.
        /// </summary>
        LessThan,

        /// <summary>
        /// The field is greater than the value.
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Field is less than or equal to value.
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// The field is greater than or equal to the value.
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// The field is contains the value.
        /// </summary>
        Contains,

        /// <summary>
        /// The field is not contains the value.
        /// </summary>
        NotContains,

        /// <summary>
        /// The field is start with value.
        /// </summary>
        StartsWith,

        /// <summary>
        /// The field is not start with value.
        /// </summary>
        NotStartsWith,

        /// <summary>
        /// Collection contains an element.
        /// </summary>
        InCollection,

        /// <summary>
        /// Collection not contains an element.
        /// </summary>
        NotInCollection,

        /// <summary>
        /// Collection contains data.
        /// </summary>
        Any,

        /// <summary>
        /// Collection not contains data.
        /// </summary>
        NotAny
    }
}