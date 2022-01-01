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
            var ret = _Invoke(nameof(TestingMethods.Foo1), "-abc1 1 -ccc2 2", out var remainArgs);
            
            Assert.True(string.Join(' ', remainArgs) == "-abc1 -ccc2");
            Assert.True(ret != null && (ret as string) == "1,2");
        }
        
        [Test]
        public void TestAttr()
        {
            var ret = _Invoke(nameof(TestingMethods.Foo2), "--ccc 3 1 -b2 4", out var remainArgs);

            var tmp = string.Join(' ', remainArgs);
            Assert.True(tmp == "", tmp);
            Assert.True(ret != null && (ret as string) == "1,2,3,4");
        }
        
        [Test]
        public void TestUnInputDefaultParam()
        {
            var ret = _Invoke(nameof(TestingMethods.Foo3), "1 -a123", out var remainArgs);

            var tmp = string.Join(' ', remainArgs);
            Assert.True(tmp == "-a123", tmp);
            Assert.True(ret != null && (ret as string) == "1,2", (ret as string));
        }
        
        [Test]
        public void TestInputDefaultParm()
        {
            var ret = _Invoke(nameof(TestingMethods.Foo3), "1 -a123 3 4", out var remainArgs);

            var tmp = string.Join(' ', remainArgs);
            Assert.True(tmp == "-a123 4", tmp);
            Assert.True(ret != null && (ret as string) == "1,3", (ret as string));
        }
        
        [Test]
        public void TestParmArray()
        {
            var ret = _Invoke(nameof(TestingMethods.Foo4), "1 2 3", out var remainArgs);

            var tmp = string.Join(' ', remainArgs);
            Assert.True(tmp == "", tmp);
            Assert.True(ret != null && (ret as string) == "1,2,3", (ret as string));
        }

        private object? _Invoke(string funName, string commandline, out LinkedList<string> remainArgs)
        {
            CommandLineDeserializer deserializer = new();
            return deserializer.InvokeMethod(typeof(TestingMethods).GetMethod(funName)!, commandline.Split(' '), out remainArgs);
        }
    }
}