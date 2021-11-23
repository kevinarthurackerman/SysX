using System;
using System.Collections.Generic;
using System.Linq;

namespace Sysx.Linq
{
    public static class Enumerate
    {
        public static IEnumerable<T> Descendants<T>(this T root, Func<T, IEnumerable<T>> childSelector, bool includeRoot = false, int maxDepth = int.MaxValue)
        {
            if (includeRoot) yield return root;

            var descendantsChecked = new HashSet<T>();
            var descendantsToCheck = new List<T>();
            var childrenBeingChecked = childSelector(root);
            var depth = 0;

            while (childrenBeingChecked.Any() && depth++ < maxDepth)
            {
                foreach (var childBeingChecked in childrenBeingChecked)
                {
                    if (!descendantsChecked.Add(childBeingChecked)) continue;

                    var children = childSelector(childBeingChecked);

                    descendantsToCheck.AddRange(children);

                    yield return childBeingChecked;
                }

                childrenBeingChecked = descendantsToCheck;
                descendantsToCheck = new List<T>();
            }
        }

        public static IEnumerable<T> Ancestors<T>(this T root, Func<T, T> ancestorSelector, bool includeRoot = false, int maxDepth = int.MaxValue)
        {
            if (includeRoot) yield return root;

            var ancestor = ancestorSelector(root);
            var depth = 0;

            while (ancestor != null && depth++ < maxDepth)
            {
                yield return ancestor;

                ancestor = ancestorSelector(ancestor);
            }
        }
    }
}
