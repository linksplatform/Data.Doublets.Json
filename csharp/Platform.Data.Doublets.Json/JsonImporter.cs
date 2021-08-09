using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;

namespace Platform.Data.Doublets.Json
{
    public class JsonImporter<TLink>
    {
        public readonly IJsonStorage<TLink> Storage;
        public readonly EqualityComparer<TLink> EqualityComparer;
        public readonly Stack<TLink> Parents;
        public JsonImporter(IJsonStorage<TLink> storage)
        {
            Storage = storage;
            EqualityComparer = EqualityComparer<TLink>.Default;
            Parents = new ();
        }

        private void PopIfParentIsMember()
        {
            var parent = Parents.Peek();
            var parentMarker = Storage.GetValueMarker(parent);
            if (EqualityComparer.Equals(parentMarker, Storage.MemberMarker))
            {
                Parents.Pop();
            }
        }

        public TLink Import(string documentName, ref Utf8JsonReader utf8JsonReader, in CancellationToken cancellationToken)
        {
            Parents.Clear();
            if (!EqualityComparer.Equals(Storage.GetDocumentOrDefault(documentName), default))
            {
                throw new Exception("The document with the specified name already exists.");
            }
            TLink document = Storage.CreateDocument(documentName);
            Parents.Push(document);
            while (utf8JsonReader.Read())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var parent = Parents.Peek();
                var parentMarker = Storage.GetValueMarker(parent);
                var tokenType = utf8JsonReader.TokenType;
                if (utf8JsonReader.TokenType == JsonTokenType.PropertyName)
                {
                    Parents.Push(Storage.AttachMemberToObject(Storage.GetObject(parent), utf8JsonReader.GetString()));
                }
                if (tokenType == JsonTokenType.StartObject)
                {
                    var value = Storage.CreateObjectValue();
                    if (EqualityComparer.Equals(parentMarker, Storage.ArrayMarker))
                    {
                        Parents.Pop();
                        var newArray = Storage.AppendArrayValue(parent, value);
                        Parents.Push(newArray);
                        Parents.Push(value);
                    }
                    else
                    {
                        var @object = Storage.AttachObjectValue(parent, value);
                        Parents.Push(@object);
                    }
                }
                else if (tokenType == JsonTokenType.EndObject)
                {
                    Parents.Pop(); 
                }
                else if (tokenType == JsonTokenType.StartArray)
                {
                    var value = Storage.CreateArrayValue(Array.Empty<TLink>());
                    Parents.Push(value);
                }
                else if (tokenType == JsonTokenType.EndArray)
                {
                    var arrayValue = Parents.Pop();
                    parent = Parents.Peek();
                    parentMarker = Storage.GetValueMarker(parent);
                    if (EqualityComparer.Equals(parentMarker, Storage.ArrayMarker))
                    {
                        var parentArray = Parents.Pop();
                        var newParentArray = Storage.AppendArrayValue(parentArray, arrayValue);
                        Parents.Push(newParentArray);
                    }
                    Storage.AttachArrayValue(parent, arrayValue);
                }
                else if (tokenType == JsonTokenType.String)
                {
                    var @string = utf8JsonReader.GetString();
                    var value = Storage.CreateStringValue(@string);
                    if (EqualityComparer.Equals(parentMarker, Storage.ArrayMarker))
                    {
                        Parents.Pop();
                        var newArray = Storage.AppendArrayValue(parent, value);
                        Parents.Push(newArray);
                    }
                    else
                    {
                        Storage.AttachStringValue(parent, value);
                    }
                }
                else if (tokenType == JsonTokenType.Number)
                {
                    var value = Storage.CreateNumberValue(utf8JsonReader.GetDecimal());
                    if (EqualityComparer.Equals(parentMarker, Storage.ArrayMarker))
                    {
                        Parents.Pop();
                        var newArray = Storage.AppendArrayValue(parent, value);
                        Parents.Push(newArray);
                    }
                    else
                    {
                        Storage.AttachNumberValue(parent, value);
                    }
                }
                else if (tokenType == JsonTokenType.True)
                {
                    var value = Storage.CreateBooleanValue(true);
                    if (EqualityComparer.Equals(parentMarker, Storage.ArrayMarker))
                    {
                        Parents.Pop();
                        var newArray = Storage.AppendArrayValue(parent, value);
                        Parents.Push(newArray);
                    }
                    else
                    {
                        Storage.AttachBooleanValue(parent, value);
                    }
                }
                else if (tokenType == JsonTokenType.False)
                {
                    var value = Storage.CreateBooleanValue(false);
                    if (EqualityComparer.Equals(parentMarker, Storage.ArrayMarker))
                    {
                        Parents.Pop();
                        var newArray = Storage.AppendArrayValue(parent, value);
                        Parents.Push(newArray);
                    }
                    else
                    {
                        Storage.AttachBooleanValue(parent, value);
                    }
                }
                else if (tokenType == JsonTokenType.Null)
                {
                    var value = Storage.CreateNullValue();
                    if (EqualityComparer.Equals(parentMarker, Storage.ArrayMarker))
                    {
                        Parents.Pop();
                        var newArray = Storage.AppendArrayValue(parent, value);
                        Parents.Push(newArray);
                    }
                    else
                    {
                        Storage.AttachNullValue(parent, value);
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
