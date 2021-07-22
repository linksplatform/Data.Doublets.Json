using Platform.Data.Doublets.Json;
using TLink = System.UInt64;

namespace JsonImporter
{
    class Program
    {
        static void Main(params string[] args)
        {
            new JsonImporterCLI<TLink>().Run(args);
        }
    }
}