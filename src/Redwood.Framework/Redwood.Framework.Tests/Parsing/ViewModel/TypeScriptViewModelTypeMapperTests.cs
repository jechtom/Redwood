using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redwood.Framework.Parsing.ViewModel;

namespace Redwood.Framework.Tests.Parsing.ViewModel
{
    [TestClass]
    public class TypeScriptViewModelTypeMapperTests
    {

        /// <summary>
        /// Tests the handling of arrays.
        /// </summary>
        [TestMethod]
        public void ArrayTest()
        {
            var mapper = new TypeScriptViewModelTypeMapper();
            var result = mapper.MapType(typeof (int[]));
            Assert.AreEqual("number[]", result);
        }


        /// <summary>
        /// Tests the handling of collections.
        /// </summary>
        [TestMethod]
        public void IEnumerableTest()
        {
            var mapper = new TypeScriptViewModelTypeMapper();
            var result = mapper.MapType(typeof(List<string>));
            Assert.AreEqual("string[]", result);
        }

        /// <summary>
        /// Tests the custom type mapping with unique name generation for different types with same name.
        /// </summary>
        [TestMethod]
        public void CustomTypeTest()
        {
            var mapper = new TypeScriptViewModelTypeMapper();
            var result = mapper.MapType(typeof(C1.CustomType));
            var result2 = mapper.MapType(typeof(C2.CustomType));
            var result3 = mapper.MapType(typeof(C3.CustomType));
            Assert.AreEqual("CustomType", result);
            Assert.AreEqual("CustomType1", result2);
            Assert.AreEqual("CustomType2", result3);
        }


        public class C1
        {
            public class CustomType
            {
            }
        }
        public class C2
        {
            public class CustomType
            {
            }
        }
        public class C3
        {
            public class CustomType
            {
            }
        }
    }
}
