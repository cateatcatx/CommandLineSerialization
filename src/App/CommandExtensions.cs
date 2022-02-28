using System;
using Decoherence.SystemExtensions;

namespace Decoherence.CommandLineSerialization
{
    public static class CommandExtensions
    {
        public static void AddMethod(this Command self, Type type, string methodName, object? obj)
        {
            self.AddMethod(type.GetMethodThrow(methodName), () => obj);
        }
        
        public static void AddMethod(this Command self, Type type, string methodName, Func<object?>? objGetter)
        {
            self.AddMethod(type.GetMethodThrow(methodName), objGetter);
        }
        
        public static void AddSubcommand(this Command self, Type type, string methodName, object? obj)
        {
            AddSubcommand(self, type, methodName, obj, null);
        }
        
        public static void AddSubcommand(this Command self, Type type, string methodName, object? obj, string? group)
        {
            // todo 支持attribute
            var command = new Command(methodName, null);
            command.AddMethod(type, methodName, obj);
            
            self.AddSubcommand(command, group);
        }
        
        public static void AddSubcommand(this Command self, string commandName, Type type, string methodName, Func<object?>? objGetter)
        {
            // todo 支持attribute
            var command = new Command(commandName, null);
            command.AddMethod(type, methodName, objGetter);
            
            self.AddSubcommand(command);
        }
    }
}