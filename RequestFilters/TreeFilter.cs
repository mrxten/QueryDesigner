namespace RequestFilters
{
    using System.Collections.Generic;

    /// <summary>
    /// Filters with infinite nesting and boolean operations therebetween.
    /// </summary>
    public class TreeFilter : WhereFilter
    {
        /// <summary>
        /// Type of logical operator.
        /// </summary>
        public TreeFilterType OperatorType { get; set; } = TreeFilterType.None;

        /// <summary>
        /// Operands of boolean expressions.
        /// </summary>
        public List<TreeFilter> Operands { get; set; } 
    }
}
