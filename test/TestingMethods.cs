using Decoherence.CommandLineSerialization.Attributes;

namespace Decoherence.CommandLineSerialization.Test
{
    public class TestingMethods
    {
        public static string Foo1(int a, int b)
        {
            return $"{a},{b}";
        }

        public static string Foo2(int a, [Option] int b, [Option(name: "ccc")] int c, int d)
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
    }
}