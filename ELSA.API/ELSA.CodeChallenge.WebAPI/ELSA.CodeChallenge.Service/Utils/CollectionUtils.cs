namespace ELSA.Services.Utils
{
    internal static class CollectionUtils
    {
        internal static bool EqualsWithoutOrdering<T>(this IEnumerable<T> source, IEnumerable<T> destination)
        {
            if (source.Count() != destination.Count())
            {
                return false;
            }
            var orderedSource = source.OrderBy(d => d).ToArray();
            var orderedDestination = destination.OrderBy(d => d).ToArray();
            return orderedSource.SequenceEqual(orderedDestination);
        }
    }
}
