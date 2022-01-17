using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Decoherence.CommandLineSerialization.Test
{
    public class SerializeObjectTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestSerializeBack()
        {
            var obj = new TestingClass5()
            {
                FieldA = 1,
                FieldB = new TestingClass4()
                {
                    FieldA = 11,
                    FieldB = new TestingClass1()
                    {
                        FieldA = 21,
                        FieldB = 22,
                    },
                },
            };

            CommandLineSerializer serializer = new();
            var commandLine = serializer.SerializeObject(obj);
            Console.WriteLine(commandLine);

            var obj2 = serializer.DeserializeObject(typeof(TestingClass5), commandLine, out var remainArgs);

            Assert.IsTrue(obj.Equals(obj2));
        }

        [Test]
        public void TestSerializeComplexList()
        {
            var list = new List<TestingClass4>
            {
                new() { FieldA = 1, FieldB = new TestingClass1() { FieldA = 11, FieldB = 12} },
                new() { FieldA = 2, FieldB = new TestingClass1() { FieldA = 21, FieldB = 22} },
            };

            CommandLineSerializer serializer = new();
            var argList = serializer.SerializeObject(list);
            var commandLine = ImplUtil.MergeCommandLine(argList);
            Console.WriteLine(commandLine);
            
            Assert.True(commandLine == "-- \"--FieldA 1 --FieldB \\\"--FieldA 11 --FieldB 12\\\"\" \"--FieldA 2 --FieldB \\\"--FieldA 21 --FieldB 22\\\"\"");
        }
    }
}