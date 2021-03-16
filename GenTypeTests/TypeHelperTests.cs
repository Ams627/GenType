using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace GenType.Tests
{
    [TestClass()]
    public class TypeHelperTests
    {
        [TestMethod()]
        public void GetTypeFromStringTest()
        {
            var typenames = new [] { "byte", "short", "int",  "long", "float", "double", "decimal", "char", "string" };
            var mapped = typenames.Select(TypeHelper.GetTypeFromString);
            var expected = new[] { typeof(byte), typeof(short), typeof(int), typeof(uint), typeof(long), typeof(float), typeof(double), typeof(decimal), typeof(char), typeof(string) };
            mapped.Should().HaveCount(expected.Count()).And.ContainInOrder(expected);
        }

        [TestMethod()]
        public void GetParserTest()
        {
            var typenames = new[] { "string", "abcdef12345", "byte", "short", "int", "long", "float", "double", "decimal", "char"};

            var mapped = typenames.Select(TypeHelper.GetParser).ToList();

            mapped[0].Should().BeNull(); // there is no string.Parse(...)
            mapped[1].Should().BeNull();
            foreach (var m in mapped.Skip(2))
            {
                m.Should().NotBeNull();
            }

            mapped[2].Invoke("12").Should().Be(12);
            mapped[3].Invoke("12345").Should().Be(12345);
            mapped[4].Invoke("123456789").Should().Be(123456789);
            mapped[5].Invoke("123456789012345").Should().Be(123456789012345);
            mapped[6].Invoke("12.3").Should().Be(12.3F);
            mapped[7].Invoke("123.456").Should().Be(123.456);
            mapped[8].Invoke("1234567.89").Should().Be(1234567.89);
            mapped[9].Invoke("A").Should().Be('A');
        }
    }
}