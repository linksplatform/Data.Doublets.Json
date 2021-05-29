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
            DefaultJsonStorage<uint> testObject = new DefaultJsonStorage<uint>(links);
        }
        [Fact]
        public static void CreateDocumentTest()
        {
            using var links = new UnitedMemoryLinks<uint>("db.links");
            DefaultJsonStorage<uint> testObject = new DefaultJsonStorage<uint>(links);
            testObject.CreateDocument("testDocumentName");
        }
        [Fact]
        public static void CreateObjectTest()
        {
            using var links = new UnitedMemoryLinks<uint>("db.links");
            DefaultJsonStorage<uint> testObject = new DefaultJsonStorage<uint>(links);
            testObject.CreateObject("testObjectName");
        }
        [Fact]
        public static void CreateStringTest()
        {
            using var links = new UnitedMemoryLinks<uint>("db.links");
            DefaultJsonStorage<uint> testObject = new DefaultJsonStorage<uint>(links);
            testObject.CreateString("test string");
        }
        [Fact]
        public static void CreateKeyTest()
        {
            using var links = new UnitedMemoryLinks<uint>("db.links");
            DefaultJsonStorage<uint> testObject = new DefaultJsonStorage<uint>(links);
            testObject.CreateKey("test string");
        }
    }
}