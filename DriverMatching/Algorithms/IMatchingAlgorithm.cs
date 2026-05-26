using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverMatching.Models;

namespace DriverMatching.Algorithms;

public interface IMatchingAlgorithm
{
    string Name { get; }
    void Initialize(IReadOnlyList<Driver> drivers);
    IReadOnlyList<Driver> FindNearest(OrderLocation order, int count);
}