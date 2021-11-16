using Platform.Collections.Stacks;
using Platform.Data.Doublets.Memory;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.IO;
using Platform.Memory;
using Xunit;
using Xunit.Abstractions;
using TLink = System.UInt32;

namespace Platform.Data.Doublets.Json.Tests
{
    /// <summary>
    ///     <para>
    ///         Represents the json storage tests.
    ///     </para>
    ///     <para></para>
    /// </summary>
    public class JsonStorageTests
    {
        /// <summary>
        ///     <para>
        ///         The balanced variant converter.
        ///     </para>
        ///     <para></para>
        /// </summary>
        public static BalancedVariantConverter<uint> BalancedVariantConverter;

        private readonly ITestOutputHelper output;

        /// <summary>
        ///     <para>
        ///         Initializes a new <see cref="JsonStorageTests" /> instance.
        ///     </para>
        ///     <para></para>
        /// </summary>
        /// <param name="output">
        ///     <para>A output.</para>
        ///     <para></para>
        /// </param>
        public JsonStorageTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        /// <summary>
        ///     <para>
        ///         Creates the links.
        ///     </para>
        ///     <para></para>
        /// </summary>
        /// <returns>
        ///     <para>A links of t link</para>
        ///     <para></para>
        /// </returns>
        public static ILinks<uint> CreateLinks()
        {
            return CreateLinks<uint>(new TemporaryFile());
        }

        /// <summary>
        ///     <para>
        ///         Creates the links using the specified data db filename.
        ///     </para>
        ///     <para></para>
        /// </summary>
        /// <typeparam name="TLink">
        ///     <para>The link.</para>
        ///     <para></para>
        /// </typeparam>
        /// <param name="dataDBFilename">
        ///     <para>The data db filename.</para>
        ///     <para></para>
        /// </param>
        /// <returns>
        ///     <para>A links of t link</para>
        ///     <para></para>
        /// </returns>
        public static ILinks<TLink> CreateLinks<TLink>(string dataDBFilename)
        {
            var linksConstants = new LinksConstants<TLink>(true);
            return new UnitedMemoryLinks<TLink>(new FileMappedResizableDirectMemory(dataDBFilename), UnitedMemoryLinks<TLink>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }

        /// <summary>
        ///     <para>
        ///         Creates the json storage.
        ///     </para>
        ///     <para></para>
        /// </summary>
        /// <returns>
        ///     <para>A default json storage of t link</para>
        ///     <para></para>
        /// </returns>
        public static DefaultJsonStorage<uint> CreateJsonStorage()
        {
            var links = CreateLinks();
            return CreateJsonStorage(links);
        }

        /// <summary>
        ///     <para>
        ///         Creates the json storage using the specified links.
        ///     </para>
        ///     <para></para>
        /// </summary>
        /// <param name="links">
        ///     <para>The links.</para>
        ///     <para></para>
        /// </param>
        /// <returns>
        ///     <para>A default json storage of t link</para>
        ///     <para></para>
        /// </returns>
        public static DefaultJsonStorage<uint> CreateJsonStorage(ILinks<uint> links)
        {
            BalancedVariantConverter = new BalancedVariantConverter<uint>(links);
            return new DefaultJsonStorage<uint>(links, BalancedVariantConverter);
        }

        /// <summary>
        ///     <para>
        ///         Tests that constructors test.
        ///     </para>
        ///     <para></para>
        /// </summary>
        [Fact]
        public void ConstructorsTest()
        {
            CreateJsonStorage();
        }

        /// <summary>
        ///     <para>
        ///         Tests that create document test.
        ///     </para>
        ///     <para></para>
        /// </summary>
        [Fact]
        public void CreateDocumentTest()
        {
            var defaultJsonStorage = CreateJsonStorage();
            defaultJsonStorage.CreateDocument("documentName");
        }

        /// <summary>
        ///     <para>
        ///         Tests that get document test.
        ///     </para>
        ///     <para></para>
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
        ///     <para>
        ///         Tests that create object test.
        ///     </para>
        ///     <para></para>
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
        ///     <para>
        ///         Tests that create string test.
        ///     </para>
        ///     <para></para>
        /// </summary>
        [Fact]
        public void CreateStringTest()
        {
            var defaultJsonStorage = CreateJsonStorage();
            defaultJsonStorage.CreateString("string");
        }

        /// <summary>
        ///     <para>
        ///         Tests that create member test.
        ///     </para>
        ///     <para></para>
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
        ///     <para>
        ///         Tests that attach object value to document test.
        ///     </para>
        ///     <para></para>
        /// </summary>
        [Fact]
        public void AttachObjectValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage = CreateJsonStorage(links);
            var document = defaultJsonStorage.CreateDocument("documentName");
            var documentValueLink = defaultJsonStorage.AttachObject(document);
            var createdObjectValue = links.GetTarget(documentValueLink);
            var valueMarker = links.GetSource(createdObjectValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);
            var createdObject = links.GetTarget(createdObjectValue);
            var objectMarker = links.GetSource(createdObject);
            Assert.Equal(objectMarker, defaultJsonStorage.ObjectMarker);
            var foundDocumentValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdObjectValue, foundDocumentValue);
        }

        /// <summary>
        ///     <para>
        ///         Tests that attach string value to document test.
        ///     </para>
        ///     <para></para>
        /// </summary>
        [Fact]
        public void AttachStringValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage = CreateJsonStorage(links);
            var document = defaultJsonStorage.CreateDocument("documentName");
            var documentStringLink = defaultJsonStorage.AttachString(document, "stringName");
            var createdStringValue = links.GetTarget(documentStringLink);
            var valueMarker = links.GetSource(createdStringValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);
            var createdString = links.GetTarget(createdStringValue);
            var stringMarker = links.GetSource(createdString);
            Assert.Equal(stringMarker, defaultJsonStorage.StringMarker);
            var foundStringValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdStringValue, foundStringValue);
        }

        /// <summary>
        ///     <para>
        ///         Tests that attach number to document test.
        ///     </para>
        ///     <para></para>
        /// </summary>
        [Fact]
        public void AttachNumberToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage = CreateJsonStorage(links);
            var document = defaultJsonStorage.CreateDocument("documentName");
            var documentNumberLink = defaultJsonStorage.AttachNumber(document, 2021);
            var createdNumberValue = links.GetTarget(documentNumberLink);
            var valueMarker = links.GetSource(createdNumberValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);
            var createdNumber = links.GetTarget(createdNumberValue);
            var numberMarker = links.GetSource(createdNumber);
            Assert.Equal(numberMarker, defaultJsonStorage.NumberMarker);
            var foundNumberValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdNumberValue, foundNumberValue);
        }

        /// <summary>
        ///     <para>
        ///         Tests that attach true value to document test.
        ///     </para>
        ///     <para></para>
        /// </summary>
        [Fact]
        public void AttachTrueValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage = CreateJsonStorage(links);
            var document = defaultJsonStorage.CreateDocument("documentName");
            var documentTrueValueLink = defaultJsonStorage.AttachBoolean(document, true);
            var createdTrueValue = links.GetTarget(documentTrueValueLink);
            var valueMarker = links.GetSource(createdTrueValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);
            var trueMarker = links.GetTarget(createdTrueValue);
            Assert.Equal(trueMarker, defaultJsonStorage.TrueMarker);
            var foundTrueValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdTrueValue, foundTrueValue);
        }

        /// <summary>
        ///     <para>
        ///         Tests that attach false value to document test.
        ///     </para>
        ///     <para></para>
        /// </summary>
        [Fact]
        public void AttachFalseValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage = CreateJsonStorage(links);
            var document = defaultJsonStorage.CreateDocument("documentName");
            var documentFalseValueLink = defaultJsonStorage.AttachBoolean(document, false);
            var createdFalseValue = links.GetTarget(documentFalseValueLink);
            var valueMarker = links.GetSource(createdFalseValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);
            var falseMarker = links.GetTarget(createdFalseValue);
            Assert.Equal(falseMarker, defaultJsonStorage.FalseMarker);
            var foundFalseValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdFalseValue, foundFalseValue);
        }

        /// <summary>
        ///     <para>
        ///         Tests that attach null value to document test.
        ///     </para>
        ///     <para></para>
        /// </summary>
        [Fact]
        public void AttachNullValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage = CreateJsonStorage(links);
            var document = defaultJsonStorage.CreateDocument("documentName");
            var documentNullValueLink = defaultJsonStorage.AttachNull(document);
            var createdNullValue = links.GetTarget(documentNullValueLink);
            var valueMarker = links.GetSource(createdNullValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);
            var nullMarker = links.GetTarget(createdNullValue);
            Assert.Equal(nullMarker, defaultJsonStorage.NullMarker);
            var foundNullValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdNullValue, foundNullValue);
        }

        /// <summary>
        ///     <para>
        ///         Tests that attach empty array value to document test.
        ///     </para>
        ///     <para></para>
        /// </summary>
        [Fact]
        public void AttachEmptyArrayValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage = CreateJsonStorage(links);
            var document = defaultJsonStorage.CreateDocument("documentName");
            var documentArrayValueLink = defaultJsonStorage.AttachArray(document, new uint[0]);
            var createdArrayValue = links.GetTarget(documentArrayValueLink);
            output.WriteLine(links.Format(createdArrayValue));
            var valueMarker = links.GetSource(createdArrayValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);
            var createdArrayLink = links.GetTarget(createdArrayValue);
            var arrayMarker = links.GetSource(createdArrayLink);
            Assert.Equal(arrayMarker, defaultJsonStorage.ArrayMarker);
            var createArrayContents = links.GetTarget(createdArrayLink);
            Assert.Equal(createArrayContents, defaultJsonStorage.EmptyArrayMarker);
            var foundArrayValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdArrayValue, foundArrayValue);
        }

        /// <summary>
        ///     <para>
        ///         Tests that attach array value to document test.
        ///     </para>
        ///     <para></para>
        /// </summary>
        [Fact]
        public void AttachArrayValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage = CreateJsonStorage(links);
            var document = defaultJsonStorage.CreateDocument("documentName");
            var arrayElement = defaultJsonStorage.CreateString("arrayElement");
            uint[] array = { arrayElement, arrayElement, arrayElement };
            var documentArrayValueLink = defaultJsonStorage.AttachArray(document, array);
            var createdArrayValue = links.GetTarget(documentArrayValueLink);
            DefaultStack<uint> stack = new();
            RightSequenceWalker<uint> rightSequenceWalker = new(links, stack, arrayElementLink => links.GetSource(arrayElementLink) == defaultJsonStorage.ValueMarker);
            var arrayElementsValuesLink = rightSequenceWalker.Walk(createdArrayValue);
            Assert.NotEmpty(arrayElementsValuesLink);
            output.WriteLine(links.Format(createdArrayValue));
            var valueMarker = links.GetSource(createdArrayValue);
            Assert.Equal(valueMarker, defaultJsonStorage.ValueMarker);
            var createdArrayLink = links.GetTarget(createdArrayValue);
            var arrayMarker = links.GetSource(createdArrayLink);
            Assert.Equal(arrayMarker, defaultJsonStorage.ArrayMarker);
            var createdArrayContents = links.GetTarget(createdArrayLink);
            Assert.Equal(links.GetTarget(createdArrayContents), arrayElement);
            var foundArrayValue = defaultJsonStorage.GetValueLink(document);
            Assert.Equal(createdArrayValue, foundArrayValue);
        }

        /// <summary>
        ///     <para>
        ///         Tests that get object from document object value link test.
        ///     </para>
        ///     <para></para>
        /// </summary>
        [Fact]
        public void GetObjectFromDocumentObjectValueLinkTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage = CreateJsonStorage(links);
            var document = defaultJsonStorage.CreateDocument("documentName");
            var documentObjectValueLink = defaultJsonStorage.AttachObject(document);
            var objectValueLink = links.GetTarget(documentObjectValueLink);
            var objectFromGetObject = defaultJsonStorage.GetObject(documentObjectValueLink);
            output.WriteLine(links.Format(objectValueLink));
            output.WriteLine(links.Format(objectFromGetObject));
            Assert.Equal(links.GetTarget(objectValueLink), objectFromGetObject);
        }

        /// <summary>
        ///     <para>
        ///         Tests that get object from object value link test.
        ///     </para>
        ///     <para></para>
        /// </summary>
        [Fact]
        public void GetObjectFromObjectValueLinkTest()
        {
            var links = CreateLinks();
            var defaultJsonStorage = CreateJsonStorage(links);
            var document = defaultJsonStorage.CreateDocument("documentName");
            var documentObjectValueLink = defaultJsonStorage.AttachObject(document);
            var objectValueLink = links.GetTarget(documentObjectValueLink);
            var objectFromGetObject = defaultJsonStorage.GetObject(objectValueLink);
            Assert.Equal(links.GetTarget(objectValueLink), objectFromGetObject);
        }

        /// <summary>
        ///     <para>
        ///         Tests that attach string value to key.
        ///     </para>
        ///     <para></para>
        /// </summary>
        [Fact]
        public void AttachStringValueToKey()
        {
            var links = CreateLinks();
            var defaultJsonStorage = CreateJsonStorage(links);
            var document = defaultJsonStorage.CreateDocument("documentName");
            var documentObjectValue = defaultJsonStorage.AttachObject(document);
            var @object = defaultJsonStorage.GetObject(documentObjectValue);
            var memberLink = defaultJsonStorage.AttachMemberToObject(@object, "keyName");
            var memberStringValueLink = defaultJsonStorage.AttachString(memberLink, "stringValue");
            var stringValueLink = links.GetTarget(memberStringValueLink);
            var objectMembersLinks = defaultJsonStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(stringValueLink, defaultJsonStorage.GetValueLink(objectMembersLinks[0]));
        }

        /// <summary>
        ///     <para>
        ///         Tests that attach number value to key.
        ///     </para>
        ///     <para></para>
        /// </summary>
        [Fact]
        public void AttachNumberValueToKey()
        {
            var links = CreateLinks();
            var defaultJsonStorage = CreateJsonStorage(links);
            var document = defaultJsonStorage.CreateDocument("documentName");
            var documentObjectValue = defaultJsonStorage.AttachObject(document);
            var @object = defaultJsonStorage.GetObject(documentObjectValue);
            var memberLink = defaultJsonStorage.AttachMemberToObject(@object, "keyName");
            var memberNumberValueLink = defaultJsonStorage.AttachNumber(memberLink, 123);
            var numberValueLink = links.GetTarget(memberNumberValueLink);
            var objectMembersLinks = defaultJsonStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(numberValueLink, defaultJsonStorage.GetValueLink(objectMembersLinks[0]));
        }

        /// <summary>
        ///     <para>
        ///         Tests that attach object value to key.
        ///     </para>
        ///     <para></para>
        /// </summary>
        [Fact]
        public void AttachObjectValueToKey()
        {
            var links = CreateLinks();
            var defaultJsonStorage = CreateJsonStorage(links);
            var document = defaultJsonStorage.CreateDocument("documentName");
            var documentObjectValue = defaultJsonStorage.AttachObject(document);
            var @object = defaultJsonStorage.GetObject(documentObjectValue);
            var memberLink = defaultJsonStorage.AttachMemberToObject(@object, "keyName");
            var memberObjectValueLink = defaultJsonStorage.AttachObject(memberLink);
            var objectValueLink = links.GetTarget(memberObjectValueLink);
            var objectMembersLinks = defaultJsonStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(objectValueLink, defaultJsonStorage.GetValueLink(objectMembersLinks[0]));
        }

        /// <summary>
        ///     <para>
        ///         Tests that attach array value to key.
        ///     </para>
        ///     <para></para>
        /// </summary>
        [Fact]
        public void AttachArrayValueToKey()
        {
            var links = CreateLinks();
            var defaultJsonStorage = CreateJsonStorage(links);
            var document = defaultJsonStorage.CreateDocument("documentName");
            var documentObjectValue = defaultJsonStorage.AttachObject(document);
            var @object = defaultJsonStorage.GetObject(documentObjectValue);
            var memberLink = defaultJsonStorage.AttachMemberToObject(@object, "keyName");
            var arrayElement = defaultJsonStorage.CreateString("arrayElement");
            uint[] array = { arrayElement, arrayElement, arrayElement };
            var memberArrayValueLink = defaultJsonStorage.AttachArray(memberLink, array);
            var arrayValueLink = links.GetTarget(memberArrayValueLink);
            var objectMembersLinks = defaultJsonStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(arrayValueLink, defaultJsonStorage.GetValueLink(objectMembersLinks[0]));
        }

        /// <summary>
        ///     <para>
        ///         Tests that attach true value to key.
        ///     </para>
        ///     <para></para>
        /// </summary>
        [Fact]
        public void AttachTrueValueToKey()
        {
            var links = CreateLinks();
            var defaultJsonStorage = CreateJsonStorage(links);
            var document = defaultJsonStorage.CreateDocument("documentName");
            var documentObjectValue = defaultJsonStorage.AttachObject(document);
            var @object = defaultJsonStorage.GetObject(documentObjectValue);
            var memberLink = defaultJsonStorage.AttachMemberToObject(@object, "keyName");
            var memberTrueValueLink = defaultJsonStorage.AttachBoolean(memberLink, true);
            var trueValueLink = links.GetTarget(memberTrueValueLink);
            var objectMembersLinks = defaultJsonStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(trueValueLink, defaultJsonStorage.GetValueLink(objectMembersLinks[0]));
        }

        /// <summary>
        ///     <para>
        ///         Tests that attach false value to key.
        ///     </para>
        ///     <para></para>
        /// </summary>
        [Fact]
        public void AttachFalseValueToKey()
        {
            var links = CreateLinks();
            var defaultJsonStorage = CreateJsonStorage(links);
            var document = defaultJsonStorage.CreateDocument("documentName");
            var documentObjectValue = defaultJsonStorage.AttachObject(document);
            var @object = defaultJsonStorage.GetObject(documentObjectValue);
            var memberLink = defaultJsonStorage.AttachMemberToObject(@object, "keyName");
            var memberFalseValueLink = defaultJsonStorage.AttachBoolean(memberLink, false);
            var falseValueLink = links.GetTarget(memberFalseValueLink);
            var objectMembersLinks = defaultJsonStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(falseValueLink, defaultJsonStorage.GetValueLink(objectMembersLinks[0]));
        }

        /// <summary>
        ///     <para>
        ///         Tests that attach null value to key.
        ///     </para>
        ///     <para></para>
        /// </summary>
        [Fact]
        public void AttachNullValueToKey()
        {
            var links = CreateLinks();
            var defaultJsonStorage = CreateJsonStorage(links);
            var document = defaultJsonStorage.CreateDocument("documentName");
            var documentObjectValue = defaultJsonStorage.AttachObject(document);
            var @object = defaultJsonStorage.GetObject(documentObjectValue);
            var memberLink = defaultJsonStorage.AttachMemberToObject(@object, "keyName");
            var memberNullValueLink = defaultJsonStorage.AttachNull(memberLink);
            var nullValueLink = links.GetTarget(memberNullValueLink);
            var objectMembersLinks = defaultJsonStorage.GetMembersLinks(@object);
            Assert.Equal(nullValueLink, defaultJsonStorage.GetValueLink(objectMembersLinks[0]));
        }
    }
}
