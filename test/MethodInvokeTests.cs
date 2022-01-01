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
            
            MethodSpecs methodSpecs = new(typeof(MethodsForTest).GetMethod(nameof(MethodsForTest.Foo1))!);
            methodSpecs.AnalyseMethod();

            Specs specs = new();
            foreach (var option in methodSpecs.Options.Values)
            {
                specs.AddOption(option);
            }
            foreach (var argument in methodSpecs.Arguments)
            {
                specs.AddArgument(argument);
            }

            Dictionary<ISpec, object?> specParameters = new();
            CommandLineDeserializer deserializer = new();
            var remainArgs = deserializer.Deserialize(commandline.Split(' '), specs,
                (option, obj) =>
                {
                    if (methodSpecs.IsParameterSpec(option))
                    {
                        specParameters.Add(option, obj);
                    }
                },
                (argument, obj) =>
                {
                    if (methodSpecs.IsParameterSpec(argument))
                    {
                        specParameters.Add(argument, obj);
                    }
                });
            
            Assert.True(string.Join(' ', remainArgs) == "-abc1 -ccc2");
            
            var ret = methodSpecs.Invoke(null, specParameters);
            Assert.True(ret != null && (ret as string) == "1,2");
        }
    }
}