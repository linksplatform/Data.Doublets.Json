using System;
using System.Diagnostics;
using System.IO;
using System.Text.Encodings.Web;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.IO;
using System.Text.Json;
using System.Text.Unicode;
using Platform.Data.Doublets.Memory;
using Platform.Memory;

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
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            Utf8JsonWriter utf8JsonWriter = new(jsonFileStream, utf8JsonWriterOptions);
            var linksConstants = new LinksConstants<TLink>(enableExternalReferencesSupport: true);
            using UnitedMemoryLinks<TLink> memoryAdapter = new (new FileMappedResizableDirectMemory(linksFilePath), UnitedMemoryLinks<TLink>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
            var links = memoryAdapter.DecorateWithAutomaticUniquenessAndUsagesResolution();
            var storage = new DefaultJsonStorage<TLink>(links);
            var exporter = new JsonExporter<TLink>(storage);
            var document = storage.GetDocumentOrDefault(documentName);
            if (storage.EqualityComparer.Equals(document, default))
            {
                Console.WriteLine("No document with this name.");
            }
            using ConsoleCancellation cancellation = new ();
            var cancellationToken = cancellation.Token;
            Console.WriteLine("Press CTRL+C to stop.");
            try
            {
                exporter.Export(document, ref utf8JsonWriter, in cancellationToken);
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