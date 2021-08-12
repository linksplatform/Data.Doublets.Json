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
using Platform.Interfaces;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Json
{
    public class JsonArrayElementCriterionMatcher<TLink> : ICriterionMatcher<TLink>
    {
        public readonly IJsonStorage<TLink> Storage;
        public JsonArrayElementCriterionMatcher(IJsonStorage<TLink> storage) => Storage = storage;
        public bool IsMatched(TLink link) => EqualityComparer<TLink>.Default.Equals(Storage.Links.GetSource(link), Storage.ValueMarker);
    }
}
