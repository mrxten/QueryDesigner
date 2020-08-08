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
        IsNotNullOrEmpty,


        /// <summary>
        /// Collection count equal the value.
        /// </summary>
        CountEquals,


        /// <summary>
        /// Collection count less than the value.
        /// </summary>
        CountLessThan,

         /// <summary>
        /// Collection count is  less than or equal to value.
        /// </summary>
        CountLessThanOrEqual,
        
        /// <summary>
        /// Collection count greater than the value.
        /// </summary>
        CountGreaterThan,

       

        /// <summary>
        /// Collection count is greater than or equal to the value.
        /// </summary>
        CountGreaterThanOrEqual,


        /// <summary>
        /// The string field value length is equals to the value.
        /// </summary>
        LengthEquals,

        /// <summary>
        /// The string field value length is less than to the value.
        /// </summary>
        LengthLessThan,


        /// <summary>
        /// The string field value length is greater than to the value.
        /// </summary>
        LengthGreaterThan,


        /// <summary>
        /// The string field value length is less than or equal to the value.
        /// </summary>
        LengthLessThanOrEqual,


        /// <summary>
        /// The string field value length is greater than or equal to the value.
        /// </summary>
        LengthGreaterThanOrEqual,

        /// <summary>
        /// Collection first or default object is equal to the null.
        /// </summary>
        FirstOrDefaultIsNull,


        /// <summary>
        /// Collection first or default object is bot equal not to the null.
        /// </summary>
        FirstOrDefaultNotNull,



    }
}