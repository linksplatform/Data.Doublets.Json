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
            var valueLink = _storage.GetValueLink(documentLink);
            var valueMarker = _storage.GetValueMarker(valueLink);
            if (equalityComparer.Equals(valueMarker, _storage.ObjectMarker))
            {
                utf8JsonWriter.WriteStartObject();
                utf8JsonWriter.WriteEndObject();
                utf8JsonWriter.Flush();
            }
            if (equalityComparer.Equals(valueMarker, _storage.StringMarker))
            {
                utf8JsonWriter.WriteStringValue(_storage.GetString(valueLink));
                utf8JsonWriter.Flush();
            }
        }

        public void Export(string documentName, Utf8JsonWriter utf8JsonWriter, CancellationToken cancellationToken) => Export(_storage.GetDocumentOrDefault(documentName), utf8JsonWriter, cancellationToken);
    }
}
