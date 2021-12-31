using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public interface ISpec : IValueSerializer
    {
        /// <summary>
        /// 值对应的C#类型
        /// </summary>
        Type ObjType { get; }
        
        /// <summary>
        /// 是否控制序列化和反序列化
        /// </summary>
        bool IsControlSerialization { get; }
    }
}