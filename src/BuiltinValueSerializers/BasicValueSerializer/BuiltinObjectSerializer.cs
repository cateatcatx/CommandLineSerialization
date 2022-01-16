using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Decoherence.CommandLineSerialization.Attributes;
using Decoherence.SystemExtensions;
// ReSharper disable ReturnTypeCanBeNotNullable

namespace Decoherence.CommandLineSerialization
{
    public class BuiltinObjectSerializer : IValueSerializer
    {
        private readonly MethodInvoker mMethodInvoker = new();
        
        public bool CanHandleType(Type objType)
        {
            return !objType.IsPrimitive;
        }

        public object? DeserializeNonValue(CommandLineSerializer serializer, Type objType, bool matched)
        {
            throw new InvalidOperationException();
        }

        public object? DeserializeSingleValue(CommandLineSerializer serializer, Type objType, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            
            var argList = ImplUtil.SplitCommandLine(value);
            return serializer.DeserializeObject(objType, argList);
        }

        public object? DeserializeMultiValue(CommandLineSerializer serializer, Type objType, List<string> values)
        {
            throw new InvalidOperationException();
        }

        public bool SerializeNonValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            throw new NotImplementedException();
        }

        public string SerializeSingleValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            
            var memberSpecs = new MemberSpecs(objType);
            memberSpecs.Init();

            var argList = serializer.Serialize(memberSpecs, spec =>
            {
                Debug.Assert(memberSpecs.TryGetMember(spec, out var memberInfo));
                return memberInfo.GetValue(obj);
            });

            return ImplUtil.MergeCommandLine(argList);
        }

        public IEnumerable<string> SerializeMultiValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            throw new NotImplementedException();
        }
    }
}