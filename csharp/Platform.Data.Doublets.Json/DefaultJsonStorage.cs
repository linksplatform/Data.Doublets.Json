using Platform.Numbers;
using Platform.Data.Doublets.Unicode;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Data.Doublets.CriterionMatchers;
using Platform.Data.Numbers.Raw;
using Platform.Data.Doublets.Time;
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
        private readonly TLink _any;
        private static readonly TLink _zero = default;
        private static readonly TLink _one = Arithmetic.Increment(_zero);
        private readonly BalancedVariantConverter<TLink> _balancedVariantConverter;
        private readonly ILinks<TLink> _disposableLinks;
        private readonly TLink _meaningRoot;
        private readonly TLink _unicodeSymbolMarker;
        private readonly TLink _unicodeSequenceMarker;
        private readonly RawNumberToAddressConverter<TLink> _numberToAddressConverter;
        private readonly AddressToRawNumberConverter<TLink> _addressToNumberConverter;
        private readonly LongRawNumberSequenceToDateTimeConverter<TLink> _longRawNumberToDateTimeConverter;
        private readonly IConverter<string, TLink> _stringToUnicodeSequenceConverter;
        private readonly IConverter<TLink, string> _unicodeSequenceToStringConverter;
        public readonly EqualityComparer<TLink> _defaultEqualityComparer;
        // For sequences
        public readonly JsonArrayElementCriterionMatcher<TLink> _jsonArrayElementCriterionMatcher;
        public readonly DefaultSequenceRightHeightProvider<TLink> _defaultSequenceRightHeightProvider;
        public readonly DefaultSequenceAppender<TLink> _defaultSequenceAppender;
        public ILinks<TLink> Links { get; }
        public TLink DocumentMarker { get; }
        public TLink ObjectMarker { get; }
        public TLink StringMarker { get; }
        public TLink MemberMarker { get; }
        public TLink ValueMarker { get; }
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
            _any = Links.Constants.Any;
            var markerIndex = _one;
            _meaningRoot = links.GetOrCreate(markerIndex, markerIndex);
            _unicodeSymbolMarker = links.GetOrCreate(_meaningRoot, Arithmetic.Increment(ref markerIndex));
            _unicodeSequenceMarker = links.GetOrCreate(_meaningRoot, Arithmetic.Increment(ref markerIndex));
            DocumentMarker = links.GetOrCreate(_meaningRoot, Arithmetic.Increment(ref markerIndex));
            ObjectMarker = links.GetOrCreate(_meaningRoot, Arithmetic.Increment(ref markerIndex));
            MemberMarker = links.GetOrCreate(_meaningRoot, Arithmetic.Increment(ref markerIndex));
            ValueMarker = links.GetOrCreate(_meaningRoot, Arithmetic.Increment(ref markerIndex));
            StringMarker = links.GetOrCreate(_meaningRoot, Arithmetic.Increment(ref markerIndex));
            NumberMarker = links.GetOrCreate(_meaningRoot, Arithmetic.Increment(ref markerIndex));
            ArrayMarker = links.GetOrCreate(_meaningRoot, Arithmetic.Increment(ref markerIndex));
            EmptyArrayMarker = links.GetOrCreate(_meaningRoot, Arithmetic.Increment(ref markerIndex));
            TrueMarker = links.GetOrCreate(_meaningRoot, Arithmetic.Increment(ref markerIndex));
            FalseMarker = links.GetOrCreate(_meaningRoot, Arithmetic.Increment(ref markerIndex));
            NullMarker = links.GetOrCreate(_meaningRoot, Arithmetic.Increment(ref markerIndex));
            _defaultEqualityComparer = EqualityComparer<TLink>.Default;
            // Creates converters that are able to convert link's address (UInt64 value) to a raw number represented with another UInt64 value and back
            _numberToAddressConverter = new RawNumberToAddressConverter<TLink>();
            _addressToNumberConverter = new AddressToRawNumberConverter<TLink>();
            // Creates converters that are able to convert string to unicode sequence stored as link and back
            _balancedVariantConverter = new BalancedVariantConverter<TLink>(links);
            var unicodeSymbolCriterionMatcher = new TargetMatcher<TLink>(Links, _unicodeSymbolMarker);
            var unicodeSequenceCriterionMatcher = new TargetMatcher<TLink>(Links, _unicodeSequenceMarker);
            var charToUnicodeSymbolConverter = new CharToUnicodeSymbolConverter<TLink>(Links, _addressToNumberConverter, _unicodeSymbolMarker);
            var unicodeSymbolToCharConverter = new UnicodeSymbolToCharConverter<TLink>(Links, _numberToAddressConverter, unicodeSymbolCriterionMatcher);
            var sequenceWalker = new RightSequenceWalker<TLink>(Links, new DefaultStack<TLink>(), unicodeSymbolCriterionMatcher.IsMatched);
            _stringToUnicodeSequenceConverter = new CachingConverterDecorator<string, TLink>(new StringToUnicodeSequenceConverter<TLink>(Links, charToUnicodeSymbolConverter, _balancedVariantConverter, _unicodeSequenceMarker));
            _unicodeSequenceToStringConverter = new CachingConverterDecorator<TLink, string>(new UnicodeSequenceToStringConverter<TLink>(Links, unicodeSequenceCriterionMatcher, sequenceWalker, unicodeSymbolToCharConverter));
            // For sequences
            _jsonArrayElementCriterionMatcher = new(this);
            _defaultSequenceRightHeightProvider = new(Links, _jsonArrayElementCriterionMatcher);
            _defaultSequenceAppender = new(Links, new DefaultStack<TLink>(), _defaultSequenceRightHeightProvider);
        }

        private TLink Create(TLink marker, string content)
        {
            var utf8Content = _stringToUnicodeSequenceConverter.Convert(content);
            return Links.GetOrCreate(marker, utf8Content);
        }

        private TLink GetOrDefault(TLink marker, string content)
        {
            var utf8Content = _stringToUnicodeSequenceConverter.Convert(content);
            return Links.SearchOrDefault(marker, utf8Content);
        }

        public TLink CreateString(string content) => Create(StringMarker, content);

        public TLink CreateStringValue(string content) => CreateValue(CreateString(content));

        public TLink CreateNumber(TLink number)
        {
            var numberAddress = _addressToNumberConverter.Convert(number);
            return Links.GetOrCreate(NumberMarker, numberAddress);
        }

        public TLink CreateNumberValue(TLink number) => CreateValue(CreateNumber(number));

        public TLink CreateBooleanValue(bool value) => CreateValue(value ? TrueMarker : FalseMarker);

        public TLink CreateNullValue() => CreateValue(NullMarker);

        public TLink CreateDocument(string name) => Create(DocumentMarker, name);

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
                    var convertedArray = _balancedVariantConverter.Convert(array);
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
            if (_defaultEqualityComparer.Equals(arraySequence, EmptyArrayMarker))
            {
                return CreateArrayValue(appendant);
            }
            else
            {
                newArraySequence = _defaultSequenceAppender.Append(arraySequence, appendant);
                return CreateArrayValue(newArraySequence);
            }
        }

        public TLink GetDocumentOrDefault(string name) => GetOrDefault(DocumentMarker, name);

        public string GetString(TLink stringValue)
        {
            TLink current = stringValue;
            for (int i = 0; i < 3; i++)
            {
                TLink source = Links.GetSource(current);
                if (_defaultEqualityComparer.Equals(source, StringMarker))
                {
                    return _unicodeSequenceToStringConverter.Convert(Links.GetTarget(current));
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
                if (_defaultEqualityComparer.Equals(source, NumberMarker))
                {
                    return _numberToAddressConverter.Convert(Links.GetTarget(current));
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
                if (_defaultEqualityComparer.Equals(source, ObjectMarker))
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
                if (_defaultEqualityComparer.Equals(source, ArrayMarker))
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
            var query = new Link<TLink>(index: _any, source: parent, target: _any);
            var resultLinks = Links.All(query);

            // A value must be one link
            switch (resultLinks.Count)
            {
                case 0:
                    return default;
                case 1:
                    var resultLinkTarget = Links.GetTarget(resultLinks[0]);
                    if (_defaultEqualityComparer.Equals(Links.GetSource(resultLinkTarget), ValueMarker))
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
            };
        }

        public TLink GetValueMarker(TLink value)
        {
            var target = Links.GetTarget(value);
            var targetSource = Links.GetSource(target);
            if (_defaultEqualityComparer.Equals(_meaningRoot, targetSource))
            {
                return target;
            }
            return targetSource;
        }

        public List<TLink> GetMembersLinks(TLink @object)
        {
            Link<TLink> query = new(index: _any, source: @object, target: _any);
            List<TLink> members = new();
            Links.Each((IList<TLink> objectMemberLink) =>
            {
                TLink memberLink = Links.GetTarget(objectMemberLink);
                TLink memberMarker = Links.GetSource(memberLink);
                if (_defaultEqualityComparer.Equals(memberMarker, MemberMarker)) { members.Add(Links.GetIndex(objectMemberLink)); }
                return Links.Constants.Continue;
            }, query);
            return members;
        }

        public bool IsMember(TLink link)
        {
            var memberLink = Links.GetTarget(link);
            var memberMarker = Links.GetSource(memberLink);
            return _defaultEqualityComparer.Equals(memberMarker, MemberMarker);
        }
    }
}



