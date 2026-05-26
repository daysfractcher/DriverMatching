using System.Collections.Generic;
using System.Linq;
using DriverMatching.Algorithms;
using DriverMatching.Models;
using NUnit.Framework;

namespace DriverMatching.Tests;

[TestFixture]
public class MatchingAlgorithmTests
{
    private IMatchingAlgorithm[] _algorithms;
    private List<Driver> _drivers;
    private OrderLocation _order;

    [SetUp]
    public void Setup()
    {
        _drivers = new List<Driver>
        {
            new("D1", 0, 0), new("D2", 10, 10), new("D3", 5, 5),
            new("D4", 2, 2), new("D5", 8, 8), new("D6", 1, 1),
            new("D7", 100, 100), new("D8", 3, 3)
        };
        _order = new OrderLocation(4, 4);

        _algorithms = new IMatchingAlgorithm[]
        {
            new BruteForceAlgorithm(),
            new PriorityQueueAlgorithm(),
            new SpatialGridAlgorithm(5)
        };

        foreach (var alg in _algorithms)
            alg.Initialize(_drivers);
    }

    [Test]
    public void AllAlgorithmsReturnSameTop5Drivers()
    {
        var results = _algorithms
            .Select(a => a.FindNearest(_order, 5).Select(d => d.Id).OrderBy(id => id).ToList())
            .ToList();

        for (int i = 1; i < results.Count; i++)
            Assert.That(results[i], Is.EqualTo(results[0]), $"Алгоритм {_algorithms[i].Name} вернул другой результат");
    }

    [Test]
    public void ReturnsExactlyFiveWhenAvailable()
    {
        foreach (var alg in _algorithms)
        {
            var res = alg.FindNearest(_order, 5);
            Assert.That(res, Has.Count.EqualTo(5), alg.Name);
        }
    }

    [Test]
    public void HandlesLessDriversThanRequested()
    {
        foreach (var alg in _algorithms)
        {
            alg.Initialize(new List<Driver> { new("A", 1, 1), new("B", 2, 2) });
            var res = alg.FindNearest(_order, 5);
            Assert.That(res, Has.Count.EqualTo(2), alg.Name);
        }
    }

    [Test]
    public void CorrectOrderingByDistance()
    {
        var res = _algorithms[0].FindNearest(_order, 5);
        Assert.That(res.Select(d => d.Id).ToArray(), Is.EqualTo(new[] { "D3", "D8", "D4", "D6", "D1" }));
    }
}