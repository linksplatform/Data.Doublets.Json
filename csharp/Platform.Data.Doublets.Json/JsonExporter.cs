using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using Platform.Converters;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.Collections.Stacks;

namespace Platform.Data.Doublets.Json
{
    public class JsonExporter<TLink>
    {
        public readonly IJsonStorage<TLink> Storage;
        public readonly EqualityComparer<TLink> EqualityComparer;

        public JsonExporter(IJsonStorage<TLink> storage)
        {
            Storage = storage;
            EqualityComparer = EqualityComparer<TLink>.Default;
        }

        private bool IsElement(TLink link)
        {
            var marker = Storage.Links.GetSource(link);
            return EqualityComparer.Equals(marker, Storage.ValueMarker);
        }
        
        public void WriteStringValue(ref Utf8JsonWriter utf8JsonWriter, TLink valueLink) => utf8JsonWriter.WriteStringValue(Storage.GetString(valueLink));

        public void WriteString(ref Utf8JsonWriter utf8JsonWriter, string parent, TLink valueLink) => utf8JsonWriter.WriteString(parent, Storage.GetString(valueLink));

        public void WriteNumberValue(ref Utf8JsonWriter utf8JsonWriter, TLink valueLink)
        {
            var uncheckedConverter = UncheckedConverter<TLink, int>.Default;
            utf8JsonWriter.WriteNumberValue(uncheckedConverter.Convert(Storage.GetNumber(valueLink)));
        }

        public void WriteNumber(ref Utf8JsonWriter utf8JsonWriter, string parent, TLink valueLink)
        {
            var uncheckedConverter = UncheckedConverter<TLink, int>.Default;
            utf8JsonWriter.WriteNumber(parent, uncheckedConverter.Convert(Storage.GetNumber(valueLink)));
        }

        public void Write(ref Utf8JsonWriter utf8JsonWriter, string parent, TLink valueLink)
        {
            var valueMarker = Storage.GetValueMarker(valueLink);
            if (EqualityComparer.Equals(valueMarker, Storage.ObjectMarker))
            {
                utf8JsonWriter.WriteStartObject(parent);
                var membersLinks = Storage.GetMembersLinks(Storage.GetObject(valueLink));
                foreach (var memberLink in membersLinks)
                {
                    Write(ref utf8JsonWriter, Storage.GetString(memberLink), Storage.GetValueLink(memberLink));
                }
                utf8JsonWriter.WriteEndObject();
            }
            else if (EqualityComparer.Equals(valueMarker, Storage.ArrayMarker))
            {
                var array = Storage.GetArray(valueLink);
                var sequence = Storage.GetArraySequence(array);
                utf8JsonWriter.WriteStartArray(parent);
                if (!EqualityComparer.Equals(sequence, Storage.EmptyArrayMarker))
                {
                    DefaultStack<TLink> stack = new();
                    RightSequenceWalker<TLink> rightSequenceWalker = new(Storage.Links, stack, IsElement);
                    var elements = rightSequenceWalker.Walk(sequence);
                    foreach (var element in elements)
                    {
                        Write(ref utf8JsonWriter, element);
                    }
                }
                utf8JsonWriter.WriteEndArray();
            }
            else if (EqualityComparer.Equals(valueMarker, Storage.StringMarker))
            {
                WriteString(ref utf8JsonWriter, parent, valueLink);
            }
            else if (EqualityComparer.Equals(valueMarker, Storage.NumberMarker))
            {
                WriteNumber(ref utf8JsonWriter, parent, valueLink);
            }
            else if (EqualityComparer.Equals(valueMarker, Storage.TrueMarker))
            {
                utf8JsonWriter.WriteBoolean(parent, true);
            }
            else if (EqualityComparer.Equals(valueMarker, Storage.FalseMarker))
            {
                utf8JsonWriter.WriteBoolean(parent, false);
            }
            else if (EqualityComparer.Equals(valueMarker, Storage.NullMarker))
            {
                utf8JsonWriter.WriteNull(parent);
            }
        }

        public void Write(ref Utf8JsonWriter utf8JsonWriter, TLink valueLink)
        {
            var valueMarker = Storage.GetValueMarker(valueLink);
            if (EqualityComparer.Equals(valueMarker, Storage.ObjectMarker))
            {
                utf8JsonWriter.WriteStartObject();
                var membersLinks = Storage.GetMembersLinks(Storage.GetObject(valueLink));
                foreach (var memberLink in membersLinks)
                {
                    Write(ref utf8JsonWriter, Storage.GetString(memberLink), Storage.GetValueLink(memberLink));
                }
                utf8JsonWriter.WriteEndObject();
            }
            else if (EqualityComparer.Equals(valueMarker, Storage.ArrayMarker))
            {
                var array = Storage.GetArray(valueLink);
                var sequence = Storage.GetArraySequence(array);
                utf8JsonWriter.WriteStartArray();
                if (!EqualityComparer.Equals(sequence, Storage.EmptyArrayMarker))
                {
                    DefaultStack<TLink> stack = new();
                    RightSequenceWalker<TLink> rightSequenceWalker = new(Storage.Links, stack, IsElement);
                    var elements = rightSequenceWalker.Walk(sequence);
                    foreach (var element in elements)
                    {
                        Write(ref utf8JsonWriter, element);
                    }
                }
                utf8JsonWriter.WriteEndArray();
            }
            else if (EqualityComparer.Equals(valueMarker, Storage.StringMarker))
            {
                WriteStringValue(ref utf8JsonWriter, valueLink);
            }
            else if (EqualityComparer.Equals(valueMarker, Storage.NumberMarker))
            {
                WriteNumberValue(ref utf8JsonWriter, valueLink);
            }
            else if (EqualityComparer.Equals(valueMarker, Storage.TrueMarker))
            {
                utf8JsonWriter.WriteBooleanValue(true);
            }
            else if (EqualityComparer.Equals(valueMarker, Storage.FalseMarker))
            {
                utf8JsonWriter.WriteBooleanValue(false);
            }
            else if (EqualityComparer.Equals(valueMarker, Storage.NullMarker))
            {
                utf8JsonWriter.WriteNullValue();
            }
        }

        public void Export(TLink documentLink, ref Utf8JsonWriter utf8JsonWriter, CancellationToken cancellationToken)
        {
            var valueLink = Storage.GetValueLink(documentLink);
            Write(ref utf8JsonWriter, valueLink);
            utf8JsonWriter.Flush();
        }

        public void Export(string documentName, Utf8JsonWriter utf8JsonWriter, CancellationToken cancellationToken) => Export(Storage.GetDocumentOrDefault(documentName), ref utf8JsonWriter, cancellationToken);
    }
}
