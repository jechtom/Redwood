using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redwood.Framework.Hosting.ViewModel;
using Redwood.Framework.ViewModel;

namespace Redwood.Framework.Tests.Hosting.ViewModels
{
    [TestClass]
    public class ViewModelMapBuilderTests
    {

        [TestMethod]
        public void ViewModelMapBuilder_Primitive_Create()
        {
            var builder = new ViewModelMapBuilder();
            var map = builder.CreateMap(15);

            Assert.IsInstanceOfType(map, typeof(ViewModelMapPrimitiveNode));
            Assert.AreEqual(15, ((ViewModelMapPrimitiveNode)map).Value);
        }

        [TestMethod]
        public void ViewModelMapBuilder_Primitive_Update()
        {
            var builder = new ViewModelMapBuilder();
            var map = builder.CreateMap(15);
            builder.CreateOrUpdateMap(18, null, ref map);

            Assert.IsInstanceOfType(map, typeof(ViewModelMapPrimitiveNode));
            Assert.AreEqual(18, ((ViewModelMapPrimitiveNode)map).Value);
        }

        [TestMethod]
        public void ViewModelMapBuilder_Object_Create()
        {
            var builder = new ViewModelMapBuilder();
            var map = builder.CreateMap(new TestObject() { Prop1 = 15, Prop2 = "Hello" });

            Assert.IsInstanceOfType(map, typeof(ViewModelMapObjectNode));

            var obj1 = (ViewModelMapObjectNode)map;
            Assert.AreEqual(2, obj1.Properties.Count);
            Assert.AreEqual(15, ((ViewModelMapPrimitiveNode)obj1.Properties["Prop1"]).Value);
            Assert.AreEqual("Hello", ((ViewModelMapPrimitiveNode)obj1.Properties["Prop2"]).Value);
        }

        [TestMethod]
        public void ViewModelMapBuilder_Object_Update()
        {
            var builder = new ViewModelMapBuilder();
            var map = builder.CreateMap(new TestObject() { Prop1 = 15, Prop2 = "Hello" });
            builder.CreateOrUpdateMap(new TestObject2() { Prop1 = 10, Prop4 = new TestObject() { Prop1 = 10, Prop2 = "Help" } }, null, ref map);

            Assert.IsInstanceOfType(map, typeof(ViewModelMapObjectNode));

            var obj1 = (ViewModelMapObjectNode)map;
            Assert.AreEqual(3, obj1.Properties.Count);
            Assert.AreEqual(10, ((ViewModelMapPrimitiveNode)obj1.Properties["Prop1"]).Value);
            Assert.AreEqual("Hello", ((ViewModelMapPrimitiveNode)obj1.Properties["Prop2"]).Value);

            var obj2 = (ViewModelMapObjectNode)obj1.Properties["Prop4"];
            Assert.IsInstanceOfType(obj2, typeof(ViewModelMapObjectNode));
            Assert.AreEqual(10, ((ViewModelMapPrimitiveNode)obj2.Properties["Prop1"]).Value);
            Assert.AreEqual("Help", ((ViewModelMapPrimitiveNode)obj2.Properties["Prop2"]).Value);
            Assert.AreEqual(2, obj2.Properties.Count);
        }


        [TestMethod]
        public void ViewModelMapBuilder_Array_Create()
        {
            var builder = new ViewModelMapBuilder();
            var map = builder.CreateMap(new TestObject3() { Array = new[] { "aaa", "bbb", "ccc" } });

            Assert.IsInstanceOfType(map, typeof(ViewModelMapObjectNode));

            var obj1 = (ViewModelMapObjectNode)map;
            Assert.AreEqual(1, obj1.Properties.Count);
            Assert.IsInstanceOfType(obj1.Properties["Array"], typeof(ViewModelMapArrayNode));

            var obj2 = (ViewModelMapArrayNode)obj1.Properties["Array"];
            Assert.AreEqual(3, obj2.Items.Count);
            Assert.AreEqual(0, obj2.KeyMap.Count);

            Assert.AreEqual("aaa", ((ViewModelMapPrimitiveNode)obj2.Items[0]).Value);
            Assert.AreEqual("bbb", ((ViewModelMapPrimitiveNode)obj2.Items[1]).Value);
            Assert.AreEqual("ccc", ((ViewModelMapPrimitiveNode)obj2.Items[2]).Value);
        }

        [TestMethod]
        public void ViewModelMapBuilder_Array_UpdateWithReplace()
        {
            var builder = new ViewModelMapBuilder();
            var map = builder.CreateMap(new TestObject3() { Array = new[] { "ddd", "eee" } });
            builder.CreateOrUpdateMap(new TestObject3() { Array = new[] { "aaa", "bbb", "ccc" } }, null, ref map);

            Assert.IsInstanceOfType(map, typeof(ViewModelMapObjectNode));

            var obj1 = (ViewModelMapObjectNode)map;
            Assert.AreEqual(1, obj1.Properties.Count);
            Assert.IsInstanceOfType(obj1.Properties["Array"], typeof(ViewModelMapArrayNode));

            var obj2 = (ViewModelMapArrayNode)obj1.Properties["Array"];
            Assert.AreEqual(3, obj2.Items.Count);
            Assert.AreEqual(0, obj2.KeyMap.Count);

            Assert.AreEqual("aaa", ((ViewModelMapPrimitiveNode)obj2.Items[0]).Value);
            Assert.AreEqual("bbb", ((ViewModelMapPrimitiveNode)obj2.Items[1]).Value);
            Assert.AreEqual("ccc", ((ViewModelMapPrimitiveNode)obj2.Items[2]).Value);
        }

        [TestMethod]
        public void ViewModelMapBuilder_Array_UpdateWithSyncByKey_UsingKeyPropertyAttribute()
        {
            var builder = new ViewModelMapBuilder();
            var map = builder.CreateMap(new TestObject4()
            {
                Items = new[]
                {
                    new TestObjectExtended() { Prop1 = 1, Prop2 = "aaa", Prop3 = "xxx" },
                    new TestObject() { Prop1 = 2, Prop2 = "bbb" }
                }
            });
            builder.CreateOrUpdateMap(new TestObject4()
            {
                Items = new[]
                {
                    new TestObject() { Prop1 = 1, Prop2 = "ccc" },
                    new TestObject() { Prop1 = 3, Prop2 = "ddd" }
                }
            }, null, ref map);
            // KeyProperty is Prop1: item 1 update, item 2 removed, item 4 added

            Assert.IsInstanceOfType(map, typeof(ViewModelMapObjectNode));

            var obj1 = (ViewModelMapObjectNode)map;
            Assert.AreEqual(1, obj1.Properties.Count);
            Assert.IsInstanceOfType(obj1.Properties["Items"], typeof(ViewModelMapArrayNode));

            var obj2 = (ViewModelMapArrayNode)obj1.Properties["Items"];
            Assert.AreEqual(2, obj2.Items.Count);
            Assert.AreEqual(2, obj2.KeyMap.Count);
            Assert.AreEqual(obj2.Items[0], obj2.KeyMap["1"]);
            Assert.AreEqual(obj2.Items[1], obj2.KeyMap["3"]);

            var item1 = (ViewModelMapObjectNode)obj2.Items[0];
            Assert.AreEqual(1, ((ViewModelMapPrimitiveNode)item1.Properties["Prop1"]).Value);
            Assert.AreEqual("ccc", ((ViewModelMapPrimitiveNode)item1.Properties["Prop2"]).Value);
            Assert.AreEqual("xxx", ((ViewModelMapPrimitiveNode)item1.Properties["Prop3"]).Value);

            var item2 = (ViewModelMapObjectNode)obj2.Items[1];
            Assert.AreEqual(3, ((ViewModelMapPrimitiveNode)item2.Properties["Prop1"]).Value);
            Assert.AreEqual("ddd", ((ViewModelMapPrimitiveNode)item2.Properties["Prop2"]).Value);
        }

        [TestMethod]
        public void ViewModelMapBuilder_Array_UpdateWithSyncByKey_UsingKeyAttribute()
        {
            var builder = new ViewModelMapBuilder();
            var map = builder.CreateMap(new TestObject5()
            {
                Items = new[]
                {
                    new TestObjectExtended() { Prop1 = 1, Prop2 = "aaa", Prop3 = "xxx" },
                    new TestObject() { Prop1 = 2, Prop2 = "bbb" }
                }
            });
            builder.CreateOrUpdateMap(new TestObject5()
            {
                Items = new[]
                {
                    new TestObject() { Prop1 = 1, Prop2 = "ccc" },
                    new TestObject() { Prop1 = 3, Prop2 = "ddd" }
                }
            }, null, ref map);
            // KeyProperty is Prop1: item 1 update, item 2 removed, item 4 added

            Assert.IsInstanceOfType(map, typeof(ViewModelMapObjectNode));

            var obj1 = (ViewModelMapObjectNode)map;
            Assert.AreEqual(1, obj1.Properties.Count);
            Assert.IsInstanceOfType(obj1.Properties["Items"], typeof(ViewModelMapArrayNode));

            var obj2 = (ViewModelMapArrayNode)obj1.Properties["Items"];
            Assert.AreEqual(2, obj2.Items.Count);
            Assert.AreEqual(2, obj2.KeyMap.Count);
            Assert.AreEqual(obj2.Items[0], obj2.KeyMap["1"]);
            Assert.AreEqual(obj2.Items[1], obj2.KeyMap["3"]);

            var item1 = (ViewModelMapObjectNode)obj2.Items[0];
            Assert.AreEqual(1, ((ViewModelMapPrimitiveNode)item1.Properties["Prop1"]).Value);
            Assert.AreEqual("ccc", ((ViewModelMapPrimitiveNode)item1.Properties["Prop2"]).Value);
            Assert.AreEqual("xxx", ((ViewModelMapPrimitiveNode)item1.Properties["Prop3"]).Value);

            var item2 = (ViewModelMapObjectNode)obj2.Items[1];
            Assert.AreEqual(3, ((ViewModelMapPrimitiveNode)item2.Properties["Prop1"]).Value);
            Assert.AreEqual("ddd", ((ViewModelMapPrimitiveNode)item2.Properties["Prop2"]).Value);
        }


        public class TestObject
        {
            [Key]
            public int Prop1 { get; set; }

            public string Prop2 { get; set; }

        }

        public class TestObjectExtended : TestObject
        {

            public string Prop3 { get; set; }

        }

        public class TestObject2
        {

            public int Prop1 { get; set; }

            public TestObject Prop4 { get; set; }
        }

        public class TestObject3
        {

            public string[] Array { get; set; }
        }

        public class TestObject4
        {

            [KeyProperty("Prop1")]
            public TestObject[] Items { get; set; }
        }

        public class TestObject5
        {

            [KeyProperty("Prop1")]
            public TestObject[] Items { get; set; }
        }
    }


}
