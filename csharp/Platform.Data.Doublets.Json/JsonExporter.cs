using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.Collections.Stacks;

namespace Platform.Data.Doublets.Json
{
    public class JsonExporter<TLink>
    {
        public readonly IJsonStorage<TLink> Storage;
        public readonly EqualityComparer<TLink> EqualityComparer = EqualityComparer<TLink>.Default;

        public JsonExporter(IJsonStorage<TLink> storage)
        {
            Storage = storage;
        }

        private bool IsElement(TLink link)
        {
            var marker = Storage.Links.GetSource(link);
            return EqualityComparer.Equals(marker, Storage.ValueMarker);
        }

        private void WriteStringValue(in Utf8JsonWriter utf8JsonWriter, TLink valueLink) => utf8JsonWriter.WriteStringValue(Storage.GetString(valueLink));

        private void WriteString(in Utf8JsonWriter utf8JsonWriter, string parent, TLink valueLink) => utf8JsonWriter.WriteString(parent, Storage.GetString(valueLink));

        private void WriteNumberValue(in Utf8JsonWriter utf8JsonWriter, TLink valueLink)
        {
            utf8JsonWriter.WriteNumberValue(Storage.GetNumber(valueLink));
        }

        private void WriteNumber(in Utf8JsonWriter utf8JsonWriter, string parent, TLink valueLink) => utf8JsonWriter.WriteNumber(parent, Storage.GetNumber(valueLink));

        private void Write(ref Utf8JsonWriter utf8JsonWriter, string parent, TLink valueLink, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            var valueMarker = Storage.GetValueMarker(valueLink);
            if (EqualityComparer.Equals(valueMarker, Storage.ObjectMarker))
            {
                utf8JsonWriter.WriteStartObject(parent);
                var membersLinks = Storage.GetMembersLinks(Storage.GetObject(valueLink));
                foreach (var memberLink in membersLinks)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                    Write(ref utf8JsonWriter, Storage.GetString(memberLink), Storage.GetValueLink(memberLink), cancellationToken);
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
                    RightSequenceWalker<TLink> rightSequenceWalker = new(Storage.Links, new DefaultStack<TLink>(), IsElement);
                    var elements = rightSequenceWalker.Walk(sequence);
                    foreach (var element in elements)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }
                        Write(ref utf8JsonWriter, element, in cancellationToken);
                    }
                }
                utf8JsonWriter.WriteEndArray();
            }
            else if (EqualityComparer.Equals(valueMarker, Storage.StringMarker))
            {
                WriteString(in utf8JsonWriter, parent, valueLink);
            }
            else if (EqualityComparer.Equals(valueMarker, Storage.NumberMarker))
            {
                WriteNumber(in utf8JsonWriter, parent, valueLink);
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

        private void Write(ref Utf8JsonWriter utf8JsonWriter, TLink valueLink, in CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            var valueMarker = Storage.GetValueMarker(valueLink);
            if (EqualityComparer.Equals(valueMarker, Storage.ObjectMarker))
            {
                utf8JsonWriter.WriteStartObject();
                var membersLinks = Storage.GetMembersLinks(Storage.GetObject(valueLink));
                foreach (var memberLink in membersLinks)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                    Write(ref utf8JsonWriter, Storage.GetString(memberLink), Storage.GetValueLink(memberLink), cancellationToken);
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
                    RightSequenceWalker<TLink> rightSequenceWalker = new(Storage.Links, new DefaultStack<TLink>(), IsElement);
                    var elements = rightSequenceWalker.Walk(sequence);
                    foreach (var element in elements)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }
                        Write(ref utf8JsonWriter, element, in cancellationToken);
                    }
                }
                utf8JsonWriter.WriteEndArray();
            }
            else if (EqualityComparer.Equals(valueMarker, Storage.StringMarker))
            {
                WriteStringValue(in utf8JsonWriter, valueLink);
            }
            else if (EqualityComparer.Equals(valueMarker, Storage.NumberMarker))
            {
                WriteNumberValue(in utf8JsonWriter, valueLink);
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

        public void Export(TLink documentLink, ref Utf8JsonWriter utf8JsonWriter, in CancellationToken cancellationToken)
        {
            var valueLink = Storage.GetValueLink(documentLink);
            Write(ref utf8JsonWriter, valueLink, in cancellationToken);
            utf8JsonWriter.Flush();
        }

        public void Export(string documentName, Utf8JsonWriter utf8JsonWriter, CancellationToken cancellationToken) => Export(Storage.GetDocumentOrDefault(documentName), ref utf8JsonWriter, in cancellationToken);
    }
}
