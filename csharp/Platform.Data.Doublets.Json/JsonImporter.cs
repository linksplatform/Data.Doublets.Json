using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Json
{
    /// <summary>
    /// <para>
    /// Represents the json importer.
    /// </para>
    /// <para></para>
    /// </summary>
    public class JsonImporter<TLinkAddress>
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
        /// The parents.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly Stack<TLinkAddress> Parents = new ();
        /// <summary>
        /// <para>
        /// Initializes a new <see cref="JsonImporter"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="storage">
        /// <para>A storage.</para>
        /// <para></para>
        /// </param>
        public JsonImporter(IJsonStorage<TLinkAddress> storage) => Storage = storage;
            private void PopIfParentIsMember()
        {
            var parent = Parents.Peek();
            var parentType = Storage.GetValueType(parent);
            if (EqualityComparer.Equals(parentType, Storage.MemberType))
            {
                Parents.Pop();
            }
        }

        /// <summary>
        /// <para>
        /// Imports the document name.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="documentName">
        /// <para>The document name.</para>
        /// <para></para>
        /// </param>
        /// <param name="utf8JsonReader">
        /// <para>The utf json reader.</para>
        /// <para></para>
        /// </param>
        /// <param name="cancellationToken">
        /// <para>The cancellation token.</para>
        /// <para></para>
        /// </param>
        /// <exception cref="Exception">
        /// <para>The document with the specified name already exists.</para>
        /// <para></para>
        /// </exception>
        /// <returns>
        /// <para>The document.</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress Import(string documentName, ref Utf8JsonReader utf8JsonReader, in CancellationToken cancellationToken)
        {
            Parents.Clear();
            if (!EqualityComparer.Equals(Storage.GetDocumentOrDefault(documentName), default))
            {
                throw new Exception("The document with the specified name already exists.");
            }
            var document = Storage.CreateDocument(documentName);
            Parents.Push(document);
            TLinkAddress parent;
            TLinkAddress parentType;
            JsonTokenType tokenType;
            TLinkAddress value;
            TLinkAddress newParentArray;
            while (utf8JsonReader.Read())
            {
                cancellationToken.ThrowIfCancellationRequested();
                parent = Parents.Peek();
                parentType = Storage.GetValueType(parent);
                tokenType = utf8JsonReader.TokenType;
                if (utf8JsonReader.TokenType == JsonTokenType.PropertyName)
                {
                    var @object = Storage.GetObject(parent);
                    var property = utf8JsonReader.GetString();
                    Parents.Push(Storage.AttachMemberToObject(@object, property));
                }
                switch (tokenType)
                {
                    case JsonTokenType.StartObject:
                    {
                        value = Storage.CreateObjectValue();
                        if (EqualityComparer.Equals(parentType, Storage.ArrayType))
                        {
                            Parents.Pop();
                            newParentArray = Storage.AppendArrayValue(parent, value);
                            Parents.Push(newParentArray);
                            Parents.Push(value);
                        }
                        else
                        {
                            var @object = Storage.Attach(parent, value);
                            Parents.Push(@object);
                        }
                        break;
                    }
                    case JsonTokenType.EndObject:
                        Parents.Pop();
                        break;
                    case JsonTokenType.StartArray:
                        value = Storage.CreateArrayValue(Array.Empty<TLinkAddress>());
                        Parents.Push(value);
                        break;
                    case JsonTokenType.EndArray:
                    {
                        var arrayValue = Parents.Pop();
                        parent = Parents.Peek();
                        parentType = Storage.GetValueType(parent);
                        if (EqualityComparer.Equals(parentType, Storage.ArrayType))
                        {
                            Parents.Pop();
                            newParentArray = Storage.AppendArrayValue(parent, arrayValue);
                            Parents.Push(newParentArray);
                        }
                        Storage.Attach(parent, arrayValue);
                        break;
                    }
                    case JsonTokenType.String:
                    {
                        var @string = utf8JsonReader.GetString();
                        value = Storage.CreateStringValue(@string);
                        if (EqualityComparer.Equals(parentType, Storage.ArrayType))
                        {
                            Parents.Pop();
                            newParentArray = Storage.AppendArrayValue(parent, value);
                            Parents.Push(newParentArray);
                        }
                        else
                        {
                            Storage.Attach(parent, value);
                        }
                        break;
                    }
                    case JsonTokenType.Number:
                    {
                        value = Storage.CreateNumberValue(utf8JsonReader.GetDecimal());
                        if (EqualityComparer.Equals(parentType, Storage.ArrayType))
                        {
                            Parents.Pop();
                            newParentArray = Storage.AppendArrayValue(parent, value);
                            Parents.Push(newParentArray);
                        }
                        else
                        {
                            Storage.Attach(parent, value);
                        }
                        break;
                    }
                    case JsonTokenType.True:
                    {
                        value = Storage.CreateBooleanValue(true);
                        if (EqualityComparer.Equals(parentType, Storage.ArrayType))
                        {
                            Parents.Pop();
                            newParentArray = Storage.AppendArrayValue(parent, value);
                            Parents.Push(newParentArray);
                        }
                        else
                        {
                            Storage.Attach(parent, value);
                        }
                        break;
                    }
                    case JsonTokenType.False:
                    {
                        value = Storage.CreateBooleanValue(false);
                        if (EqualityComparer.Equals(parentType, Storage.ArrayType))
                        {
                            Parents.Pop();
                            newParentArray = Storage.AppendArrayValue(parent, value);
                            Parents.Push(newParentArray);
                        }
                        else
                        {
                            Storage.Attach(parent, value);
                        }
                        break;
                    }
                    case JsonTokenType.Null:
                    {
                        value = Storage.CreateNullValue();
                        if (EqualityComparer.Equals(parentType, Storage.ArrayType))
                        {
                            Parents.Pop();
                            newParentArray = Storage.AppendArrayValue(parent, value);
                            Parents.Push(newParentArray);
                        }
                        else
                        {
                            Storage.Attach(parent, value);
                        }
                        break;
                    }
                }
                if (tokenType != JsonTokenType.PropertyName && tokenType != JsonTokenType.StartObject && tokenType != JsonTokenType.StartArray)
                {
                    PopIfParentIsMember();
                }
            }
            return document;
        }
    }
}
