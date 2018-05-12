using System;
using System.Collections.Generic;
using System.Text;
using Grooph.Model;

namespace Grooph
{
    public static class DotExtensions
    {
        /// <summary>
        /// Dot format printout
        /// http://viz-js.com/
        /// </summary>
        /// <param name="graph">Graph</param>
        /// <param name="toString">ToString() override if given</param>
        /// <returns>dot graph format visualisation</returns>
        public static string GetDot(this Graph graph, Func<object, string> toString = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine("digraph {");

            toString = toString ?? (o =>
            {
                if (o == null) return string.Empty;
                switch (o)
                {
                    case Vertex v: return v.Id.ToString();
                    case Edge v: return v.Id.ToString();
                    default: return o.ToString();
                }
            });

            string escape(object value) => value.ToString().Replace("\"", "\"\"");

            foreach (var vertex in graph.Vertexes)
            {
                sb.AppendLine($"    \"{escape(vertex.Id)}\";");
            }

            foreach (var edge in graph.Edges)
            {
                sb.AppendLine($"    \"{escape(edge.From)}\" -> \"{escape(edge.To)}\" [label=\"{escape(edge.Id)}\"];");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
