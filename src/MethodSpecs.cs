using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Decoherence.CommandLineSerialization.Attributes;

namespace Decoherence.CommandLineSerialization
{
    public class MethodSpecs : ISpecs
    {
        public IReadOnlyDictionary<string, IOption> Options
        {
            get
            {
                _TryAnalyseMethod();
                
                var dic = new Dictionary<string, IOption>();
                foreach (var spec in mSpecs)
                {
                    if (spec is IOption option)
                    {
                        dic.Add(option.Name, option);
                    }
                }

                return dic;
            }
        }

        public IReadOnlyList<IArgument> Arguments
        {
            get
            {
                _TryAnalyseMethod();
                
                return new List<IArgument>(
                    from spec in mSpecs
                    where spec is IArgument
                    select (IArgument)spec);
            }
        }

        public MethodBase Method { get; }

        private readonly List<ISpec> mSpecs = new();
        private bool mAnalysed;

        public MethodSpecs(MethodBase method)
        {
            Method = method;
        }

        /// <summary>
        /// 获取spec对应的函数参数的index
        /// </summary>
        public int GetParameterIndex(ISpec spec)
        {
            _TryAnalyseMethod();
            
            return mSpecs.FindIndex(innerSpec => innerSpec == spec);
        }
        
        /// <summary>
        /// 分析方法，生成参数对应的option和argument
        /// </summary>
        private void _TryAnalyseMethod()
        {
            if (mAnalysed)
            {
                return;
            }
            mAnalysed = true;
            
            var parameters = Method.GetParameters();
            foreach (var paramInfo in parameters)
            {
                var attr = paramInfo.GetCustomAttribute<SpecAttribute>();
                ISpec spec = _GenerateSpec(attr, paramInfo);
                mSpecs.Add(spec);
            }
        }

        private ISpec _GenerateSpec(SpecAttribute? attr, ParameterInfo paramInfo)
        {
            // params的参数是Sequence
            var defaultValueType = paramInfo.GetCustomAttribute<ParamArrayAttribute>() != null ? ValueType.Sequence : ValueType.Single;
            
            if (attr is null or ArgumentAttribute)
            {
                ArgumentAttribute? argumentAttr = null;
                if (attr != null)
                {
                    argumentAttr = (ArgumentAttribute)attr;
                }
                
                return new Argument(
                    argumentAttr != null && argumentAttr.ValueType != ValueType.Default ? argumentAttr.ValueType : defaultValueType, 
                    paramInfo.ParameterType, 
                    argumentAttr?.Serializer);
            }

            var optionAttr = (OptionAttribute)attr;
            return new Option(
                optionAttr.Name ?? paramInfo.Name, 
                optionAttr.ValueType != ValueType.Default ? optionAttr.ValueType : defaultValueType, 
                paramInfo.ParameterType, 
                optionAttr.Serializer);
        }
    }
}