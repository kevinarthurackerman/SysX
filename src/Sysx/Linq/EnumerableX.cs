namespace Sysx.Linq;

public static class EnumerableX
{
    /// <summary>
    /// Returns the descendants of the root nearest to furthest, breadth first.
    /// </summary>
    public static IEnumerable<T> Descendants<T>(this T root, Func<T, IEnumerable<T>> childSelector, bool includeRoot = false, int maxDepth = int.MaxValue)
    {
        EnsureArg.HasValue(root, nameof(root));
        EnsureArg.IsNotNull(childSelector, nameof(childSelector));
        EnsureArg.IsInRange(maxDepth, 0, int.MaxValue, nameof(maxDepth));
            
        if (includeRoot) yield return root;

        var descendantsChecked = new HashSet<T>();
        var descendantsToCheck = new List<T>();
        var childrenBeingChecked = (IEnumerable<T>)childSelector(root).ToArray();
        var depth = 0;
        while (childrenBeingChecked.Any() && depth++ < maxDepth)
        {
            foreach (var childBeingChecked in childrenBeingChecked)
            {
                if (!descendantsChecked.Add(childBeingChecked)) continue;

                var children = childSelector(childBeingChecked).ToArray();

                descendantsToCheck.AddRange(children);

                yield return childBeingChecked;
            }

            childrenBeingChecked = descendantsToCheck;
            descendantsToCheck = new List<T>();
        }
    }
        
    /// <summary>
    /// Returns the ancestors of the root nearest to furthest.
    /// </summary>
    public static IEnumerable<T> Ancestors<T>(this T root, Func<T, T?> ancestorSelector, bool includeRoot = false, int maxDepth = int.MaxValue)
    {
        EnsureArg.HasValue(root, nameof(root));
        EnsureArg.IsNotNull(ancestorSelector, nameof(ancestorSelector));
        EnsureArg.IsInRange(maxDepth, 0, int.MaxValue, nameof(maxDepth));

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