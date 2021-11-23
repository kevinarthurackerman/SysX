using System;
using System.Collections.Generic;
using System.Linq;

namespace Sysx.Linq
{
    public static class Enumerate
    {
        public static IEnumerable<T> Descendants<T>(this T root, Func<T, IEnumerable<T>> childSelector, bool includeRoot = false)
        {
            var descendantsChecked = new HashSet<T>();
            var descendantsToCheck = new Stack<T>();

            if(includeRoot)
            {
                descendantsToCheck.Push(root);
            }
            else
            {
                var children = childSelector(root);

                foreach (var child in children)
                    descendantsToCheck.Push(child);
            }

            while (descendantsToCheck.Any())
            {
                var parent = descendantsToCheck.Pop();

                if (!descendantsChecked.Add(parent)) continue;

                var children = childSelector(parent);

                foreach (var child in children)
                    descendantsToCheck.Push(child);

                yield return parent;
            }
        }

        public static IEnumerable<T> Ancestors<T>(this T root, Func<T, T> ancestorSelector, bool includeRoot = false)
        {
            if (includeRoot)
            {
                yield return root;
            }

            var ancestor = root;

            while (ancestor != null)
            {
                ancestor = ancestorSelector(ancestor);

                yield return ancestor;
            }
        }
    }
}
