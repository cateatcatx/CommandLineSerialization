using System;

namespace Decoherence.CommandLineSerialization.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Constructor)]
public class IgnoreAttribute : Attribute
{
        
}