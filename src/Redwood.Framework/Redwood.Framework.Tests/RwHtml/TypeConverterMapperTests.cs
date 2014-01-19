using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redwood.Framework.RwHtml;

namespace Redwood.Framework.Tests.RwHtml
{
    [TestClass]
    public class TypeConverterMapperTests
    {

        [TestMethod]
        public void TypeConverterMapper_ConvertString()
        {
            var converter = new TypeConverterMapper().GetConverterForType(typeof (string));
            object result;
            var success = converter.TryConvertFromString("hello", out result);

            Assert.IsTrue(success);
            Assert.AreEqual("hello", (string)result);
        }
        
        [TestMethod]
        public void TypeConverterMapper_ConvertInt32()
        {
            var converter = new TypeConverterMapper().GetConverterForType(typeof(int));
            object result;
            var success = converter.TryConvertFromString("-1567", out result);

            Assert.IsTrue(success);
            Assert.AreEqual(-1567, (int)result);
        }

        [TestMethod]
        public void TypeConverterMapper_ConvertDateTime()
        {
            var converter = new TypeConverterMapper().GetConverterForType(typeof(DateTime));
            object result;
            var success = converter.TryConvertFromString("2000-01-05", out result);

            Assert.IsTrue(success);
            Assert.AreEqual(new DateTime(2000, 1, 5), (DateTime)result);
        }

        [TestMethod]
        public void TypeConverterMapper_ConvertDateTime_Invalid()
        {
            var converter = new TypeConverterMapper().GetConverterForType(typeof(DateTime));
            object result;
            var success = converter.TryConvertFromString("heureka", out result);

            Assert.IsFalse(success);
        }

        [TestMethod]
        public void TypeConverterMapper_ConvertEnumMember()
        {
            var converter = new TypeConverterMapper().GetConverterForType(typeof(TestEnum));
            object result;
            var success = converter.TryConvertFromString("deux", out result);
            
            Assert.IsTrue(success);
            Assert.AreEqual(TestEnum.Deux, (TestEnum)result);
        }

        [TestMethod]
        public void TypeConverterMapper_ConvertNullableInt32_Empty()
        {
            var converter = new TypeConverterMapper().GetConverterForType(typeof(int?));
            object result;
            var success = converter.TryConvertFromString("", out result);
            
            Assert.IsTrue(success);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TypeConverterMapper_ConvertNullableInt32_ValueSpecified()
        {
            var converter = new TypeConverterMapper().GetConverterForType(typeof(int?));
            object result;
            var success = converter.TryConvertFromString("15", out result);

            Assert.IsTrue(success);
            Assert.AreEqual(15, (int?)result);
        }


        [TestMethod]
        public void TypeConverterMapper_CustomConverter()
        {
            var converter = new TypeConverterMapper().GetConverterForType(typeof(TestStruct));
            object result;
            var success = converter.TryConvertFromString("15,16", out result);

            Assert.IsTrue(success);
            Assert.AreEqual(new TestStruct(15, 16), (TestStruct)result);
        }





        public enum TestEnum
        {
            Un,
            Deux,
            Trois
        }

        [RwHtmlMarkupConverter(typeof (TestStructRwHtmlMarkupConverter))]
        public struct TestStruct
        {
            public int X;
            public int Y;

            public TestStruct(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        public class TestStructRwHtmlMarkupConverter : RwHtmlMarkupConverter<TestStruct>
        {
            protected override bool TryConvertFromStringCore(string value, out object result)
            {
                var parts = value.Split(',');

                var output = new TestStruct();
                var success = parts.Length == 2 && int.TryParse(parts[0], out output.X) && int.TryParse(parts[1], out output.Y);

                result = output;
                return success;
            }
        }
    }
}
