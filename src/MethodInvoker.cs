using System;
using System.Collections.Generic;
using System.Reflection;
using Decoherence.SystemExtensions;

namespace Decoherence.CommandLineSerialization;

public class MethodInvoker
{
    public static TRet InvokeMethod<TRet>(
        CommandLineSerializer serializer,
        Type type,
        string methodName,
        object? obj,
        LinkedList<string> argList)
    {
        var retObj = InvokeMethod(serializer, type, methodName, obj, argList);
        return (TRet)retObj!;
    }
        
    public static object? InvokeMethod(
        CommandLineSerializer serializer,
        Type type,
        string methodName,
        object? obj,
        LinkedList<string> argList)
    {
        var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
        ThrowUtil.ThrowIfArgumentNull(method);

        return InvokeMethod(serializer, method, obj, argList);
    }
        
    public static TRet InvokeMethod<TRet>(
        CommandLineSerializer serializer,
        MethodBase method,
        object? obj,
        LinkedList<string> argList)
    {
        var retObj = InvokeMethod(serializer, new MethodSpecs(method), obj, argList, null);
        return (TRet)retObj!;
    }
        
    /// <summary>
    /// 调用函数
    /// </summary>
    /// <param name="serializer">命令行反序列器</param>
    /// <param name="method">待调用函数</param>
    /// <param name="obj">调用函数时的this对象</param>
    /// <param name="argList">命令行参数，本集合会被修改，调用完毕后集合内是剩余的命令行参数</param>
    /// <returns>函数返回值</returns>
    public static object? InvokeMethod(
        CommandLineSerializer serializer,
        MethodBase method,
        object? obj,
        LinkedList<string> argList)
    {
        return InvokeMethod(serializer, new MethodSpecs(method), obj, argList, null);
    }

    /// <summary>
    /// 调用函数
    /// </summary>
    /// <param name="serializer">命令行反序列器</param>
    /// <param name="method">待调用函数</param>
    /// <param name="obj">调用函数时的this对象</param>
    /// <param name="args">命令行参数</param>
    /// <param name="remainArgs">调用完毕后的剩余命令行参数</param>
    /// <returns>函数返回值</returns>
    public static object? InvokeMethod(
        CommandLineSerializer serializer,
        MethodBase method,
        object? obj,
        IEnumerable<string> args,
        out LinkedList<string> remainArgs)
    {
        remainArgs = new LinkedList<string>(args);
        return InvokeMethod(serializer, new MethodSpecs(method), obj, remainArgs, null);
    }

    /// <summary>
    /// 调用函数
    /// </summary>
    /// <param name="serializer">命令行反序列器</param>
    /// <param name="methodSpecs">函数的参数说明</param>
    /// <param name="obj">调用函数时的this对象</param>
    /// <param name="argList">命令行参数，本集合会被修改，调用完毕后集合内是剩余的命令行参数</param>
    /// <returns>函数返回值</returns>
    public static object? InvokeMethod(
        CommandLineSerializer serializer,
        MethodSpecs methodSpecs,
        object? obj,
        LinkedList<string> argList) 
        => InvokeMethod(serializer, methodSpecs, obj, argList, null);

    /// <summary>
    /// 调用函数
    /// </summary>
    /// <param name="serializer">命令行反序列器</param>
    /// <param name="methodSpecs">函数的参数说明</param>
    /// <param name="obj">调用函数时的this对象</param>
    /// <param name="argList">命令行参数，本集合会被修改，调用完毕后集合内是剩余的命令行参数</param>
    /// <param name="beforeInvoke">获取到函数参数后调用函数前触发</param>
    /// <returns>函数返回值</returns>
    public static object? InvokeMethod(
        CommandLineSerializer serializer,
        MethodSpecs methodSpecs,
        object? obj,
        LinkedList<string> argList,
        Action<ParameterInfo[], object?[]>? beforeInvoke)
    {
        var method = methodSpecs.Method;
        var paramInfos = method.GetParameters();
        var length = paramInfos.Length;
        var parameters = new object?[length];
        var touchedIndexes = new bool[length];
            
        serializer.Deserialize(argList, methodSpecs,
            (spec, paramObj) =>
            {
                if (methodSpecs.TryGetParameterIndex(spec, out var index) && 0 <= index && index < length)
                {
                    parameters[index] = paramObj;
                    touchedIndexes[index] = true;
                }
            });

        // 设置函数参数自身的默认值
        for (var i = 0; i < length; ++i)
        {
            var paramInfo = paramInfos[i];
            if (!touchedIndexes[i])
            {
                if (paramInfo.DefaultValue == DBNull.Value)
                {
                    throw new InvalidOperationException($"Lack of {i + 1}th non-default parameter: {paramInfo.Name}.");
                }

                parameters[i] = paramInfo.DefaultValue;
            }
        }
            
        beforeInvoke?.Invoke(paramInfos, parameters);

        if (method is ConstructorInfo constructorInfo)
        {
            return constructorInfo.Invoke(parameters);
        }

        return method.Invoke(obj, parameters);
    }
}