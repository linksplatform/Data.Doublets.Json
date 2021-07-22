using System;
using System.IO;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.IO;
using System.Text.Json;

namespace Platform.Data.Doublets.Json
{
    public class JsonExporterCLI<TLink>
    {
        public void Run(params string[] args)
        {
            var linksFilePath = ConsoleHelpers.GetOrReadArgument(0, "Links file path", args);
            var jsonFilePath = ConsoleHelpers.GetOrReadArgument(0, "JSON file path", args);
            if (!File.Exists(linksFilePath))
            {
                Console.WriteLine("Entered links file does not exist.");
            }
            if (!File.Exists(jsonFilePath))
            {
                Console.WriteLine("Entered JSON file does not exist.");
            }
            var documentName = Path.GetFileName(jsonFilePath);
            using FileStream jsonFileStream = new(jsonFilePath, FileMode.Append);
            Utf8JsonWriter utf8JsonWriter = new(jsonFileStream);
            using ConsoleCancellation cancellation = new ();
            using UnitedMemoryLinks<TLink> memoryAdapter = new (linksFilePath);
            var links = memoryAdapter.DecorateWithAutomaticUniquenessAndUsagesResolution();
            var storage = new DefaultJsonStorage<TLink>(links);
            var exporter = new JsonExporter<TLink>(storage);
            var document = storage.GetDocumentOrDefault(documentName);
            Console.WriteLine("Press CTRL+C to stop.");
            var cancellationToken = cancellation.Token;
            try
            {
                exporter.Export(document, ref utf8JsonWriter, ref cancellationToken);
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
            finally
            {
                utf8JsonWriter.Dispose();
            }
            if (cancellation.NotRequested)
            {
                Console.WriteLine("Export completed successfully.");
            }
        }
    }
}