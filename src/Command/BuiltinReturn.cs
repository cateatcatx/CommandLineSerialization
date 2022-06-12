namespace Decoherence.CommandLineSerialization;

public enum BuiltinReturn
{
    /// <summary>
    /// 继续执行后续函数
    /// </summary>
    Continue,
        
    /// <summary>
    /// 截断后续函数的执行
    /// </summary>
    Truncate,
}