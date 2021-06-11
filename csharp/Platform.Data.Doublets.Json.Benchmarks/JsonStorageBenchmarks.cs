using BenchmarkDotNet.Attributes;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Memory;
using Platform.Memory;
using TLink = System.UInt32;
using System.IO;
using Platform.Collections.Stacks;
using Platform.Data.Doublets.Sequences.Walkers;
using System.Collections.Generic;
using System;

#pragma warning disable CA1822 // Mark members as static

namespace Platform.Data.Doublets.Json.Benchmarks
{
    public class JsonStorageBenchmarks
    {
        public static ILinks<TLink> CreateLinks() => CreateLinks<TLink>(Path.GetTempFileName());
        public static ILinks<TLink> CreateLinks<TLink>(string dataDBFilename)
        {
            var linksConstants = new LinksConstants<TLink>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLink>(new FileMappedResizableDirectMemory(dataDBFilename), UnitedMemoryLinks<TLink>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }

        [Benchmark]
        public TLink GetObject()
        {
            ILinks<TLink> links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new DefaultJsonStorage<TLink>(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");
            TLink documentObjectValueLink = defaultJsonStorage.AttachObject(document);
            TLink objectValueLink = links.GetTarget(documentObjectValueLink);
            return defaultJsonStorage.GetObject(objectValueLink);
        }

        [Benchmark]
        public TLink GetObjectWithoutLoop()
        {
            ILinks<TLink> links = CreateLinks();
            DefaultJsonStorage<TLink> defaultJsonStorage = new DefaultJsonStorage<TLink>(links);
            TLink document = defaultJsonStorage.CreateDocument("documentName");
            TLink documentobjectValueLinkLink = defaultJsonStorage.AttachObject(document);
            TLink objectValueLink = links.GetTarget(documentobjectValueLinkLink);

            EqualityComparer<TLink> equalityComparer = EqualityComparer<TLink>.Default;

            TLink current = objectValueLink;
            TLink source = links.GetSource(current);
            if (equalityComparer.Equals(source, defaultJsonStorage.ObjectMarker)) return objectValueLink;

            current = links.GetTarget(current);
            source = links.GetSource(current);
            if (equalityComparer.Equals(source, defaultJsonStorage.ObjectMarker)) return objectValueLink;

            current = links.GetTarget(current);
            source = links.GetSource(current);
            if (equalityComparer.Equals(source, defaultJsonStorage.ObjectMarker)) return objectValueLink;

            throw new Exception("Not an object.");
        }
    }
}