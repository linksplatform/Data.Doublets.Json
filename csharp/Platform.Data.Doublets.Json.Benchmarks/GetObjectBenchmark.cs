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
    public class GetObjectBenchmark
    {
        private ILinks<TLink> links;
        private DefaultJsonStorage<TLink> _defaultJsonStorage;
        private TLink _document;
        private TLink _documentObjectValueLink;
        private TLink _objectValueLink;
        public static ILinks<TLink> CreateLinks() => CreateLinks<TLink>(Path.GetTempFileName());
        public static ILinks<TLink> CreateLinks<TLink>(string dataDBFilename)
        {
            var linksConstants = new LinksConstants<TLink>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLink>(new FileMappedResizableDirectMemory(dataDBFilename), UnitedMemoryLinks<TLink>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }
        
        [GlobalSetup]
        public static void Setup()
        {
            ILinks<TLink> _links = CreateLinks();
            DefaultJsonStorage<TLink> _defaultJsonStorage = new DefaultJsonStorage<TLink>(_links);
            TLink _document = _defaultJsonStorage.CreateDocument("documentName");
            TLink _documentObjectValueLink = _defaultJsonStorage.AttachObject(_document);
            TLink _objectValueLink = _links.GetTarget(_documentObjectValueLink);
        }
        [Benchmark]
        public TLink GetObject()
        {

            return _defaultJsonStorage.GetObject(_objectValueLink);
        }

        [Benchmark]
        public TLink GetObjectWithoutLoop()
        {
            EqualityComparer<TLink> equalityComparer = EqualityComparer<TLink>.Default;

            TLink current = _objectValueLink;
            TLink source = links.GetSource(current);
            if (equalityComparer.Equals(source, _defaultJsonStorage.ObjectMarker)) return _objectValueLink;

            current = links.GetTarget(current);
            source = links.GetSource(current);
            if (equalityComparer.Equals(source, _defaultJsonStorage.ObjectMarker)) return _objectValueLink;

            current = links.GetTarget(current);
            source = links.GetSource(current);
            if (equalityComparer.Equals(source, _defaultJsonStorage.ObjectMarker)) return _objectValueLink;

            throw new Exception("Not an object.");
        }
    }
}