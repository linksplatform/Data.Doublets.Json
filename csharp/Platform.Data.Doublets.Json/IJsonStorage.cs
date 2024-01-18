using System.Collections.Generic;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Json
{
    /// <summary>
    /// <para>
    /// Defines the json storage.
    /// </para>
    /// <para></para>
    /// </summary>
    public interface IJsonStorage<TLinkAddress>
    {
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
        /// Gets the number type value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLinkAddress NumberType { get; }
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
        TLinkAddress CreateString(string content);
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
        TLinkAddress CreateStringValue(string content);
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
        TLinkAddress CreateNumber(decimal number);
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
        TLinkAddress CreateNumberValue(decimal number);
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
        TLinkAddress CreateBooleanValue(bool value);
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
        TLinkAddress CreateNullValue();
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
        TLinkAddress CreateDocument(string name);
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
        TLinkAddress GetDocumentOrDefault(string name);
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
        TLinkAddress CreateObject();
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
        TLinkAddress CreateObjectValue();
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
        TLinkAddress CreateArray(IList<TLinkAddress>? array);
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
        TLinkAddress CreateArrayValue(IList<TLinkAddress>? array) => CreateValue(CreateArray(array));
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
        TLinkAddress CreateArrayValue(TLinkAddress array) => CreateValue(array);
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
        TLinkAddress CreateMember(string name);
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
        TLinkAddress CreateValue(TLinkAddress value);
        /// <summary>
        /// <para>
        /// Attaches the source.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="source">
        /// <para>The source.</para>
        /// <para></para>
        /// </param>
        /// <param name="target">
        /// <para>The target.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLinkAddress Attach(TLinkAddress source, TLinkAddress target);
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
        TLinkAddress AttachObject(TLinkAddress parent);
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
        TLinkAddress AttachString(TLinkAddress parent, string content);
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
        TLinkAddress AttachNumber(TLinkAddress parent, decimal number);
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
        TLinkAddress AttachBoolean(TLinkAddress parent, bool value);
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
        TLinkAddress AttachNull(TLinkAddress parent);
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
        TLinkAddress AttachArray(TLinkAddress parent, IList<TLinkAddress>? array);
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
        TLinkAddress AttachMemberToObject(TLinkAddress @object, string keyName);
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
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLinkAddress AppendArrayValue(TLinkAddress arrayValue, TLinkAddress appendant);
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
        /// <returns>
        /// <para>The string</para>
        /// <para></para>
        /// </returns>
        string GetString(TLinkAddress stringValue);
        /// <summary>
        /// <para>
        /// Gets the number using the specified value.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="value">
        /// <para>The value.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The decimal</para>
        /// <para></para>
        /// </returns>
        decimal GetNumber(TLinkAddress value);
        /// <summary>
        /// <para>
        /// Gets the object using the specified object value.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="objectValue">
        /// <para>The object value.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLinkAddress GetObject(TLinkAddress objectValue);
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
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLinkAddress GetArray(TLinkAddress arrayValueLink);
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
        TLinkAddress GetArraySequence(TLinkAddress array);
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
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLinkAddress GetValueLink(TLinkAddress parent);
        /// <summary>
        /// <para>
        /// Gets the value type using the specified link.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="link">
        /// <para>The link.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLinkAddress GetValueType(TLinkAddress link);
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
        /// <para>A list of t link</para>
        /// <para></para>
        /// </returns>
        List<TLinkAddress> GetMembersLinks(TLinkAddress @object);
    }
}