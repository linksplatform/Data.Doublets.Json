using TLink = System.UInt64;

namespace Platform.Data.Doublets.Json.Importer
{
    class Program
    {
        static void Main(params string[] args)
        {
            new JsonImporterCli<TLink>().Run(args);
        }
    }
}