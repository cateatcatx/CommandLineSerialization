using System;

namespace Decoherence.CommandLineSerialization
{
    public interface ISpec : IValueSerializer
    {
        ValueType ValueType { get; }
        
        /// <summary>
        /// 值对应的C#类型
        /// </summary>
        Type ObjType { get; }
    }
}