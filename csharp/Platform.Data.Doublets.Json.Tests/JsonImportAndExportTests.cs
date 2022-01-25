using System.Text;
using System.Text.Json;
using System.Threading;
using System.IO;
using Xunit;
using TLinkAddress = System.UInt64;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Memory;
using Platform.Data.Doublets.Memory;
using System.Text.RegularExpressions;
using Platform.Data.Doublets.Sequences.Converters;

namespace Platform.Data.Doublets.Json.Tests
{
    public class JsonImportAndExportTests
    {
        public static BalancedVariantConverter<TLinkAddress> BalancedVariantConverter;
        
        public static ILinks<TLinkAddress> CreateLinks() => CreateLinks<TLinkAddress>(new IO.TemporaryFile());

        public static ILinks<TLinkAddress> CreateLinks<TLinkAddress>(string dataDBFilename)
        {
            var linksConstants = new LinksConstants<TLinkAddress>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLinkAddress>(new FileMappedResizableDirectMemory(dataDBFilename), UnitedMemoryLinks<TLinkAddress>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }

        public static DefaultJsonStorage<TLinkAddress> CreateJsonStorage(ILinks<TLinkAddress> links) => new (links, BalancedVariantConverter);
        
        public TLinkAddress Import(IJsonStorage<TLinkAddress> storage, string documentName, byte[] json)
        {
            Utf8JsonReader utf8JsonReader = new(json);
            JsonImporter<TLinkAddress> jsonImporter = new(storage);
            CancellationTokenSource importCancellationTokenSource = new();
            CancellationToken cancellationToken = importCancellationTokenSource.Token;
            return jsonImporter.Import(documentName, ref utf8JsonReader, in cancellationToken);
        }

        public void Export(TLinkAddress documentLink, IJsonStorage<TLinkAddress> storage, in MemoryStream stream)
        {
            Utf8JsonWriter writer = new(stream);
            JsonExporter<TLinkAddress> jsonExporter = new(storage);
            CancellationTokenSource exportCancellationTokenSource = new();
            CancellationToken exportCancellationToken = exportCancellationTokenSource.Token;
            jsonExporter.Export(documentLink, ref writer, in exportCancellationToken);
            writer.Dispose();
        }
        
        [Theory]
        [InlineData("{}")]
        [InlineData("\"stringValue\"")]
        [InlineData("228")]
        [InlineData("0.5")]
        [InlineData("[]")]
        [InlineData("true")]
        [InlineData("false")]
        [InlineData("null")]
        [InlineData("{ \"string\": \"string\" }")]
        [InlineData("{ \"null\": null }")]
        [InlineData("{ \"boolean\": false }")]
        [InlineData("{ \"boolean\": true }")]
        [InlineData("{ \"array\": [] }")]
        [InlineData("{ \"array\": [1] }")]
        [InlineData("{ \"object\": {} }")]
        [InlineData("{ \"number\": 1 }")]
        [InlineData("{ \"decimal\": 0.5 }")]
        [InlineData("[null]")]
        [InlineData("[true]")]
        [InlineData("[false]")]
        [InlineData("[[]]")]
        [InlineData("[[1]]")]
        [InlineData("[[0.5]]")]
        [InlineData("[{}]")]
        [InlineData("[\"The Venus Project\"]")]
        [InlineData("[{ \"title\": \"The Venus Project\" }]")]
        [InlineData("[1,2,3,4]")]
        [InlineData("[-0.5, 0.5]")]
        public void Test(string initialJson)
        {
            var links = CreateLinks();
            BalancedVariantConverter = new(links);
            var storage = CreateJsonStorage(links);
            var json = Encoding.UTF8.GetBytes(initialJson);
            var documentLink = Import(storage, "documentName", json);
            MemoryStream stream = new();
            Export(documentLink, storage, in stream);
            string exportedJson = Encoding.UTF8.GetString(stream.ToArray());
            stream.Dispose();
            var minimizedInitialJson = Regex.Replace(initialJson, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");
            Assert.Equal(minimizedInitialJson, exportedJson);
        }
    }
}
