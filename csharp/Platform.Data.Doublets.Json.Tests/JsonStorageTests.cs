using Xunit;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Memory;
using Platform.Memory;
using TLinkAddress = System.UInt32;
using Xunit.Abstractions;
using Platform.Collections.Stacks;
using Platform.Data.Doublets.Sequences.Walkers;
using System.Collections.Generic;
using Platform.Data.Doublets.Sequences.Converters;

namespace Platform.Data.Doublets.Json.Tests
{
    public class JsonStorageTests
    {
        private readonly ITestOutputHelper output;
        public static BalancedVariantConverter<TLinkAddress> BalancedVariantConverter;

        public JsonStorageTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        public static ILinks<TLinkAddress> CreateLinks() => CreateLinks<TLinkAddress>(new Platform.IO.TemporaryFile());

        public static ILinks<TLinkAddress> CreateLinks<TLinkAddress>(string dataDBFilename)
        {
            var linksConstants = new LinksConstants<TLinkAddress>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLinkAddress>(new FileMappedResizableDirectMemory(dataDBFilename), UnitedMemoryLinks<TLinkAddress>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }

        public static DefaultJsonStorage<TLinkAddress> CreateJsonStorage()
        {
            var links = CreateLinks();
            return CreateJsonStorage(links);
        }

        public static DefaultJsonStorage<TLinkAddress> CreateJsonStorage(ILinks<TLinkAddress> links)
        {
            BalancedVariantConverter = new(links);
            return new DefaultJsonStorage<TLinkAddress>(links, BalancedVariantConverter);
        }

        [Fact]
        public void ConstructorsTest() => CreateJsonStorage();

        [Fact]
        public void CreateDocumentTest()
        {
            var defaultJsonStorage = CreateJsonStorage();
            defaultJsonStorage.CreateDocument("documentName");
        }

        [Fact]
        public void GetDocumentTest()
        {
            var defaultJsonStorage = CreateJsonStorage();
            var createdDocumentLink = defaultJsonStorage.CreateDocument("documentName");
            var foundDocumentLink = defaultJsonStorage.GetDocumentOrDefault("documentName");
            Assert.Equal(createdDocumentLink, foundDocumentLink);
        }

        [Fact]
        public void CreateObjectTest()
        {
            var defaultJsonStorage = CreateJsonStorage();
            var object0 = defaultJsonStorage.CreateObjectValue();
            var object1 = defaultJsonStorage.CreateObjectValue();
            Assert.NotEqual(object0, object1);
        }

        [Fact]
        public void CreateStringTest()
        {
            var defaultJsonStorage = CreateJsonStorage();
            defaultJsonStorage.CreateString("string");
        }

        [Fact]
        public void CreateMemberTest()
        {
            var defaultJsonStorage = CreateJsonStorage();
            var document = defaultJsonStorage.CreateDocument("documentName");
            defaultJsonStorage.AttachObject(document);
            defaultJsonStorage.CreateMember("keyName");
        }

        [Fact]
        public void AttachObjectValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLinkAddress document = defaultJsonStorage.CreateDocument("documentName");
            TLinkAddress documentValueLink = defaultJsonStorage.AttachObject(document);
            TLinkAddress createdObjectValue = links.GetTarget(documentValueLink);

            TLinkAddress valueMarker = links.GetSource(createdObjectValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLinkAddress createdObject = links.GetTarget(createdObjectValue);
            TLinkAddress objectMarker = links.GetSource(createdObject);
            Assert.Equal(objectMarker, defaultJsonStorage.ObjectMarker);

            TLinkAddress foundDocumentValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdObjectValue, foundDocumentValue);
        }

        [Fact]
        public void AttachStringValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLinkAddress document = defaultJsonStorage.CreateDocument("documentName");
            TLinkAddress documentStringLink = defaultJsonStorage.AttachString(document, "stringName");
            TLinkAddress createdStringValue = links.GetTarget(documentStringLink);

            TLinkAddress valueMarker = links.GetSource(createdStringValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLinkAddress createdString = links.GetTarget(createdStringValue);
            TLinkAddress stringMarker = links.GetSource(createdString);
            Assert.Equal(stringMarker, defaultJsonStorage.StringMarker);

            TLinkAddress foundStringValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdStringValue, foundStringValue);
        }

        [Fact]
        public void AttachNumberToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage = CreateJsonStorage(links);
            TLinkAddress document = defaultJsonStorage.CreateDocument("documentName");
            TLinkAddress documentNumberLink = defaultJsonStorage.AttachNumber(document, 2021);
            TLinkAddress createdNumberValue = links.GetTarget(documentNumberLink);

            TLinkAddress valueMarker = links.GetSource(createdNumberValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLinkAddress createdNumber = links.GetTarget(createdNumberValue);
            TLinkAddress numberMarker = links.GetSource(createdNumber);
            Assert.Equal(numberMarker, defaultJsonStorage.NumberMarker);

            TLinkAddress foundNumberValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdNumberValue, foundNumberValue);
        }

        [Fact]
        public void AttachTrueValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLinkAddress document = defaultJsonStorage.CreateDocument("documentName");

            TLinkAddress documentTrueValueLink = defaultJsonStorage.AttachBoolean(document, true);
            TLinkAddress createdTrueValue = links.GetTarget(documentTrueValueLink);

            TLinkAddress valueMarker = links.GetSource(createdTrueValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLinkAddress trueMarker = links.GetTarget(createdTrueValue);
            Assert.Equal(trueMarker, defaultJsonStorage.TrueMarker);

            TLinkAddress foundTrueValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdTrueValue, foundTrueValue);
        }

        [Fact]
        public void AttachFalseValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLinkAddress document = defaultJsonStorage.CreateDocument("documentName");

            TLinkAddress documentFalseValueLink = defaultJsonStorage.AttachBoolean(document, false);
            TLinkAddress createdFalseValue = links.GetTarget(documentFalseValueLink);

            TLinkAddress valueMarker = links.GetSource(createdFalseValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLinkAddress falseMarker = links.GetTarget(createdFalseValue);
            Assert.Equal(falseMarker, defaultJsonStorage.FalseMarker);

            TLinkAddress foundFalseValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdFalseValue, foundFalseValue);
        }

        [Fact]
        public void AttachNullValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLinkAddress document = defaultJsonStorage.CreateDocument("documentName");

            TLinkAddress documentNullValueLink = defaultJsonStorage.AttachNull(document);
            TLinkAddress createdNullValue = links.GetTarget(documentNullValueLink);

            TLinkAddress valueMarker = links.GetSource(createdNullValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLinkAddress nullMarker = links.GetTarget(createdNullValue);
            Assert.Equal(nullMarker, defaultJsonStorage.NullMarker);

            TLinkAddress foundNullValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdNullValue, foundNullValue);
        }

        [Fact]
        public void AttachEmptyArrayValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLinkAddress document = defaultJsonStorage.CreateDocument("documentName");

            TLinkAddress documentArrayValueLink = defaultJsonStorage.AttachArray(document, new TLinkAddress[0]);
            TLinkAddress createdArrayValue = links.GetTarget(documentArrayValueLink);
            output.WriteLine(links.Format(createdArrayValue));


            TLinkAddress valueMarker = links.GetSource(createdArrayValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLinkAddress createdArrayLink = links.GetTarget(createdArrayValue);
            TLinkAddress arrayMarker = links.GetSource(createdArrayLink);
            Assert.Equal(arrayMarker, defaultJsonStorage.ArrayMarker);

            TLinkAddress createArrayContents = links.GetTarget(createdArrayLink);
            Assert.Equal(createArrayContents, defaultJsonStorage.EmptyArrayMarker);

            TLinkAddress foundArrayValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdArrayValue, foundArrayValue);
        }

        [Fact]
        public void AttachArrayValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLinkAddress document = defaultJsonStorage.CreateDocument("documentName");

            TLinkAddress arrayElement = defaultJsonStorage.CreateString("arrayElement");
            TLinkAddress[] array = new TLinkAddress[] { arrayElement, arrayElement, arrayElement };


            TLinkAddress documentArrayValueLink = defaultJsonStorage.AttachArray(document, array);
            TLinkAddress createdArrayValue = links.GetTarget(documentArrayValueLink);

            DefaultStack<TLinkAddress> stack = new();
            RightSequenceWalker<TLinkAddress> rightSequenceWalker = new(links, stack, arrayElementLink => links.GetSource(arrayElementLink) == defaultJsonStorage.ValueMarker);
            IEnumerable<TLinkAddress> arrayElementsValuesLink = rightSequenceWalker.Walk(createdArrayValue);
            Assert.NotEmpty(arrayElementsValuesLink);

            output.WriteLine(links.Format(createdArrayValue));


            TLinkAddress valueMarker = links.GetSource(createdArrayValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLinkAddress createdArrayLink = links.GetTarget(createdArrayValue);
            TLinkAddress arrayMarker = links.GetSource(createdArrayLink);
            Assert.Equal(arrayMarker, defaultJsonStorage.ArrayMarker);

            TLinkAddress createdArrayContents = links.GetTarget(createdArrayLink);
            Assert.Equal(links.GetTarget(createdArrayContents), arrayElement);


            TLinkAddress foundArrayValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdArrayValue, foundArrayValue);
        }

        [Fact]
        public void GetObjectFromDocumentObjectValueLinkTest()
        {
            ILinks<TLinkAddress> links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLinkAddress document = defaultJsonStorage.CreateDocument("documentName");
            TLinkAddress documentObjectValueLink = defaultJsonStorage.AttachObject(document);
            TLinkAddress objectValueLink = links.GetTarget(documentObjectValueLink);
            TLinkAddress objectFromGetObject = defaultJsonStorage.GetObject(documentObjectValueLink);
            output.WriteLine(links.Format(objectValueLink));
            output.WriteLine(links.Format(objectFromGetObject));
            Assert.Equal(links.GetTarget(objectValueLink), objectFromGetObject);
        }

        [Fact]
        public void GetObjectFromObjectValueLinkTest()
        {
            ILinks<TLinkAddress> links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLinkAddress document = defaultJsonStorage.CreateDocument("documentName");
            TLinkAddress documentObjectValueLink = defaultJsonStorage.AttachObject(document);
            TLinkAddress objectValueLink = links.GetTarget(documentObjectValueLink);
            TLinkAddress objectFromGetObject = defaultJsonStorage.GetObject(objectValueLink);
            Assert.Equal(links.GetTarget(objectValueLink), objectFromGetObject);
        }

        [Fact]
        public void AttachStringValueToKey()
        {
            ILinks<TLinkAddress> links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLinkAddress document = defaultJsonStorage.CreateDocument("documentName");
            TLinkAddress documentObjectValue = defaultJsonStorage.AttachObject(document);
            TLinkAddress @object = defaultJsonStorage.GetObject(documentObjectValue);
            TLinkAddress memberLink = defaultJsonStorage.AttachMemberToObject(@object, "keyName");
            TLinkAddress memberStringValueLink = defaultJsonStorage.AttachString(memberLink, "stringValue");
            TLinkAddress stringValueLink = links.GetTarget(memberStringValueLink);
            List<TLinkAddress> objectMembersLinks = defaultJsonStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(stringValueLink, defaultJsonStorage.GetValueLink(objectMembersLinks[0]));
        }

        [Fact]
        public void AttachNumberValueToKey()
        {
            ILinks<TLinkAddress> links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLinkAddress document = defaultJsonStorage.CreateDocument("documentName");
            TLinkAddress documentObjectValue = defaultJsonStorage.AttachObject(document);
            TLinkAddress @object = defaultJsonStorage.GetObject(documentObjectValue);
            TLinkAddress memberLink = defaultJsonStorage.AttachMemberToObject(@object, "keyName");
            TLinkAddress memberNumberValueLink = defaultJsonStorage.AttachNumber(memberLink, 123);
            TLinkAddress numberValueLink = links.GetTarget(memberNumberValueLink);
            List<TLinkAddress> objectMembersLinks = defaultJsonStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(numberValueLink, defaultJsonStorage.GetValueLink(objectMembersLinks[0]));
        }

        [Fact]
        public void AttachObjectValueToKey()
        {
            ILinks<TLinkAddress> links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLinkAddress document = defaultJsonStorage.CreateDocument("documentName");
            TLinkAddress documentObjectValue = defaultJsonStorage.AttachObject(document);
            TLinkAddress @object = defaultJsonStorage.GetObject(documentObjectValue);
            TLinkAddress memberLink = defaultJsonStorage.AttachMemberToObject(@object, "keyName");
            TLinkAddress memberObjectValueLink = defaultJsonStorage.AttachObject(memberLink);
            TLinkAddress objectValueLink = links.GetTarget(memberObjectValueLink);
            List<TLinkAddress> objectMembersLinks = defaultJsonStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(objectValueLink, defaultJsonStorage.GetValueLink(objectMembersLinks[0]));
        }

        [Fact]
        public void AttachArrayValueToKey()
        {
            ILinks<TLinkAddress> links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLinkAddress document = defaultJsonStorage.CreateDocument("documentName");
            TLinkAddress documentObjectValue = defaultJsonStorage.AttachObject(document);
            TLinkAddress @object = defaultJsonStorage.GetObject(documentObjectValue);
            TLinkAddress memberLink = defaultJsonStorage.AttachMemberToObject(@object, "keyName");
            TLinkAddress arrayElement = defaultJsonStorage.CreateString("arrayElement");
            TLinkAddress[] array = { arrayElement, arrayElement, arrayElement };
            TLinkAddress memberArrayValueLink = defaultJsonStorage.AttachArray(memberLink, array);
            TLinkAddress arrayValueLink = links.GetTarget(memberArrayValueLink);
            List<TLinkAddress> objectMembersLinks = defaultJsonStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(arrayValueLink, defaultJsonStorage.GetValueLink(objectMembersLinks[0]));
        }

        [Fact]
        public void AttachTrueValueToKey()
        {
            ILinks<TLinkAddress> links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLinkAddress document = defaultJsonStorage.CreateDocument("documentName");
            TLinkAddress documentObjectValue = defaultJsonStorage.AttachObject(document);
            TLinkAddress @object = defaultJsonStorage.GetObject(documentObjectValue);
            TLinkAddress memberLink = defaultJsonStorage.AttachMemberToObject(@object, "keyName");
            TLinkAddress memberTrueValueLink = defaultJsonStorage.AttachBoolean(memberLink, true);
            TLinkAddress trueValueLink = links.GetTarget(memberTrueValueLink);
            List<TLinkAddress> objectMembersLinks = defaultJsonStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(trueValueLink, defaultJsonStorage.GetValueLink(objectMembersLinks[0]));
        }

        [Fact]
        public void AttachFalseValueToKey()
        {
            ILinks<TLinkAddress> links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLinkAddress document = defaultJsonStorage.CreateDocument("documentName");
            TLinkAddress documentObjectValue = defaultJsonStorage.AttachObject(document);
            TLinkAddress @object = defaultJsonStorage.GetObject(documentObjectValue);
            TLinkAddress memberLink = defaultJsonStorage.AttachMemberToObject(@object, "keyName");
            TLinkAddress memberFalseValueLink = defaultJsonStorage.AttachBoolean(memberLink, false);
            TLinkAddress falseValueLink = links.GetTarget(memberFalseValueLink);
            List<TLinkAddress> objectMembersLinks = defaultJsonStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(falseValueLink, defaultJsonStorage.GetValueLink(objectMembersLinks[0]));
        }

        [Fact]
        public void AttachNullValueToKey()
        {
            ILinks<TLinkAddress> links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLinkAddress document = defaultJsonStorage.CreateDocument("documentName");
            TLinkAddress documentObjectValue = defaultJsonStorage.AttachObject(document);
            TLinkAddress @object = defaultJsonStorage.GetObject(documentObjectValue);
            TLinkAddress memberLink = defaultJsonStorage.AttachMemberToObject(@object, "keyName");
            TLinkAddress memberNullValueLink = defaultJsonStorage.AttachNull(memberLink);
            TLinkAddress nullValueLink = links.GetTarget(memberNullValueLink);
            List<TLinkAddress> objectMembersLinks = defaultJsonStorage.GetMembersLinks(@object);
            Assert.Equal(nullValueLink, defaultJsonStorage.GetValueLink(objectMembersLinks[0]));
        }
    }
}
