using Decoherence.CommandLineSerialization.Attributes;

namespace Decoherence.CommandLineSerialization.Test
{
    public class TestingClass1
    {
        public int FieldA;
        public int FieldB;

        public override string ToString()
        {
            return $"{FieldA},{FieldB}";
        }
    }
    
    public class TestingClass2
    {
        public int FieldA;
        public int FieldB;

        public TestingClass2(int fieldA)
        {
            FieldA = fieldA;
        }
    }
    
    public class TestingClass3
    {
        [Argument(Priority = 1)]
        public int FieldA;
        
        [Argument(Priority = 0)]
        public int FieldB;

        public int FieldC;
    }
    
    public class TestingClass4
    {
        public int FieldA;
        public TestingClass1 FieldB;

        public override string ToString()
        {
            return $"{FieldA},{FieldB}";
        }
    }
}