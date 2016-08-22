namespace RequestFilters
{
    /// <summary>
    /// Sort by the field.
    /// </summary>
    public class OrderFilter
    {
        /// <summary>
        /// Sort field name.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Sorting order.
        /// </summary>
        public OrderFilterType Order { get; set; }

        /// <summary>
        /// Convert filter to string.
        /// </summary>
        /// <returns>Converted filter.</returns>
        public override string ToString()
        {
            return $"{FieldName} {(int)Order}";
        }
    }
}