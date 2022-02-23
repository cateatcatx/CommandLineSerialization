using System.Collections.Generic;
using NUnit.Framework;

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
            var ret = _Invoke(nameof(TestingMethods.Foo2), "-c3 1 -b2 4", out var remainArgs);

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
        public void TestParamArrayViaArgument()
        {
            var ret = _Invoke(nameof(TestingMethods.Foo4), "1 2 3", out var remainArgs);

            var tmp = string.Join(' ', remainArgs);
            Assert.True(tmp == "", tmp);
            Assert.True(ret != null && (ret as string) == "1,2,3", (ret as string));
        }
        
        [Test]
        public void TestParamArrayViaShortOption()
        {
            var ret = _Invoke(nameof(TestingMethods.Foo5), "1 -b2 3", out var remainArgs);

            var tmp = string.Join(' ', remainArgs);
            Assert.True(tmp == "", tmp);
            Assert.True(ret != null && (ret as string) == "1,2,3", (ret as string));
        }
        
        [Test]
        public void TestParamArrayViaMultiShortOption()
        {
            var ret = _Invoke(nameof(TestingMethods.Foo7), "1 -b2 -b 3", out var remainArgs);

            var tmp = string.Join(' ', remainArgs);
            Assert.True(tmp == "", tmp);
            Assert.True(ret != null && (ret as string) == "1,2,3", (ret as string));
        }
        
        [Test]
        public void TestParamArrayViaLongOption()
        {
            var ret = _Invoke(nameof(TestingMethods.Foo6), "1 --bbb=2 3", out var remainArgs);

            var tmp = string.Join(' ', remainArgs);
            Assert.True(tmp == "", tmp);
            Assert.True(ret != null && (ret as string) == "1,2,3", (ret as string));
        }
        
        [Test]
        public void TestParamArrayViaMultiLongOption()
        {
            var ret = _Invoke(nameof(TestingMethods.Foo8), "1 --bbb=2 --bbb 3", out var remainArgs);

            var tmp = string.Join(' ', remainArgs);
            Assert.True(tmp == "", tmp);
            Assert.True(ret != null && (ret as string) == "1,2,3", (ret as string));
        }
        
        [Test]
        public void TestOptionObject()
        {
            var ret = _Invoke(nameof(TestingMethods.Foo9), new[] { "1", "-b--FieldA 1 --FieldB=--FieldA\\ 2\\ --FieldB\\ 3" }, out var remainArgs);

            var tmp = string.Join(' ', remainArgs);
            Assert.True(tmp == "", tmp);
            Assert.True(ret != null && (ret as string) == "1,1,2,3", (ret as string));
        }

        private object? _Invoke(string funName, string commandLine, out LinkedList<string> remainArgs)
        {
            return _Invoke(funName, commandLine.Split(' '), out remainArgs);
        }
        
        private object? _Invoke(string funName, IEnumerable<string> args, out LinkedList<string> remainArgs)
        {
            CommandLineSerializer serializer = new();
            
            return MethodInvoker.InvokeMethod(serializer, typeof(TestingMethods).GetMethod(funName)!, null, args, out remainArgs);
        }
    }
}