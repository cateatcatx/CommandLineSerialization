using System;
using Decoherence.CommandLineParsing;;


Console.WriteLine("hello");
            
CommandLineSerializer serializer = new CommandLineSerializer();

var specs = new Specs();
var argument1 = specs.AddArgument(Argument.NewRequired(typeof(string), ArgumentType.Scalar, "argument1"));
var argument2 = specs.AddArgument(Argument.NewRequired(typeof(TestEnum), ArgumentType.Scalar, "argument2"));
var option1 = specs.AddOption(Option.NewOptional(typeof(int), OptionType.Scalar, () => 0, "opt1", 'o'));


serializer.DeserializeValues(args, specs, out var values, out var remainArgs);

if (values.TryGetValue<string>(argument1, out var argValue1))
{
    Console.WriteLine($"argument1: {argValue1}");
}
if (values.TryGetValue<TestEnum>(argument2, out var argValue2))
{
    Console.WriteLine($"argument2: {argValue2}");
}
if (values.TryGetValue<int>(option1, out var optValue1))
{
    Console.WriteLine($"option1: {optValue1}");
}

return 0;

public enum TestEnum
{
    A = 1,
    B = 2,
    C = A | B,
}