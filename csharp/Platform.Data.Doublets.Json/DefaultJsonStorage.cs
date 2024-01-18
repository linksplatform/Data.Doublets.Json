using Platform.Numbers;
using Platform.Data.Doublets.Unicode;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Data.Doublets.CriterionMatchers;
using Platform.Data.Numbers.Raw;
using Platform.Converters;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.Collections.Stacks;
using System;
using System.Collections.Generic;
using Platform.Data.Doublets.Numbers.Rational;
using Platform.Data.Doublets.Numbers.Raw;
using Platform.Data.Doublets.Sequences.HeightProviders;
using Platform.Data.Doublets.Sequences;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Json
{
    /// <summary>
    /// <para>
    /// Represents the default json storage.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="IJsonStorage{TLinkAddress}"/>
    public class DefaultJsonStorage<TLinkAddress> : IJsonStorage<TLinkAddress>
        where TLinkAddress : struct
    {
        /// <summary>
        /// <para>
        /// The any.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly TLinkAddress Any;
        /// <summary>
        /// <para>
        /// The zero.
        /// </para>
        /// <para></para>
        /// </summary>
        public static readonly TLinkAddress Zero = default;
        /// <summary>
        /// <para>
        /// The zero.
        /// </para>
        /// <para></para>
        /// </summary>
        public static readonly TLinkAddress One = Arithmetic.Increment(Zero);
        /// <summary>
        /// <para>
        /// The balanced variant converter.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly BalancedVariantConverter<TLinkAddress> BalancedVariantConverter;
        /// <summary>
        /// <para>
        /// The list to sequence converter.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly IConverter<IList<TLinkAddress>?, TLinkAddress> ListToSequenceConverter;
        /// <summary>
        /// <para>
        /// The meaning root.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly TLinkAddress Type;
        /// <summary>
        /// <para>
        /// The default.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly EqualityComparer<TLinkAddress> EqualityComparer = EqualityComparer<TLinkAddress>.Default;
        // Converters that are able to convert link's address (UInt64 value) to a raw number represented with another UInt64 value and back
        /// <summary>
        /// <para>
        /// The number to address converter.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly RawNumberToAddressConverter<TLinkAddress> NumberToAddressConverter = new();
        /// <summary>
        /// <para>
        /// The address to number converter.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly AddressToRawNumberConverter<TLinkAddress> AddressToNumberConverter = new();
        // Converters between BigInteger and raw number sequence
        /// <summary>
        /// <para>
        /// The big integer to raw number sequence converter.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly BigIntegerToRawNumberSequenceConverter<TLinkAddress> BigIntegerToRawNumberSequenceConverter;
        /// <summary>
        /// <para>
        /// The raw number sequence to big integer converter.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly RawNumberSequenceToBigIntegerConverter<TLinkAddress> RawNumberSequenceToBigIntegerConverter;
        // Converters between decimal and rational number sequence
        /// <summary>
        /// <para>
        /// The decimal to rational converter.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly DecimalToRationalConverter<TLinkAddress> DecimalToRationalConverter;
        /// <summary>
        /// <para>
        /// The rational to decimal converter.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly RationalToDecimalConverter<TLinkAddress> RationalToDecimalConverter;
        // Converters between string and unicode sequence
        /// <summary>
        /// <para>
        /// The string to unicode sequence converter.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly IConverter<string, TLinkAddress> StringToUnicodeSequenceConverter;
        /// <summary>
        /// <para>
        /// The unicode sequence to string converter.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly IConverter<TLinkAddress, string> UnicodeSequenceToStringConverter;
        // For sequences
        /// <summary>
        /// <para>
        /// The json array element criterion matcher.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly JsonArrayElementCriterionMatcher<TLinkAddress> JsonArrayElementCriterionMatcher;
        /// <summary>
        /// <para>
        /// The default sequence right height provider.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly DefaultSequenceRightHeightProvider<TLinkAddress> DefaultSequenceRightHeightProvider;
        /// <summary>
        /// <para>
        /// The default sequence appender.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly DefaultSequenceAppender<TLinkAddress> DefaultSequenceAppender;
        /// <summary>
        /// <para>
        /// Gets the links value.
        /// </para>
        /// <para></para>
        /// </summary>
        public ILinks<TLinkAddress> Links { get; }
        /// <summary>
        /// <para>
        /// Gets the document type value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLinkAddress DocumentType { get; }
        /// <summary>
        /// <para>
        /// Gets the object type value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLinkAddress ObjectType { get; }
        /// <summary>
        /// <para>
        /// Gets the member type value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLinkAddress MemberType { get; }
        /// <summary>
        /// <para>
        /// Gets the value type value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLinkAddress ValueType { get; }
        /// <summary>
        /// <para>
        /// Gets the string type value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLinkAddress StringType { get; }
        /// <summary>
        /// <para>
        /// Gets the empty string type value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLinkAddress EmptyStringType { get; }
        /// <summary>
        /// <para>
        /// Gets the number type value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLinkAddress NumberType { get; }
        /// <summary>
        /// <para>
        /// Gets the negative number type value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLinkAddress NegativeNumberType { get; }
        /// <summary>
        /// <para>
        /// Gets the array type value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLinkAddress ArrayType { get; }
        /// <summary>
        /// <para>
        /// Gets the empty array type value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLinkAddress EmptyArrayType { get; }
        /// <summary>
        /// <para>
        /// Gets the true type value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLinkAddress TrueType { get; }
        /// <summary>
        /// <para>
        /// Gets the false type value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLinkAddress FalseType { get; }
        /// <summary>
        /// <para>
        /// Gets the null type value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLinkAddress NullType { get; }

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="DefaultJsonStorage"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>A links.</para>
        /// <para></para>
        /// </param>
        /// <param name="listToSequenceConverter">
        /// <para>A list to sequence converter.</para>
        /// <para></para>
        /// </param>
        public DefaultJsonStorage(ILinks<TLinkAddress> links, IConverter<IList<TLinkAddress>?, TLinkAddress> listToSequenceConverter)
        {
            Links = links;
            ListToSequenceConverter = listToSequenceConverter;
            // Initializes constants
            Any = Links.Constants.Any;
            var typeAddress = One;
            Type = links.GetOrCreate(typeAddress, typeAddress);
            var unicodeSymbolType = links.GetOrCreate(Type, Arithmetic.Increment(ref typeAddress));
            var unicodeSequenceType = links.GetOrCreate(Type, Arithmetic.Increment(ref typeAddress));
            DocumentType = links.GetOrCreate(Type, Arithmetic.Increment(ref typeAddress));
            ObjectType = links.GetOrCreate(Type, Arithmetic.Increment(ref typeAddress));
            MemberType = links.GetOrCreate(Type, Arithmetic.Increment(ref typeAddress));
            ValueType = links.GetOrCreate(Type, Arithmetic.Increment(ref typeAddress));
            StringType = links.GetOrCreate(Type, Arithmetic.Increment(ref typeAddress));
            EmptyStringType = links.GetOrCreate(Type, Arithmetic.Increment(ref typeAddress));
            NumberType = links.GetOrCreate(Type, Arithmetic.Increment(ref typeAddress));
            NegativeNumberType = links.GetOrCreate(Type, Arithmetic.Increment(ref typeAddress));
            ArrayType = links.GetOrCreate(Type, Arithmetic.Increment(ref typeAddress));
            EmptyArrayType = links.GetOrCreate(Type, Arithmetic.Increment(ref typeAddress));
            TrueType = links.GetOrCreate(Type, Arithmetic.Increment(ref typeAddress));
            FalseType = links.GetOrCreate(Type, Arithmetic.Increment(ref typeAddress));
            NullType = links.GetOrCreate(Type, Arithmetic.Increment(ref typeAddress));
            BalancedVariantConverter = new(links);
            TargetMatcher<TLinkAddress> unicodeSymbolCriterionMatcher = new(Links, unicodeSymbolType);
            TargetMatcher<TLinkAddress> unicodeSequenceCriterionMatcher = new(Links, unicodeSequenceType);
            CharToUnicodeSymbolConverter<TLinkAddress> charToUnicodeSymbolConverter =
                new(Links, AddressToNumberConverter, unicodeSymbolType);
            UnicodeSymbolToCharConverter<TLinkAddress> unicodeSymbolToCharConverter =
                new(Links, NumberToAddressConverter, unicodeSymbolCriterionMatcher);
            StringToUnicodeSequenceConverter = new CachingConverterDecorator<string, TLinkAddress>(
                new StringToUnicodeSequenceConverter<TLinkAddress>(Links, charToUnicodeSymbolConverter,
                    BalancedVariantConverter, unicodeSequenceType));
            RightSequenceWalker<TLinkAddress> sequenceWalker =
                new(Links, new DefaultStack<TLinkAddress>(), unicodeSymbolCriterionMatcher.IsMatched);
            UnicodeSequenceToStringConverter = new CachingConverterDecorator<TLinkAddress, string>(
                new UnicodeSequenceToStringConverter<TLinkAddress>(Links, unicodeSequenceCriterionMatcher, sequenceWalker,
                    unicodeSymbolToCharConverter));
            BigIntegerToRawNumberSequenceConverter =
                new(links, AddressToNumberConverter, ListToSequenceConverter, NegativeNumberType);
            RawNumberSequenceToBigIntegerConverter = new(links, NumberToAddressConverter, NegativeNumberType);
            DecimalToRationalConverter = new(links, BigIntegerToRawNumberSequenceConverter);
            RationalToDecimalConverter = new(links, RawNumberSequenceToBigIntegerConverter);
            JsonArrayElementCriterionMatcher = new(this);
            DefaultSequenceRightHeightProvider = new(Links, JsonArrayElementCriterionMatcher);
            DefaultSequenceAppender = new(Links, new DefaultStack<TLinkAddress>(), DefaultSequenceRightHeightProvider);
        }
        
        /// <summary>
        /// <para>
        /// Creates the string using the specified content.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="content">
        /// <para>The content.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress CreateString(string content)
        {
            var @string = GetStringSequence(content);
            return Links.GetOrCreate(StringType, @string);
        }

        /// <summary>
        /// <para>
        /// Creates the string value using the specified content.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="content">
        /// <para>The content.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress CreateStringValue(string content)
        {
            var @string = CreateString(content);
            return CreateValue(@string);
        }

        /// <summary>
        /// <para>
        /// Creates the number using the specified number.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="number">
        /// <para>The number.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress CreateNumber(decimal number)
        {
            var numberSequence = DecimalToRationalConverter.Convert(number);
            return Links.GetOrCreate(NumberType, numberSequence);
        }

        /// <summary>
        /// <para>
        /// Creates the number value using the specified number.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="number">
        /// <para>The number.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress CreateNumberValue(decimal number)
        {
            var numberLink = CreateNumber(number);
            return CreateValue(numberLink);
        }

        /// <summary>
        /// <para>
        /// Creates the boolean value using the specified value.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="value">
        /// <para>The value.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress CreateBooleanValue(bool value) => CreateValue(value ? TrueType : FalseType);

        /// <summary>
        /// <para>
        /// Creates the null value.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress CreateNullValue() => CreateValue(NullType);

        /// <summary>
        /// <para>
        /// Creates the document using the specified name.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="name">
        /// <para>The name.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress CreateDocument(string name)
        {
            var documentName = CreateString(name);
            return Links.GetOrCreate(DocumentType, documentName);
        }

        /// <summary>
        /// <para>
        /// Creates the object.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress CreateObject()
        {
            var @object = Links.Create();
            return Links.Update(@object, newSource: ObjectType, newTarget: @object);
        }

        /// <summary>
        /// <para>
        /// Creates the object value.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress CreateObjectValue()
        {
            var @object = CreateObject();
            return CreateValue(@object);
        }

        /// <summary>
        /// <para>
        /// Creates the array using the specified array.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="array">
        /// <para>The array.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress CreateArray(IList<TLinkAddress>? array)
        {
            var arraySequence = array.Count == 0 ? EmptyArrayType : BalancedVariantConverter.Convert(array);
            return CreateArray(arraySequence);
        }

        /// <summary>
        /// <para>
        /// Creates the array using the specified sequence.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="sequence">
        /// <para>The sequence.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress CreateArray(TLinkAddress sequence) => Links.GetOrCreate(ArrayType, sequence);

        /// <summary>
        /// <para>
        /// Creates the array value using the specified array.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="array">
        /// <para>The array.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress CreateArrayValue(IList<TLinkAddress>? array)
        {
            var arrayLink = CreateArray(array);
            return CreateValue(arrayLink);
        }

        /// <summary>
        /// <para>
        /// Creates the array value using the specified sequence.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="sequence">
        /// <para>The sequence.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress CreateArrayValue(TLinkAddress sequence)
        {
            var array = CreateArray(sequence);
            return CreateValue(array);
        }

        /// <summary>
        /// <para>
        /// Creates the member using the specified name.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="name">
        /// <para>The name.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress CreateMember(string name)
        {
            var nameLink = CreateString(name);
            return Links.GetOrCreate(MemberType, nameLink);
        }

        /// <summary>
        /// <para>
        /// Creates the value using the specified value.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="value">
        /// <para>The value.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress CreateValue(TLinkAddress value) => Links.GetOrCreate(ValueType, value);

        /// <summary>
        /// <para>
        /// Attaches the object using the specified parent.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="parent">
        /// <para>The parent.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress AttachObject(TLinkAddress parent) => Attach(parent, CreateObjectValue());

        /// <summary>
        /// <para>
        /// Attaches the string using the specified parent.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="parent">
        /// <para>The parent.</para>
        /// <para></para>
        /// </param>
        /// <param name="content">
        /// <para>The content.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress AttachString(TLinkAddress parent, string content)
        {
            var @string = CreateString(content);
            var stringValue = CreateValue(@string);
            return Attach(parent, stringValue);
        }

        /// <summary>
        /// <para>
        /// Attaches the number using the specified parent.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="parent">
        /// <para>The parent.</para>
        /// <para></para>
        /// </param>
        /// <param name="number">
        /// <para>The number.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress AttachNumber(TLinkAddress parent, decimal number)
        {
            var numberLink = CreateNumber(number);
            var numberValue = CreateValue(numberLink);
            return Attach(parent, numberValue);
        }

        /// <summary>
        /// <para>
        /// Attaches the boolean using the specified parent.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="parent">
        /// <para>The parent.</para>
        /// <para></para>
        /// </param>
        /// <param name="value">
        /// <para>The value.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress AttachBoolean(TLinkAddress parent, bool value)
        {
            var booleanValue = CreateBooleanValue(value);
            return Attach(parent, booleanValue);
        }

        /// <summary>
        /// <para>
        /// Attaches the null using the specified parent.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="parent">
        /// <para>The parent.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress AttachNull(TLinkAddress parent)
        {
            var nullValue = CreateNullValue();
            return Attach(parent, nullValue);
        }

        /// <summary>
        /// <para>
        /// Attaches the array using the specified parent.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="parent">
        /// <para>The parent.</para>
        /// <para></para>
        /// </param>
        /// <param name="array">
        /// <para>The array.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress AttachArray(TLinkAddress parent, IList<TLinkAddress>? array)
        {
            var arrayValue = CreateArrayValue(array);
            return Attach(parent, arrayValue);
        }

        /// <summary>
        /// <para>
        /// Attaches the member to object using the specified object.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="@object">
        /// <para>The object.</para>
        /// <para></para>
        /// </param>
        /// <param name="keyName">
        /// <para>The key name.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress AttachMemberToObject(TLinkAddress @object, string keyName)
        {
            var member = CreateMember(keyName); 
            return Attach(@object, member);
        }

        /// <summary>
        /// <para>
        /// Attaches the parent.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="parent">
        /// <para>The parent.</para>
        /// <para></para>
        /// </param>
        /// <param name="child">
        /// <para>The child.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress Attach(TLinkAddress parent, TLinkAddress child) => Links.GetOrCreate(parent, child);

        /// <summary>
        /// <para>
        /// Appends the array value using the specified array value.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="arrayValue">
        /// <para>The array value.</para>
        /// <para></para>
        /// </param>
        /// <param name="appendant">
        /// <para>The appendant.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The new array value.</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress AppendArrayValue(TLinkAddress arrayValue, TLinkAddress appendant)
        {
            var array = GetArray(arrayValue);
            var arraySequence = Links.GetTarget(array);
            TLinkAddress newArrayValue;
            if (EqualityComparer.Equals(arraySequence, EmptyArrayType))
            {
                newArrayValue = CreateArrayValue(appendant);
            }
            else
            {
                arraySequence = DefaultSequenceAppender.Append(arraySequence, appendant);
                newArrayValue = CreateArrayValue(arraySequence);
            }
            return newArrayValue;
        }

        /// <summary>
        /// <para>
        /// Gets the document or default using the specified name.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="name">
        /// <para>The name.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress GetDocumentOrDefault(string name)
        {
            var stringSequence = GetStringSequence(name);
            var @string = Links.SearchOrDefault(StringType, stringSequence);
            if (EqualityComparer.Equals(@string, default))
            {
                return default;
            }
            return Links.SearchOrDefault(DocumentType, @string);
        }
        private TLinkAddress GetStringSequence(string content) => content == "" ? EmptyStringType : StringToUnicodeSequenceConverter.Convert(content);
        
        /// <summary>
        /// <para>
        /// Gets the string using the specified string value.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="stringValue">
        /// <para>The string value.</para>
        /// <para></para>
        /// </param>
        /// <exception cref="Exception">
        /// <para>The passed link does not contain a string.</para>
        /// <para></para>
        /// </exception>
        /// <returns>
        /// <para>The string</para>
        /// <para></para>
        /// </returns>
        public string GetString(TLinkAddress stringValue)
        {
            var current = stringValue;
            TLinkAddress source;
            for (int i = 0; i < 3; i++)
            {
                source = Links.GetSource(current);
                if (EqualityComparer.Equals(source, StringType))
                {
                    var sequence = Links.GetTarget(current);
                    var isEmpty = EqualityComparer.Equals(sequence, EmptyStringType);
                    return isEmpty ? "" : UnicodeSequenceToStringConverter.Convert(sequence);
                }
                current = Links.GetTarget(current);
            }
            throw new Exception("The passed link does not contain a string.");
        }

        /// <summary>
        /// <para>
        /// Gets the number using the specified value link.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="valueLink">
        /// <para>The value link.</para>
        /// <para></para>
        /// </param>
        /// <exception cref="Exception">
        /// <para>The passed link does not contain a number.</para>
        /// <para></para>
        /// </exception>
        /// <returns>
        /// <para>The decimal</para>
        /// <para></para>
        /// </returns>
        public decimal GetNumber(TLinkAddress valueLink)
        {
            var current = valueLink;
            TLinkAddress source;
            TLinkAddress target;
            for (int i = 0; i < 3; i++)
            {
                source = Links.GetSource(current);
                target = Links.GetTarget(current);
                if (EqualityComparer.Equals(source, NumberType))
                {
                    return RationalToDecimalConverter.Convert(target);
                }
                current = target;
            }
            throw new Exception("The passed link does not contain a number.");
        }


        /// <summary>
        /// <para>
        /// Gets the object using the specified object value link.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="objectValueLink">
        /// <para>The object value link.</para>
        /// <para></para>
        /// </param>
        /// <exception cref="Exception">
        /// <para>The passed link does not contain an object.</para>
        /// <para></para>
        /// </exception>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress GetObject(TLinkAddress objectValueLink)
        {
            var current = objectValueLink;
            TLinkAddress source;
            for (int i = 0; i < 3; i++)
            {
                source = Links.GetSource(current);
                if (EqualityComparer.Equals(source, ObjectType))
                {
                    return current;
                }
                current = Links.GetTarget(current);
            }
            throw new Exception("The passed link does not contain an object.");
        }

        /// <summary>
        /// <para>
        /// Gets the array using the specified array value link.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="arrayValueLink">
        /// <para>The array value link.</para>
        /// <para></para>
        /// </param>
        /// <exception cref="Exception">
        /// <para>The passed link does not contain an array.</para>
        /// <para></para>
        /// </exception>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress GetArray(TLinkAddress arrayValueLink)
        {
            var current = arrayValueLink;
            TLinkAddress source;
            for (int i = 0; i < 3; i++)
            {
                source = Links.GetSource(current);
                if (EqualityComparer.Equals(source, ArrayType))
                {
                    return current;
                }
                current = Links.GetTarget(current);
            }
            throw new Exception("The passed link does not contain an array.");
        }

        /// <summary>
        /// <para>
        /// Gets the array sequence using the specified array.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="array">
        /// <para>The array.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress GetArraySequence(TLinkAddress array) => Links.GetTarget(array);

        /// <summary>
        /// <para>
        /// Gets the value link using the specified parent.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="parent">
        /// <para>The parent.</para>
        /// <para></para>
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// <para>More than 1 value found.</para>
        /// <para></para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>The list elements length is negative.</para>
        /// <para></para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>The passed link is not a value.</para>
        /// <para></para>
        /// </exception>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress GetValueLink(TLinkAddress parent)
        {
            var query = new Link<TLinkAddress>(index: Any, source: parent, target: Any);
            var resultLinks = Links.All(query);
            switch (resultLinks.Count)
            {
                case 0:
                    return default;
                case 1:
                    var resultLinkTarget = Links.GetTarget(resultLinks[0]);
                    if (EqualityComparer.Equals(Links.GetSource(resultLinkTarget), ValueType))
                    {
                        return resultLinkTarget;
                    }
                    else
                    {
                        throw new InvalidOperationException("The passed link is not a value.");
                    }
                case > 1:
                    throw new InvalidOperationException("More than 1 value found.");
                default:
                    throw new InvalidOperationException("The list elements length is negative.");
            }
        }

        /// <summary>
        /// <para>
        /// Gets the value type using the specified value.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="value">
        /// <para>The value.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The target source.</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress GetValueType(TLinkAddress value)
        {
            var target = Links.GetTarget(value);
            var targetSource = Links.GetSource(target);
            if (EqualityComparer.Equals(Type, targetSource))
            {
                return target;
            }
            return targetSource;
        }

        /// <summary>
        /// <para>
        /// Gets the members links using the specified object.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="@object">
        /// <para>The object.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The members.</para>
        /// <para></para>
        /// </returns>
        public List<TLinkAddress> GetMembersLinks(TLinkAddress @object)
        {
            Link<TLinkAddress> query = new(index: Any, source: @object, target: Any);
            List<TLinkAddress> members = new();
            Links.Each(objectMemberLink =>
            {
                var memberLink = Links.GetTarget(objectMemberLink);
                var memberType = Links.GetSource(memberLink);
                if (EqualityComparer.Equals(memberType, MemberType))
                {
                    members.Add(Links.GetIndex(objectMemberLink));
                }
                return Links.Constants.Continue;
            }, query);
            return members;
        }
    }
}



