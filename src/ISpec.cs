using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization;

public interface ISpec : IValueSerializer
{
    string ValueName { get; }
        
    ValueType ValueType { get; }
        
    string? Desc { get; }
        
    /// <summary>
    /// 值对应的C#类型
    /// </summary>
    Type ObjType { get; }
        
    /// <summary>
    /// 获取被限制的可选值范围
    /// </summary>
    /// <returns>null代表不做限制</returns>
    HashSet<string>? GetLimitValues();

    string GetDrawUsageHead();
    string GetDrawExplainHead();
}