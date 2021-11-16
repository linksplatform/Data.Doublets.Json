using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Platform.Data.Doublets.Memory;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.IO;
using Platform.Memory;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Json
{
    /// <summary>
    ///     <para>
    ///         Represents the json importer cli.
    ///     </para>
    ///     <para></para>
    /// </summary>
    public class JsonImporterCli<TLink> where TLink : struct
    {
        /// <summary>
        ///     <para>
        ///         Runs the args.
        ///     </para>
        ///     <para></para>
        /// </summary>
        /// <param name="args">
        ///     <para>The args.</para>
        ///     <para></para>
        /// </param>
        public void Run(params string[] args)
        {
            var argumentIndex = 0;
            var jsonFilePath = ConsoleHelpers.GetOrReadArgument(argumentIndex++, "JSON file path", args);
            var linksFilePath = ConsoleHelpers.GetOrReadArgument(argumentIndex++, "Links file path", args);
            var defaultDocumentName = Path.GetFileNameWithoutExtension(jsonFilePath);
            var documentName = ConsoleHelpers.GetOrReadArgument(argumentIndex, $"Document name (default: {defaultDocumentName})", args);
            if (string.IsNullOrWhiteSpace(documentName))
            {
                documentName = defaultDocumentName;
            }
            if (!File.Exists(jsonFilePath))
            {
                Console.WriteLine($"${jsonFilePath} file does not exist.");
            }
            var json = File.ReadAllText(jsonFilePath);
            var encodedJson = Encoding.UTF8.GetBytes(json);
            ReadOnlySpan<byte> readOnlySpanEncodedJson = new(encodedJson);
            Utf8JsonReader utf8JsonReader = new(readOnlySpanEncodedJson);
            LinksConstants<TLink> linksConstants = new(true);
            FileMappedResizableDirectMemory fileMappedResizableDirectMemory = new(linksFilePath);
            var unitedMemoryLinks = UnitedMemoryLinks<TLink>.DefaultLinksSizeStep;
            const IndexTreeType indexTreeType = IndexTreeType.Default;
            using UnitedMemoryLinks<TLink> memoryAdapter = new(fileMappedResizableDirectMemory, unitedMemoryLinks, linksConstants, indexTreeType);
            var links = memoryAdapter.DecorateWithAutomaticUniquenessAndUsagesResolution();
            BalancedVariantConverter<TLink> balancedVariantConverter = new(links);
            DefaultJsonStorage<TLink> storage = new(links, balancedVariantConverter);
            JsonImporter<TLink> importer = new(storage);
            using ConsoleCancellation cancellation = new();
            var cancellationToken = cancellation.Token;
            Console.WriteLine("Press CTRL+C to stop.");
            try
            {
                importer.Import(documentName, ref utf8JsonReader, in cancellationToken);
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
