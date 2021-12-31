namespace Decoherence.CommandLineSerialization
{
    public enum ArgumentValueType
    {
        Single,
        Sequence,
    }
    
    public interface IArgument : ISpec
    {
        ArgumentValueType ValueType { get; }
    }
}