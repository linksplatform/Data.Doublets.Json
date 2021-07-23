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
            var jsonFilePath = ConsoleHelpers.GetOrReadArgument(1, "JSON file path", args);
            var documentName = ConsoleHelpers.GetOrReadArgument(2, "Document name", args);
            if (!File.Exists(linksFilePath))
            {
                Console.WriteLine("Entered links file does not exist.");
            }
            using FileStream jsonFileStream = new(jsonFilePath, FileMode.Append);
            JsonWriterOptions jsonWriterOptions = new() {Indented = true};
            Utf8JsonWriter utf8JsonWriter = new(jsonFileStream, jsonWriterOptions);
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
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return;
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