using System.Collections.Generic;
using System.Linq;
using DriverMatching.Models;

namespace DriverMatching.Algorithms;

public class SpatialGridAlgorithm : IMatchingAlgorithm
{
    public string Name => "SpatialGrid (Bucketing)";
    private readonly int _cellSize;
    private Dictionary<(int, int), List<Driver>> _grid = new();

    public SpatialGridAlgorithm(int cellSize = 100) => _cellSize = cellSize;

    public void Initialize(IReadOnlyList<Driver> drivers)
    {
        _grid.Clear();
        foreach (var d in drivers)
        {
            var key = (d.X / _cellSize, d.Y / _cellSize);
            if (!_grid.ContainsKey(key)) _grid[key] = new List<Driver>();
            _grid[key].Add(d);
        }
    }

    public IReadOnlyList<Driver> FindNearest(OrderLocation order, int count)
    {
        var candidates = new List<(Driver Driver, long DistSq)>();
        int ring = 0;
        long maxDistSq = long.MaxValue;

        while (candidates.Count < count || Math.Sqrt(maxDistSq) > (ring + 1) * _cellSize)
        {
            int cx = order.X / _cellSize;
            int cy = order.Y / _cellSize;

            for (int dx = -ring; dx <= ring; dx++)
            {
                for (int dy = -ring; dy <= ring; dy++)
                {
                    if (Math.Abs(dx) < ring && Math.Abs(dy) < ring) continue;
                    var key = (cx + dx, cy + dy);
                    if (_grid.TryGetValue(key, out var bucket))
                    {
                        foreach (var d in bucket)
                        {
                            long distSq = (long)(d.X - order.X) * (d.X - order.X) + (long)(d.Y - order.Y) * (d.Y - order.Y);
                            if (distSq < maxDistSq || candidates.Count < count)
                                candidates.Add((d, distSq));
                        }
                    }
                }
            }
            candidates = candidates
                .OrderBy(c => c.DistSq)
                .ThenBy(c => c.Driver.Id)
                .ToList();
            if (candidates.Count >= count) maxDistSq = candidates[count - 1].DistSq;
            ring++;
        }

        return candidates.Take(count).Select(c => c.Driver).ToList();
    }
}