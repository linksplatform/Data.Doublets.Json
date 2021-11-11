using Xunit;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Memory;
using Platform.Memory;
using TLink = System.UInt32;
using Xunit.Abstractions;
using Platform.Collections.Stacks;
using Platform.Data.Doublets.Sequences.Walkers;
using System.Collections.Generic;
using Platform.Data.Doublets.Sequences.Converters;

namespace Platform.Data.Doublets.Json.Tests
{
    /// <summary>
    /// <para>
    /// Represents the json storage tests.
    /// </para>
    /// <para></para>
    /// </summary>
    public class JsonStorageTests
    {
        private readonly ITestOutputHelper output;
        /// <summary>
        /// <para>
        /// The balanced variant converter.
        /// </para>
        /// <para></para>
        /// </summary>
        public static BalancedVariantConverter<TLink> BalancedVariantConverter;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="JsonStorageTests"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="output">
        /// <para>A output.</para>
        /// <para></para>
        /// </param>
        public JsonStorageTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        /// <summary>
        /// <para>
        /// Creates the links.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>A links of t link</para>
        /// <para></para>
        /// </returns>
        public static ILinks<TLink> CreateLinks() => CreateLinks<TLink>(new Platform.IO.TemporaryFile());

        /// <summary>
        /// <para>
        /// Creates the links using the specified data db filename.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <typeparam name="TLink">
        /// <para>The link.</para>
        /// <para></para>
        /// </typeparam>
        /// <param name="dataDBFilename">
        /// <para>The data db filename.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>A links of t link</para>
        /// <para></para>
        /// </returns>
        public static ILinks<TLink> CreateLinks<TLink>(string dataDBFilename)
        {
            var linksConstants = new LinksConstants<TLink>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLink>(new FileMappedResizableDirectMemory(dataDBFilename), UnitedMemoryLinks<TLink>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }

        /// <summary>
        /// <para>
        /// Creates the json storage.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>A default json storage of t link</para>
        /// <para></para>
        /// </returns>
        public static DefaultJsonStorage<TLink> CreateJsonStorage()
        {
            var links = CreateLinks();
            return CreateJsonStorage(links);
        }

        /// <summary>
        /// <para>
        /// Creates the json storage using the specified links.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>The links.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>A default json storage of t link</para>
        /// <para></para>
        /// </returns>
        public static DefaultJsonStorage<TLink> CreateJsonStorage(ILinks<TLink> links)
        {
            BalancedVariantConverter = new(links);
            return new DefaultJsonStorage<TLink>(links, BalancedVariantConverter);
        }

        /// <summary>
        /// <para>
        /// Tests that constructors test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void ConstructorsTest() => CreateJsonStorage();

        /// <summary>
        /// <para>
        /// Tests that create document test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void CreateDocumentTest()
        {
            var defaultJsonStorage = CreateJsonStorage();
            defaultJsonStorage.CreateDocument("documentName");
        }

        /// <summary>
        /// <para>
        /// Tests that get document test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void GetDocumentTest()
        {
            var defaultJsonStorage = CreateJsonStorage();
            var createdDocumentLink = defaultJsonStorage.CreateDocument("documentName");
            var foundDocumentLink = defaultJsonStorage.GetDocumentOrDefault("documentName");
            Assert.Equal(createdDocumentLink, foundDocumentLink);
        }

        /// <summary>
        /// <para>
        /// Tests that create object test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void CreateObjectTest()
        {
            var defaultJsonStorage = CreateJsonStorage();
            var object0 = defaultJsonStorage.CreateObjectValue();
            var object1 = defaultJsonStorage.CreateObjectValue();
            Assert.NotEqual(object0, object1);
        }

        /// <summary>
        /// <para>
        /// Tests that create string test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void CreateStringTest()
        {
            var defaultJsonStorage = CreateJsonStorage();
            defaultJsonStorage.CreateString("string");
        }

        /// <summary>
        /// <para>
        /// Tests that create member test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void CreateMemberTest()
        {
            var defaultJsonStorage = CreateJsonStorage();
            var document = defaultJsonStorage.CreateDocument("documentName");
            defaultJsonStorage.AttachObject(document);
            defaultJsonStorage.CreateMember("keyName");
        }

        /// <summary>
        /// <para>
        /// Tests that attach object value to document test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void AttachObjectValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");
            TLink documentValueLink = defaultJsonStorage.AttachObject(document);
            TLink createdObjectValue = links.GetTarget(documentValueLink);

            TLink valueMarker = links.GetSource(createdObjectValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLink createdObject = links.GetTarget(createdObjectValue);
            TLink objectMarker = links.GetSource(createdObject);
            Assert.Equal(objectMarker, defaultJsonStorage.ObjectMarker);

            TLink foundDocumentValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdObjectValue, foundDocumentValue);
        }

        /// <summary>
        /// <para>
        /// Tests that attach string value to document test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void AttachStringValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");
            TLink documentStringLink = defaultJsonStorage.AttachString(document, "stringName");
            TLink createdStringValue = links.GetTarget(documentStringLink);

            TLink valueMarker = links.GetSource(createdStringValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLink createdString = links.GetTarget(createdStringValue);
            TLink stringMarker = links.GetSource(createdString);
            Assert.Equal(stringMarker, defaultJsonStorage.StringMarker);

            TLink foundStringValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdStringValue, foundStringValue);
        }

        /// <summary>
        /// <para>
        /// Tests that attach number to document test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void AttachNumberToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage = CreateJsonStorage(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");
            TLink documentNumberLink = defaultJsonStorage.AttachNumber(document, 2021);
            TLink createdNumberValue = links.GetTarget(documentNumberLink);

            TLink valueMarker = links.GetSource(createdNumberValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLink createdNumber = links.GetTarget(createdNumberValue);
            TLink numberMarker = links.GetSource(createdNumber);
            Assert.Equal(numberMarker, defaultJsonStorage.NumberMarker);

            TLink foundNumberValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdNumberValue, foundNumberValue);
        }

        /// <summary>
        /// <para>
        /// Tests that attach true value to document test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void AttachTrueValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");

            TLink documentTrueValueLink = defaultJsonStorage.AttachBoolean(document, true);
            TLink createdTrueValue = links.GetTarget(documentTrueValueLink);

            TLink valueMarker = links.GetSource(createdTrueValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLink trueMarker = links.GetTarget(createdTrueValue);
            Assert.Equal(trueMarker, defaultJsonStorage.TrueMarker);

            TLink foundTrueValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdTrueValue, foundTrueValue);
        }

        /// <summary>
        /// <para>
        /// Tests that attach false value to document test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void AttachFalseValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");

            TLink documentFalseValueLink = defaultJsonStorage.AttachBoolean(document, false);
            TLink createdFalseValue = links.GetTarget(documentFalseValueLink);

            TLink valueMarker = links.GetSource(createdFalseValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLink falseMarker = links.GetTarget(createdFalseValue);
            Assert.Equal(falseMarker, defaultJsonStorage.FalseMarker);

            TLink foundFalseValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdFalseValue, foundFalseValue);
        }

        /// <summary>
        /// <para>
        /// Tests that attach null value to document test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void AttachNullValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");

            TLink documentNullValueLink = defaultJsonStorage.AttachNull(document);
            TLink createdNullValue = links.GetTarget(documentNullValueLink);

            TLink valueMarker = links.GetSource(createdNullValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);

            TLink nullMarker = links.GetTarget(createdNullValue);
            Assert.Equal(nullMarker, defaultJsonStorage.NullMarker);

            TLink foundNullValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdNullValue, foundNullValue);
        }

        /// <summary>
        /// <para>
        /// Tests that attach empty array value to document test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void AttachEmptyArrayValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
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

            TLink foundArrayValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdArrayValue, foundArrayValue);
        }

        /// <summary>
        /// <para>
        /// Tests that attach array value to document test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void AttachArrayValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");

            TLink arrayElement = defaultJsonStorage.CreateString("arrayElement");
            TLink[] array = new TLink[] { arrayElement, arrayElement, arrayElement };


            TLink documentArrayValueLink = defaultJsonStorage.AttachArray(document, array);
            TLink createdArrayValue = links.GetTarget(documentArrayValueLink);

            DefaultStack<TLink> stack = new();
            RightSequenceWalker<TLink> rightSequenceWalker = new(links, stack, arrayElementLink => links.GetSource(arrayElementLink) == defaultJsonStorage.ValueMarker);
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


            TLink foundArrayValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdArrayValue, foundArrayValue);
        }

        /// <summary>
        /// <para>
        /// Tests that get object from document object value link test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void GetObjectFromDocumentObjectValueLinkTest()
        {
            ILinks<TLink> links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");
            TLink documentObjectValueLink = defaultJsonStorage.AttachObject(document);
            TLink objectValueLink = links.GetTarget(documentObjectValueLink);
            TLink objectFromGetObject = defaultJsonStorage.GetObject(documentObjectValueLink);
            output.WriteLine(links.Format(objectValueLink));
            output.WriteLine(links.Format(objectFromGetObject));
            Assert.Equal(links.GetTarget(objectValueLink), objectFromGetObject);
        }

        /// <summary>
        /// <para>
        /// Tests that get object from object value link test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void GetObjectFromObjectValueLinkTest()
        {
            ILinks<TLink> links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");
            TLink documentObjectValueLink = defaultJsonStorage.AttachObject(document);
            TLink objectValueLink = links.GetTarget(documentObjectValueLink);
            TLink objectFromGetObject = defaultJsonStorage.GetObject(objectValueLink);
            Assert.Equal(links.GetTarget(objectValueLink), objectFromGetObject);
        }

        /// <summary>
        /// <para>
        /// Tests that attach string value to key.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void AttachStringValueToKey()
        {
            ILinks<TLink> links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");
            TLink documentObjectValue = defaultJsonStorage.AttachObject(document);
            TLink @object = defaultJsonStorage.GetObject(documentObjectValue);
            TLink memberLink = defaultJsonStorage.AttachMemberToObject(@object, "keyName");
            TLink memberStringValueLink = defaultJsonStorage.AttachString(memberLink, "stringValue");
            TLink stringValueLink = links.GetTarget(memberStringValueLink);
            List<TLink> objectMembersLinks = defaultJsonStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(stringValueLink, defaultJsonStorage.GetValueLink(objectMembersLinks[0]));
        }

        /// <summary>
        /// <para>
        /// Tests that attach number value to key.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void AttachNumberValueToKey()
        {
            ILinks<TLink> links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");
            TLink documentObjectValue = defaultJsonStorage.AttachObject(document);
            TLink @object = defaultJsonStorage.GetObject(documentObjectValue);
            TLink memberLink = defaultJsonStorage.AttachMemberToObject(@object, "keyName");
            TLink memberNumberValueLink = defaultJsonStorage.AttachNumber(memberLink, 123);
            TLink numberValueLink = links.GetTarget(memberNumberValueLink);
            List<TLink> objectMembersLinks = defaultJsonStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(numberValueLink, defaultJsonStorage.GetValueLink(objectMembersLinks[0]));
        }

        /// <summary>
        /// <para>
        /// Tests that attach object value to key.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void AttachObjectValueToKey()
        {
            ILinks<TLink> links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");
            TLink documentObjectValue = defaultJsonStorage.AttachObject(document);
            TLink @object = defaultJsonStorage.GetObject(documentObjectValue);
            TLink memberLink = defaultJsonStorage.AttachMemberToObject(@object, "keyName");
            TLink memberObjectValueLink = defaultJsonStorage.AttachObject(memberLink);
            TLink objectValueLink = links.GetTarget(memberObjectValueLink);
            List<TLink> objectMembersLinks = defaultJsonStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(objectValueLink, defaultJsonStorage.GetValueLink(objectMembersLinks[0]));
        }

        /// <summary>
        /// <para>
        /// Tests that attach array value to key.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void AttachArrayValueToKey()
        {
            ILinks<TLink> links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");
            TLink documentObjectValue = defaultJsonStorage.AttachObject(document);
            TLink @object = defaultJsonStorage.GetObject(documentObjectValue);
            TLink memberLink = defaultJsonStorage.AttachMemberToObject(@object, "keyName");
            TLink arrayElement = defaultJsonStorage.CreateString("arrayElement");
            TLink[] array = { arrayElement, arrayElement, arrayElement };
            TLink memberArrayValueLink = defaultJsonStorage.AttachArray(memberLink, array);
            TLink arrayValueLink = links.GetTarget(memberArrayValueLink);
            List<TLink> objectMembersLinks = defaultJsonStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(arrayValueLink, defaultJsonStorage.GetValueLink(objectMembersLinks[0]));
        }

        /// <summary>
        /// <para>
        /// Tests that attach true value to key.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void AttachTrueValueToKey()
        {
            ILinks<TLink> links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");
            TLink documentObjectValue = defaultJsonStorage.AttachObject(document);
            TLink @object = defaultJsonStorage.GetObject(documentObjectValue);
            TLink memberLink = defaultJsonStorage.AttachMemberToObject(@object, "keyName");
            TLink memberTrueValueLink = defaultJsonStorage.AttachBoolean(memberLink, true);
            TLink trueValueLink = links.GetTarget(memberTrueValueLink);
            List<TLink> objectMembersLinks = defaultJsonStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(trueValueLink, defaultJsonStorage.GetValueLink(objectMembersLinks[0]));
        }

        /// <summary>
        /// <para>
        /// Tests that attach false value to key.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void AttachFalseValueToKey()
        {
            ILinks<TLink> links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");
            TLink documentObjectValue = defaultJsonStorage.AttachObject(document);
            TLink @object = defaultJsonStorage.GetObject(documentObjectValue);
            TLink memberLink = defaultJsonStorage.AttachMemberToObject(@object, "keyName");
            TLink memberFalseValueLink = defaultJsonStorage.AttachBoolean(memberLink, false);
            TLink falseValueLink = links.GetTarget(memberFalseValueLink);
            List<TLink> objectMembersLinks = defaultJsonStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(falseValueLink, defaultJsonStorage.GetValueLink(objectMembersLinks[0]));
        }

        /// <summary>
        /// <para>
        /// Tests that attach null value to key.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void AttachNullValueToKey()
        {
            ILinks<TLink> links = CreateLinks();
            var defaultJsonStorage =CreateJsonStorage(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");
            TLink documentObjectValue = defaultJsonStorage.AttachObject(document);
            TLink @object = defaultJsonStorage.GetObject(documentObjectValue);
            TLink memberLink = defaultJsonStorage.AttachMemberToObject(@object, "keyName");
            TLink memberNullValueLink = defaultJsonStorage.AttachNull(memberLink);
            TLink nullValueLink = links.GetTarget(memberNullValueLink);
            List<TLink> objectMembersLinks = defaultJsonStorage.GetMembersLinks(@object);
            Assert.Equal(nullValueLink, defaultJsonStorage.GetValueLink(objectMembersLinks[0]));
        }
    }
}
