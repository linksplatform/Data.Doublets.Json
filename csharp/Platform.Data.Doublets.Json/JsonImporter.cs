using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Threading;
using System.IO;
using Platform.Converters;
using System.Collections;
using Platform.Data.Doublets.Sequences;
using Platform.Data.Doublets.Sequences.HeightProviders;
using Platform.Data.Doublets.Sequences.CriterionMatchers;
using Platform.Collections.Stacks;

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
            DefaultStack<TLink> stack = new();
            var equalityComparer = EqualityComparer<TLink>.Default;
            // For arrays
            JsonArrayElementCriterionMatcher<TLink> jsonArrayElementCriterionMatcher = new(_storage);
            DefaultSequenceRightHeightProvider<TLink> defaultSequenceRightHeightProvider = new(_storage.Links, jsonArrayElementCriterionMatcher);
            //CachedSequenceHeightProvider<TLink> cachedSequenceHeightProvider = new(defaultSequenceRightHeightProvider);
            DefaultSequenceAppender<TLink> defaultSequenceAppender = new(_storage.Links, stack, defaultSequenceRightHeightProvider);
            while (utf8JsonReader.Read())
            {
                var parent = _storage.IsMember(parents.Peek()) ? parents.Pop() : parents.Peek();
                var tokenType = utf8JsonReader.TokenType;
                if (utf8JsonReader.TokenType == JsonTokenType.PropertyName)
                {
                    parents.Push(_storage.AttachMemberToObject(_storage.GetObject(parent), utf8JsonReader.GetString()));
                }
                if (tokenType == JsonTokenType.StartObject)
                {
                    var value = _storage.CreateObjectValue();
                    if (equalityComparer.Equals(_storage.GetValueMarker(parent), _storage.ArrayMarker))
                    {
                        parents.Pop();
                        var newArray = _storage.AppendArrayValue(parent, value);
                        parents.Push(newArray);
                        parents.Push(value);
                    }
                    else
                    {
                        var @object = _storage.AttachObjectValue(parent, value);
                        parents.Push(@object);
                    }
                }
                else if (tokenType == JsonTokenType.EndObject)
                {
                    parents.Pop();
                }
                else if (tokenType == JsonTokenType.String)
                {
                    var @string = utf8JsonReader.GetString();
                    var value = _storage.CreateStringValue(@string);
                    if (equalityComparer.Equals(_storage.GetValueMarker(parent), _storage.ArrayMarker))
                    {
                        parents.Pop();
                        var newArray = _storage.AppendArrayValue(parent, value);
                        parents.Push(newArray);
                    }
                    else
                    {
                        _storage.AttachStringValue(parent, value);
                    }
                }
                else if (tokenType == JsonTokenType.Number)
                {
                    var number = UncheckedConverter<int, TLink>.Default.Convert(utf8JsonReader.GetInt32());
                    var value = _storage.CreateNumberValue(number);
                    if (equalityComparer.Equals(_storage.GetValueMarker(parent), _storage.ArrayMarker))
                    {
                        parents.Pop();
                        var newArray = _storage.AppendArrayValue(parent, value);
                        parents.Push(newArray);
                    }
                    else
                    {
                        _storage.AttachNumberValue(parent, value);
                    }
                }
                else if (tokenType == JsonTokenType.StartArray)
                {
                    var array = _storage.CreateArray(Array.Empty<TLink>());
                    var value = _storage.CreateArrayValue(array);
                    if (equalityComparer.Equals(_storage.GetValueMarker(parent), _storage.ArrayMarker))
                    {
                        parents.Pop();
                        var newArray = _storage.AppendArrayValue(parent, value);
                        parents.Push(newArray);
                    }
                    else
                    {
                        _storage.AttachArrayValue(parent, value);
                        parents.Push(value);
                    }
                }
                else if (tokenType == JsonTokenType.EndArray)
                {
                    parents.Pop();
                }
                else if (tokenType == JsonTokenType.True)
                {
                    var value = _storage.CreateBooleanValue(true);
                    if (equalityComparer.Equals(_storage.GetValueMarker(parent), _storage.ArrayMarker))
                    {
                        parents.Pop();
                        var newArray = _storage.AppendArrayValue(parent, value);
                        parents.Push(newArray);
                    }
                    else
                    {
                        _storage.AttachBooleanValue(parent, value);
                    }
                }
                else if (tokenType == JsonTokenType.False)
                {
                    var value = _storage.CreateBooleanValue(false);
                    if (equalityComparer.Equals(_storage.GetValueMarker(parent), _storage.ArrayMarker))
                    {
                        parents.Pop();
                        var newArray = _storage.AppendArrayValue(parent, value);
                        parents.Push(newArray);
                    }
                    else
                    {
                        _storage.AttachBooleanValue(parent, value);
                    }
                }
                else if (tokenType == JsonTokenType.Null)
                {
                    var value = _storage.CreateNullValue();
                    if (equalityComparer.Equals(_storage.GetValueMarker(parent), _storage.ArrayMarker))
                    {
                        parents.Pop();
                        var newArray = _storage.AppendArrayValue(parent, value);
                        parents.Push(newArray);
                    }
                    else
                    {
                        _storage.AttachNullValue(parent, value);
                    }
                }
            }
            return document;
        }
    }
}
