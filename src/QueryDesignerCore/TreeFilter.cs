using System.Collections.Generic;

namespace QueryDesignerCore
{
    /// <summary>
    /// Filters with infinite nesting and boolean operations therebetween.
    /// </summary>
    public class TreeFilter : WhereFilter
    {
        /// <summary>
        /// Filters with infinite nesting and boolean operations therebetween.
        /// </summary>
        public TreeFilter()
        {
            OperatorType = TreeFilterType.None;
        }

        /// <summary>
        /// Type of logical operator.
        /// </summary>
        public TreeFilterType OperatorType { get; set; }

        /// <summary>
        /// Operands of boolean expressions.
        /// </summary>
        public List<TreeFilter> Operands { get; set; } 
    }
}
