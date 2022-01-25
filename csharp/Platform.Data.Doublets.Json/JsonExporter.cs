using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.Collections.Stacks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Json
{
    /// <summary>
    /// <para>
    /// Represents the json exporter.
    /// </para>
    /// <para></para>
    /// </summary>
    public class JsonExporter<TLinkAddress>
    {
        /// <summary>
        /// <para>
        /// The storage.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly IJsonStorage<TLinkAddress> Storage;
        /// <summary>
        /// <para>
        /// The default.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly EqualityComparer<TLinkAddress> EqualityComparer = EqualityComparer<TLinkAddress>.Default;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="JsonExporter"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="storage">
        /// <para>A storage.</para>
        /// <para></para>
        /// </param>
        public JsonExporter(IJsonStorage<TLinkAddress> storage) => Storage = storage;
            private bool IsElement(TLinkAddress link)
        {
            var marker = Storage.Links.GetSource(link);
            return EqualityComparer.Equals(marker, Storage.ValueMarker);
        }
        private void WriteStringValue(in Utf8JsonWriter utf8JsonWriter, TLinkAddress valueLink) => utf8JsonWriter.WriteStringValue(Storage.GetString(valueLink));
        private void WriteString(in Utf8JsonWriter utf8JsonWriter, string parent, TLinkAddress valueLink) => utf8JsonWriter.WriteString(parent, Storage.GetString(valueLink));
        private void WriteNumberValue(in Utf8JsonWriter utf8JsonWriter, TLinkAddress valueLink) => utf8JsonWriter.WriteNumberValue(Storage.GetNumber(valueLink));
        private void WriteNumber(in Utf8JsonWriter utf8JsonWriter, string parent, TLinkAddress valueLink) => utf8JsonWriter.WriteNumber(parent, Storage.GetNumber(valueLink));
        private void Write(ref Utf8JsonWriter utf8JsonWriter, string parent, TLinkAddress valueLink, CancellationToken cancellationToken)
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
                    RightSequenceWalker<TLinkAddress> rightSequenceWalker = new(Storage.Links, new DefaultStack<TLinkAddress>(), IsElement);
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
        private void Write(ref Utf8JsonWriter utf8JsonWriter, TLinkAddress valueLink, in CancellationToken cancellationToken)
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
                    RightSequenceWalker<TLinkAddress> rightSequenceWalker = new(Storage.Links, new DefaultStack<TLinkAddress>(), IsElement);
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

        /// <summary>
        /// <para>
        /// Exports the document.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="document">
        /// <para>The document.</para>
        /// <para></para>
        /// </param>
        /// <param name="utf8JsonWriter">
        /// <para>The utf json writer.</para>
        /// <para></para>
        /// </param>
        /// <param name="cancellationToken">
        /// <para>The cancellation token.</para>
        /// <para></para>
        /// </param>
        /// <exception cref="Exception">
        /// <para>No document with this name exists</para>
        /// <para></para>
        /// </exception>
        public void Export(TLinkAddress document, ref Utf8JsonWriter utf8JsonWriter, in CancellationToken cancellationToken)
        {
            if (EqualityComparer.Equals(document, default))
            {
                throw new Exception("No document with this name exists");
            }
            var valueLink = Storage.GetValueLink(document);
            Write(ref utf8JsonWriter, valueLink, in cancellationToken);
            utf8JsonWriter.Flush();
        }

        /// <summary>
        /// <para>
        /// Exports the document name.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="documentName">
        /// <para>The document name.</para>
        /// <para></para>
        /// </param>
        /// <param name="utf8JsonWriter">
        /// <para>The utf json writer.</para>
        /// <para></para>
        /// </param>
        /// <param name="cancellationToken">
        /// <para>The cancellation token.</para>
        /// <para></para>
        /// </param>
        public void Export(string documentName, Utf8JsonWriter utf8JsonWriter, CancellationToken cancellationToken) => Export(Storage.GetDocumentOrDefault(documentName), ref utf8JsonWriter, in cancellationToken);
    }
}
