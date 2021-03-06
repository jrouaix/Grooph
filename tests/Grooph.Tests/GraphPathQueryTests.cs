﻿using Microsoft.CSharp.RuntimeBinder;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Grooph.Tests
{
    public class GraphPathQueryTests
    {
        readonly ITestOutputHelper _output;

        public GraphPathQueryTests(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void GetPath_Simple()
        {
            var g = new Graph();

            _output.WriteLine("Insert A1");
            g.UpsertVertex("a", "a1", "A1");
            {
                var paths = g.GetPaths("a", "a1").ToArray();
                paths.Length.ShouldBe(1);
                var p0 = paths[0];
                p0.Depth.ShouldBe(1);
                p0.Vertexes.Length.ShouldBe(1);
                p0.Edges.Length.ShouldBe(0);
                p0.Vertexes[0].Value.ShouldBe("A1");
            }

            _output.WriteLine("Insert A2");
            g.UpsertVertex("a", "a2", "A2");
            {
                var paths = g.GetPaths("a", "a1").ToArray();
                paths.Length.ShouldBe(1);
                var p0 = paths[0];
                p0.Depth.ShouldBe(1);
                p0.Vertexes.Length.ShouldBe(1);
                p0.Edges.Length.ShouldBe(0);
                p0.Vertexes[0].Value.ShouldBe("A1");
            }

            _output.WriteLine("Insert X1(A1 -> A2)");
            g.UpsertEdge("x", "x1", "X1", "a", "a1", "a", "a2");
            {
                var paths = g.GetPaths("a", "a1").ToArray();
                paths.Length.ShouldBe(2);
                var p0 = paths[0];
                _output.WriteLine(p0.Print());
                p0.Depth.ShouldBe(1);
                p0.Vertexes.Length.ShouldBe(1);
                p0.Edges.Length.ShouldBe(0);
                p0.Vertexes[0].Value.ShouldBe("A1");
                var p1 = paths[1];
                _output.WriteLine(p1.Print());
                p1.Depth.ShouldBe(2);
                p1.Vertexes.Length.ShouldBe(2);
                p1.Edges.Length.ShouldBe(1);
                p1.Vertexes[0].Value.ShouldBe("A1");
                p1.Vertexes[1].Value.ShouldBe("A2");
                p1.Edges[0].Value.ShouldBe("X1");
            }

            _output.WriteLine("Insert X2(A1 -> A2)");
            g.UpsertEdge("x", "x2", "X2", "a", "a1", "a", "a2");
            {
                var paths = g.GetPaths("a", "a1").ToArray();
                paths.Length.ShouldBe(3);
                var p0 = paths[0];
                _output.WriteLine(p0.Print());
                p0.Depth.ShouldBe(1);
                p0.Vertexes.Length.ShouldBe(1);
                p0.Edges.Length.ShouldBe(0);
                p0.Vertexes[0].Value.ShouldBe("A1");
                var p1 = paths[1];
                _output.WriteLine(p1.Print());
                p1.Depth.ShouldBe(2);
                p1.Vertexes.Length.ShouldBe(2);
                p1.Edges.Length.ShouldBe(1);
                p1.Vertexes[0].Value.ShouldBe("A1");
                p1.Vertexes[1].Value.ShouldBe("A2");
                p1.Edges[0].Value.ShouldBe("X1");
                var p2 = paths[2];
                _output.WriteLine(p2.Print());
                p2.Depth.ShouldBe(2);
                p2.Vertexes.Length.ShouldBe(2);
                p2.Edges.Length.ShouldBe(1);
                p2.Vertexes[0].Value.ShouldBe("A1");
                p2.Vertexes[1].Value.ShouldBe("A2");
                p2.Edges[0].Value.ShouldBe("X2");
            }

            _output.WriteLine("Insert Z1(A2 -> A1)");
            g.UpsertEdge("z", "z1", "Z1", "a", "a2", "a", "a1");
            {
                var paths = g.GetPaths("a", "a1", 5).ToArray();
                paths.Length.ShouldBe(3);
                var p0 = paths[0];
                _output.WriteLine(p0.Print());
                p0.Depth.ShouldBe(1);
                p0.Vertexes.Length.ShouldBe(1);
                p0.Edges.Length.ShouldBe(0);
                p0.Vertexes[0].Value.ShouldBe("A1");
                var p1 = paths[1];
                _output.WriteLine(p1.Print());
                p1.Depth.ShouldBe(2);
                p1.Vertexes.Length.ShouldBe(2);
                p1.Edges.Length.ShouldBe(1);
                p1.Vertexes[0].Value.ShouldBe("A1");
                p1.Vertexes[1].Value.ShouldBe("A2");
                p1.Edges[0].Value.ShouldBe("X1");
                var p2 = paths[2];
                _output.WriteLine(p2.Print());
                p2.Depth.ShouldBe(2);
                p2.Vertexes.Length.ShouldBe(2);
                p2.Edges.Length.ShouldBe(1);
                p2.Vertexes[0].Value.ShouldBe("A1");
                p2.Vertexes[1].Value.ShouldBe("A2");
                p2.Edges[0].Value.ShouldBe("X2");
            }
            {
                var paths = g.GetPaths("a", "a2").ToArray();
                paths.Length.ShouldBe(2);
                var p0 = paths[0];
                _output.WriteLine(p0.Print());
                p0.Depth.ShouldBe(1);
                p0.Vertexes.Length.ShouldBe(1);
                p0.Edges.Length.ShouldBe(0);
                p0.Vertexes[0].Value.ShouldBe("A2");
                var p1 = paths[1];
                _output.WriteLine(p1.Print());
                p1.Depth.ShouldBe(2);
                p1.Vertexes.Length.ShouldBe(2);
                p1.Edges.Length.ShouldBe(1);
                p1.Vertexes[0].Value.ShouldBe("A2");
                p1.Vertexes[1].Value.ShouldBe("A1");
                p1.Edges[0].Value.ShouldBe("Z1");
            }
        }

        [Fact]
        public void GetPath_LoopNoLoop()
        {
            var g = Assets.GetTinyLoopGraph();
            _output.WriteLine(g.GetDot());

            {
                var paths = g.GetPaths("a", "a1", options: QueryOptions.Default(4)).ToArray();
                paths.Length.ShouldBe(3);
                var p0 = paths[0];
                _output.WriteLine(p0.Print());
                p0.Depth.ShouldBe(1);
                p0.Vertexes.Length.ShouldBe(1);
                p0.Edges.Length.ShouldBe(0);
                p0.Vertexes[0].Value.ShouldBe("A1");
                var p1 = paths[1];
                _output.WriteLine(p1.Print());
                p1.Depth.ShouldBe(2);
                p1.Vertexes.Length.ShouldBe(2);
                p1.Edges.Length.ShouldBe(1);
                p1.Vertexes[0].Value.ShouldBe("A1");
                p1.Edges[0].Value.ShouldBe("X1");
                p1.Vertexes[1].Value.ShouldBe("A2");
                var p2 = paths[2];
                _output.WriteLine(p2.Print());
                p2.Depth.ShouldBe(3);
                p2.Vertexes.Length.ShouldBe(3);
                p2.Edges.Length.ShouldBe(2);
                p2.Vertexes[0].Value.ShouldBe("A1");
                p2.Edges[0].Value.ShouldBe("X1");
                p2.Vertexes[1].Value.ShouldBe("A2");
                p2.Edges[1].Value.ShouldBe("Z1");
                p2.Vertexes[2].Value.ShouldBe("A1");

            }
            {
                var paths = g.GetPaths("a", "a2", options: QueryOptions.Default(2)).ToArray();
                paths.Length.ShouldBe(2);
                var p0 = paths[0];
                _output.WriteLine(p0.Print());
                p0.Depth.ShouldBe(1);
                p0.Vertexes.Length.ShouldBe(1);
                p0.Edges.Length.ShouldBe(0);
                p0.Vertexes[0].Value.ShouldBe("A2");
                var p1 = paths[1];
                _output.WriteLine(p1.Print());
                p1.Depth.ShouldBe(2);
                p1.Vertexes.Length.ShouldBe(2);
                p1.Edges.Length.ShouldBe(1);
                p1.Vertexes[0].Value.ShouldBe("A2");
                p1.Vertexes[1].Value.ShouldBe("A1");
                p1.Edges[0].Value.ShouldBe("Z1");
            }
            {
                var paths = g.GetPaths("a", "a2", options: QueryOptions.NoLoops(10)).ToArray();
                paths.Length.ShouldBe(2);
                var p0 = paths[0];
                _output.WriteLine(p0.Print());
                p0.Depth.ShouldBe(1);
                p0.Vertexes.Length.ShouldBe(1);
                p0.Edges.Length.ShouldBe(0);
                p0.Vertexes[0].Value.ShouldBe("A2");
                var p1 = paths[1];
                _output.WriteLine(p1.Print());
                p1.Depth.ShouldBe(2);
                p1.Vertexes.Length.ShouldBe(2);
                p1.Edges.Length.ShouldBe(1);
                p1.Vertexes[0].Value.ShouldBe("A2");
                p1.Vertexes[1].Value.ShouldBe("A1");
                p1.Edges[0].Value.ShouldBe("Z1");
            }
        }


        [Fact]
        public void GetPath_ComplexQuery()
        {
            var g = Assets.GetSimpleGraph();
            _output.WriteLine(g.GetDot());

            {
                var START = 100m;

                var paths = g.GetPaths(
                    "a", "a1",
                    (sum: START, mult: START),
                    (seed, item) =>
                    {
                        var d = (dynamic)item;
                        if (!d.enabled) return (false, (0, 0));

                        try
                        {
                            var value = d.value;
                            return (true, (seed.sum + value, seed.mult));
                        }
                        catch (RuntimeBinderException) { }

                        try
                        {
                            var change = d.change;
                            return (true, (seed.sum, seed.mult * change));
                        }
                        catch (RuntimeBinderException) { }

                        return (false, seed);
                    },
                    QueryOptions.Default(3)
                    ).ToArray();

                foreach (var p in paths)
                {
                    _output.WriteLine(p.Print());
                }
            }
        }
    }
}
