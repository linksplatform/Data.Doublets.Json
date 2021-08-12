using System.Collections.Generic;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Json
{
    public interface IJsonStorage<TLink>
    {
        public ILinks<TLink> Links { get; }
        public TLink DocumentMarker { get; }
        public TLink ObjectMarker { get; }
        public TLink StringMarker { get; }
        public TLink EmptyStringMarker { get; }
        public TLink MemberMarker { get; }
        public TLink ValueMarker { get; }
        public TLink NumberMarker { get; }
        public TLink ArrayMarker { get; }
        public TLink EmptyArrayMarker { get; }
        public TLink TrueMarker { get; }
        public TLink FalseMarker { get; }
        public TLink NullMarker { get; }
        TLink CreateString(string content);
        TLink CreateStringValue(string content);
        TLink CreateNumber(decimal number);
        TLink CreateNumberValue(decimal number);
        TLink CreateBooleanValue(bool value);
        TLink CreateNullValue();
        TLink CreateDocument(string name);
        TLink GetDocumentOrDefault(string name);
        TLink CreateObject();
        TLink CreateObjectValue();
        TLink CreateArray(IList<TLink> array);
        TLink CreateArrayValue(IList<TLink> array) => CreateValue(CreateArray(array));
        TLink CreateArrayValue(TLink array) => CreateValue(array);
        TLink CreateMember(string name);
        TLink CreateValue(TLink value);
        TLink Attach(TLink source, TLink target);
        TLink AttachObject(TLink parent);
        TLink AttachString(TLink parent, string content);
        TLink AttachNumber(TLink parent, decimal number);
        TLink AttachBoolean(TLink parent, bool value);
        TLink AttachNull(TLink parent);
        TLink AttachArray(TLink parent, IList<TLink> array);
        TLink AttachMemberToObject(TLink @object, string keyName);
        TLink AppendArrayValue(TLink arrayValue, TLink appendant);
        string GetString(TLink stringValue);
        decimal GetNumber(TLink value);
        TLink GetObject(TLink objectValue);
        TLink GetArray(TLink arrayValueLink);
        TLink GetArraySequence(TLink array);
        TLink GetValueLink(TLink parent);
        TLink GetValueMarker(TLink link);
        List<TLink> GetMembersLinks(TLink @object);
    }
}