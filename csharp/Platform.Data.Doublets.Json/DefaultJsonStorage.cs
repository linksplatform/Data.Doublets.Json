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

namespace Platform.Data.Doublets.Json
{
    public class DefaultJsonStorage<TLink> : IJsonStorage<TLink>
    {

        private readonly TLink _any;
        private static readonly TLink _zero = default;
        private static readonly TLink _one = Arithmetic.Increment(_zero);
        private readonly BalancedVariantConverter<TLink> _balancedVariantConverter;
        private readonly ILinks<TLink> _links;
        private readonly ILinks<TLink> _disposableLinks;
        private readonly TLink _unicodeSymbolMarker;
        private readonly TLink _unicodeSequenceMarker;
        private readonly RawNumberToAddressConverter<TLink> _numberToAddressConverter;
        private readonly AddressToRawNumberConverter<TLink> _addressToNumberConverter;
        private readonly LongRawNumberSequenceToDateTimeConverter<TLink> _longRawNumberToDateTimeConverter;
        private readonly IConverter<string, TLink> _stringToUnicodeSequenceConverter;
        private readonly IConverter<TLink, string> _unicodeSequenceToStringConverter;
        public readonly TLink DocumentMarker;
        public readonly TLink ObjectMarker;
        public readonly TLink StringMarker;
        public readonly TLink MemberMarker;
        public readonly TLink ValueMarker;
        public readonly TLink NumberMarker;
        public readonly TLink ArrayMarker;
        public readonly TLink EmptyArrayMarker;
        public readonly TLink TrueMarker;
        public readonly TLink FalseMarker;
        public readonly TLink NullMarker;


        public DefaultJsonStorage(ILinks<TLink> links)
        {
            _links = links;

            // Initializes constants
            _any = _links.Constants.Any;
            var markerIndex = _one;
            var meaningRoot = links.GetOrCreate(markerIndex, markerIndex);
            _unicodeSymbolMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            _unicodeSequenceMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            DocumentMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            ObjectMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            MemberMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            ValueMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            StringMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            NumberMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            ArrayMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            EmptyArrayMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            TrueMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            FalseMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            NullMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));

            // Creates converters that are able to convert link's address (UInt64 value) to a raw number represented with another UInt64 value and back
            _numberToAddressConverter = new RawNumberToAddressConverter<TLink>();
            _addressToNumberConverter = new AddressToRawNumberConverter<TLink>();
            // Creates converters that are able to convert string to unicode sequence stored as link and back
            _balancedVariantConverter = new BalancedVariantConverter<TLink>(links);
            var unicodeSymbolCriterionMatcher = new TargetMatcher<TLink>(_links, _unicodeSymbolMarker);
            var unicodeSequenceCriterionMatcher = new TargetMatcher<TLink>(_links, _unicodeSequenceMarker);
            var charToUnicodeSymbolConverter = new CharToUnicodeSymbolConverter<TLink>(_links, _addressToNumberConverter, _unicodeSymbolMarker);
            var unicodeSymbolToCharConverter = new UnicodeSymbolToCharConverter<TLink>(_links, _numberToAddressConverter, unicodeSymbolCriterionMatcher);
            var sequenceWalker = new RightSequenceWalker<TLink>(_links, new DefaultStack<TLink>(), unicodeSymbolCriterionMatcher.IsMatched);
            _stringToUnicodeSequenceConverter = new CachingConverterDecorator<string, TLink>(new StringToUnicodeSequenceConverter<TLink>(_links, charToUnicodeSymbolConverter, _balancedVariantConverter, _unicodeSequenceMarker));
            _unicodeSequenceToStringConverter = new CachingConverterDecorator<TLink, string>(new UnicodeSequenceToStringConverter<TLink>(_links, unicodeSequenceCriterionMatcher, sequenceWalker, unicodeSymbolToCharConverter));

        }
        private TLink Create(TLink marker, string content)
        {
            var utf8Content = _stringToUnicodeSequenceConverter.Convert(content);
            return _links.GetOrCreate(marker, utf8Content);
        }

        private TLink Get(TLink marker, string content)
        {
            var utf8Content = _stringToUnicodeSequenceConverter.Convert(content);
            return _links.SearchOrDefault(marker, utf8Content);
        }

        public TLink CreateString(string content) => Create(StringMarker, content);

        public TLink CreateNumber(TLink number)
        {
            var numberAddress = _numberToAddressConverter.Convert(number);
            return _links.GetOrCreate(NumberMarker, numberAddress);
        }

        public TLink CreateDocument(string name) => Create(DocumentMarker, name);

        public TLink GetDocument(string name) => Get(DocumentMarker, name);

        public TLink CreateObject()
        {
            var objectInstanceLink = _links.Create();
            return _links.Update(objectInstanceLink, newSource: ObjectMarker, newTarget: objectInstanceLink);
        }

        public TLink CreateObjectValue() => CreateValue(CreateObject());

        public TLink CreateArray(IList<TLink> array)
        {
            switch (array.Count)
            {
                case 0:
                    return _links.GetOrCreate(ArrayMarker, EmptyArrayMarker);
                default:
                    var convertedArray = _balancedVariantConverter.Convert(array);
                    return _links.GetOrCreate(ArrayMarker, convertedArray);
            }
        }

        public TLink CreateMember(string name) => _links.GetOrCreate(MemberMarker, CreateString(name));

        public TLink CreateValue(TLink keyLink, string @string) => CreateValue(CreateString(@string));

        public TLink CreateValue(TLink keyLink, TLink @object) => _links.GetOrCreate(keyLink, CreateValue(@object));

        public TLink CreateValue(TLink @object)
        {
            return _links.GetOrCreate(ValueMarker, @object);
        }

        public TLink GetObject(TLink objectValue)
        {
            EqualityComparer<TLink> equalityComparer = EqualityComparer<TLink>.Default;
            TLink current = objectValue;
            for (int i = 0; i < 3; i++)
            {
                TLink source = _links.GetSource(current);
                if (equalityComparer.Equals(source, ObjectMarker)) return current;
                current = _links.GetTarget(current);
            }
            throw new Exception("The passed link does not contain object link.");
        }

        public TLink AttachObject(TLink parent) => Attach(parent, CreateObjectValue());

        public TLink AttachString(TLink parent, string content) => Attach(parent, CreateValue(CreateString(content)));

        public TLink AttachNumber(TLink parent, TLink number) => Attach(parent, CreateValue(CreateNumber(number)));

        public TLink AttachBoolean(TLink parent, bool value) => Attach(parent, CreateValue(value ? TrueMarker : FalseMarker));

        public TLink AttachNull(TLink parent) => Attach(parent, CreateValue(NullMarker));
        public TLink AttachArray(TLink parent, TLink arrayLink) => Attach(parent, CreateValue(arrayLink));
        public TLink AttachArray(TLink parent, IList<TLink> array)
        {
            var arrayLink = CreateArray(array);
            return Attach(parent, CreateValue(arrayLink));
        }

        public TLink AttachMemberToObject(TLink @object, string keyName) => Attach(@object, CreateMember(keyName));

        public TLink Attach(TLink parent, TLink child) => _links.GetOrCreate(parent, child);

        public TLink GetValue(TLink parent)
        {
            var query = new Link<TLink>(index: _any, source: parent, target: _any);
            var resultLinks = _links.All(query);

            // A value must be one link
            switch (resultLinks.Count)
            {
                case 0:
                    return default;
                case 1:
                    var equalityComparer = EqualityComparer<TLink>.Default;
                    var resultLinkTarget = _links.GetTarget(resultLinks[0]);
                    if (equalityComparer.Equals(_links.GetSource(resultLinkTarget), ValueMarker))
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

        public List<TLink> GetMembersLinks(TLink @object)
        {
            EqualityComparer<TLink> equalityComparer = EqualityComparer<TLink>.Default;
            Link<TLink> query = new Link<TLink>(index: _any, source: @object, target: _any);
            var test = _links.Count(query);
            var test1 = _links.Exists(@object);
            List<TLink> members = new List<TLink>();
            _links.Each((IList<TLink> objectMemberLink) =>
            {
                TLink memberLink = _links.GetTarget(objectMemberLink);
                TLink memberMarker = _links.GetSource(memberLink);
                if (equalityComparer.Equals(memberMarker, MemberMarker)) { members.Add(_links.GetIndex(objectMemberLink)); }
                return _links.Constants.Continue;
            }, query);
            return members;
        }
    }
}



