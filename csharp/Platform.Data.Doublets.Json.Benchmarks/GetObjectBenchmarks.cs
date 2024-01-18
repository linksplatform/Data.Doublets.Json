using BenchmarkDotNet.Attributes; 
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Memory;
using Platform.Memory;
using TLinkAddress = System.UInt32;
using System.IO;
using System.Collections.Generic;
using System;
using Platform.Data.Doublets.Sequences.Converters;

#pragma warning disable CA1822 // Mark members as static

namespace Platform.Data.Doublets.Json.Benchmarks
{
    public class GetObjectBenchmarks
    {
        private ILinks<TLinkAddress> _links;
        private BalancedVariantConverter<TLinkAddress> _balancedVariantConverter;
        private DefaultJsonStorage<TLinkAddress> _defaultJsonStorage;
        private TLinkAddress _document;
        private TLinkAddress _documentObjectValueLink;
        private TLinkAddress _objectValueLink;
        private TLinkAddress _object;
        public static ILinks<TLinkAddress> CreateLinks() => CreateLinks<TLinkAddress>(Path.GetTempFileName());
        public static ILinks<TLinkAddress> CreateLinks<TLinkAddress>(string dataDBFilename)
        {
            var linksConstants = new LinksConstants<TLinkAddress>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLinkAddress>(new FileMappedResizableDirectMemory(dataDBFilename), UnitedMemoryLinks<TLinkAddress>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }

        [GlobalSetup]
        public void Setup()
        {
            _links = CreateLinks();
            _balancedVariantConverter = new(_links);
            _defaultJsonStorage = new DefaultJsonStorage<TLinkAddress>(_links, _balancedVariantConverter);
            _document = _defaultJsonStorage.CreateDocument("documentName");
            _documentObjectValueLink = _defaultJsonStorage.AttachObject(_document);
            _objectValueLink = _links.GetTarget(_documentObjectValueLink);
            _object = _links.GetTarget(_objectValueLink);
        }

        public TLinkAddress GetObjectWihoutLoop(TLinkAddress objectValue)
        {
            EqualityComparer<TLinkAddress> equalityComparer = EqualityComparer<TLinkAddress>.Default;

            TLinkAddress current = objectValue;
            TLinkAddress source = _links.GetSource(current);
            if (equalityComparer.Equals(source, _defaultJsonStorage.ObjectType))
            {
                return current;
            }
            current = _links.GetTarget(current);
            source = _links.GetSource(current);
            if (equalityComparer.Equals(source, _defaultJsonStorage.ObjectType))
            {
                return current;
            }
            current = _links.GetTarget(current);
            source = _links.GetSource(current);
            if (equalityComparer.Equals(source, _defaultJsonStorage.ObjectType))
            {
                return current;
            }
            throw new Exception("Not an object.");
        }
        [Benchmark]
        public TLinkAddress GetObjectFromDocumentObjectValueLinkBenchmark()
        {
            return _defaultJsonStorage.GetObject(_documentObjectValueLink);
        }

        [Benchmark]
        public TLinkAddress GetObjectFromObjectValueLinkBenchmark()
        {
            return _defaultJsonStorage.GetObject(_objectValueLink);
        }

        [Benchmark]
        public TLinkAddress GetObjectFromObjectBenchmark()
        {
            return _defaultJsonStorage.GetObject(_object);
        }

        [Benchmark]
        public TLinkAddress GetObjectFromDocumentObjectValueLinkWithoutLoopBenchmark()
        {
            return GetObjectWihoutLoop(_documentObjectValueLink);
        }

        [Benchmark]
        public TLinkAddress GetObjectFromObjectValueLinkWithoutLoopBenchmark()
        {
            return GetObjectWihoutLoop(_objectValueLink);
        }

        [Benchmark]
        public TLinkAddress GetObjectFromObjectWithoutLoopBenchmark()
        {
            return GetObjectWihoutLoop(_object);
        }
    }
}
