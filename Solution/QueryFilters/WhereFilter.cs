namespace QueryFilters
{
    /// <summary>
    /// Tree filter for queryable expression.
    /// </summary>
    public class WhereFilter
    {
        /// <summary>
        /// Filter field name.
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Type of field filtration.
        /// </summary>
        public WhereFilterType FilterType { get; set; }

        /// <summary>
        /// Value for filtering.
        /// </summary>
        public object Value { get; set; }
    }
}