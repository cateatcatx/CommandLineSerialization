using System;

namespace Decoherence.CommandLineSerialization
{
    public interface ISpec : IValueSerializer
    {
        string ValueName { get; }
        
        ValueType ValueType { get; }
        
        string? Desc { get; }
        
        /// <summary>
        /// 值对应的C#类型
        /// </summary>
        Type ObjType { get; }

        string GetDrawUsageHead();
        string GetDrawExplainHead();
    }
}