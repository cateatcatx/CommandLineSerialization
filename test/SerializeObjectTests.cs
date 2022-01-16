using System;
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
            BuiltinValueSerializer valueSerializer = new();
            var commandLine = valueSerializer.SerializeSingleValue(serializer, obj.GetType(), obj);
            Console.WriteLine(commandLine);

            var obj2 = valueSerializer.DeserializeSingleValue(serializer, typeof(TestingClass5), commandLine);

            Assert.IsTrue(obj.Equals(obj2));
        }
    }
}