using System;
using System.IO;
using System.Text.Encodings.Web;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.IO;
using System.Text.Json;
using System.Text.Unicode;

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
                Console.WriteLine($"${linksFilePath} file does not exist.");
            }
            using FileStream jsonFileStream = new(jsonFilePath, FileMode.Append);
            JsonWriterOptions utf8JsonWriterOptions = new()
            {
                Indented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            Utf8JsonWriter utf8JsonWriter = new(jsonFileStream, utf8JsonWriterOptions);
            using UnitedMemoryLinks<TLink> memoryAdapter = new (linksFilePath);
            var links = memoryAdapter.DecorateWithAutomaticUniquenessAndUsagesResolution();
            var storage = new DefaultJsonStorage<TLink>(links);
            var exporter = new JsonExporter<TLink>(storage);
            using ConsoleCancellation cancellation = new ();
            var document = storage.GetDocumentOrDefault(documentName);
            if (storage.EqualityComparer.Equals(document, default))
            {
                Console.WriteLine("No document with this name.");
            }
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
            Console.WriteLine("Export completed successfully.");
        }
    }
}