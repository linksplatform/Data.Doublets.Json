using System;
using System.Collections.Generic;
using System.Text;

namespace Platform.Data.Doublets.Json
{
    public interface IJsonStorage<TLink>
    {
        TLink CreateDocument(string name);
        TLink CreateMember(string key);
        TLink CreateValue(TLink keyLink, string value);
        TLink CreateString(string content);
        TLink AttachObject(TLink parent);
        TLink Attach(TLink parent, TLink child);
    }
}