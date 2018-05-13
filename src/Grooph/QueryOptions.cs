using System;
using System.Collections.Generic;
using System.Text;

namespace Grooph
{
    public class QueryOptions
    {
        public const int DEFAULT_MAX_DEPTH = 2;

        public static QueryOptions Default(int maxDepth = DEFAULT_MAX_DEPTH) => new QueryOptions() { MaxDetph = maxDepth };
        public static QueryOptions NoLoops(int maxDepth = DEFAULT_MAX_DEPTH) => new QueryOptions() { MaxDetph = maxDepth, AllowLoops = false };

        /// <summary>
        /// Max number of vertexes in the path (including the starting one)
        /// </summary>
        public int MaxDetph { get; set; } = DEFAULT_MAX_DEPTH;

        /// <summary>
        /// Allow multiple usage of a same vertex in a path
        /// </summary>
        public bool AllowLoops { get; set; } = true;

        /// <summary>
        /// Allow multiple usage of a same edge in a path
        /// </summary>
        public bool AllowEdgeMultiUsage { get; set; } = false;

    }
}
