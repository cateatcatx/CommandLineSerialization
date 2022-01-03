using System;
using System.Collections.Generic;
using Decoherence.CommandLineSerialization;
using ValueType = Decoherence.CommandLineSerialization.ValueType;

class Program
{
    static void Main(string[] args)
    {
        CommandLineSerializer serializer = new();

        var optiona = new Option("a", ValueType.Single, typeof(int));
        var optionaaa = new Option("aaa", ValueType.Single, typeof(int));
        var argument = new Argument(ValueType.Single, typeof(int));
        var specs = new Specs();
        specs.AddSpec(optiona);
        specs.AddSpec(optionaaa);
        specs.AddSpec(argument);

        var argList = serializer.Serialize(specs, _ => 1);
        Console.WriteLine(string.Join(' ', argList));
    }
}