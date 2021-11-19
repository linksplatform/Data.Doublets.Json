using BenchmarkDotNet.Running;
using Platform.Data.Doublets.Json.Benchmarks;
using System;

namespace Platform.Numbers.Benchmarks
{
    static class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<GetObjectBenchmarks>();
        }
    }
}