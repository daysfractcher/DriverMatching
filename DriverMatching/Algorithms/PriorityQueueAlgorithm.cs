using System.Collections.Generic;
using System.Linq;
using DriverMatching.Models;

namespace DriverMatching.Algorithms;

public class PriorityQueueAlgorithm : IMatchingAlgorithm
{
    public string Name => "MaxHeap (PriorityQueue)";
    private IReadOnlyList<Driver> _drivers = Array.Empty<Driver>();

    public void Initialize(IReadOnlyList<Driver> drivers) => _drivers = drivers;

    public IReadOnlyList<Driver> FindNearest(OrderLocation order, int count)
    {
        if (count <= 0) return new List<Driver>();

        var comparer = Comparer<(long Dist, string Id)>.Create((a, b) =>
        {
            int distCompare = b.Dist.CompareTo(a.Dist);
            if (distCompare != 0) return distCompare;
            return b.Id.CompareTo(a.Id);
        });

        var pq = new PriorityQueue<Driver, (long Dist, string Id)>(comparer);

        foreach (var d in _drivers)
        {
            long distSq = (long)(d.X - order.X) * (d.X - order.X) +
                          (long)(d.Y - order.Y) * (d.Y - order.Y);

            var priority = (Dist: distSq, Id: d.Id);

            if (pq.Count < count)
            {
                pq.Enqueue(d, priority);
            }
            else if (pq.Count > 0)
            {
                var worstEntry = pq.UnorderedItems.MaxBy(x => x.Priority);

                if (priority.Dist < worstEntry.Priority.Dist ||
                    (priority.Dist == worstEntry.Priority.Dist && priority.Id.CompareTo(worstEntry.Priority.Id) < 0))
                {
                    pq.Enqueue(d, priority);
                    pq.Dequeue();
                }
            }
        }

        return pq.UnorderedItems
            .OrderBy(x => x.Priority.Dist)
            .ThenBy(x => x.Priority.Id)
            .Select(x => x.Element)
            .ToList();
    }
}