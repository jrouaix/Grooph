using Grooph.Model;
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


        /// <summary>
        /// digraph {
        ///     "a/a1";
        ///     "a/a2";
        ///     "a/a1" -> "a/a2" [label="x/x1"];
        ///     "a/a2" -> "a/a1" [label="z/z1"];
        /// }
        /// </summary>
        public static Graph GetTinyLoopGraph()
        {
            var g = new Graph();

            g.UpsertVertex("a", "a1", "A1");
            g.UpsertVertex("a", "a2", "A2");

            g.UpsertEdge("x", "x1", "X1", "a", "a1", "a", "a2");
            g.UpsertEdge("z", "z1", "Z1", "a", "a2", "a", "a1");

            return g;
        }


        /// <summary>
        /// Return a linear one path graph 
        /// 1 -> 2 -> 3 -> 4 ...
        /// </summary>
        /// <param name="length">Vertex count</param>
        /// <param name="vertexGenerator"></param>
        public static Graph GenerateLinearGraph(int length, Func<int, object> vertexGenerator)
        {
            var g = new Graph();

            for (int i = 0; i < length; i++)
            {
                var currentId = i.ToString();
                g.UpsertVertex("v", currentId, vertexGenerator(i));

                if (i == 0) continue;

                var previousId = (i - 1).ToString();
                var edgeId = $"{previousId}to{currentId}";
                g.UpsertEdge("e", edgeId, edgeId, "v", previousId, "v", currentId);
            }

            return g;

        }

        /// <summary>
        /// Generating graph: each new node is connected to all existing nodes. Heavy clusterization.
        /// </summary>
        /// <param name="length">Vertex count</param>
        /// <param name="vertexGenerator"></param>
        public static Graph GenerateHeavyClusterGraph(int vertexCount, Func<int, object> vertexGenerator)
        {
            var g = new Graph();

            for (int i = 0; i < vertexCount; i++)
            {
                var currentId = i.ToString();
                var id = g.UpsertVertex("v", currentId, vertexGenerator(i));

                var count = 0;
                foreach (var v in g.Vertexes)
                {
                    if (v.Id == id) continue;

                    g.UpsertEdge(new Id("e", $"{currentId}-{count++}"), currentId, id, v.Id);
                }
            }

            return g;
        }

        /// <summary>
        /// Generating graph: each new node is connected randomly to one of the first three elements. The result is a heavily centralized network.
        /// </summary>
        /// <param name="length">Vertex count</param>
        /// <param name="vertexGenerator"></param>
        public static Graph GenerateRandomGraph(int vertexCount, Func<int, object> vertexGenerator)
        {
            var g = new Graph();
            var randy = new Random();

            var previous = new List<Id>();
            for (int i = 0; i < vertexCount; i++)
            {
                var currentId = i.ToString();
                var id = g.UpsertVertex("v", currentId, vertexGenerator(i));

                if (i != 0)
                {
                    var connectTo = i == 1 ? 0 : randy.Next(i);
                    g.UpsertEdge(new Id("e", currentId), currentId, id, previous[connectTo]);
                }
                previous.Add(id);
            }

            return g;
        }
    }
}
