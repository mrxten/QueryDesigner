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
        public string Field { get; set; }

        /// <summary>
        /// Sorting order.
        /// </summary>
        public OrderFilterType Order { get; set; } = OrderFilterType.Asc;
    }
}