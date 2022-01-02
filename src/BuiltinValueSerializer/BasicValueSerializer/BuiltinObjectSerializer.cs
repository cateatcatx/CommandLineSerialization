using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Decoherence.SystemExtensions;

namespace Decoherence.CommandLineSerialization
{
    public class BuiltinObjectSerializer : IValueSerializer
    {
        private readonly MethodInvoker mMethodInvoker = new();
        
        public bool CanHandleType(Type objType)
        {
            return !objType.IsPrimitive;
        }

        public object? DeserializeNonValue(CommandLineDeserializer deserializer, Type objType)
        {
            throw new InvalidOperationException();
        }

        public object? DeserializeSingleValue(CommandLineDeserializer deserializer, Type objType, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            
            var argList = deserializer.SplitCommandLine(value);
            return DeserializeSplitedSingleValue(deserializer, objType, argList);
        }

        public object? DeserializeSplitedSingleValue(CommandLineDeserializer deserializer, Type objType, LinkedList<string> argList)
        {
            var specs = new ObjectSpecs(objType);
            specs.InitConstructorSpecs();
            specs.InitMemberSpecs();
            
            var obj = mMethodInvoker.InvokeMethod(deserializer, specs.ConstructorSpecs, null, argList);
            Debug.Assert(obj != null);
            
            deserializer.Deserialize(
                argList, 
                specs.MemberSpecs,
                (spec, memberValue) =>
                {
                    if (specs.MemberSpecs.TryGetMember(spec, out var memberInfo))
                    {
                        memberInfo.SetValue(obj, memberValue);
                    }
                }, 
                null);

            return obj;
        }

        public object? DeserializeMultiValue(CommandLineDeserializer deserializer, Type objType, List<string> values)
        {
            throw new InvalidOperationException();
        }
    }
}