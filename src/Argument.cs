namespace Decoherence.CommandLineParsing
{
    public class Argument : Spec
    {
        public ArgumentType Type { get; }
        
        public int Position { get; }
        
        public override string Name { get; }

        public Argument(int position, string? name = null)
        {
            Position = position;
            Name = name;
        }
    }
}