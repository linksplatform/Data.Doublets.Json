using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Threading;
using System.IO;

namespace Platform.Data.Doublets.Json
{
    public class JsonExporter<TLink>
    {
        private readonly IJsonStorage<TLink> _storage;
        public JsonExporter(IJsonStorage<TLink> storage) => _storage = storage;
        public void Export(TLink documentLink, Utf8JsonWriter utf8JsonWriter, CancellationToken cancellationToken)
        {
            EqualityComparer<TLink> equalityComparer = EqualityComparer<TLink>.Default;
            if (equalityComparer.Equals(_storage.GetValueMarker(_storage.GetValue(documentLink)), _storage.ObjectMarker))
            {
                utf8JsonWriter.WriteStartObject();
                utf8JsonWriter.WriteEndObject();
                utf8JsonWriter.Flush();
            }
        }

        public void Export(string documentName, Utf8JsonWriter utf8JsonWriter, CancellationToken cancellationToken)
        {
            var documentLink = _storage.GetDocument(documentName);
            EqualityComparer<TLink> equalityComparer = EqualityComparer<TLink>.Default;
            if (equalityComparer.Equals(_storage.GetValueMarker(_storage.GetValue(documentLink)), _storage.ObjectMarker))
            {
                utf8JsonWriter.WriteStartObject();
                utf8JsonWriter.WriteEndObject();
                utf8JsonWriter.Flush();
            }
        }
    }
}
