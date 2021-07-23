using Platform.Numbers;
using Platform.Data.Doublets.Unicode;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Data.Doublets.CriterionMatchers;
using Platform.Data.Numbers.Raw;
using Platform.Converters;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.Collections.Stacks;
using System;
using System.Collections.Generic;
using Platform.Data.Doublets.Sequences.HeightProviders;
using Platform.Data.Doublets.Sequences;

namespace Platform.Data.Doublets.Json
{
    public class DefaultJsonStorage<TLink> : IJsonStorage<TLink>
    {
        public readonly TLink Any;
        public static readonly TLink Zero = default;
        public static readonly TLink One = Arithmetic.Increment(Zero);
        public readonly BalancedVariantConverter<TLink> BalancedVariantConverter;
        public readonly TLink MeaningRoot;
        public readonly RawNumberToAddressConverter<TLink> NumberToAddressConverter = new();
        public readonly AddressToRawNumberConverter<TLink> AddressToNumberConverter = new();
        public readonly IConverter<string, TLink> StringToUnicodeSequenceConverter;
        public readonly IConverter<TLink, string> UnicodeSequenceToStringConverter;

        public readonly EqualityComparer<TLink> EqualityComparer = EqualityComparer<TLink>.Default;

        // For sequences
        public readonly JsonArrayElementCriterionMatcher<TLink> JsonArrayElementCriterionMatcher;
        public readonly DefaultSequenceRightHeightProvider<TLink> DefaultSequenceRightHeightProvider;
        public readonly DefaultSequenceAppender<TLink> DefaultSequenceAppender;
        public ILinks<TLink> Links { get; }
        public TLink DocumentMarker { get; }
        public TLink ObjectMarker { get; }
        public TLink MemberMarker { get; }
        public TLink ValueMarker { get; }
        public TLink StringMarker { get; }
        public TLink EmptyStringMarker { get; }
        public TLink NumberMarker { get; }
        public TLink ArrayMarker { get; }
        public TLink EmptyArrayMarker { get; }
        public TLink TrueMarker { get; }
        public TLink FalseMarker { get; }
        public TLink NullMarker { get; }

        public DefaultJsonStorage(ILinks<TLink> links)
        {
            Links = links;
            // Initializes constants
            Any = Links.Constants.Any;
            var markerIndex = One;
            MeaningRoot = links.GetOrCreate(markerIndex, markerIndex);
            var unicodeSymbolMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            var unicodeSequenceMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            DocumentMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            ObjectMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            MemberMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            ValueMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            StringMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            EmptyStringMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            NumberMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            ArrayMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            EmptyArrayMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            TrueMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            FalseMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            NullMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            // Creates converters that are able to convert link's address (UInt64 value) to a raw number represented with another UInt64 value and back
            // Creates converters that are able to convert string to unicode sequence stored as link and back
            BalancedVariantConverter = new(links);
            TargetMatcher<TLink> unicodeSymbolCriterionMatcher = new(Links, unicodeSymbolMarker);
            TargetMatcher<TLink> unicodeSequenceCriterionMatcher = new(Links, unicodeSequenceMarker);
            CharToUnicodeSymbolConverter<TLink> charToUnicodeSymbolConverter =
                new(Links, AddressToNumberConverter, unicodeSymbolMarker);
            UnicodeSymbolToCharConverter<TLink> unicodeSymbolToCharConverter =
                new(Links, NumberToAddressConverter, unicodeSymbolCriterionMatcher);
            RightSequenceWalker<TLink> sequenceWalker =
                new(Links, new DefaultStack<TLink>(), unicodeSymbolCriterionMatcher.IsMatched);
            StringToUnicodeSequenceConverter = new CachingConverterDecorator<string, TLink>(
                new StringToUnicodeSequenceConverter<TLink>(Links, charToUnicodeSymbolConverter,
                    BalancedVariantConverter, unicodeSequenceMarker));
            UnicodeSequenceToStringConverter = new CachingConverterDecorator<TLink, string>(
                new UnicodeSequenceToStringConverter<TLink>(Links, unicodeSequenceCriterionMatcher, sequenceWalker,
                    unicodeSymbolToCharConverter));
            // For sequences
            JsonArrayElementCriterionMatcher = new(this);
            DefaultSequenceRightHeightProvider = new(Links, JsonArrayElementCriterionMatcher);
            DefaultSequenceAppender = new(Links, new DefaultStack<TLink>(), DefaultSequenceRightHeightProvider);
        }

        private TLink GetStringSequence(string content) => content == "" ? EmptyStringMarker : StringToUnicodeSequenceConverter.Convert(content);
        public TLink CreateString(string content)
        {
            TLink @string = GetStringSequence(content);
            return Links.GetOrCreate(StringMarker, @string);
        }

        public TLink CreateStringValue(string content) => CreateValue(CreateString(content));

        public TLink CreateNumber(TLink number)
        {
            var numberAddress = AddressToNumberConverter.Convert(number);
            return Links.GetOrCreate(NumberMarker, numberAddress);
        }

        public TLink CreateNumberValue(TLink number) => CreateValue(CreateNumber(number));

        public TLink CreateBooleanValue(bool value) => CreateValue(value ? TrueMarker : FalseMarker);

        public TLink CreateNullValue() => CreateValue(NullMarker);

        public TLink CreateDocument(string name) => Links.GetOrCreate(DocumentMarker, CreateString(name));

            public TLink CreateObject()
        {
            var objectInstance = Links.Create();
            return Links.Update(objectInstance, newSource: ObjectMarker, newTarget: objectInstance);
        }

        public TLink CreateObjectValue() => CreateValue(CreateObject());

        public TLink CreateArray(IList<TLink> array)
        {
            switch (array.Count)
            {
                case 0:
                    return CreateArray(EmptyArrayMarker);
                default:
                    var convertedArray = BalancedVariantConverter.Convert(array);
                    return CreateArray(convertedArray);
            }
        }

        public TLink CreateArray(TLink sequence) => Links.GetOrCreate(ArrayMarker, sequence);

        public TLink CreateArrayValue(IList<TLink> array) => CreateValue(CreateArray(array));

        public TLink CreateArrayValue(TLink sequence) => CreateValue(CreateArray(sequence));

        public TLink CreateMember(string name) => Links.GetOrCreate(MemberMarker, CreateString(name));

        public TLink CreateValue(TLink key, TLink @object) => Links.GetOrCreate(key, CreateValue(@object));

        public TLink CreateValue(TLink @object) => Links.GetOrCreate(ValueMarker, @object);

        public TLink AttachObject(TLink parent) => Attach(parent, CreateObjectValue());

        public TLink AttachObjectValue(TLink parent, TLink objectValue) => Attach(parent, objectValue);

        public TLink AttachString(TLink parent, string content) => Attach(parent, CreateValue(CreateString(content)));

        public TLink AttachStringValue(TLink parent, TLink stringValue) => Attach(parent, stringValue);

        public TLink AttachNumber(TLink parent, TLink number) => Attach(parent, CreateValue(CreateNumber(number)));

        public TLink AttachNumberValue(TLink parent, TLink numberValue) => Attach(parent, numberValue);

        public TLink AttachBoolean(TLink parent, bool value) => Attach(parent, CreateBooleanValue(value));
        public TLink AttachBooleanValue(TLink parent, TLink booleanValue) => Attach(parent, booleanValue);

        public TLink AttachNull(TLink parent) => Attach(parent, CreateNullValue());

        public TLink AttachNullValue(TLink parent, TLink nullValue) => Attach(parent, nullValue);

        public TLink AttachArray(TLink parent, TLink array) => Attach(parent, CreateValue(array));

        public TLink AttachArray(TLink parent, IList<TLink> array) => Attach(parent, CreateArrayValue(array));

        public TLink AttachArrayValue(TLink parent, TLink arrayValue) => Attach(parent, arrayValue);

        public TLink AttachMemberToObject(TLink @object, string keyName) => Attach(@object, CreateMember(keyName));

        public TLink Attach(TLink parent, TLink child) => Links.GetOrCreate(parent, child);

        public TLink AppendArrayValue(TLink arrayValue, TLink appendant)
        {
            var array = GetArray(arrayValue);
            var arraySequence = Links.GetTarget(array);
            TLink newArraySequence;
            if (EqualityComparer.Equals(arraySequence, EmptyArrayMarker))
            {
                return CreateArrayValue(appendant);
            }
            else
            {
                newArraySequence = DefaultSequenceAppender.Append(arraySequence, appendant);
                return CreateArrayValue(newArraySequence);
            }
        }

        public TLink GetDocumentOrDefault(string name)
        {
            var stringSequence = GetStringSequence(name);
            var @string = Links.SearchOrDefault(StringMarker, stringSequence);
            return Links.SearchOrDefault(DocumentMarker, @string);
        }

        public string GetString(TLink stringValue)
        {
            TLink current = stringValue;
            for (int i = 0; i < 3; i++)
            {
                TLink source = Links.GetSource(current);
                if (EqualityComparer.Equals(source, StringMarker))
                {
                    var target = Links.GetTarget(current);
                    var isEmptyString = EqualityComparer.Equals(target, EmptyArrayMarker);
                    return isEmptyString ? "" : UnicodeSequenceToStringConverter.Convert(target);
                }
                current = Links.GetTarget(current);
            }
            throw new Exception("The passed link does not contain string link.");
        }

        public TLink GetNumber(TLink valueLink)
        {
            TLink current = valueLink;
            for (int i = 0; i < 3; i++)
            {
                TLink source = Links.GetSource(current);
                if (EqualityComparer.Equals(source, NumberMarker))
                {
                    return NumberToAddressConverter.Convert(Links.GetTarget(current));
                }

                current = Links.GetTarget(current);
            }
            throw new Exception("The passed link does not contain number link.");
        }


        public TLink GetObject(TLink objectValueLink)
        {
            TLink current = objectValueLink;
            for (int i = 0; i < 3; i++)
            {
                TLink source = Links.GetSource(current);
                if (EqualityComparer.Equals(source, ObjectMarker))
                {
                    return current;
                }
                current = Links.GetTarget(current);
            }
            throw new Exception("The passed link does not contain object link.");
        }

        public TLink GetArray(TLink arrayValueLink)
        {
            TLink current = arrayValueLink;
            for (int i = 0; i < 3; i++)
            {
                TLink source = Links.GetSource(current);
                if (EqualityComparer.Equals(source, ArrayMarker))
                {
                    return current;
                }
                current = Links.GetTarget(current);
            }
            throw new Exception("The passed link does not contain array link.");
        }

        public TLink GetArraySequence(TLink array) => Links.GetTarget(array);

        public TLink GetValueLink(TLink parent)
        {
            var query = new Link<TLink>(index: Any, source: parent, target: Any);
            var resultLinks = Links.All(query);

            // A value must be one link
            switch (resultLinks.Count)
            {
                case 0:
                    return default;
                case 1:
                    var resultLinkTarget = Links.GetTarget(resultLinks[0]);
                    if (EqualityComparer.Equals(Links.GetSource(resultLinkTarget), ValueMarker))
                    {
                        return resultLinkTarget;
                    }
                    else
                    {
                        throw new InvalidOperationException("Is not a value link.");
                    }
                case > 1:
                    throw new InvalidOperationException("More than 1 value found.");
                default:
                    throw new InvalidOperationException("The list elements length is negative.");
            }
        }

        public TLink GetValueMarker(TLink value)
        {
            var target = Links.GetTarget(value);
            var targetSource = Links.GetSource(target);
            if (EqualityComparer.Equals(MeaningRoot, targetSource))
            {
                return target;
            }
            return targetSource;
        }

        public List<TLink> GetMembersLinks(TLink @object)
        {
            Link<TLink> query = new(index: Any, source: @object, target: Any);
            List<TLink> members = new();
            Links.Each(objectMemberLink =>
            {
                TLink memberLink = Links.GetTarget(objectMemberLink);
                TLink memberMarker = Links.GetSource(memberLink);
                if (EqualityComparer.Equals(memberMarker, MemberMarker)) { members.Add(Links.GetIndex(objectMemberLink)); }
                return Links.Constants.Continue;
            }, query);
            return members;
        }
    }
}



