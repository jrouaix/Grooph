using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Grooph.Tests
{
    public class MermaidExtensionsTests
    {
        private readonly ITestOutputHelper _output;

        public MermaidExtensionsTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SimpleGraph()
        {
            var g = Assets.GetSimpleGraph();
            var dot = g.GetMermaid();
            _output.WriteLine(dot);
            dot.ShouldBe(
@"graph TD;
    a/a1;
    a/a2;
    a/a3;
    a/a4;
    a/a5;
    a/a1 -->|x/x12| a/a2;
    a/a1 -->|x/x12'| a/a2;
    a/a2 -->|x/x13| a/a3;
    a/a2 -->|x/x24| a/a4;
    a/a4 -->|x/x45| a/a5;
");
        }

        [Fact]
        public void TinyLoopGraph()
        {
            var g = Assets.GetTinyLoopGraph();
            var dot = g.GetMermaid();
            _output.WriteLine(dot);
            dot.ShouldBe(
@"graph TD;
    a/a1;
    a/a2;
    a/a1 -->|x/x1| a/a2;
    a/a2 -->|z/z1| a/a1;
");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(100)]
        public void GenerateLinearGraph(int length)
        {
            var graph = Assets.GenerateLinearGraph(length, i => i.ToString());
            var dot = graph.GetMermaid();
            _output.WriteLine(dot);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(100)]
        public void GenerateHeavyClusterGraph(int vertexCount)
        {
            var graph = Assets.GenerateHeavyClusterGraph(vertexCount, i => i.ToString());
            var dot = graph.GetMermaid();
            _output.WriteLine(dot);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(100)]
        [InlineData(1000)]
        public void GenerateRandomGraph(int vertexCount)
        {
            var graph = Assets.GenerateRandomGraph(vertexCount, i => i.ToString());
            var dot = graph.GetMermaid();
            _output.WriteLine(dot);
        }

    }
}
