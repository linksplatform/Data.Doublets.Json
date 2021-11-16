using System;
using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;
using Platform.Data.Doublets.Memory;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Memory;
using TLink = System.UInt32;

#pragma warning disable CA1822 // Mark members as static

namespace Platform.Data.Doublets.Json.Benchmarks
{
    /// <summary>
    ///     <para>
    ///         Represents the get object benchmarks.
    ///     </para>
    ///     <para></para>
    /// </summary>
    public class GetObjectBenchmarks
    {
        private BalancedVariantConverter<uint> _balancedVariantConverter;
        private DefaultJsonStorage<uint> _defaultJsonStorage;
        private uint _document;
        private uint _documentObjectValueLink;
        private ILinks<uint> _links;
        private uint _object;
        private uint _objectValueLink;

        /// <summary>
        ///     <para>
        ///         Creates the links.
        ///     </para>
        ///     <para></para>
        /// </summary>
        /// <returns>
        ///     <para>A links of t link</para>
        ///     <para></para>
        /// </returns>
        public static ILinks<uint> CreateLinks()
        {
            return CreateLinks<uint>(Path.GetTempFileName());
        }

        /// <summary>
        ///     <para>
        ///         Creates the links using the specified data db filename.
        ///     </para>
        ///     <para></para>
        /// </summary>
        /// <typeparam name="TLink">
        ///     <para>The link.</para>
        ///     <para></para>
        /// </typeparam>
        /// <param name="dataDBFilename">
        ///     <para>The data db filename.</para>
        ///     <para></para>
        /// </param>
        /// <returns>
        ///     <para>A links of t link</para>
        ///     <para></para>
        /// </returns>
        public static ILinks<TLink> CreateLinks<TLink>(string dataDBFilename)
        {
            var linksConstants = new LinksConstants<TLink>(true);
            return new UnitedMemoryLinks<TLink>(new FileMappedResizableDirectMemory(dataDBFilename), UnitedMemoryLinks<TLink>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }

        /// <summary>
        ///     <para>
        ///         Setup this instance.
        ///     </para>
        ///     <para></para>
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            _links = CreateLinks();
            _balancedVariantConverter = new BalancedVariantConverter<uint>(_links);
            _defaultJsonStorage = new DefaultJsonStorage<uint>(_links, _balancedVariantConverter);
            _document = _defaultJsonStorage.CreateDocument("documentName");
            _documentObjectValueLink = _defaultJsonStorage.AttachObject(_document);
            _objectValueLink = _links.GetTarget(_documentObjectValueLink);
            _object = _links.GetTarget(_objectValueLink);
        }

        /// <summary>
        ///     <para>
        ///         Gets the object wihout loop using the specified object value.
        ///     </para>
        ///     <para></para>
        /// </summary>
        /// <param name="objectValue">
        ///     <para>The object value.</para>
        ///     <para></para>
        /// </param>
        /// <exception cref="Exception">
        ///     <para>Not an object.</para>
        ///     <para></para>
        /// </exception>
        /// <returns>
        ///     <para>The link</para>
        ///     <para></para>
        /// </returns>
        public uint GetObjectWihoutLoop(uint objectValue)
        {
            var equalityComparer = EqualityComparer<uint>.Default;
            var current = objectValue;
            var source = _links.GetSource(current);
            if (equalityComparer.Equals(source, _defaultJsonStorage.ObjectMarker))
            {
                return current;
            }
            current = _links.GetTarget(current);
            source = _links.GetSource(current);
            if (equalityComparer.Equals(source, _defaultJsonStorage.ObjectMarker))
            {
                return current;
            }
            current = _links.GetTarget(current);
            source = _links.GetSource(current);
            if (equalityComparer.Equals(source, _defaultJsonStorage.ObjectMarker))
            {
                return current;
            }
            throw new Exception("Not an object.");
        }

        /// <summary>
        ///     <para>
        ///         Gets the object from document object value link benchmark.
        ///     </para>
        ///     <para></para>
        /// </summary>
        /// <returns>
        ///     <para>The link</para>
        ///     <para></para>
        /// </returns>
        [Benchmark]
        public uint GetObjectFromDocumentObjectValueLinkBenchmark()
        {
            return _defaultJsonStorage.GetObject(_documentObjectValueLink);
        }

        /// <summary>
        ///     <para>
        ///         Gets the object from object value link benchmark.
        ///     </para>
        ///     <para></para>
        /// </summary>
        /// <returns>
        ///     <para>The link</para>
        ///     <para></para>
        /// </returns>
        [Benchmark]
        public uint GetObjectFromObjectValueLinkBenchmark()
        {
            return _defaultJsonStorage.GetObject(_objectValueLink);
        }

        /// <summary>
        ///     <para>
        ///         Gets the object from object benchmark.
        ///     </para>
        ///     <para></para>
        /// </summary>
        /// <returns>
        ///     <para>The link</para>
        ///     <para></para>
        /// </returns>
        [Benchmark]
        public uint GetObjectFromObjectBenchmark()
        {
            return _defaultJsonStorage.GetObject(_object);
        }

        /// <summary>
        ///     <para>
        ///         Gets the object from document object value link without loop benchmark.
        ///     </para>
        ///     <para></para>
        /// </summary>
        /// <returns>
        ///     <para>The link</para>
        ///     <para></para>
        /// </returns>
        [Benchmark]
        public uint GetObjectFromDocumentObjectValueLinkWithoutLoopBenchmark()
        {
            return GetObjectWihoutLoop(_documentObjectValueLink);
        }

        /// <summary>
        ///     <para>
        ///         Gets the object from object value link without loop benchmark.
        ///     </para>
        ///     <para></para>
        /// </summary>
        /// <returns>
        ///     <para>The link</para>
        ///     <para></para>
        /// </returns>
        [Benchmark]
        public uint GetObjectFromObjectValueLinkWithoutLoopBenchmark()
        {
            return GetObjectWihoutLoop(_objectValueLink);
        }

        /// <summary>
        ///     <para>
        ///         Gets the object from object without loop benchmark.
        ///     </para>
        ///     <para></para>
        /// </summary>
        /// <returns>
        ///     <para>The link</para>
        ///     <para></para>
        /// </returns>
        [Benchmark]
        public uint GetObjectFromObjectWithoutLoopBenchmark()
        {
            return GetObjectWihoutLoop(_object);
        }
    }
}
