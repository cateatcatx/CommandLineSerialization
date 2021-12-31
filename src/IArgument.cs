namespace Decoherence.CommandLineSerialization
{
    public enum ArgumentValueType
    {
        Single,
        Sequence,
    }
    
    public interface IArgument : ISpec
    {
        int Index { get; }
        ArgumentValueType ValueType { get; }
    }
}