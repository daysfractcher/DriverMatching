using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using DriverMatching.Algorithms;
using DriverMatching.Models;

namespace DriverMatching.Benchmarks;

[MemoryDiagnoser]
public class Benchmarks
{
    private readonly List<Driver> _drivers = new();
    private readonly OrderLocation _order = new(500, 500);
    private readonly IMatchingAlgorithm[] _algorithms;

    public Benchmarks()
    {
        _algorithms = new IMatchingAlgorithm[]
        {
            new BruteForceAlgorithm(),
            new PriorityQueueAlgorithm(),
            new SpatialGridAlgorithm(50)
        };
    }

    [GlobalSetup]
    public void Setup()
    {
        var rand = new Random(42);
        for (int i = 0; i < 100_000; i++)
            _drivers.Add(new($"D{i}", rand.Next(0, 1000), rand.Next(0, 1000)));

        foreach (var alg in _algorithms) alg.Initialize(_drivers);
    }

    [Benchmark] public IReadOnlyList<Driver> BruteForce() => _algorithms[0].FindNearest(_order, 5);
    [Benchmark] public IReadOnlyList<Driver> MaxHeap() => _algorithms[1].FindNearest(_order, 5);
    [Benchmark] public IReadOnlyList<Driver> SpatialGrid() => _algorithms[2].FindNearest(_order, 5);

    public static void Main(string[] args) => BenchmarkRunner.Run<Benchmarks>();
}