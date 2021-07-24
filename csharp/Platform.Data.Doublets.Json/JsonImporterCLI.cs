using System;
using System.IO;
using System.Text;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.IO;
using System.Text.Json;
using Platform.Data.Doublets.Memory;
using Platform.Memory;

namespace Platform.Data.Doublets.Json
{
    public class JsonImporterCLI<TLink>
    {
        public void Run(params string[] args)
        {
            var jsonFilePath = ConsoleHelpers.GetOrReadArgument(0, "JSON file path", args);
            var linksFilePath = ConsoleHelpers.GetOrReadArgument(1, "Links file path", args);
            if (!File.Exists(jsonFilePath))
            {
                Console.WriteLine($"${jsonFilePath} file does not exist.");
            }
            var json = File.ReadAllText(jsonFilePath);
            var encodedJson = Encoding.UTF8.GetBytes(json);
            ReadOnlySpan<byte> readOnlySpanEncodedJson = new (encodedJson);
            Utf8JsonReader utf8JsonReader = new(readOnlySpanEncodedJson);
            var linksConstants = new LinksConstants<TLink>(enableExternalReferencesSupport: true);
            using UnitedMemoryLinks<TLink> memoryAdapter = new (new FileMappedResizableDirectMemory(linksFilePath), UnitedMemoryLinks<TLink>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
            var links = memoryAdapter.DecorateWithAutomaticUniquenessAndUsagesResolution();
            var storage = new DefaultJsonStorage<TLink>(links);
            var importer = new JsonImporter<TLink>(storage);
            var documentName = Path.GetFileName(jsonFilePath);
            using var cancellation = new ConsoleCancellation();
            var cancellationToken = cancellation.Token;
            Console.WriteLine("Press CTRL+C to stop.");
            try
            {
                importer.Import(documentName, ref utf8JsonReader, ref cancellationToken);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return;
            }
            Console.WriteLine("Import completed successfully.");
        }
    }
}