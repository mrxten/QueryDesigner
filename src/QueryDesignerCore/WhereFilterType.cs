namespace QueryDesignerCore
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
        /// The field is end with value.
        /// </summary>
        EndsWith,

        /// <summary>
        /// The field is not end with value.
        /// </summary>
        NotEndsWith,

        /// <summary>
        /// Collection contains data.
        /// </summary>
        Any,

        /// <summary>
        /// Collection not contains data.
        /// </summary>
        NotAny,

        /// <summary>
        /// The field is null
        /// </summary>
        IsNull,

        /// <summary>
        /// The field is not null
        /// </summary>
        IsNotNull,

        /// <summary>
        /// The field is empty
        /// </summary>
        IsEmpty,

        /// <summary>
        /// The field is not empty
        /// </summary>
        IsNotEmpty,

        /// <summary>
        /// The field is null or empty
        /// </summary>
        IsNullOrEmpty,

        /// <summary>
        /// The field is not null or empty
        /// </summary>
        IsNotNullOrEmpty
    }
}