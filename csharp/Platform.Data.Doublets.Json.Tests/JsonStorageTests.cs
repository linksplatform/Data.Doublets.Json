using System;
using Xunit;

namespace Platform.Data.Doublets.Json.Tests
{
    public static class JsonStorageTests
    {
        [Fact]
        public static void ConstructorsTest()
        {
           
        }

        [Fact]
        public static void ContainsTest()
        {
            var range = new Range<int>(1, 3);
            Assert.True(range.Contains(1));
            Assert.True(range.Contains(2));
            Assert.True(range.Contains(3));
            Assert.True(range.Contains((2, 3)));
            Assert.False(range.Contains((3, 4)));
        }

        [Fact]
        public static void DifferenceTest()
        {
            var range = new Range<int>(1, 3);
            Assert.Equal(2, range.Difference());
        }

        [Fact]
        public static void ToStringTest()
        {
            var range = new Range<int>(1, 3);
            Assert.Equal("[1, 3]", range.ToString());
        }

        [Fact]
        public static void EqualityTest()
        {
            var range1 = new Range<int>(1, 3);
            var range1Duplicate = new Range<int>(1, 3);
            var range2 = new Range<int>(2, 5);
            Assert.True(range1 == range1Duplicate);
            Assert.Equal(range1, range1Duplicate);
            Assert.True(range1 != range2);
            Assert.NotEqual(range1, range2);
        }
    }
}