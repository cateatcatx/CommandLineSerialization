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

        public IReadOnlyList<IArgument> Arguments => new List<IArgument>(
            from spec in mSpecs
            where spec is IArgument
            select (IArgument)spec);
        
        public MethodBase Method { get; }

        private readonly List<ISpec> mSpecs = new();

        public MethodSpecs(MethodBase method)
        {
            Method = method;
        }

        /// <summary>
        /// 分析方法，生成参数对应的option和argument
        /// </summary>
        public void AnalyseMethod()
        {
            var parameters = Method.GetParameters();
            foreach (var paramInfo in parameters)
            {
                var attr = paramInfo.GetCustomAttribute<SpecAttribute>();
                ISpec spec = _GenerateSpec(attr, paramInfo);
                mSpecs.Add(spec);
            }
        }

        /// <summary>
        /// 判断指定spec是否属于本函数的参数
        /// </summary>
        public bool IsParameterSpec(ISpec spec)
        {
            return mSpecs.FindIndex(innerSpec => innerSpec == spec) >= 0;
        }

        public object? Invoke(object? obj, IReadOnlyDictionary<ISpec, object?> specParameters)
        {
            var paramInfos = Method.GetParameters();
            var parameters = new object?[paramInfos.Length];
            for (var i = 0; i < parameters.Length; ++i)
            {
                var spec = mSpecs[i];
                var setAny = false;
                if (paramInfos[i].DefaultValue != DBNull.Value)
                {
                    parameters[i] = paramInfos[i].DefaultValue;
                    setAny = true;
                }
                if (specParameters.TryGetValue(spec, out var parameter))
                {
                    parameters[i] = parameter;
                    setAny = true;
                }
                
                if (!setAny)
                {
                    throw new ArgumentException($"Lack of {i}th non-default parameter object.");
                }
            }

            return Method.Invoke(obj, parameters);
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