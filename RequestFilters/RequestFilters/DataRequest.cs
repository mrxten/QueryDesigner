namespace RequestFilters
{
    using System.Collections.Generic;
    
    /// <summary>
    /// Container for filters.
    /// </summary>
    public class DataRequest
    {
        /// <summary>
        /// Container for filters.
        /// </summary>
        public DataRequest()
        {
            WhereFilters = new List<WhereFilter>();
            OrderFilters = new List<OrderFilter>();
            Skip = 0;
            Take = 100;
        }

        /// <summary>
        /// Where filters.
        /// </summary>
        public List<WhereFilter> WhereFilters { get; set; }

        /// <summary>
        /// Order filters.
        /// </summary>
        public List<OrderFilter> OrderFilters { get; set; }

        /// <summary>
        /// Skip number of elements.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Take number of elements.
        /// </summary>
        public int Take { get; set; }
    }
}
