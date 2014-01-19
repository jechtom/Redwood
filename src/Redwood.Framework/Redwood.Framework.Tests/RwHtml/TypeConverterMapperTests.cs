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
            var mapper = new TypeConverterMapper();
            object result;
            var success = mapper.TryConvertFromString("hello", typeof (string), out result);

            Assert.IsTrue(success);
            Assert.AreEqual("hello", (string)result);
        }
        
        [TestMethod]
        public void TypeConverterMapper_ConvertInt32()
        {
            var mapper = new TypeConverterMapper();
            object result;
            var success = mapper.TryConvertFromString("-1567", typeof(int), out result);

            Assert.IsTrue(success);
            Assert.AreEqual(-1567, (int)result);
        }

        [TestMethod]
        public void TypeConverterMapper_ConvertDateTime()
        {
            var mapper = new TypeConverterMapper();
            object result;
            var success = mapper.TryConvertFromString("2000-01-05", typeof(DateTime), out result);

            Assert.IsTrue(success);
            Assert.AreEqual(new DateTime(2000, 1, 5), (DateTime)result);
        }

        [TestMethod]
        public void TypeConverterMapper_ConvertDateTime_Invalid()
        {
            var mapper = new TypeConverterMapper();
            object result;
            var success = mapper.TryConvertFromString("heureka", typeof(DateTime), out result);

            Assert.IsFalse(success);
        }

        [TestMethod]
        public void TypeConverterMapper_ConvertEnumMember()
        {
            var mapper = new TypeConverterMapper();
            object result;
            var success = mapper.TryConvertFromString("deux", typeof(TestEnum), out result);
            
            Assert.IsTrue(success);
            Assert.AreEqual(TestEnum.Deux, (TestEnum)result);
        }

        [TestMethod]
        public void TypeConverterMapper_ConvertNullableInt32_Empty()
        {
            var mapper = new TypeConverterMapper();
            object result;
            var success = mapper.TryConvertFromString("", typeof(int?), out result);
            
            Assert.IsTrue(success);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TypeConverterMapper_ConvertNullableInt32_ValueSpecified()
        {
            var mapper = new TypeConverterMapper();
            object result;
            var success = mapper.TryConvertFromString("15", typeof(int?), out result);

            Assert.IsTrue(success);
            Assert.AreEqual(15, (int?)result);
        }


        [TestMethod]
        public void TypeConverterMapper_CustomConverter()
        {
            var mapper = new TypeConverterMapper();
            object result;
            var success = mapper.TryConvertFromString("15,16", typeof(TestStruct), out result);

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
