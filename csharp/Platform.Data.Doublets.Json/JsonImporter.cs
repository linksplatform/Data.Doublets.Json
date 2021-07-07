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
    public class JsonImporter<TLink>
    {
        private readonly IJsonStorage<TLink> _storage;
        public JsonImporter(IJsonStorage<TLink> storage) => _storage = storage;
        public TLink Import(string documentName, ref Utf8JsonReader utf8JsonReader, CancellationToken cancellationToken)
        {
            TLink document = _storage.CreateDocument(documentName);
            Stack<TLink> parents = new();
            parents.Push(document);
            var isLastParentProperty = false;
            while (utf8JsonReader.Read())
            {
                var tokenType = utf8JsonReader.TokenType;
                if (utf8JsonReader.TokenType == JsonTokenType.PropertyName)
                {
                    parents.Push(_storage.AttachMemberToObject(_storage.GetObject(parents.First()), utf8JsonReader.GetString()));
                    isLastParentProperty = true;
                }
                if (tokenType == JsonTokenType.StartObject)
                {
                    parents.Push(_storage.AttachObject(parents.First()));
                }
                else if (tokenType == JsonTokenType.EndObject)
                {
                    parents.Pop();
                }
                else if (tokenType == JsonTokenType.String)
                {
                    _storage.AttachString(parents.First(), utf8JsonReader.GetString());
                }
                else if (tokenType == JsonTokenType.Number)
                {
                    _storage.AttachNumber(parents.First(), UncheckedConverter<int, TLink>.Default.Convert(utf8JsonReader.GetInt32()));
                }
                else if (tokenType == JsonTokenType.StartArray)
                {
                    var array = Array.Empty<TLink>();
                    parents.Push(_storage.AttachArray(parents.First(), array));
                }
                else if (tokenType == JsonTokenType.EndArray)
                {
                    parents.Pop();
                }
                else if (tokenType == JsonTokenType.True)
                {
                    _storage.AttachBoolean(parents.First(), true);
                }
                else if (tokenType == JsonTokenType.False)
                {
                    _storage.AttachBoolean(parents.First(), false);
                }
                else if (tokenType == JsonTokenType.Null)
                {
                    _storage.AttachNull(parents.First());
                }
                if (isLastParentProperty && tokenType != JsonTokenType.PropertyName)
                {
                    isLastParentProperty = false;
                    parents.Pop();
                }
            }

            return document;
        }
    }
}
