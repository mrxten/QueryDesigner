namespace RequestFilters
{
    public class WhereFilter
    {
        /// <summary>
        /// Class constructor.
        /// </summary>
        public WhereFilter()
        {
            Strict = true;
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

        /// <summary>
        /// Strict conditions.
        /// </summary>
        public bool Strict { get; set; }
    }
}