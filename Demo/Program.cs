using System;
using System.Collections.Generic;
using Decoherence.CommandLineParsing;;


var argArray = "1 2 -a100 --arg 200 -a 300 --arg=400".Split();
var serializer = new CommandLineSerializer();

var specs = new Specs();
var opt = specs.AddOption(Option.NewRequired(typeof(string), OptionType.Sequence, "arg", 'a'));
var values = serializer.DeserializeValues(argArray, specs);
values.TryGetValue<IEnumerable<string>>(opt, out var value);
Console.WriteLine(string.Join(',', value));