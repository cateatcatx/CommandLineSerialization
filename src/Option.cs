namespace Decoherence.CommandLineParsing
{
    public class Option : Spec
    {
        public OptionType Type { get; }
        
        public string? LongName { get; }
        
        public string? ShortName { get; }
        
        public override string Name { get; }
    }
}