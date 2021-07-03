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
    public class GetObjectBenchmarks
    {
        private ILinks<TLink> _links;
        private DefaultJsonStorage<TLink> _defaultJsonStorage;
        private TLink _document;
        private TLink _documentObjectValueLink;
        private TLink _objectValueLink;
        private TLink _object;
        public static ILinks<TLink> CreateLinks() => CreateLinks<TLink>(Path.GetTempFileName());
        public static ILinks<TLink> CreateLinks<TLink>(string dataDBFilename)
        {
            var linksConstants = new LinksConstants<TLink>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLink>(new FileMappedResizableDirectMemory(dataDBFilename), UnitedMemoryLinks<TLink>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }

        [GlobalSetup]
        public void Setup()
        {
            _links = CreateLinks();
            _defaultJsonStorage = new DefaultJsonStorage<TLink>(_links);
            _document = _defaultJsonStorage.CreateDocument("documentName");
            _documentObjectValueLink = _defaultJsonStorage.AttachObject(_document);
            _objectValueLink = _links.GetTarget(_documentObjectValueLink);
            _object = _links.GetTarget(_objectValueLink);
        }

        public TLink GetObjectWihoutLoop(TLink objectValue)
        {
            EqualityComparer<TLink> equalityComparer = EqualityComparer<TLink>.Default;

            TLink current = objectValue;
            TLink source = _links.GetSource(current);
            if (equalityComparer.Equals(source, _defaultJsonStorage.ObjectMarker)) return current;

            current = _links.GetTarget(current);
            source = _links.GetSource(current);
            if (equalityComparer.Equals(source, _defaultJsonStorage.ObjectMarker)) return current;

            current = _links.GetTarget(current);
            source = _links.GetSource(current);
            if (equalityComparer.Equals(source, _defaultJsonStorage.ObjectMarker)) return current;

            throw new Exception("Not an object.");
        }
        [Benchmark]
        public TLink GetObjectFromDocumentObjectValueLinkBenchmark()
        {
            return _defaultJsonStorage.GetObject(_documentObjectValueLink);
        }

        [Benchmark]
        public TLink GetObjectFromObjectValueLinkBenchmark()
        {
            return _defaultJsonStorage.GetObject(_objectValueLink);
        }

        [Benchmark]
        public TLink GetObjectFromObjectBenchmark()
        {
            return _defaultJsonStorage.GetObject(_object);
        }

        [Benchmark]
        public TLink GetObjectFromDocumentObjectValueLinkWithoutLoopBenchmark()
        {
            return GetObjectWihoutLoop(_documentObjectValueLink);
        }

        [Benchmark]
        public TLink GetObjectFromObjectValueLinkWithoutLoopBenchmark()
        {
            return GetObjectWihoutLoop(_objectValueLink);
        }

        [Benchmark]
        public TLink GetObjectFromObjectWithoutLoopBenchmark()
        {
            return GetObjectWihoutLoop(_object);
        }
    }
}