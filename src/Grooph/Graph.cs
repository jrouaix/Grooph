﻿using Grooph.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Grooph
{
    public class Graph
    {
        readonly Dictionary<Id, Vertex> _vertexes = new Dictionary<Id, Vertex>();
        readonly Dictionary<Id, Edge> _edges = new Dictionary<Id, Edge>();
        readonly Dictionary<Id, HashSet<Id>> _outbounds = new Dictionary<Id, HashSet<Id>>();

        #region Upsert
        public Id UpsertVertex(string collection, string key, object value)
        {
            var id = new Id(collection, key);
            return UpsertVertex(id, value);
        }

        public Id UpsertVertex(Id id, object value)
        {
            _vertexes[id] = new Vertex(id, value);
            return id;
        }

        public Id UpsertEdge(string collection, string key, object value, string fromCollection, string fromKey, string toCollection, string toKey)
            => UpsertEdge(new Id(collection, key), value, new Id(fromCollection, fromKey), new Id(toCollection, toKey));

        public Id UpsertEdge(Id id, object value, Id from, Id to)
        {
            _edges[id] = new Edge(id, value, from, to);

            if (!_outbounds.TryGetValue(from, out var set))
            {
                set = new HashSet<Id> { id };
                _outbounds[from] = set;
            }
            else
            {
                if (!set.Contains(id)) set.Add(id);
            }

            return id;
        }
        #endregion

        #region Get
        public T GetVertex<T>(string collection, string key)
            where T : class
        {
            var id = new Id(collection, key);
            return GetVertex<T>(id);
        }

        T GetVertex<T>(Id id) where T : class
        {
            return (T)InternalGetVertex(id)?.Value ?? default;
        }

        Vertex InternalGetVertex(Id id)
        {
            return _vertexes.TryGetValue(id, out var vertex)
                ? vertex
                : default;
        }

        public T GetEdge<T>(string collection, string key) where T : class
        {
            var id = new Id(collection, key);
            return GetEdge<T>(id);
        }

        T GetEdge<T>(Id id) where T : class
        {
            return (T)InternalGetEdge(id)?.Value ?? default;
        }

        Edge InternalGetEdge(Id id)
        {
            return _edges.TryGetValue(id, out var edge)
                ? edge
                : default;
        }
        #endregion

        #region Select
        public IEnumerable<Vertex> Vertexes { get => _vertexes.Values; }
        public IEnumerable<Edge> Edges { get => _edges.Values; }
        #endregion

        #region Merge
        public Id MergeVertex<T>(string collection, string key, Func<T, T> mergeFunction) where T : class
        {
            var value = GetVertex<T>(collection, key);
            value = mergeFunction(value);
            return UpsertVertex(collection, key, value);
        }

        public Id MergeEdge<T>(string collection, string key, Func<T, T> mergeFunction, string fromCollection, string fromKey, string toCollection, string toKey)
            where T : class
            => MergeEdge<T>(new Id(collection, key), mergeFunction, new Id(fromCollection, fromKey), new Id(toCollection, toKey));

        Id MergeEdge<T>(Id id, Func<T, T> mergeFunction, Id from, Id to)
            where T : class
        {
            var value = GetEdge<T>(id);
            value = mergeFunction(value);
            return UpsertEdge(id, value, from, to);
        }
        #endregion

        #region Get Paths
        public IEnumerable<IPath> GetPaths(string collection, string key, QueryOptions options = null)
           => GetPaths(new Id(collection, key), (true, true), (_state, _value) => (true, (true, true)), options);

        public IEnumerable<IPath<TSeed>> GetPaths<TSeed>(string collection, string key, TSeed seed, Func<TSeed, object, (bool, TSeed)> filter = null, QueryOptions options = null)
            where TSeed : struct
            => GetPaths(new Id(collection, key), seed, filter, options);

        IEnumerable<Path<TSeed>> GetPaths<TSeed>(Id from, TSeed seed, Func<TSeed, object, (bool, TSeed)> filter = null, QueryOptions options = null)
            where TSeed : struct
        {
            if (filter == null) filter = (_state, _value) => (true, default);
            if (options == null) options = QueryOptions.Default();

            var vertexes = new Stack<Vertex>();
            var edges = new Stack<Edge>();
            var seeds = new Stack<TSeed>();

            foreach (var path in InternalGetPaths(from, vertexes, edges, seeds, 1, options, seed, filter))
            {
                yield return path;
            }
        }

        private IEnumerable<Path<TSeed>> InternalGetPaths<TSeed>(Id currentVertex, Stack<Vertex> vertexes, Stack<Edge> edges, Stack<TSeed> seeds, int currentDetph, QueryOptions options, TSeed seed, Func<TSeed, object, (bool, TSeed)> filter)
            where TSeed : struct
        {
            var vertex = InternalGetVertex(currentVertex);
            var (ok_vertex, seed_vertex) = filter(seed, vertex.Value);
            if (!ok_vertex) yield break;

            seed = seed_vertex;

            vertexes.Push(vertex);
            seeds.Push(seed);
            yield return new Path<TSeed>(vertexes.Reverse().ToArray(), edges.Reverse().ToArray(), seeds.Reverse().ToArray(), currentDetph);

            if (currentDetph != options.MaxDetph)
            {
                currentDetph++;

                if (_outbounds.TryGetValue(currentVertex, out var outbound))
                    foreach (var edgeId in outbound)
                    {
                        if (!options.AllowEdgeMultiUsage && edges.Any(e => e.Id == edgeId)) continue; // 1 use of an edge in a path

                        var edge = InternalGetEdge(edgeId);
                        var (ok_edge, seed_edge) = filter(seed, edge.Value);
                        if (!ok_edge) continue;

                        if (!options.AllowLoops && vertexes.Any(v => v.Id == edge.To)) continue; // 1 use of a vertex in a path

                        edges.Push(edge);
                        seeds.Push(seed_edge);
                        foreach (var path in InternalGetPaths(edge.To, vertexes, edges, seeds, currentDetph, options, seed_edge, filter))
                        {
                            yield return path;
                        }
                        seeds.Pop();
                        edges.Pop();
                    }
            }
            seeds.Pop();
            vertexes.Pop();
        }
        #endregion

    }
}
