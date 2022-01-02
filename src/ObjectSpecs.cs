using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Decoherence.CommandLineSerialization.Attributes;

namespace Decoherence.CommandLineSerialization
{
    public class ObjectSpecs
    {
        public Type ObjType { get; }

        public MethodSpecs ConstructorSpecs
        {
            get
            {
                InitConstructorSpecs();
                return mConstructorSpecs!;
            }
        }

        public ObjectMemberSpecs MemberSpecs
        {
            get
            {
                InitMemberSpecs();
                return mMemberSpecs!;
            }
        }

        private MethodSpecs? mConstructorSpecs;
        private ObjectMemberSpecs? mMemberSpecs;

        public ObjectSpecs(Type objType)
        {
            ObjType = objType;
        }

        public void InitConstructorSpecs()
        {
            if (mConstructorSpecs != null)
            {
                return;
            }

            var constructors = ObjType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            MethodBase? method = null;
            
            // 1 有标签的
            foreach (var constructor in constructors)
            {
                var attr = constructor.GetCustomAttribute<CommandLineConstructorAttribute>();
                if (attr != null)
                {
                    method = constructor;
                    break;
                }
            }
            
            // 2 参数里有标签的
            if (method == null)
            {
                foreach (var constructor in constructors)
                {
                    if (constructor.GetParameters().Any(paramInfo => paramInfo.GetCustomAttribute<SpecAttribute>() != null))
                    {
                        method = constructor;
                        break;
                    }
                }
            }
            
            // 3 参数最多的public构造函数
            if (method == null)
            {
                var maxParamCount = int.MinValue;
                foreach (var constructor in constructors)
                {
                    if (!constructor.IsPublic)
                        continue;
                    
                    var l = constructor.GetParameters().Length;
                    if (l > maxParamCount)
                    {
                        method = constructor;
                        maxParamCount = l;
                    }
                }
            }

            if (method == null)
            {
                throw new InvalidOperationException($"No valid constructor for {ObjType}.");
            }

            mConstructorSpecs = new MethodSpecs(method);
            mConstructorSpecs.Init();
        }

        public void InitMemberSpecs()
        {
            if (mMemberSpecs != null)
            {
                return;
            }

            mMemberSpecs = new ObjectMemberSpecs(ObjType);
            mMemberSpecs.Init();
        }
    }
}