using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public class ObjectSpecs : ISpecs
    {
        public static ObjectSpecs NewSingleSpec(ISpec spec, Type objType)
        {
            string getError(object obj)
            {
                return $"Type mismatched, got: {obj.GetType()}, expected: {objType}.";
            }
            
            var specs = new Specs();
            specs.AddSpec(spec);
            
            object? deserializeValue = null;
            object? serializeValue = null;
            
            return new ObjectSpecs(
                specs,
                
                (_, _) => null,
                (_, v) =>
                {
                    if (v != null && !objType.IsInstanceOfType(v))
                        throw new InvalidOperationException(getError(v));
                        
                    deserializeValue = v;
                },
                (_, _) => deserializeValue,
                
                (_, obj) =>
                {
                    if (obj != null && !objType.IsInstanceOfType(obj))
                        throw new InvalidOperationException(getError(obj));
                    
                    serializeValue = obj;
                },
                _ => serializeValue,
                (_, _) => { });
        }

        public IReadOnlyDictionary<string, IOption> Options => mSpecs.Options;
        public IReadOnlyList<IArgument> Arguments => mSpecs.Arguments;

        private readonly ISpecs mSpecs;

        private readonly OnDeserializeObject mOnBeginDeserializeObject;
        private readonly OnDeserialized mOnDeserialized;
        private readonly OnDeserializeObject mOnEndDeserializeObject;

        private readonly OnSerializeObject mOnBeginSerializeObject;
        private readonly OnSerialized mOnSerialized;
        private readonly OnSerializeObject mOnEndSerializeObject;

        public ObjectSpecs(
            ISpecs specs,
            OnDeserializeObject onBeginDeserializeObject,
            OnDeserialized onDeserialized,
            OnDeserializeObject onEndDeserializeObject,
            OnSerializeObject onBeginSerializeObject,
            OnSerialized onSerialized,
            OnSerializeObject onEndSerializeObject)
        {
            mSpecs = specs;
            mOnBeginDeserializeObject = onBeginDeserializeObject;
            mOnDeserialized = onDeserialized;
            mOnEndDeserializeObject = onEndDeserializeObject;
            mOnBeginSerializeObject = onBeginSerializeObject;
            mOnSerialized = onSerialized;
            mOnEndSerializeObject = onEndSerializeObject;
        }

        public void BeginDeserializeObject(CommandLineSerializer serializer, LinkedList<string> argList)
        {
            mOnBeginDeserializeObject(serializer, argList);
        }
        
        public void SpecDeserialized(ISpec spec, object? obj)
        {
            mOnDeserialized(spec, obj);
        }
        
        public object? EndDeserializeObject(CommandLineSerializer serializer, LinkedList<string> argList)
        {
            return mOnEndDeserializeObject(serializer, argList);
        }

        public void BeginSerializeObject(CommandLineSerializer serializer, object? obj)
        {
            mOnBeginSerializeObject(serializer, obj);
        }

        public object? SpecSerialized(ISpec spec)
        {
            return mOnSerialized(spec);
        }
        
        public void EndSerializeObject(CommandLineSerializer serializer, object? obj)
        {
            mOnEndSerializeObject(serializer, obj);
        }
    }
}