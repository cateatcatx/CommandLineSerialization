using System.Collections.Generic;
using NUnit.Framework;
using Decoherence.CommandLineSerialization;
using System.Linq;

namespace Decoherence.CommandLineSerialization.Test
{
    public class MethodInvokeTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestNormal()
        {
            var commandline = "-abc1 1 -ccc2 2";
            
            CommandLineDeserializer deserializer = new();
            var ret = deserializer.InvokeMethod(typeof(TestingMethods).GetMethod(nameof(TestingMethods.Foo1))!, commandline.Split(' '), out var remainArgs);
            
            Assert.True(string.Join(' ', remainArgs) == "-abc1 -ccc2");
            Assert.True(ret != null && (ret as string) == "1,2");
        }
        
        [Test]
        public void TestAttr()
        {
            var commandline = "--ccc 3 1 -b2 4";
            
            CommandLineDeserializer deserializer = new();
            var ret = deserializer.InvokeMethod(typeof(TestingMethods).GetMethod(nameof(TestingMethods.Foo2))!, commandline.Split(' '), out var remainArgs);

            var tmp = string.Join(' ', remainArgs);
            Assert.True(tmp == "", tmp);
            Assert.True(ret != null && (ret as string) == "1,2,3,4");
        }
        
        [Test]
        public void TestUnInputDefaultParam()
        {
            var commandline = "1 -a123";
            
            CommandLineDeserializer deserializer = new();
            var ret = deserializer.InvokeMethod(typeof(TestingMethods).GetMethod(nameof(TestingMethods.Foo3))!, commandline.Split(' '), out var remainArgs);

            var tmp = string.Join(' ', remainArgs);
            Assert.True(tmp == "-a123", tmp);
            Assert.True(ret != null && (ret as string) == "1,2", (ret as string));
        }
        
        [Test]
        public void TestInputDefaultParm()
        {
            var commandline = "1 -a123 3 4";
            
            CommandLineDeserializer deserializer = new();
            var ret = deserializer.InvokeMethod(typeof(TestingMethods).GetMethod(nameof(TestingMethods.Foo3))!, commandline.Split(' '), out var remainArgs);

            var tmp = string.Join(' ', remainArgs);
            Assert.True(tmp == "-a123 4", tmp);
            Assert.True(ret != null && (ret as string) == "1,3", (ret as string));
        }
    }
}