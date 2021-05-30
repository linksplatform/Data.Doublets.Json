using System;
using Xunit;
using Platform.Data.Doublets.Json;
using Platform.Data.Doublets.Memory.United.Generic;

namespace Platform.Data.Doublets.Json.Tests
{
    public static class JsonStorageTests
    {
        [Fact]
        public static void ConstructorsTest()
        {
            using var links = new UnitedMemoryLinks<uint>("db.links");
            DefaultJsonStorage<uint> defaultJsonStorage = new DefaultJsonStorage<uint>(links);
        }
        [Fact]
        public static void CreateDocumentTest()
        {
            using var links = new UnitedMemoryLinks<uint>("db.links");
            DefaultJsonStorage<uint> defaultJsonStorage = new DefaultJsonStorage<uint>(links);
            defaultJsonStorage.CreateDocument("documentName");
        }
        [Fact]
        public static void GetDocumentTest()
        {
            using var links = new UnitedMemoryLinks<uint>("db.links");
            DefaultJsonStorage<uint> defaultJsonStorage = new DefaultJsonStorage<uint>(links);
            var createdDocumentLink = defaultJsonStorage.CreateDocument("documentName");
            var foundDocumentLink = defaultJsonStorage.GetDocument("documentName");
            Assert.Equal(createdDocumentLink, foundDocumentLink);
        }
        [Fact]
        public static void CreateObjectTest()
        {
            using var links = new UnitedMemoryLinks<uint>("db.links");
            DefaultJsonStorage<uint> defaultJsonStorage = new DefaultJsonStorage<uint>(links);
            var document = defaultJsonStorage.CreateDocument("testDocumentName");
            defaultJsonStorage.AttachObject(document);
        }
        [Fact]
        public static void CreateStringTest()
        {
            using var links = new UnitedMemoryLinks<uint>("db.links");
            DefaultJsonStorage<uint> defaultJsonStorage = new DefaultJsonStorage<uint>(links);
            defaultJsonStorage.CreateString("string");
        }
        [Fact]
        public static void CreateKeyTest()
        {
            using var links = new UnitedMemoryLinks<uint>("db.links");
            DefaultJsonStorage<uint> defaultJsonStorage = new DefaultJsonStorage<uint>(links);
            var document = defaultJsonStorage.CreateDocument("documentName");
            var @object = defaultJsonStorage.AttachObject(document);
            defaultJsonStorage.CreateKey(@object, "keyName");
        } 
        [Fact]
        public static void CreateValueTest()
        {
            using var links = new UnitedMemoryLinks<uint>("db.links");
            DefaultJsonStorage<uint> defaultJsonStorage = new DefaultJsonStorage<uint>(links);
            var document = defaultJsonStorage.CreateDocument("documentName");
            var @object = defaultJsonStorage.AttachObject(document);
            var key = defaultJsonStorage.CreateKey(@object, "keyName");
            var value = defaultJsonStorage.CreateValue(key, "valueName");
        }
    }
}