using Grooph.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grooph
{
    public static class MermaidExtensions
    {
        /// <summary>
        /// Mermaid printout
        /// https://mermaidjs.github.io/
        /// </summary>
        /// <param name="graph">Graph</param>
        /// <param name="toString">ToString() override if given</param>
        /// <returns>mermaid js format visualisation</returns>
        public static string GetMermaid(this Graph graph, Func<object, string> toString = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine("graph TD;");

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

            string escape(object value) => value.ToString();

            foreach (var vertex in graph.Vertexes)
            {
                sb.AppendLine($"    {escape(vertex.Id)};");
            }

            foreach (var edge in graph.Edges)
            {
                sb.AppendLine($"    {escape(edge.From)} -->|{escape(edge.Id)}| {escape(edge.To)};");
            }

            return sb.ToString();
        }
    }
}
