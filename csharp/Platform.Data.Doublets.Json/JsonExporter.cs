using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Threading;
using System.IO;
using Platform.Converters;

namespace Platform.Data.Doublets.Json
{
    public class JsonExporter<TLink>
    {
        private readonly IJsonStorage<TLink> _storage;
        public JsonExporter(IJsonStorage<TLink> storage) => _storage = storage;

        public void WriteString(ref Utf8JsonWriter utf8JsonWriter, TLink valueLink) => utf8JsonWriter.WriteStringValue(_storage.GetString(valueLink));

        public void WriteNumber(ref Utf8JsonWriter utf8JsonWriter, TLink valueLink) => utf8JsonWriter.WriteNumberValue(UncheckedConverter<TLink, int>.Default.Convert(_storage.GetNumber(valueLink)));

        public void ChangeNameLater(ref Utf8JsonWriter utf8JsonWriter, TLink valueLink)
        {
            EqualityComparer<TLink> equalityComparer = EqualityComparer<TLink>.Default;
            var valueMarker = _storage.GetValueMarker(valueLink);
            if (equalityComparer.Equals(valueMarker, _storage.ObjectMarker))
            {
                utf8JsonWriter.WriteStartObject();
                var membersLinks = _storage.GetMembersLinks(_storage.GetObject(valueLink));
                foreach (var memberLink in membersLinks)
                {
                    ChangeNameLater(ref utf8JsonWriter, memberLink);
                }
                utf8JsonWriter.WriteEndObject();
                utf8JsonWriter.Flush();
            }
            if (equalityComparer.Equals(valueMarker, _storage.StringMarker))
            {
                WriteString(ref utf8JsonWriter, valueLink);
            }
            if (equalityComparer.Equals(valueMarker, _storage.NumberMarker))
            {
                WriteNumber(ref utf8JsonWriter, valueLink);
            }
            if (equalityComparer.Equals(valueMarker, _storage.ArrayMarker))
            {
                utf8JsonWriter.WriteStartArray();
                utf8JsonWriter.WriteEndArray();
                utf8JsonWriter.Flush();
            }
            if (equalityComparer.Equals(valueMarker, _storage.TrueMarker))
            {
                utf8JsonWriter.WriteBooleanValue(true);
                utf8JsonWriter.Flush();
            }
            if (equalityComparer.Equals(valueMarker, _storage.FalseMarker))
            {
                utf8JsonWriter.WriteBooleanValue(false);
                utf8JsonWriter.Flush();
            }
            if (equalityComparer.Equals(valueMarker, _storage.NullMarker))
            {
                utf8JsonWriter.WriteNullValue();
                utf8JsonWriter.Flush();
            }
        }

        public void Export(TLink documentLink, Utf8JsonWriter utf8JsonWriter, CancellationToken cancellationToken)
        {
            var valueLink = _storage.GetValueLink(documentLink);
            ChangeNameLater(ref utf8JsonWriter, valueLink);
        }

        public void Export(string documentName, Utf8JsonWriter utf8JsonWriter, CancellationToken cancellationToken) => Export(_storage.GetDocumentOrDefault(documentName), utf8JsonWriter, cancellationToken);
    }
}
