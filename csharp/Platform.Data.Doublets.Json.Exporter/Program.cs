using TLink = System.UInt64;

namespace Platform.Data.Doublets.Json.Exporter
{
    class Program
    {
        static void Main(params string[] args)
        {
            new JsonExporterCLI<TLink>().Run(args);
        }
    }
}