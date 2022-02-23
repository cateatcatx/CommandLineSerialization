using Decoherence.CommandLineSerialization.Attributes;

namespace Decoherence.CommandLineSerialization.Test
{
    public class TestingMethods
    {
        public static string Foo1(int a, int b)
        {
            return $"{a},{b}";
        }

        public static string Foo2(int a, [Option] int b, [Option(LongName = "ccc", ShortName = "c")] int c, int d)
        {
            return $"{a},{b},{c},{d}";
        }
        
        public static string Foo3(int a, int b = 2)
        {
            return $"{a},{b}";
        }

        public static string Foo4(int a, params int[] b)
        {
            return $"{a},{string.Join(',', b)}";
        }
        
        public static string Foo5(int a, [Option] params int[] b)
        {
            return $"{a},{string.Join(',', b)}";
        }
        
        public static string Foo6(int a, [Option(LongName = "bbb")] params int[] b)
        {
            return $"{a},{string.Join(',', b)}";
        }
        
        public static string Foo7(int a, [Option(ValueType = ValueType.Multi)] params int[] b)
        {
            return $"{a},{string.Join(',', b)}";
        }
        
        public static string Foo8(int a, [Option(LongName = "bbb", ValueType = ValueType.Multi)] params int[] b)
        {
            return $"{a},{string.Join(',', b)}";
        }

        public static string Foo9(int a, [Option] TestingClass4 b)
        {
            return $"{a},{b}";
        }
    }
}