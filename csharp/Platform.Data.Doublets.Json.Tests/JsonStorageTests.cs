using Xunit;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Memory;
using Platform.Memory;
using TLink = System.UInt32;
using System.IO;
using Xunit.Abstractions;
using Platform.Collections.Stacks;
using Platform.Data.Doublets.Sequences.Walkers;
using System.Collections.Generic;

namespace Platform.Data.Doublets.Json.Tests
{
    public class JsonStorageTests
    {
        private readonly ITestOutputHelper output;

        public JsonStorageTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        public static ILinks<TLink> CreateLinks() => CreateLinks<TLink>(new Platform.IO.TemporaryFile());
        public static ILinks<TLink> CreateLinks<TLink>(string dataDBFilename)
        {
            var linksConstants = new LinksConstants<TLink>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLink>(new FileMappedResizableDirectMemory(dataDBFilename), UnitedMemoryLinks<TLink>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }
        [Fact]
        public void ConstructorsTest() => new DefaultJsonStorage<TLink>(CreateLinks());
        [Fact]
        public void CreateDocumentTest()
        {
            DefaultJsonStorage<TLink> defaultJsonStorage = new(CreateLinks());
            defaultJsonStorage.CreateDocument("documentName");
        }
        [Fact]
        public void GetDocumentTest()
        {
            DefaultJsonStorage<TLink> defaultJsonStorage = new(CreateLinks());
            var createdDocumentLink = defaultJsonStorage.CreateDocument("documentName");
            var foundDocumentLink = defaultJsonStorage.GetDocument("documentName");
            Assert.Equal(createdDocumentLink, foundDocumentLink);
        }
        [Fact]
        public void CreateObjectTest()
        {
            DefaultJsonStorage<TLink> defaultJsonStorage = new(CreateLinks());
            var object0 = defaultJsonStorage.CreateObjectValue();
            var object1 = defaultJsonStorage.CreateObjectValue();
            Assert.NotEqual(object0, object1);
        }
        [Fact]
        public void CreateStringTest()
        {
            DefaultJsonStorage<TLink> defaultJsonStorage = new(CreateLinks());
            defaultJsonStorage.CreateString("string");
        }
        [Fact]
        public void CreateMemberTest()
        {
            DefaultJsonStorage<TLink> defaultJsonStorage = new(CreateLinks());
            var document = defaultJsonStorage.CreateDocument("documentName");
            defaultJsonStorage.AttachObject(document);
            defaultJsonStorage.CreateMember("keyName");
        }
        [Fact]
        public void AttachObjectValueToDocumentTest()
        {
            var links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");
            TLink documentValueLink = defaultJsonStorage.AttachObject(document);
            TLink createdObjectValue = links.GetTarget(documentValueLink);

            TLink valueMarker = links.GetSource(createdObjectValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLink createdObject = links.GetTarget(createdObjectValue);
            TLink objectMarker = links.GetSource(createdObject);
            Assert.Equal(objectMarker, defaultJsonStorage.ObjectMarker);

            TLink foundDocumentValue = defaultJsonStorage.GetValue(document);
            Assert.Equal(createdObjectValue, foundDocumentValue);
        }
        [Fact]
        public void AttachStringValueToDocumentTest()
        {
            var links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");
            TLink documentStringLink = defaultJsonStorage.AttachString(document, "stringName");
            TLink createdStringValue = links.GetTarget(documentStringLink);

            TLink valueMarker = links.GetSource(createdStringValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLink createdString = links.GetTarget(createdStringValue);
            TLink stringMarker = links.GetSource(createdString);
            Assert.Equal(stringMarker, defaultJsonStorage.StringMarker);

            TLink foundStringValue = defaultJsonStorage.GetValue(document);
            Assert.Equal(createdStringValue, foundStringValue);
        }
        [Fact]
        public void AttachNumberToDocumentTest()
        {
            var links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");
            TLink documentNumberLink = defaultJsonStorage.AttachNumber(document, 2021);
            TLink createdNumberValue = links.GetTarget(documentNumberLink);

            TLink valueMarker = links.GetSource(createdNumberValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLink createdNumber = links.GetTarget(createdNumberValue);
            TLink numberMarker = links.GetSource(createdNumber);
            Assert.Equal(numberMarker, defaultJsonStorage.NumberMarker);

            TLink foundNumberValue = defaultJsonStorage.GetValue(document);
            Assert.Equal(createdNumberValue, foundNumberValue);
        }
        [Fact]
        public void AttachTrueValueToDocumentTest()
        {
            var links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");

            TLink documentTrueValueLink = defaultJsonStorage.AttachBoolean(document, true);
            TLink createdTrueValue = links.GetTarget(documentTrueValueLink);

            TLink valueMarker = links.GetSource(createdTrueValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLink trueMarker = links.GetTarget(createdTrueValue);
            Assert.Equal(trueMarker, defaultJsonStorage.TrueMarker);

            TLink foundTrueValue = defaultJsonStorage.GetValue(document);
            Assert.Equal(createdTrueValue, foundTrueValue);
        }
        [Fact]
        public void AttachFalseValueToDocumentTest()
        {
            var links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");

            TLink documentFalseValueLink = defaultJsonStorage.AttachBoolean(document, false);
            TLink createdFalseValue = links.GetTarget(documentFalseValueLink);

            TLink valueMarker = links.GetSource(createdFalseValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLink falseMarker = links.GetTarget(createdFalseValue);
            Assert.Equal(falseMarker, defaultJsonStorage.FalseMarker);

            TLink foundFalseValue = defaultJsonStorage.GetValue(document);
            Assert.Equal(createdFalseValue, foundFalseValue);
        }
        [Fact]
        public void AttachNullValueToDocumentTest()
        {
            var links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");

            TLink documentNullValueLink = defaultJsonStorage.AttachNull(document);
            TLink createdNullValue = links.GetTarget(documentNullValueLink);

            TLink valueMarker = links.GetSource(createdNullValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLink nullMarker = links.GetTarget(createdNullValue);
            Assert.Equal(nullMarker, defaultJsonStorage.NullMarker);

            TLink foundNullValue = defaultJsonStorage.GetValue(document);
            Assert.Equal(createdNullValue, foundNullValue);
        }
        [Fact]
        public void AttachEmptyArrayValueToDocumentTest()
        {
            var links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");

            TLink documentArrayValueLink = defaultJsonStorage.AttachArray(document, new TLink[0]);
            TLink createdArrayValue = links.GetTarget(documentArrayValueLink);
            output.WriteLine(links.Format(createdArrayValue));


            TLink valueMarker = links.GetSource(createdArrayValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLink createdArrayLink = links.GetTarget(createdArrayValue);
            TLink arrayMarker = links.GetSource(createdArrayLink);
            Assert.Equal(arrayMarker, defaultJsonStorage.ArrayMarker);

            TLink createArrayContents = links.GetTarget(createdArrayLink);
            Assert.Equal(createArrayContents, defaultJsonStorage.EmptyArrayMarker);

            TLink foundArrayValue = defaultJsonStorage.GetValue(document);
            Assert.Equal(createdArrayValue, foundArrayValue);
        }
        [Fact]
        public void AttachArrayValueToDocumentTest()
        {
            var links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");

            TLink arrayElement = defaultJsonStorage.CreateString("arrayElement");
            TLink[] array = new TLink[3] { arrayElement, arrayElement, arrayElement };


            TLink documentArrayValueLink = defaultJsonStorage.AttachArray(document, array);
            TLink createdArrayValue = links.GetTarget(documentArrayValueLink);

            DefaultStack<TLink> stack = new();
            RightSequenceWalker<TLink> rightSequenceWalker = new(links, stack, (TLink arrayElementLink) => links.GetSource(arrayElementLink) == defaultJsonStorage.ValueMarker);
            IEnumerable<TLink> arrayElementsValuesLink = rightSequenceWalker.Walk(createdArrayValue);
            Assert.NotEmpty(arrayElementsValuesLink);

            output.WriteLine(links.Format(createdArrayValue));


            TLink valueMarker = links.GetSource(createdArrayValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLink createdArrayLink = links.GetTarget(createdArrayValue);
            TLink arrayMarker = links.GetSource(createdArrayLink);
            Assert.Equal(arrayMarker, defaultJsonStorage.ArrayMarker);

            TLink createdArrayContents = links.GetTarget(createdArrayLink);
            Assert.Equal(links.GetTarget(createdArrayContents), arrayElement);


            TLink foundArrayValue = defaultJsonStorage.GetValue(document);
            Assert.Equal(createdArrayValue, foundArrayValue);
        }
        [Fact]
        public void GetObjectTest()
        {
            ILinks<TLink> links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");
            TLink documentObjectValueLink = defaultJsonStorage.AttachObject(document);
            TLink objectValueLink = links.GetTarget(documentObjectValueLink);
            TLink objectFromGetObject = defaultJsonStorage.GetObject(documentObjectValueLink);
            output.WriteLine($"objectValueLink: {links.Format(objectValueLink)}");
            output.WriteLine($"documentObjectValueLink: {links.Format(documentObjectValueLink)}");
            output.WriteLine($"objectValueLink Target: {links.Format(links.GetTarget(objectValueLink))}");
            output.WriteLine($"objectFromGetObject: {links.Format(objectFromGetObject)}");
            Assert.Equal(links.GetTarget(objectValueLink), objectFromGetObject);
        }
        [Fact]
        public void AttachStringValueToKey()
        {
            ILinks<TLink> links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");
            TLink documentObjectValue = defaultJsonStorage.AttachObject(document);
            TLink @object = defaultJsonStorage.GetObject(documentObjectValue);
            TLink memberLink = defaultJsonStorage.AttachMemberToObject(@object, "keyName");
            defaultJsonStorage.AttachString(memberLink, "stringValue");
            List<TLink> objectMembersLinks = defaultJsonStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
        }
        [Fact]
        public void GetObjectMembers()
        {
            /* EqualityComparer<TLink> equalityComparer = EqualityComparer<TLink>.Default;
            Comparer<TLink> comparer = Comparer<TLink>.Default;
            return comparer.Compare(_links.GetSource(arrayElementLink), ValueMarker) == 0;
            */
        }
    }
}
