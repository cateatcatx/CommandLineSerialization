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
        public void TestFoo1()
        {
            var commandline = "-abc1 1 -ccc2 2";
            
            CommandLineDeserializer deserializer = new();
            var ret = deserializer.InvokeMethod(typeof(TestingMethods).GetMethod(nameof(TestingMethods.Foo1))!, commandline.Split(' '), out var remainArgs);
            
            Assert.True(string.Join(' ', remainArgs) == "-abc1 -ccc2");
            Assert.True(ret != null && (ret as string) == "1,2");
        }
    }
}