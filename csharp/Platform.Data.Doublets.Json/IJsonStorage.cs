using System;
using System.Collections.Generic;
using System.Text;

namespace Platform.Data.Doublets.Json
{
    public interface IJsonStorage<TLink>
    {
        TLink CreateString(string content);
        TLink CreateNumber(TLink number);
        TLink CreateDocument(string name);
        TLink GetDocument(string name);
        TLink CreateObject();
        TLink CreateObjectValue();
        TLink CreateArray(IList<TLink> array);
        TLink CreateMember(string name);
        TLink CreateValue(TLink keyLink, string @string);
        TLink CreateValue(TLink keyLink, TLink @object);
        TLink CreateValue(TLink @object);
        TLink GetObject(TLink objectValue);
        TLink AttachObject(TLink parent);
        TLink AttachString(TLink parent, string content);
        TLink AttachNumber(TLink parent, TLink number);
        TLink AttachBoolean(TLink parent, bool value);
        TLink AttachNull(TLink parent);
        TLink AttachArray(TLink parent, TLink arrayLink);
        TLink AttachArray(TLink parent, IList<TLink> array);
        TLink AttachMemberToObject(TLink @object, string keyName);
        TLink Attach(TLink parent, TLink child);
        TLink GetValue(TLink parent);
        List<TLink> GetMembersLinks(TLink @object);
    }
}