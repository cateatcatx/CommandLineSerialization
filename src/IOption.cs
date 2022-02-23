namespace Decoherence.CommandLineSerialization
{
   public interface IOption : ISpec
    {
        string? LongName { get; }
        char? ShortName { get; }
    }
}