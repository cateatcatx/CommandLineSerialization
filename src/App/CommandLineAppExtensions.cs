using System;
using Decoherence.SystemExtensions;

namespace Decoherence.CommandLineSerialization
{
    public static class CommandLineAppExtensions
    {
        public static void AddGlobalMethod(this ICommandLineApp self, Type type, string methodName, object? obj)
        {
            self.AddGlobalMethod(type.GetMethodThrow(methodName), () => obj);
        }
        
        public static void AddGlobalMethod(this ICommandLineApp self, Type type, string methodName, Func<object?>? objGetter)
        {
            self.AddGlobalMethod(type.GetMethodThrow(methodName), objGetter);
        }
        
        public static void AddCommand(this ICommandLineApp self, Type type, string methodName, object? obj, string? commandName = null)
        {
            self.AddCommand(type.GetMethodThrow(methodName), () => obj, commandName);
        }
        
        public static void AddCommand(this ICommandLineApp self, Type type, string methodName, Func<object?>? objGetter, string? commandName = null)
        {
            self.AddCommand(type.GetMethodThrow(methodName), objGetter, commandName);
        }
    }
}