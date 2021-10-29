using BenchmarkDotNet.Attributes;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Memory;
using Platform.Memory;
using TLink = System.UInt32;
using System.IO;
using System.Collections.Generic;
using System;
using Platform.Data.Doublets.Sequences.Converters;

#pragma warning disable CA1822 // Mark members as static

namespace Platform.Data.Doublets.Json.Benchmarks
{
    /// <summary>
    /// <para>
    /// Represents the get object benchmarks.
    /// </para>
    /// <para></para>
    /// </summary>
    public class GetObjectBenchmarks
    {
        /// <summary>
        /// <para>
        /// The links.
        /// </para>
        /// <para></para>
        /// </summary>
        private ILinks<TLink> _links;
        /// <summary>
        /// <para>
        /// The balanced variant converter.
        /// </para>
        /// <para></para>
        /// </summary>
        private BalancedVariantConverter<TLink> _balancedVariantConverter;
        /// <summary>
        /// <para>
        /// The default json storage.
        /// </para>
        /// <para></para>
        /// </summary>
        private DefaultJsonStorage<TLink> _defaultJsonStorage;
        /// <summary>
        /// <para>
        /// The document.
        /// </para>
        /// <para></para>
        /// </summary>
        private TLink _document;
        /// <summary>
        /// <para>
        /// The document object value link.
        /// </para>
        /// <para></para>
        /// </summary>
        private TLink _documentObjectValueLink;
        /// <summary>
        /// <para>
        /// The object value link.
        /// </para>
        /// <para></para>
        /// </summary>
        private TLink _objectValueLink;
        /// <summary>
        /// <para>
        /// The object.
        /// </para>
        /// <para></para>
        /// </summary>
        private TLink _object;
        /// <summary>
        /// <para>
        /// Creates the links.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>A links of t link</para>
        /// <para></para>
        /// </returns>
        public static ILinks<TLink> CreateLinks() => CreateLinks<TLink>(Path.GetTempFileName());
        /// <summary>
        /// <para>
        /// Creates the links using the specified data db filename.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <typeparam name="TLink">
        /// <para>The link.</para>
        /// <para></para>
        /// </typeparam>
        /// <param name="dataDBFilename">
        /// <para>The data db filename.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>A links of t link</para>
        /// <para></para>
        /// </returns>
        public static ILinks<TLink> CreateLinks<TLink>(string dataDBFilename)
        {
            var linksConstants = new LinksConstants<TLink>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLink>(new FileMappedResizableDirectMemory(dataDBFilename), UnitedMemoryLinks<TLink>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }

        /// <summary>
        /// <para>
        /// Setup this instance.
        /// </para>
        /// <para></para>
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            _links = CreateLinks();
            _balancedVariantConverter = new(_links);
            _defaultJsonStorage = new DefaultJsonStorage<TLink>(_links, _balancedVariantConverter);
            _document = _defaultJsonStorage.CreateDocument("documentName");
            _documentObjectValueLink = _defaultJsonStorage.AttachObject(_document);
            _objectValueLink = _links.GetTarget(_documentObjectValueLink);
            _object = _links.GetTarget(_objectValueLink);
        }

        /// <summary>
        /// <para>
        /// Gets the object wihout loop using the specified object value.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="objectValue">
        /// <para>The object value.</para>
        /// <para></para>
        /// </param>
        /// <exception cref="Exception">
        /// <para>Not an object.</para>
        /// <para></para>
        /// </exception>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
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
        /// <summary>
        /// <para>
        /// Gets the object from document object value link benchmark.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        [Benchmark]
        public TLink GetObjectFromDocumentObjectValueLinkBenchmark()
        {
            return _defaultJsonStorage.GetObject(_documentObjectValueLink);
        }

        /// <summary>
        /// <para>
        /// Gets the object from object value link benchmark.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        [Benchmark]
        public TLink GetObjectFromObjectValueLinkBenchmark()
        {
            return _defaultJsonStorage.GetObject(_objectValueLink);
        }

        /// <summary>
        /// <para>
        /// Gets the object from object benchmark.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        [Benchmark]
        public TLink GetObjectFromObjectBenchmark()
        {
            return _defaultJsonStorage.GetObject(_object);
        }

        /// <summary>
        /// <para>
        /// Gets the object from document object value link without loop benchmark.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        [Benchmark]
        public TLink GetObjectFromDocumentObjectValueLinkWithoutLoopBenchmark()
        {
            return GetObjectWihoutLoop(_documentObjectValueLink);
        }

        /// <summary>
        /// <para>
        /// Gets the object from object value link without loop benchmark.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        [Benchmark]
        public TLink GetObjectFromObjectValueLinkWithoutLoopBenchmark()
        {
            return GetObjectWihoutLoop(_objectValueLink);
        }

        /// <summary>
        /// <para>
        /// Gets the object from object without loop benchmark.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        [Benchmark]
        public TLink GetObjectFromObjectWithoutLoopBenchmark()
        {
            return GetObjectWihoutLoop(_object);
        }
    }
}