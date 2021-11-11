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
    public class JsonExporter<TLink>
    {
        /// <summary>
        /// <para>
        /// The storage.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly IJsonStorage<TLink> Storage;
        /// <summary>
        /// <para>
        /// The default.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly EqualityComparer<TLink> EqualityComparer = EqualityComparer<TLink>.Default;

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
        public JsonExporter(IJsonStorage<TLink> storage) => Storage = storage;

            /// <summary>
            /// <para>
            /// Determines whether this instance is element.
            /// </para>
            /// <para></para>
            /// </summary>
            /// <param name="link">
            /// <para>The link.</para>
            /// <para></para>
            /// </param>
            /// <returns>
            /// <para>The bool</para>
            /// <para></para>
            /// </returns>
            private bool IsElement(TLink link)
        {
            var marker = Storage.Links.GetSource(link);
            return EqualityComparer.Equals(marker, Storage.ValueMarker);
        }

        /// <summary>
        /// <para>
        /// Writes the string value using the specified utf 8 json writer.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="utf8JsonWriter">
        /// <para>The utf json writer.</para>
        /// <para></para>
        /// </param>
        /// <param name="valueLink">
        /// <para>The value link.</para>
        /// <para></para>
        /// </param>
        private void WriteStringValue(in Utf8JsonWriter utf8JsonWriter, TLink valueLink) => utf8JsonWriter.WriteStringValue(Storage.GetString(valueLink));

        /// <summary>
        /// <para>
        /// Writes the string using the specified utf 8 json writer.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="utf8JsonWriter">
        /// <para>The utf json writer.</para>
        /// <para></para>
        /// </param>
        /// <param name="parent">
        /// <para>The parent.</para>
        /// <para></para>
        /// </param>
        /// <param name="valueLink">
        /// <para>The value link.</para>
        /// <para></para>
        /// </param>
        private void WriteString(in Utf8JsonWriter utf8JsonWriter, string parent, TLink valueLink) => utf8JsonWriter.WriteString(parent, Storage.GetString(valueLink));

        /// <summary>
        /// <para>
        /// Writes the number value using the specified utf 8 json writer.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="utf8JsonWriter">
        /// <para>The utf json writer.</para>
        /// <para></para>
        /// </param>
        /// <param name="valueLink">
        /// <para>The value link.</para>
        /// <para></para>
        /// </param>
        private void WriteNumberValue(in Utf8JsonWriter utf8JsonWriter, TLink valueLink) => utf8JsonWriter.WriteNumberValue(Storage.GetNumber(valueLink));

        /// <summary>
        /// <para>
        /// Writes the number using the specified utf 8 json writer.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="utf8JsonWriter">
        /// <para>The utf json writer.</para>
        /// <para></para>
        /// </param>
        /// <param name="parent">
        /// <para>The parent.</para>
        /// <para></para>
        /// </param>
        /// <param name="valueLink">
        /// <para>The value link.</para>
        /// <para></para>
        /// </param>
        private void WriteNumber(in Utf8JsonWriter utf8JsonWriter, string parent, TLink valueLink) => utf8JsonWriter.WriteNumber(parent, Storage.GetNumber(valueLink));

        /// <summary>
        /// <para>
        /// Writes the utf 8 json writer.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="utf8JsonWriter">
        /// <para>The utf json writer.</para>
        /// <para></para>
        /// </param>
        /// <param name="parent">
        /// <para>The parent.</para>
        /// <para></para>
        /// </param>
        /// <param name="valueLink">
        /// <para>The value link.</para>
        /// <para></para>
        /// </param>
        /// <param name="cancellationToken">
        /// <para>The cancellation token.</para>
        /// <para></para>
        /// </param>
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

        /// <summary>
        /// <para>
        /// Writes the utf 8 json writer.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="utf8JsonWriter">
        /// <para>The utf json writer.</para>
        /// <para></para>
        /// </param>
        /// <param name="valueLink">
        /// <para>The value link.</para>
        /// <para></para>
        /// </param>
        /// <param name="cancellationToken">
        /// <para>The cancellation token.</para>
        /// <para></para>
        /// </param>
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
        public void Export(TLink document, ref Utf8JsonWriter utf8JsonWriter, in CancellationToken cancellationToken)
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
