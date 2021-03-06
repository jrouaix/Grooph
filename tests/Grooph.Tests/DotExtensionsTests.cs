﻿using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Grooph.Tests
{
    public class DotExtensionsTests
    {
        private readonly ITestOutputHelper _output;

        public DotExtensionsTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SimpleGraph()
        {
            var g = Assets.GetSimpleGraph();
            var dot = g.GetDot();
            _output.WriteLine(dot);
            dot.ShouldBe(
@"digraph {
    ""a/a1"";
    ""a/a2"";
    ""a/a3"";
    ""a/a4"";
    ""a/a5"";
    ""a/a1"" -> ""a/a2"" [label=""x/x12""];
    ""a/a1"" -> ""a/a2"" [label=""x/x12'""];
    ""a/a2"" -> ""a/a3"" [label=""x/x13""];
    ""a/a2"" -> ""a/a4"" [label=""x/x24""];
    ""a/a4"" -> ""a/a5"" [label=""x/x45""];
}
");
        }

        [Fact]
        public void TinyLoopGraph()
        {
            var g = Assets.GetTinyLoopGraph();
            var dot = g.GetDot();
            _output.WriteLine(dot);
            dot.ShouldBe(
@"digraph {
    ""a/a1"";
    ""a/a2"";
    ""a/a1"" -> ""a/a2"" [label=""x/x1""];
    ""a/a2"" -> ""a/a1"" [label=""z/z1""];
}
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
            var dot = graph.GetDot();
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
            var dot = graph.GetDot();
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
            var dot = graph.GetDot();
            _output.WriteLine(dot);
        }

    }
}
