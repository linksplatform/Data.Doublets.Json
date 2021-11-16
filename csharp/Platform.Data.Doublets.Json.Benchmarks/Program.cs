using BenchmarkDotNet.Running;
using Platform.Data.Doublets.Json.Benchmarks;

namespace Platform.Numbers.Benchmarks
{
    /// <summary>
    ///     <para>
    ///         Represents the program.
    ///     </para>
    ///     <para></para>
    /// </summary>
    internal static class Program
    {
        /// <summary>
        ///     <para>
        ///         Main.
        ///     </para>
        ///     <para></para>
        /// </summary>
        private static void Main()
        {
            BenchmarkRunner.Run<GetObjectBenchmarks>();
        }
    }
}
