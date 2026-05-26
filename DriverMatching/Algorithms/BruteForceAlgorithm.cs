using System.Collections.Generic;
using System.Linq;
using DriverMatching.Models;

namespace DriverMatching.Algorithms;

public class BruteForceAlgorithm : IMatchingAlgorithm
{
    public string Name => "BruteForce (LINQ OrderBy)";
    private IReadOnlyList<Driver> _drivers = Array.Empty<Driver>();

    public void Initialize(IReadOnlyList<Driver> drivers) => _drivers = drivers;

    public IReadOnlyList<Driver> FindNearest(OrderLocation order, int count)
    {
        return _drivers
            .Select(d => new { Driver = d, DistSq = (long)(d.X - order.X) * (d.X - order.X) + (long)(d.Y - order.Y) * (d.Y - order.Y) })
            .OrderBy(x => x.DistSq)
            .ThenBy(x => x.Driver.Id)
            .Take(count)
            .Select(x => x.Driver)
            .ToList();
    }

}
