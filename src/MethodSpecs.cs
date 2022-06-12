using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using Decoherence.CommandLineSerialization.Attributes;

namespace Decoherence.CommandLineSerialization;

public class MethodSpecs : ISpecs
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

    public MethodBase Method { get; }

    private readonly List<ISpec> mParameterOrderSpecs = new();
    private readonly Specs mSpecs = new();
        
    private bool mInited;

    public MethodSpecs(MethodBase method)
    {
        Method = method;
    }

    /// <summary>
    /// 获取spec对应的函数参数的index
    /// </summary>
    public bool TryGetParameterIndex(ISpec spec, out int index)
    {
        Init();
            
        index = mParameterOrderSpecs.FindIndex(innerSpec => innerSpec == spec);
        return index >= 0;
    }
        
    /// <summary>
    /// 分析方法，生成参数对应的option和argument
    /// </summary>
    public void Init()
    {
        if (mInited)
        {
            return;
        }
        mInited = true;
            
        var parameters = Method.GetParameters();
        foreach (var paramInfo in parameters)
        {
            var attr = paramInfo.GetCustomAttribute<SpecAttribute>();

            ValueType getDefaultValueType(bool isOption) 
                => paramInfo.GetCustomAttribute<ParamArrayAttribute>() != null ? ValueType.Sequence : ImplUtil.GetDefaultValueType(isOption, paramInfo.ParameterType);
                
            var spec = attr != null 
                ? ImplUtil.GenerateSpecByAttribute(
                    attr,
                    paramInfo.ParameterType,
                    paramInfo.Name,
                    getDefaultValueType,
                    null)
                : new Argument(getDefaultValueType(false), paramInfo.ParameterType, paramInfo.Name);

            mParameterOrderSpecs.Add(spec);
            mSpecs.AddSpec(spec);
        }
    }
}