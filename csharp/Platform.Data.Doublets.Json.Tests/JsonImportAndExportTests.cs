using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Threading;
using System.IO;
using Platform.Data.Doublets.Json;
using Xunit;
using TLink = System.UInt32;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Memory;
using Platform.Data.Doublets.Memory;

namespace Platform.Data.Doublets.Json.Tests
{
    public class JsonImportAndExportTests
    {
        public static ILinks<TLink> CreateLinks() => CreateLinks<TLink>(new Platform.IO.TemporaryFile());

        public static ILinks<TLink> CreateLinks<TLink>(string dataDBFilename)
        {
            var linksConstants = new LinksConstants<TLink>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLink>(new FileMappedResizableDirectMemory(dataDBFilename), UnitedMemoryLinks<TLink>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }

        public static DefaultJsonStorage<TLink> CreateJsonStorage() => new DefaultJsonStorage<TLink>(CreateLinks());
        public static DefaultJsonStorage<TLink> CreateJsonStorage(ILinks<TLink> links) => new DefaultJsonStorage<TLink>(links);
        public TLink Import(IJsonStorage<TLink> storage, string documentName, byte[] json)
        {
            Utf8JsonReader utf8JsonReader = new(json);
            JsonImporter<TLink> jsonImporter = new(storage);
            CancellationTokenSource importCancellationTokenSource = new();
            CancellationToken importCancellationToken = importCancellationTokenSource.Token;
            return jsonImporter.Import(documentName, ref utf8JsonReader, importCancellationToken);
        }
        [Fact]
        public void EmptyObjectTest()
        {
            var storage = CreateJsonStorage();
            var initialJson = "{}";
            var json = Encoding.UTF8.GetBytes(initialJson);
            var documentLink = Import(storage, "documentName", json);
            var options = new JsonWriterOptions
            {
                Indented = true
            };
            using MemoryStream stream = new();
            using Utf8JsonWriter writer = new(stream, options);
            JsonExporter<TLink> jsonExporter = new(storage);
            CancellationTokenSource exportCancellationTokenSource = new();
            CancellationToken exportCancellationToken = exportCancellationTokenSource.Token;
            jsonExporter.Export(documentLink, writer, exportCancellationToken);
            var exportedJson = Encoding.UTF8.GetString(stream.ToArray());
            Assert.Equal(initialJson, exportedJson);
        }

        [Fact]
        public void StringTest()
        {
            var storage = CreateJsonStorage();
            var json = Encoding.UTF8.GetBytes("\"stringValue\"");
            var documentLink = Import(storage, "documentName", json);
            var options = new JsonWriterOptions
            {
                Indented = true
            };
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream, options);
            JsonExporter<TLink> jsonExporter = new(storage);
            CancellationTokenSource exportCancellationTokenSource = new();
            CancellationToken exportCancellationToken = exportCancellationTokenSource.Token;
            jsonExporter.Export(documentLink, writer, exportCancellationToken);
            var exportedJson = Encoding.UTF8.GetString(stream.ToArray());
            var initialString = Encoding.UTF8.GetString(json);
            Assert.Equal(initialString, exportedJson);
        }

        [Fact]
        public void NumberTest()
        {
            var storage = CreateJsonStorage();
            var initialJson = "228";
            var json = Encoding.UTF8.GetBytes(initialJson);
            var documentLink = Import(storage, "documentName", json);
            var options = new JsonWriterOptions
            {
                Indented = true
            };
            using MemoryStream stream = new();
            using Utf8JsonWriter writer = new(stream, options);
            JsonExporter<TLink> jsonExporter = new(storage);
            CancellationTokenSource exportCancellationTokenSource = new();
            CancellationToken exportCancellationToken = exportCancellationTokenSource.Token;
            jsonExporter.Export(documentLink, writer, exportCancellationToken);
            var exportedJson = Encoding.UTF8.GetString(stream.ToArray());
            Assert.Equal(initialJson, exportedJson);
        }
    }
}
