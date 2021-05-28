using System;
using Xunit;
using Platform.Data.Doublets.Json;

namespace Platform.Data.Doublets.Json.Tests
{
    public static class JsonStorageTests
    {
        [Fact]
        public static void ConstructorsTest()
        {
            Platform.Data.Doublets.Json.DefaultJsonStorage<uint> testObject = new DefaultJsonStorage<uint>();
        }
        [Fact]
        public static void CreateDocumentTest()
        {

        }
    }
}