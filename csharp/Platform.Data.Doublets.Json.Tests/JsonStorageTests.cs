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
            defaultJsonStorage.CreateDocument("testDocumentName");
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
            defaultJsonStorage.AttachObject(document);
            var @object = defaultJsonStorage.CreateObject("defaultJsonStorageName");
            defaultJsonStorage.CreateKey(@object, "keyName");
        } 
        public static void CreateValueTest()
        {
            using var links = new UnitedMemoryLinks<uint>("db.links");
            DefaultJsonStorage<uint> defaultJsonStorage = new DefaultJsonStorage<uint>(links);
            var document = defaultJsonStorage.CreateDocument("testDocumentName");
            defaultJsonStorage.AttachObject(document);
            var @object = defaultJsonStorage.CreateObject("defaultJsonStorageName");
            var key = defaultJsonStorage.CreateKey(@object, "keyName");
            var value = defaultJsonStorage.CreateValue(key, "valueName");
        }
    }
}