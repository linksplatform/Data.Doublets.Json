using BenchmarkDotNet.Running;
using Platform.Data.Doublets.Json.Benchmarks;
using System;

namespace Platform.Numbers.Benchmarks
{
    /// <summary>
    /// <para>
    /// Represents the program.
    /// </para>
    /// <para></para>
    /// </summary>
    static class Program
    {
        /// <summary>
        /// <para>
        /// Main.
        /// </para>
        /// <para></para>
        /// </summary>
        static void Main()
        {
            BenchmarkRunner.Run<GetObjectBenchmarks>();
        }
    }
}