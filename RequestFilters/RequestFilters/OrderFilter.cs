namespace RequestFilters
{
    /// <summary>
    /// Sort by the field.
    /// </summary>
    public class OrderFilter
    {
        /// <summary>
        /// Sort by the field.
        /// </summary>
        public OrderFilter()
        {
            Order = OrderFilterType.Asc;
        }

        /// <summary>
        /// Sort field name.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Sorting order.
        /// </summary>
        public OrderFilterType Order { get; set; }
    }
}