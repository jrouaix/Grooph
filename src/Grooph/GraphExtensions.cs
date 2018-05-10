﻿using Grooph.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Grooph
{
    public static class GraphExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string N<T>() => typeof(T).Name;


        public static void UpsertVertex<T>(this Graph graph, string key, T value) where T : class => graph.UpsertVertex(N<T>(), key, value);

        public static T GetVertex<T>(this Graph graph, string key) where T : class => graph.GetVertex<T>(N<T>(), key);

        public static void MergeVertex<T>(this Graph graph, string key, Func<T, T> mergeFunction)
            where T : class => graph.MergeVertex<T>(typeof(T).Name, key, mergeFunction);

        public static void InsertVertexIfNotExists<T>(this Graph graph, string key, T value)
            where T : class => graph.MergeVertex<T>(N<T>(), key, t => t ?? value);



        public static void UpsertEdge<T, TFrom, TTo>(this Graph graph, string key, T value, string fromKey, string toKey)
            where T : class => graph.UpsertEdge(N<T>(), key, value, N<TFrom>(), fromKey, N<TTo>(), toKey);

        public static T GetEdge<T>(this Graph graph, string key) where T : class => graph.GetEdge<T>(N<T>(), key);

        public static void MergeEdge<T, TFrom, TTo>(this Graph graph, string key, Func<T, T> mergeFunction, string fromKey, string toKey)
            where T : class => graph.MergeEdge(N<T>(), key, mergeFunction, N<TFrom>(), fromKey, N<TTo>(), toKey);

        public static void InsertEdgeIfNotExists<T, TFrom, TTo>(this Graph graph, string key, T value, string fromKey, string toKey)
            where T : class => graph.MergeEdge<T>(N<T>(), key, t => t ?? value, N<TFrom>(), fromKey, N<TTo>(), toKey);




        /// <summary>
        /// Dot format printout
        /// http://viz-js.com/
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="toString"></param>
        /// <returns></returns>
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
                sb.AppendLine($"\"{escape(vertex.Id)}\";");
            }

            foreach (var edge in graph.Edges)
            {
                sb.AppendLine($"\"{escape(edge.From)}\" -> \"{escape(edge.To)}\" [label=\"{escape(edge.Id)}\"];");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
