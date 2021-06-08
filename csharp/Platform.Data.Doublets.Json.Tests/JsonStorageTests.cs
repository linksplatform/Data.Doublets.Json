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
        public static void AttachObjectValueToDocumentTest()
        {
            var links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new DefaultJsonStorage<TLink>(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");
            TLink documentValueLink = defaultJsonStorage.AttachObject(document);
            TLink createdDocumentValue = links.GetTarget(documentValueLink);

            TLink objectMarker = links.GetSource(createdDocumentValue);
            Assert.Equal(objectMarker, defaultJsonStorage.ObjectMarker);

            TLink foundDocumentValue = defaultJsonStorage.GetValue(document);
            Assert.Equal(createdDocumentValue, foundDocumentValue);
        }
        [Fact]
        public static void AttachStringValueToDocumentTest()
        {
            var links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new DefaultJsonStorage<TLink>(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");
            TLink documentStringLink = defaultJsonStorage.AttachString(document, "stringName");
            TLink createdStringValue = links.GetTarget(documentStringLink);

            TLink stringMarker = links.GetSource(createdStringValue);
            Assert.Equal(stringMarker, defaultJsonStorage.StringMarker);

            TLink foundStringValue = defaultJsonStorage.GetValue(document);
            Assert.Equal(createdStringValue, foundStringValue);
        }
        [Fact]
        public static void AttachNumberToDocumentTest()
        {
            var links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new DefaultJsonStorage<TLink>(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");
            TLink documentNumberLink = defaultJsonStorage.AttachNumber(document, 2021);
            TLink createdNumberValue = links.GetTarget(documentNumberLink);

            TLink numberMarker = links.GetSource(createdNumberValue);
            Assert.Equal(numberMarker, defaultJsonStorage.NumberMarker);

            TLink foundNumberValue = defaultJsonStorage.GetValue(document);
            Assert.Equal(createdNumberValue, foundNumberValue);
        }
        [Fact]
        public static void AttachTrueValueToDocumentTest()
        {
            var links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new DefaultJsonStorage<TLink>(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");

            TLink documentTrueValueLink = defaultJsonStorage.AttachBoolean(document, true);
            TLink createdTrueValue = links.GetTarget(documentTrueValueLink);

            TLink trueMarker = links.GetTarget(createdTrueValue);
            Assert.Equal(trueMarker, defaultJsonStorage.TrueMarker);

            TLink foundTrueValue = defaultJsonStorage.GetValue(document);
            Assert.Equal(createdTrueValue, foundTrueValue);
        }
        [Fact]
        public static void AttachFalseValueToDocumentTest()
        {
            var links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new DefaultJsonStorage<TLink>(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");

            TLink documentFalseValueLink = defaultJsonStorage.AttachBoolean(document, false);
            TLink createdFalseValue = links.GetTarget(documentFalseValueLink);

            TLink valueMarker = links.GetSource(createdFalseValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLink createdFalse = links.GetSource(createdFalseValue);
            TLink falseMarker = links.GetTarget(createdFalse);
            Assert.Equal(falseMarker, defaultJsonStorage.FalseMarker);

            TLink foundFalseValue = defaultJsonStorage.GetValue(document);
            Assert.Equal(createdFalseValue, foundFalseValue);
        }
        [Fact]
        public static void AttachNullValueToDocumentTest()
        {
            var links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new DefaultJsonStorage<TLink>(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");

            TLink documentNullValueLink = defaultJsonStorage.AttachNull(document);
            TLink createdNullValue = links.GetTarget(documentNullValueLink);

            TLink valueMarker = links.GetSource(createdNullValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLink createdNull = links.GetSource(createdNullValue);
            TLink nullMarker = links.GetTarget(createdNull);
            Assert.Equal(nullMarker, defaultJsonStorage.NullMarker);

            TLink foundNullValue = defaultJsonStorage.GetValue(document);
            Assert.Equal(createdNullValue, foundNullValue);
        }
        [Fact]
        public static void AttachEmptyArrayValueToDocumentTest()
        {
            var links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new DefaultJsonStorage<TLink>(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");

            TLink[] array = new TLink[0];
            TLink documentArrayValueLink = defaultJsonStorage.AttachArray(document, array);
            TLink createdArrayValue = links.GetTarget(documentArrayValueLink);

            TLink valueMarker = links.GetSource(createdArrayValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLink createdArray = links.GetTarget(createdArrayValue);
            TLink arrayMarker = links.GetSource(createdArray);
            Assert.Equal(arrayMarker, defaultJsonStorage.ArrayMarker);

            TLink emptyArrayMarker = links.GetTarget(createdArray);
            Assert.Equal(emptyArrayMarker, defaultJsonStorage.EmptyArrayMarker);

            TLink foundArrayValue = defaultJsonStorage.GetValue(document);
            Assert.Equal(createdArrayValue, foundArrayValue);
        }
        [Fact]
        public static void AttachArrayValueToDocumentTest()
        {
            var links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new DefaultJsonStorage<TLink>(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");

            TLink link = links.Create();
            TLink[] array = new TLink[3] { link, link, link };
            TLink documentArrayValueLink = defaultJsonStorage.AttachArray(document, array);
            TLink createdArrayValue = links.GetTarget(documentArrayValueLink);

            TLink arrayMarker = links.GetSource(createdArrayValue);
            Assert.Equal(arrayMarker, defaultJsonStorage.ArrayMarker);

            TLink emptyArrayMarker = links.GetTarget(createdArrayValue);
            //Assert.Equal(emptyArrayMarker, defaultJsonStorage.EmptyArrayMarker);

            TLink foundArrayValue = defaultJsonStorage.GetValue(document);
            Assert.Equal(createdArrayValue, foundArrayValue);
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
