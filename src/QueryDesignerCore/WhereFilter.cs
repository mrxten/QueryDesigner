namespace QueryDesignerCore
{
    /// <summary>
    /// Tree filter for queryable expression.
    /// </summary>
    public class WhereFilter
    {
        /// <summary>
        /// Tree filter for queryable expression.
        /// </summary>
        public WhereFilter()
        {
            FilterType = WhereFilterType.None;
        }

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