using System.Collections.Generic;
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

        public override bool Equals(object? obj)
        {
            if (obj is not TestingClass1 other)
            {
                return false;
            }

            return FieldA == other.FieldA && FieldB == other.FieldB;
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

        public override bool Equals(object? obj)
        {
            if (obj is not TestingClass4 other)
            {
                return false;
            }

            return FieldA == other.FieldA && FieldB.Equals(other.FieldB);
        }
    }

    public class TestingClass5
    {
        public int FieldA;
        public TestingClass4 FieldB;
        
        public override string ToString()
        {
            return $"{FieldA},{FieldB}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not TestingClass5 other)
            {
                return false;
            }

            return FieldA == other.FieldA && FieldB.Equals(other.FieldB);
        }
    }

    public class TestingClass6
    {
        [Argument]
        public List<TestingClass1> FieldA;
    }
}