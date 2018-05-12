using System;
using System.Collections.Generic;
using System.Text;

namespace Grooph.Tests
{
    public static class Assets
    {
        /// <summary>
        /// digraph {
        ///     "a/a1";
        ///     "a/a2";
        ///     "a/a3";
        ///     "a/a4";
        ///     "a/a5";
        ///     "a/a1" -> "a/a2" [label="x/x12"];
        ///     "a/a1" -> "a/a2" [label="x/x12'"];
        ///     "a/a2" -> "a/a3" [label="x/x13"];
        ///     "a/a2" -> "a/a4" [label="x/x24"];
        ///     "a/a4" -> "a/a5" [label="x/x45"];
        /// }
        /// </summary>
        public static Graph GetSimpleGraph()
        {
            var g = new Graph();

            g.UpsertVertex("a", "a1", new { enabled = true, value = 1 });
            g.UpsertVertex("a", "a2", new { enabled = true, value = 2 });
            g.UpsertVertex("a", "a3", new { enabled = true, value = 3 });
            g.UpsertVertex("a", "a4", new { enabled = true, value = 4 });
            g.UpsertVertex("a", "a5", new { enabled = true, value = 5 });

            g.UpsertEdge("x", "x12", new { enabled = true, change = 1m }, "a", "a1", "a", "a2");
            g.UpsertEdge("x", "x12'", new { enabled = true, change = 0.5m }, "a", "a1", "a", "a2");
            g.UpsertEdge("x", "x13", new { enabled = false, change = 1m }, "a", "a2", "a", "a3");
            g.UpsertEdge("x", "x24", new { enabled = true, change = 1.1m }, "a", "a2", "a", "a4");
            g.UpsertEdge("x", "x45", new { enabled = true, change = 1.2m }, "a", "a4", "a", "a5");

            return g;
        }

        public static Graph GetTinyLoopGraph()
        {
            var g = new Graph();
            
            g.UpsertVertex("a", "a1", "A1");
            g.UpsertVertex("a", "a2", "A2");

            g.UpsertEdge("x", "x1", "X1", "a", "a1", "a", "a2");
            g.UpsertEdge("z", "z1", "Z1", "a", "a2", "a", "a1");
            
            return g;
        }
    }
}
