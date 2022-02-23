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
        
        public bool CanHandleType(Type objType)
        {
            return !objType.IsPrimitive;
        }

        public ObjectSpecs? GetObjectSpecs(Type objType)
        {
            var memberSpecs = new MemberSpecs(objType);
            memberSpecs.Init();

            object? deserializeObj = null;
            object? serializeObj = null;
            
            return new ObjectSpecs(
                memberSpecs,
                
                (serializer, argList) =>
                {
                    var constructorSpecs = new MethodSpecs(FindConstructor(objType));
                    constructorSpecs.Init();

                    deserializeObj = MethodInvoker.InvokeMethod(serializer, constructorSpecs, null, argList);
                    Debug.Assert(deserializeObj != null);
                    return deserializeObj;
                },
                (spec, value) =>
                {
                    memberSpecs.TryGetMember(spec, out var memberInfo);
                    memberInfo.SetValue(deserializeObj, value);
                },
                (_, _) => deserializeObj,

                (_, obj) => serializeObj = obj,
                spec =>
                {
                    memberSpecs.TryGetMember(spec, out var memberInfo);
                    return memberInfo.GetValue(serializeObj);
                },
                (_, _) => { });
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
                memberSpecs.TryGetMember(spec, out var memberInfo);
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