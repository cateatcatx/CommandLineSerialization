using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Decoherence.CommandLineSerialization.Attributes;
using Decoherence.SystemExtensions;

namespace Decoherence.CommandLineSerialization
{
    public class BuiltinObjectSerializer : IValueSerializer
    {
        public static ConstructorInfo FindConstructor(Type ObjType)
        {
            var constructors = ObjType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            ConstructorInfo? ret = null;
            
            // 1 有标签的
            foreach (var constructor in constructors)
            {
                var attr = constructor.GetCustomAttribute<ConstructorAttribute>();
                if (attr != null)
                {
                    ret = constructor;
                    break;
                }
            }
            
            // 2 参数里有标签的
            if (ret == null)
            {
                foreach (var constructor in constructors)
                {
                    if (constructor.GetCustomAttribute<IgnoreAttribute>() != null)
                    {
                        continue;
                    }
                    
                    if (constructor.GetParameters().Any(paramInfo => paramInfo.GetCustomAttribute<SpecAttribute>() != null))
                    {
                        ret = constructor;
                        break;
                    }
                }
            }
            
            // 3 参数最多的public构造函数
            if (ret == null)
            {
                var maxParamCount = int.MinValue;
                foreach (var constructor in constructors)
                {
                    if (!constructor.IsPublic || constructor.GetCustomAttribute<IgnoreAttribute>() != null)
                        continue;
                    
                    var l = constructor.GetParameters().Length;
                    if (l > maxParamCount)
                    {
                        ret = constructor;
                        maxParamCount = l;
                    }
                }
            }

            if (ret == null)
            {
                throw new InvalidOperationException($"No valid constructor for {ObjType}.");
            }

            return ret;
        }
        
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
            
            var argList = ImplUtil.SplitCommandLine(value);
            return DeserializeSplitedSingleValue(deserializer, objType, argList);
        }

        public object? DeserializeSplitedSingleValue(CommandLineDeserializer deserializer, Type objType, LinkedList<string> argList)
        {
            var constructorSpecs = new MethodSpecs(FindConstructor(objType));
            constructorSpecs.Init();
            var memberSpecs = new MemberSpecs(objType);
            memberSpecs.Init();

            var obj = mMethodInvoker.InvokeMethod(deserializer, constructorSpecs, null, argList);
            Debug.Assert(obj != null);
            
            deserializer.Deserialize(
                argList, 
                memberSpecs,
                (spec, memberValue) =>
                {
                    if (memberSpecs.TryGetMember(spec, out var memberInfo))
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