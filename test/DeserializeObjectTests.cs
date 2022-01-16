﻿using System.Collections.Generic;
using NUnit.Framework;

namespace Decoherence.CommandLineSerialization.Test
{
    public class DeserializeObjectTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestFields()
        {
            var obj = _Deserialize<TestingClass1>("--FieldA 1 --FieldB 2 3", out var remainArgs);
            
            Assert.True(obj.FieldA == 1 && obj.FieldB == 2);
            Assert.True(string.Join(' ', remainArgs) == "3");
        }
        
        [Test]
        public void TestConstructorFields()
        {
            var obj = _Deserialize<TestingClass2>("1 --FieldB 2 3", out var remainArgs);
            
            Assert.True(obj.FieldA == 1 && obj.FieldB == 2);
            Assert.True(string.Join(' ', remainArgs) == "3");
        }
        
        [Test]
        public void TestArgumentFields()
        {
            var obj = _Deserialize<TestingClass3>("--FieldC 3 2 1 3", out var remainArgs);
            
            Assert.True(obj.FieldA == 1 && obj.FieldB == 2 && obj.FieldC == 3);
            Assert.True(string.Join(' ', remainArgs) == "3");
        }

        private T _Deserialize<T>(string commandLine, out LinkedList<string> remainArgs)
        {
            CommandLineSerializer serializer = new();
            BuiltinValueSerializer valueSerializer = new BuiltinValueSerializer();
            return (T)valueSerializer.DeserializeSplitedSingleValue(serializer, typeof(T), commandLine.Split(' '), out remainArgs)!;
        }
    }
}