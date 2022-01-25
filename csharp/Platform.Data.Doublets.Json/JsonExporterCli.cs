using System;
using System.IO;
using System.Text.Encodings.Web;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.IO;
using System.Text.Json;
using Platform.Data.Doublets.Memory;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Memory;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Json
{
    /// <summary>
    /// <para>
    /// Represents the json exporter cli.
    /// </para>
    /// <para></para>
    /// </summary>
    public class JsonExporterCli<TLinkAddress>
        where TLinkAddress : struct
    {
        /// <summary>
        /// <para>
        /// Runs the args.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="args">
        /// <para>The args.</para>
        /// <para></para>
        /// </param>
        public void Run(params string[] args)
        {
            var argumentIndex = 0;
            var linksFilePath = ConsoleHelpers.GetOrReadArgument(argumentIndex++, "Links file path", args);
            var jsonFilePath = ConsoleHelpers.GetOrReadArgument(argumentIndex++, "JSON file path", args);
            var defaultDocumentName = Path.GetFileNameWithoutExtension(jsonFilePath);
            var documentName = ConsoleHelpers.GetOrReadArgument(argumentIndex, $"Document name (default: {defaultDocumentName})", args);
            if (string.IsNullOrWhiteSpace(documentName))
            {
                documentName = defaultDocumentName;
            }
            if (!File.Exists(linksFilePath))
            {
                Console.WriteLine($"${linksFilePath} file does not exist.");
            }
            using FileStream jsonFileStream = new(jsonFilePath, FileMode.Append);
            JsonWriterOptions utf8JsonWriterOptions = new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Indented = true
            };
            Utf8JsonWriter utf8JsonWriter = new(jsonFileStream, utf8JsonWriterOptions);
            var linksConstants = new LinksConstants<TLinkAddress>(enableExternalReferencesSupport: true);
            using UnitedMemoryLinks<TLinkAddress> memoryAdapter = new (new FileMappedResizableDirectMemory(linksFilePath), UnitedMemoryLinks<TLinkAddress>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
            var links = memoryAdapter.DecorateWithAutomaticUniquenessAndUsagesResolution();
            BalancedVariantConverter<TLinkAddress> balancedVariantConverter = new(links);
            var storage = new DefaultJsonStorage<TLinkAddress>(links, balancedVariantConverter);
            var exporter = new JsonExporter<TLinkAddress>(storage);
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