using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform.Data.Doublets.Json;

namespace Platform.Data.Doublets.Json.Tests
{
    public class JsonImporterTests
    {
        [Fact]
        public void ConstructorTest()
        {
            JsonImporter<Tlink> jsonImporter = new();
        }
    }
}
