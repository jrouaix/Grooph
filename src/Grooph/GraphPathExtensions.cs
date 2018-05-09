using Grooph.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Grooph.Graph;

namespace Grooph
{
    public static class GraphPathExtensions
    {
        const string DEFAULT_PATH_SEPARATOR = " -> ";

        public static IEnumerable<object> GetFullPathValues(this IPath path)
            => GetFullPath(path).Select(v => v.Value);

        public static IEnumerable<(object value, TSeed seed)> GetFullPathValues<TSeed>(this IPath<TSeed> path)
            where TSeed : struct
            => GetFullPath(path).Select(v => (v.value.Value, v.seed));

        public static IEnumerable<(IHaveValue value, TSeed seed)> GetFullPath<TSeed>(this IPath<TSeed> path)
            where TSeed : struct
        {
            var items = GetFullPath((IPath)path);
            var i = 0;
            foreach (var item in items)
            {
                var seed = path.Seeds[i];
                yield return (item, seed);
                i++;
            }
        }

        public static IEnumerable<IHaveValue> GetFullPath(this IPath path)
        {
            var depth = path.Depth;

            for (int i = 0; i < depth - 1; i++)
            {
                yield return path.Vertexes[i];
                yield return path.Edges[i];
            }

            yield return path.Vertexes[depth - 1];
        }

        public static string Print(this IPath path, bool withValue = true, string separator = DEFAULT_PATH_SEPARATOR, Func<object, string> toString = null)
        {
            var items = GetFullPath(path);
            toString = toString ?? (o => o.ToString());
            var strings = items.Select(i => withValue ? $"{i}{{{ toString(i.Value) }}}" : $"{i}");
            return string.Join(separator, strings);
        }


        public static string Print<TSeed>(this IPath<TSeed> path, bool withValue = true, bool withSeeds = true, string separator = DEFAULT_PATH_SEPARATOR, Func<object, string> toString = null)
            where TSeed : struct
        {
            var items = GetFullPath(path).ToArray();
            toString = toString ?? (o => o.ToString());

            var strings = items
                .Select(i =>
                {
                    var item = i.value;
                    var seed = i.seed;

                    return withValue
                        ? withSeeds
                            ? $"{item}{{{ toString(item.Value) }}}/{{{seed}}}"
                            : $"{item}{{{ toString(item.Value) }}}"
                        : withSeeds
                            ? $"{item}/{{{seed}}}"
                            : $"{item}";
                });

            return string.Join(separator, strings);
        }
    }
}
