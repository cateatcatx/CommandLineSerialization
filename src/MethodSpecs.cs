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
                ISpec spec = attr != null ? attr.GenerateSpec(paramInfo.Name, paramInfo.ParameterType) : new Argument(_GetDefaultArgumentValueType(paramInfo), paramInfo.ParameterType);
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

        private ArgumentValueType _GetDefaultArgumentValueType(ParameterInfo paramInfo)
        {
            // 只有params的参数是Sequence
            return paramInfo.GetCustomAttribute<ParamArrayAttribute>() != null ? ArgumentValueType.Sequence : ArgumentValueType.Single;
        }
    }
}