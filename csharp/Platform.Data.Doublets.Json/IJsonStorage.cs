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
    public interface IJsonStorage<TLink>
    {
        /// <summary>
        /// <para>
        /// Gets the links value.
        /// </para>
        /// <para></para>
        /// </summary>
        public ILinks<TLink> Links { get; }
        /// <summary>
        /// <para>
        /// Gets the document marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLink DocumentMarker { get; }
        /// <summary>
        /// <para>
        /// Gets the object marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLink ObjectMarker { get; }
        /// <summary>
        /// <para>
        /// Gets the string marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLink StringMarker { get; }
        /// <summary>
        /// <para>
        /// Gets the empty string marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLink EmptyStringMarker { get; }
        /// <summary>
        /// <para>
        /// Gets the member marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLink MemberMarker { get; }
        /// <summary>
        /// <para>
        /// Gets the value marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLink ValueMarker { get; }
        /// <summary>
        /// <para>
        /// Gets the number marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLink NumberMarker { get; }
        /// <summary>
        /// <para>
        /// Gets the array marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLink ArrayMarker { get; }
        /// <summary>
        /// <para>
        /// Gets the empty array marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLink EmptyArrayMarker { get; }
        /// <summary>
        /// <para>
        /// Gets the true marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLink TrueMarker { get; }
        /// <summary>
        /// <para>
        /// Gets the false marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLink FalseMarker { get; }
        /// <summary>
        /// <para>
        /// Gets the null marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLink NullMarker { get; }
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
        TLink CreateString(string content);
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
        TLink CreateStringValue(string content);
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
        TLink CreateNumber(decimal number);
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
        TLink CreateNumberValue(decimal number);
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
        TLink CreateBooleanValue(bool value);
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
        TLink CreateNullValue();
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
        TLink CreateDocument(string name);
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
        TLink GetDocumentOrDefault(string name);
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
        TLink CreateObject();
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
        TLink CreateObjectValue();
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
        TLink CreateArray(IList<TLink> array);
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
        TLink CreateArrayValue(IList<TLink> array) => CreateValue(CreateArray(array));
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
        TLink CreateArrayValue(TLink array) => CreateValue(array);
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
        TLink CreateMember(string name);
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
        TLink CreateValue(TLink value);
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
        TLink Attach(TLink source, TLink target);
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
        TLink AttachObject(TLink parent);
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
        TLink AttachString(TLink parent, string content);
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
        TLink AttachNumber(TLink parent, decimal number);
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
        TLink AttachBoolean(TLink parent, bool value);
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
        TLink AttachNull(TLink parent);
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
        TLink AttachArray(TLink parent, IList<TLink> array);
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
        TLink AttachMemberToObject(TLink @object, string keyName);
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
        TLink AppendArrayValue(TLink arrayValue, TLink appendant);
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
        string GetString(TLink stringValue);
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
        decimal GetNumber(TLink value);
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
        TLink GetObject(TLink objectValue);
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
        TLink GetArray(TLink arrayValueLink);
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
        TLink GetArraySequence(TLink array);
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
        TLink GetValueLink(TLink parent);
        /// <summary>
        /// <para>
        /// Gets the value marker using the specified link.
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
        TLink GetValueMarker(TLink link);
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
        List<TLink> GetMembersLinks(TLink @object);
    }
}