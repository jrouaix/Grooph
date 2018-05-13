using Microsoft.CSharp.RuntimeBinder;
using Shouldly;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Grooph.Tests
{
    public class GraphTests
    {
        readonly ITestOutputHelper _output;

        public GraphTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void UpsertVertex()
        {
            var g = new Graph();

            g.GetVertex<string>("a", "a1").ShouldBeNull();
            g.UpsertVertex("a", "a1", "A1");
            g.GetVertex<string>("a", "a1").ShouldBe("A1");
            g.UpsertVertex("a", "a1", "A2");
            g.GetVertex<string>("a", "a1").ShouldBe("A2");

            g.UpsertVertex("b", "a1", "B1");
            g.GetVertex<string>("b", "a1").ShouldBe("B1");
            g.GetVertex<string>("a", "a1").ShouldBe("A2");
            g.UpsertVertex("b", "a1", "B2");
            g.GetVertex<string>("b", "a1").ShouldBe("B2");
            g.GetVertex<string>("a", "a1").ShouldBe("A2");
        }

        [Fact]
        public void UpsertEdge()
        {
            var g = new Graph();

            g.GetEdge<string>("x", "x1").ShouldBeNull();
            g.UpsertEdge("x", "x1", "X1", "a", "a1", "a", "a2");
            g.GetEdge<string>("x", "x1").ShouldBe("X1");
            g.UpsertEdge("x", "x1", "X2", "a", "a1", "a", "a2");
            g.GetEdge<string>("x", "x1").ShouldBe("X2");

            g.UpsertEdge("z", "x1", "Z1", "a", "a1", "a", "a2");
            g.GetEdge<string>("z", "x1").ShouldBe("Z1");
            g.GetEdge<string>("x", "x1").ShouldBe("X2");
            g.UpsertEdge("z", "x1", "Z2", "a", "a1", "a", "a2");
            g.GetEdge<string>("z", "x1").ShouldBe("Z2");
            g.GetEdge<string>("x", "x1").ShouldBe("X2");
        }
    }
}