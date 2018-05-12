using Microsoft.CSharp.RuntimeBinder;
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
        public void GetPath_NoLoop()
        {
            var g = Assets.GetTinyLoopGraph();
            _output.WriteLine(g.GetDot());

            {
                var paths = g.GetPaths("a", "a1", 3).ToArray();
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
                    3
                    ).ToArray();

                foreach (var p in paths)
                {
                    _output.WriteLine(p.Print());
                }
            }
        }
    }
}
