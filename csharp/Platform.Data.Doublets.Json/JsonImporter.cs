using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Threading;

namespace Platform.Data.Doublets.Json
{
    public class JsonImporter<TLink>
    {
        private readonly IJsonStorage<TLink> _storage;
        private readonly TLink _document;
        public JsonImporter(IJsonStorage<TLink> storage)
        {
            _storage = storage;
            _document = _storage.CreateDocument("documentName");
        }
        public TLink Import(ref Utf8JsonReader utf8JsonReader, CancellationToken cancellationToken)
        {
            Stack<TLink> parents = new();
            parents.Push(_document);
            while (utf8JsonReader.Read())
            {
                switch (utf8JsonReader.TokenType)
                {
                    case JsonTokenType.StartObject:
                        parents.Push(_storage.CreateObjectValue());
                        break;
                    case JsonTokenType.EndObject:
                        parents.Pop();
                        break;
                    case JsonTokenType.String:
                        _storage.AttachString(_document, utf8JsonReader.GetString());
                        break;
                }
            }
            return _document;
        }
    }
}
