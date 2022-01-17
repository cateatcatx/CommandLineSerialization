using System;
using System.Collections.Generic;
using System.Reflection;

namespace Decoherence.CommandLineSerialization
{
    public class MethodInvoker
    {
        /// <summary>
        /// <inheritdoc cref="InvokeMethod(Decoherence.CommandLineSerialization.CommandLineSerializer,Decoherence.CommandLineSerialization.MethodSpecs,object?,System.Collections.Generic.LinkedList{string})"/>
        /// </summary>
        /// <param name="serializer">命令行反序列器</param>
        /// <param name="method">待调用函数</param>
        /// <param name="obj">调用函数时的this对象</param>
        /// <param name="argList">命令行参数，本集合会被修改，调用完毕后集合内是剩余的命令行参数</param>
        /// <returns>函数返回值</returns>
        /// <exception cref="ArgumentException">
        ///     <paramref name="argList"/>中的参数不足</exception>
        public static object? InvokeMethod(
            CommandLineSerializer serializer,
            MethodBase method,
            object? obj,
            LinkedList<string> argList)
        {
            return InvokeMethod(serializer, new MethodSpecs(method), obj, argList);
        }

        /// <summary>
        /// <inheritdoc cref="InvokeMethod(Decoherence.CommandLineSerialization.CommandLineSerializer,Decoherence.CommandLineSerialization.MethodSpecs,object?,System.Collections.Generic.LinkedList{string})"/>
        /// </summary>
        /// <param name="serializer">命令行反序列器</param>
        /// <param name="method">待调用函数</param>
        /// <param name="obj">调用函数时的this对象</param>
        /// <param name="args">命令行参数</param>
        /// <param name="remainArgs">调用完毕后的剩余命令行参数</param>
        /// <returns>函数返回值</returns>
        /// <exception cref="ArgumentException">
        ///     <paramref name="args"/>中的参数不足</exception>
        public static object? InvokeMethod(
            CommandLineSerializer serializer,
            MethodBase method,
            object? obj,
            IEnumerable<string> args,
            out LinkedList<string> remainArgs)
        {
            remainArgs = new LinkedList<string>(args);
            return InvokeMethod(serializer, new MethodSpecs(method), obj, remainArgs);
        }

        /// <summary>
        /// 调用函数
        /// </summary>
        /// <param name="serializer">命令行反序列器</param>
        /// <param name="methodSpecs">函数的参数说明</param>
        /// <param name="obj">调用函数时的this对象</param>
        /// <param name="argList">命令行参数，本集合会被修改，调用完毕后集合内是剩余的命令行参数</param>
        /// <returns>函数返回值</returns>
        /// <exception cref="ArgumentException">
        ///     <paramref name="argList"/>中的参数不足</exception>
        public static object? InvokeMethod(
            CommandLineSerializer serializer,
            MethodSpecs methodSpecs,
            object? obj,
            LinkedList<string> argList)
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
                if (touchedIndexes[i])
                {
                    continue;
                }
                
                if (paramInfos[i].DefaultValue == DBNull.Value)
                {
                    throw new ArgumentException($"Lack of {i}th non-default parameter object.", nameof(argList));
                }
                
                parameters[i] = paramInfos[i].DefaultValue;
            }

            if (method is ConstructorInfo constructorInfo)
            {
                return constructorInfo.Invoke(parameters);
            }

            return method.Invoke(obj, parameters);
        }
    }
}