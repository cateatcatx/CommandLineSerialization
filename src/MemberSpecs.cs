using System;
using System.Collections.Generic;
using System.Reflection;
using Decoherence.CommandLineSerialization.Attributes;
using Decoherence.SystemExtensions;

namespace Decoherence.CommandLineSerialization
{
    public class MemberSpecs : ISpecs
    {
        public IReadOnlyList<IOption> Options 
        {
            get
            {
                Init();
                return mSpecs.Options;
            }
        }
        
        public IReadOnlyList<IArgument> Arguments
        {
            get
            {
                Init();
                return mSpecs.Arguments;
            }
        }

        private readonly Type mObjType;
        private readonly Dictionary<ISpec, MemberInfo> mSpec2MemberInfo;
        private readonly Specs mSpecs;

        private bool mInited;

        public MemberSpecs(Type objType)
        {
            mObjType = objType;
            mSpec2MemberInfo = new Dictionary<ISpec, MemberInfo>();
            mSpecs = new Specs();
        }
        
        public void Init()
        {
            if (mInited)
            {
                return;
            }
            mInited = true;
            
            var fields = mObjType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var fieldInfo in fields)
            {
                _HandlePublicMember(fieldInfo);
            }
            
            fields = mObjType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var fieldInfo in fields)
            {
                _HandleNonPublicMember(fieldInfo);
            }

            var properties = mObjType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var propertyInfo in properties)
            {
                _HandlePublicMember(propertyInfo);
            }
            
            properties = mObjType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var propertyInfo in properties)
            {
                _HandleNonPublicMember(propertyInfo);
            }
        }

        public bool TryGetMember(ISpec spec, out MemberInfo memberInfo)
        {
            Init();
            return mSpec2MemberInfo.TryGetValue(spec, out memberInfo);
        }

        private void _HandlePublicMember(MemberInfo memberInfo)
        {
            if (!memberInfo.CanWrite() || memberInfo.GetCustomAttribute<IgnoreAttribute>() != null)
            {
                return;
            }

            var spec = _GenerateSpec(memberInfo, memberInfo.GetCustomAttribute<SpecAttribute>());
            mSpec2MemberInfo.Add(spec, memberInfo);
            mSpecs.AddSpec(spec);
        }

        private void _HandleNonPublicMember(MemberInfo memberInfo)
        {
            if (!memberInfo.CanWrite() || memberInfo.GetCustomAttribute<IgnoreAttribute>() != null)
            {
                return;
            }

            var attr = memberInfo.GetCustomAttribute<SpecAttribute>();
            if (attr == null)
            {// 非public没有attribute不处理
                return;
            }

            var spec = _GenerateSpec(memberInfo, attr);
            mSpec2MemberInfo.Add(spec, memberInfo);
            mSpecs.AddSpec(spec);
        }

        private ISpec _GenerateSpec(MemberInfo memberInfo, SpecAttribute? attr)
        {
            var objType = (memberInfo is PropertyInfo propertyInfo) ? propertyInfo.PropertyType : ((FieldInfo)memberInfo).FieldType;
            var defaultValueType = ImplUtil.GetDefaultValueType(objType);
            return attr != null ?
                ImplUtil.GenerateSpecByAttribute(attr, objType, memberInfo.Name, defaultValueType, null) :
                new Option(memberInfo.Name, defaultValueType, objType, memberInfo.Name);
        }
    }
}