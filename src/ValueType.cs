namespace Decoherence.CommandLineSerialization
{
    public enum ValueType
    {
        /// <summary>
        /// 默认，不改变调用函数的默认值类型
        /// </summary>
        Default,
        
        /// <summary>
        /// 无值 option only
        /// </summary>
        Non,
        
        /// <summary>
        /// 单个值
        /// </summary>
        Single,
        
        /// <summary>
        /// 多重值 option only
        /// </summary>
        Multi,
        
        /// <summary>
        /// 序列值
        /// </summary>
        Sequence,
    }
}