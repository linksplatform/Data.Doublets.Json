using System;
using Xunit;
using Platform.Data.Doublets.Json;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Memory;
using Platform.Memory;
using TLink = System.UInt32;
using System.IO;
namespace Platform.Data.Doublets.Json.Tests
{
    public static class JsonStorageTests
    {
        public static ILinks<TLink> CreateLinks() => CreateLinks<TLink>(Path.GetTempFileName());
        public static ILinks<TLink> CreateLinks<TLink>(string dataDBFilename)
        {
            var linksConstants = new LinksConstants<TLink>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLink>(new FileMappedResizableDirectMemory(dataDBFilename), UnitedMemoryLinks<TLink>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }
        [Fact]
        public static void ConstructorsTest()
        {
            DefaultJsonStorage<TLink> defaultJsonStorage = new DefaultJsonStorage<TLink>(CreateLinks());
        }
        [Fact]
        public static void CreateDocumentTest()
        {
            DefaultJsonStorage<TLink> defaultJsonStorage = new DefaultJsonStorage<TLink>(CreateLinks());
            defaultJsonStorage.CreateDocument("documentName");
        }
        [Fact]
        public static void GetDocumentTest()
        {
            DefaultJsonStorage<TLink> defaultJsonStorage = new DefaultJsonStorage<TLink>(CreateLinks());
            var createdDocumentLink = defaultJsonStorage.CreateDocument("documentName");
            var foundDocumentLink = defaultJsonStorage.GetDocument("documentName");
            Assert.Equal(createdDocumentLink, foundDocumentLink);
        }
        [Fact]
        public static void CreateObjectTest()
        {
            DefaultJsonStorage<TLink> defaultJsonStorage = new DefaultJsonStorage<TLink>(CreateLinks());
            var object0 = defaultJsonStorage.CreateObjectValue();
            var object1 = defaultJsonStorage.CreateObjectValue();
            Assert.NotEqual(object0, object1);
        }
        [Fact]
        public static void CreateStringTest()
        {
            DefaultJsonStorage<TLink> defaultJsonStorage = new DefaultJsonStorage<TLink>(CreateLinks());
            defaultJsonStorage.CreateString("string");
        }
        [Fact]
        public static void CreateKeyTest()
        {
            DefaultJsonStorage<TLink> defaultJsonStorage = new DefaultJsonStorage<TLink>(CreateLinks());
            var document = defaultJsonStorage.CreateDocument("documentName");
            var @object = defaultJsonStorage.AttachObject(document);
            defaultJsonStorage.CreateKey(@object, "keyName");
        }
        [Fact]
        public static void CreateValueTest()
        {
            DefaultJsonStorage<TLink> defaultJsonStorage = new DefaultJsonStorage<TLink>(CreateLinks());
            var document = defaultJsonStorage.CreateDocument("documentName");
            var @object = defaultJsonStorage.AttachObject(document);
            var key = defaultJsonStorage.CreateKey(@object, "keyName");
            var value = defaultJsonStorage.CreateValue(key, "valueName");
        }
        [Fact]
        public static void AttachValueToDocumentTest()
        {
            var links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new DefaultJsonStorage<TLink>(links);
            var document = defaultJsonStorage.CreateDocument("documentName");
            var documentValueLink = defaultJsonStorage.AttachObject(document);
            var createdDocumentValue = links.GetTarget(documentValueLink);
            var foundDocumentValue = defaultJsonStorage.GetValue(document);
            Assert.Equal(createdDocumentValue, foundDocumentValue);
        }
        [Fact]
        public static void AttachStringToDocumentTest()
        {
            var links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new DefaultJsonStorage<TLink>(links);
            var document = defaultJsonStorage.CreateDocument("documentName");
            var documentStringLink = defaultJsonStorage.AttachString(document, "stringName");
            var createdStringValue = links.GetTarget(documentStringLink);
            var foundStringValue = defaultJsonStorage.GetValue(document);
            Assert.Equal(createdStringValue, foundStringValue);
        }
        [Fact]
        public static void AttachNumberToDocumentTest()
        {
            var links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new DefaultJsonStorage<TLink>(links);
            var document = defaultJsonStorage.CreateDocument("documentName");
            var documentNumberLink = defaultJsonStorage.AttachNumber(document, 2021);
            var createdNumberValue = links.GetTarget(documentNumberLink);
            var foundNumberValue = defaultJsonStorage.GetValue(document);
            Assert.Equal(createdNumberValue, foundNumberValue);
        }
        [Fact]
        public static void GetValueTest()
        {
            DefaultJsonStorage<TLink> defaultJsonStorage = new DefaultJsonStorage<TLink>(CreateLinks());
            var document = defaultJsonStorage.CreateDocument("documentName");
            var @object = defaultJsonStorage.AttachObject(document);
            var key = defaultJsonStorage.CreateKey(@object, "keyName");
            var createdValueLink = defaultJsonStorage.CreateValue(key, "valueName");
            var foundValueLink = defaultJsonStorage.GetValue(key);
            Assert.Equal(createdValueLink, foundValueLink);
        }
    }
}
