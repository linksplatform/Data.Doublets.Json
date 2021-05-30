using Platform.Numbers;
using Platform.Data.Doublets.Unicode;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Data.Doublets.CriterionMatchers;
using Platform.Data.Numbers.Raw;
using Platform.Data.Doublets.Time;
using Platform.Converters;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.Collections.Stacks;

namespace Platform.Data.Doublets.Json
{
    public class DefaultJsonStorage<TLink> : IJsonStorage<TLink>
    {
        private static readonly TLink _zero = default;
        private static readonly TLink _one = Arithmetic.Increment(_zero);
        private readonly BalancedVariantConverter<TLink> _balancedVariantConverter;
        private readonly ILinks<TLink> _links;
        private TLink _unicodeSymbolMarker;
        private TLink _unicodeSequenceMarker;
        private readonly RawNumberToAddressConverter<TLink> _numberToAddressConverter;
        private readonly AddressToRawNumberConverter<TLink> _addressToNumberConverter;
        private readonly LongRawNumberSequenceToDateTimeConverter<TLink> _longRawNumberToDateTimeConverter;
        private readonly IConverter<string, TLink> _stringToUnicodeSequenceConverter;
        private readonly IConverter<TLink, string> _unicodeSequenceToStringConverter;
        private TLink _documentMarker;
        private TLink _objectMarker;
        private TLink _stringMarker;
        private TLink _keyMarker;
        private TLink _valueMarker;

        public DefaultJsonStorage(ILinks<TLink> links)
        {
            InitConstants(links);
            _links = links;

            // Create converters that are able to convert link's address (UInt64 value) to a raw number represented with another UInt64 value and back
            _numberToAddressConverter = new RawNumberToAddressConverter<TLink>();
            _addressToNumberConverter = new AddressToRawNumberConverter<TLink>();
            // Create converters that are able to convert string to unicode sequence stored as link and back
            var balancedVariantConverter = new BalancedVariantConverter<TLink>(links);
            var unicodeSymbolCriterionMatcher = new TargetMatcher<TLink>(_links, _unicodeSymbolMarker);
            var unicodeSequenceCriterionMatcher = new TargetMatcher<TLink>(_links, _unicodeSequenceMarker);
            var charToUnicodeSymbolConverter = new CharToUnicodeSymbolConverter<TLink>(_links, _addressToNumberConverter, _unicodeSymbolMarker);
            var unicodeSymbolToCharConverter = new UnicodeSymbolToCharConverter<TLink>(_links, _numberToAddressConverter, unicodeSymbolCriterionMatcher);
            var sequenceWalker = new RightSequenceWalker<TLink>(_links, new DefaultStack<TLink>(), unicodeSymbolCriterionMatcher.IsMatched);
            _stringToUnicodeSequenceConverter = new CachingConverterDecorator<string, TLink>(new StringToUnicodeSequenceConverter<TLink>(_links, charToUnicodeSymbolConverter, balancedVariantConverter, _unicodeSequenceMarker));
            _unicodeSequenceToStringConverter = new CachingConverterDecorator<TLink, string>(new UnicodeSequenceToStringConverter<TLink>(_links, unicodeSequenceCriterionMatcher, sequenceWalker, unicodeSymbolToCharConverter));

        }
        private void InitConstants(ILinks<TLink> links)
        {
            var markerIndex = _one;
            var meaningRoot = links.GetOrCreate(markerIndex, markerIndex);
            _unicodeSymbolMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            _unicodeSequenceMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            _documentMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            _objectMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            _keyMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            _valueMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            _stringMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
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

        public TLink CreateDocument(string name) => Create(_documentMarker, name);
        public TLink GetDocument(string name) => Get(_documentMarker, name);
        public TLink CreateObject(string name) => Create(_objectMarker, name);
        public TLink GetObject(string name) => Get(_objectMarker, name);
        public TLink CreateString(string content) => Create(_stringMarker, content);
        public TLink CreateKey(TLink objectLink, string @string) => CreateKey(objectLink, CreateString(@string));
        public TLink CreateKey(TLink @object)
        {
            return _links.GetOrCreate(_keyMarker, @object);
        }
        public TLink CreateKey(TLink objectLink, TLink @object)
        {
            return _links.GetOrCreate(objectLink, CreateKey(@object));
        }

        public TLink CreateValue(TLink keyLink, string @string) => CreateValue(keyLink, CreateString(@string));
        public TLink CreateValue(TLink @object)
        {
            return _links.GetOrCreate(_valueMarker, @object);
        }
        public TLink CreateValue(TLink keyLink, TLink @object)
        {
            return _links.GetOrCreate(keyLink, CreateValue(@object));
        }
        public TLink AttachObject(TLink parent) => AttachElementToParent(_objectMarker, parent);
        public TLink AttachElementToParent(TLink elementToAttach, TLink parent) => _links.GetOrCreate(parent, elementToAttach);


    }
}



