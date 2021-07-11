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

        public void WriteStringValue(ref Utf8JsonWriter utf8JsonWriter, TLink valueLink) => utf8JsonWriter.WriteStringValue(_storage.GetString(valueLink));

        public void WriteString(ref Utf8JsonWriter utf8JsonWriter, string parent, TLink valueLink) => utf8JsonWriter.WriteString(parent, _storage.GetString(valueLink));

        public void WriteNumberValue(ref Utf8JsonWriter utf8JsonWriter, TLink valueLink)
        {
            var uncheckedConverter = UncheckedConverter<TLink, int>.Default;
            utf8JsonWriter.WriteNumberValue(uncheckedConverter.Convert(_storage.GetNumber(valueLink)));
        }

        public void WriteNumber(ref Utf8JsonWriter utf8JsonWriter, string parent, TLink valueLink)
        {
            var uncheckedConverter = UncheckedConverter<TLink, int>.Default;
            utf8JsonWriter.WriteNumber(parent, uncheckedConverter.Convert(_storage.GetNumber(valueLink)));
        }

        public void Write(ref Utf8JsonWriter utf8JsonWriter, string parent, TLink valueLink)
        {
            EqualityComparer<TLink> equalityComparer = EqualityComparer<TLink>.Default;
            var valueMarker = _storage.GetValueMarker(valueLink);
            if (equalityComparer.Equals(valueMarker, _storage.ObjectMarker))
            {
                utf8JsonWriter.WriteStartObject(parent);
                var membersLinks = _storage.GetMembersLinks(_storage.GetObject(valueLink));
                foreach (var memberLink in membersLinks)
                {
                    Write(ref utf8JsonWriter, _storage.GetString(memberLink), _storage.GetValueLink(memberLink));
                }
                utf8JsonWriter.WriteEndObject();
            }
            if (equalityComparer.Equals(valueMarker, _storage.StringMarker))
            {
                WriteString(ref utf8JsonWriter, parent, valueLink);
            }
            if (equalityComparer.Equals(valueMarker, _storage.NumberMarker))
            {
                WriteNumber(ref utf8JsonWriter, parent, valueLink);
            }
            if (equalityComparer.Equals(valueMarker, _storage.ArrayMarker))
            {
                utf8JsonWriter.WriteStartArray();
                Write(ref utf8JsonWriter, valueLink);
                utf8JsonWriter.WriteEndArray();
            }
            if (equalityComparer.Equals(valueMarker, _storage.TrueMarker))
            {
                utf8JsonWriter.WriteBoolean(parent, true);
            }
            if (equalityComparer.Equals(valueMarker, _storage.FalseMarker))
            {
                utf8JsonWriter.WriteBoolean(parent, false);
            }
            if (equalityComparer.Equals(valueMarker, _storage.NullMarker))
            {
                utf8JsonWriter.WriteNull(parent);
            }
            utf8JsonWriter.Flush();
        }

        public void Write(ref Utf8JsonWriter utf8JsonWriter, TLink valueLink)
        {
            EqualityComparer<TLink> equalityComparer = EqualityComparer<TLink>.Default;
            var valueMarker = _storage.GetValueMarker(valueLink);
            if (equalityComparer.Equals(valueMarker, _storage.ObjectMarker))
            {
                utf8JsonWriter.WriteStartObject();
                var membersLinks = _storage.GetMembersLinks(_storage.GetObject(valueLink));
                foreach (var memberLink in membersLinks)
                {
                    Write(ref utf8JsonWriter, _storage.GetString(memberLink), _storage.GetValueLink(memberLink));
                }
                utf8JsonWriter.WriteEndObject();
            }
            else if (equalityComparer.Equals(valueMarker, _storage.StringMarker))
            {
                WriteStringValue(ref utf8JsonWriter, valueLink);
            }
            else if (equalityComparer.Equals(valueMarker, _storage.NumberMarker))
            {
                WriteNumberValue(ref utf8JsonWriter, valueLink);
            }
            else if (equalityComparer.Equals(valueMarker, _storage.ArrayMarker))
            {
                utf8JsonWriter.WriteStartArray();
                Write(ref utf8JsonWriter, valueLink);
                utf8JsonWriter.WriteEndArray();
            }
            else if (equalityComparer.Equals(valueMarker, _storage.TrueMarker))
            {
                utf8JsonWriter.WriteBooleanValue(true);
            }
            else if (equalityComparer.Equals(valueMarker, _storage.FalseMarker))
            {
                utf8JsonWriter.WriteBooleanValue(false);
            }
            else if (equalityComparer.Equals(valueMarker, _storage.NullMarker))
            {
                utf8JsonWriter.WriteNullValue();
            }
            utf8JsonWriter.Flush();
        }

        public void Export(TLink documentLink, ref Utf8JsonWriter utf8JsonWriter, CancellationToken cancellationToken)
        {
            var valueLink = _storage.GetValueLink(documentLink);
            Write(ref utf8JsonWriter, valueLink);
        }

        public void Export(string documentName, Utf8JsonWriter utf8JsonWriter, CancellationToken cancellationToken) => Export(_storage.GetDocumentOrDefault(documentName), ref utf8JsonWriter, cancellationToken);
    }
}
