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
        public readonly TLink KeyMarker;
        public readonly TLink ValueMarker;

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
            KeyMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            ValueMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            StringMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));

            // Creates converters that are able to convert link's address (UInt64 value) to a raw number represented with another UInt64 value and back
            _numberToAddressConverter = new RawNumberToAddressConverter<TLink>();
            _addressToNumberConverter = new AddressToRawNumberConverter<TLink>();
            // Creates converters that are able to convert string to unicode sequence stored as link and back
            var balancedVariantConverter = new BalancedVariantConverter<TLink>(links);
            var unicodeSymbolCriterionMatcher = new TargetMatcher<TLink>(_links, _unicodeSymbolMarker);
            var unicodeSequenceCriterionMatcher = new TargetMatcher<TLink>(_links, _unicodeSequenceMarker);
            var charToUnicodeSymbolConverter = new CharToUnicodeSymbolConverter<TLink>(_links, _addressToNumberConverter, _unicodeSymbolMarker);
            var unicodeSymbolToCharConverter = new UnicodeSymbolToCharConverter<TLink>(_links, _numberToAddressConverter, unicodeSymbolCriterionMatcher);
            var sequenceWalker = new RightSequenceWalker<TLink>(_links, new DefaultStack<TLink>(), unicodeSymbolCriterionMatcher.IsMatched);
            _stringToUnicodeSequenceConverter = new CachingConverterDecorator<string, TLink>(new StringToUnicodeSequenceConverter<TLink>(_links, charToUnicodeSymbolConverter, balancedVariantConverter, _unicodeSequenceMarker));
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

        public TLink CreateDocument(string name) => Create(DocumentMarker, name);
        public TLink GetDocument(string name) => Get(DocumentMarker, name);
        public TLink CreateObject()
        {
            var objectInstanceLink = _links.Create();
            return _links.Update(objectInstanceLink, newSource: ObjectMarker, newTarget: objectInstanceLink);
        }
        public TLink CreateObjectValue() => CreateValue(CreateObject());
        public TLink CreateString(string content) => Create(StringMarker, content);
        public TLink CreateKey(TLink objectLink, string @string) => CreateKey(objectLink, CreateString(@string));
        public TLink CreateKey(TLink @object)
        {
            return _links.GetOrCreate(KeyMarker, @object);
        }
        public TLink CreateKey(TLink objectLink, TLink @object)
        {
            return _links.GetOrCreate(objectLink, CreateKey(@object));
        }

        public TLink CreateValue(TLink keyLink, string @string) => CreateValue(keyLink, CreateString(@string));
        public TLink CreateValue(TLink @object)
        {
            return _links.GetOrCreate(ValueMarker, @object);
        }
        public TLink CreateValue(TLink keyLink, TLink @object)
        {
            return _links.GetOrCreate(keyLink, CreateValue(@object));
        }

        public TLink AttachObject(TLink parent) => Attach(parent, CreateObjectValue());
        public TLink Attach(TLink parent, TLink child) => _links.GetOrCreate(parent, child);
        public TLink GetValue(TLink parent)
        {
            var query = new Link<TLink>(index: _any, source: parent, target: _any);
            var resultLink = _links.All(query);

            // A value must be one link
            switch (resultLink.Count)
            {
                case 0:
                    return default;
                case 1:
                    var equalityComparer = EqualityComparer<TLink>.Default;
                    var resultLinkTarget = _links.GetTarget(resultLink[0]);
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
    }
}



