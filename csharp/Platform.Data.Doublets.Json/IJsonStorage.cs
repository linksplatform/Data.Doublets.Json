using System;
using System.Collections.Generic;
using System.Text;

namespace Platform.Data.Doublets.Json
{
    public interface IJsonStorage<TLink>
    {
        TLink CreateDocument(string name);
        TLink CreateKey(TLink objectLink, string key);
        TLink CreateValue(TLink keyLink, string value);
        TLink CreateString(string content);
    }
}