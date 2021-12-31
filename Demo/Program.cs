using System;
using System.Collections.Generic;
using Decoherence.CommandLineSerialization;

var commandline = "--aaa=111 -bca123";

CommandLineDeserializer deserializer = new();

Specs specs = new();
specs.AddOption(new Option("aaa", OptionValueType.Single, typeof(int)));
specs.AddOption(new Option('a', OptionValueType.Single, typeof(int)));

var remainArgs =deserializer.Deserialize(commandline.Split(' '), specs, 
    (option, obj) => Console.WriteLine($"{option.Name} = {obj}"), 
    null);

foreach (var arg in remainArgs)
{
    Console.Write($"{arg} ");
}