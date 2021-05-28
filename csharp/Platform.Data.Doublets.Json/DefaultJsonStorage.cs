using Platform.Numbers;
using Platform.Data.Doublets.Unicode;
using TLink = System.UInt32;
namespace Platform.Data.Doublets.Json
{
    public class DefaultJsonStorage : IJsonStorage<TLink>
    {
        private static readonly TLink _zero = default;
        private static readonly TLink _one = Arithmetic.Increment(_zero);

        private readonly StringToUnicodeSequenceConverter<TLink> _stringToUnicodeSequenceConverter;
        private readonly ILinks<TLink> _links;
        private TLink _unicodeSymbolMarker;
        private TLink _unicodeSequenceMarker;
        private TLink _documentMarker;
        private TLink _objectMarker;
        private TLink _stringMarker;
        private TLink _keyMarker;
        private TLink _valueMarker;

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

        public TLink CreateDocument(string name) => Create(_documentMarker, name);
        public TLink CreateObject(string name) => Create(_objectMarker, name);
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


    }
}

}


