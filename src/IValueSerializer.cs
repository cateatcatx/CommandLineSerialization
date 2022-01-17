using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public interface IValueSerializer
    {
        /// <summary>
        /// 判断是否可以处理某类型
        /// </summary>
        /// <param name="objType">对象类型</param>
        /// <returns>是否可以处理</returns>
        bool CanHandleType(Type objType);

        ObjectSpecs? GetObjectSpecs(Type objType);

        //-------------------------------------------------
        // 反序列化相关
        //-------------------------------------------------
        object? DeserializeNonValue(CommandLineSerializer serializer, Type objType, bool matched);
        object? DeserializeSingleValue(CommandLineSerializer serializer, Type objType, string? value);
        object? DeserializeMultiValue(CommandLineSerializer serializer, Type objType, List<string> values);

        //-------------------------------------------------
        // 序列化相关
        //-------------------------------------------------
        bool SerializeNonValue(CommandLineSerializer serializer, Type objType, object? obj);
        string SerializeSingleValue(CommandLineSerializer serializer, Type objType, object? obj);
        IEnumerable<string> SerializeMultiValue(CommandLineSerializer serializer, Type objType, object? obj);
    }
}