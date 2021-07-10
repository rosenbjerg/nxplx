using System.Collections.Generic;

namespace NxPlx.Application.Services
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IReadOnlyCollection<T>> Batch<T>(this IEnumerable<T> objects, int batchSize)
        {
            var currentBatch = new List<T>(batchSize);
            foreach (var obj in objects)
            {
                currentBatch.Add(obj);

                if (currentBatch.Count == batchSize)
                {
                    yield return currentBatch.ToArray();
                    currentBatch.Clear();
                }
            }
        }
    }
}