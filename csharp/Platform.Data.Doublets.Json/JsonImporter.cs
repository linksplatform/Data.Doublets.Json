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
            while (utf8JsonReader.Read())
            {
                switch (utf8JsonReader.TokenType)
                {
                    case JsonTokenType.StartObject:
                        parents.Push(_storage.AttachObject(document));
                        break;
                    case JsonTokenType.EndObject:
                        parents.Pop();
                        break;
                    case JsonTokenType.String:
                        _storage.AttachString(document, utf8JsonReader.GetString());
                        break;
                    case JsonTokenType.Number:
                        _storage.AttachNumber(document, UncheckedConverter<int, TLink>.Default.Convert(utf8JsonReader.GetInt32()));
                        break;
                    case JsonTokenType.StartArray:
                        var array = Array.Empty<TLink>();
                        parents.Push(_storage.AttachArray(document, array));
                        break;
                    case JsonTokenType.EndArray:
                        parents.Pop();
                        break;
                    case JsonTokenType.True:
                        _storage.AttachBoolean(document, true);
                        break;
                }
            }

            return document;
        }
    }
}
