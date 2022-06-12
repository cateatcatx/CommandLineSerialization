using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization;

public delegate void OnSerializeObject(CommandLineSerializer serializer, object? obj);
public delegate object? OnSerialized(ISpec spec);

public delegate object? OnDeserializeObject(CommandLineSerializer serializer, LinkedList<string> argList);
public delegate void OnDeserialized(ISpec spec, object? value);