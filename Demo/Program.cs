using System;
using Decoherence.CommandLineParsing;;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLineSerializer serializer = new CommandLineSerializer();

            var specs = new Specs();
            var argument0 = specs.AddArgument(new Argument(0));

            var values = serializer.DeserializeValues(args, specs);

            if (values.TryGetArgumentValue<int>(argument0, out var value0))
            {
                Console.WriteLine(value0);
            }
            
        }

        static void Foo(int a)
        {
            
        }
    }
}