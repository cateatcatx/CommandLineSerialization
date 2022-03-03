using System;
using System.Collections.Generic;
using Decoherence.SystemExtensions;

namespace Decoherence.CommandLineSerialization
{
    public static class CommandExtensions
    {
        public static int? Run(this ICommand command, IEnumerable<string> args, out LinkedList<string> remainArgs)
        {
            remainArgs = new LinkedList<string>(args);
            return command.Run(remainArgs);
        }
        
        public static void AddMethod(this Command self, Type type, string methodName, object? obj)
        {
            self.AddMethod(type.GetMethodThrow(methodName), () => obj);
        }
        
        public static void AddMethod(this Command self, Type type, string methodName, Func<object?>? objGetter)
        {
            self.AddMethod(type.GetMethodThrow(methodName), objGetter);
        }
        
        public static Command AddSubcommand(this Command self, 
            Type type, 
            string methodName, 
            object? obj, 
            string? commandName = null, 
            string? group = null)
        {
            return AddSubcommand(self, type, methodName, () => obj, commandName, group);
        }
        
        public static Command AddSubcommand(this Command self, 
            Type type, 
            string methodName, 
            Func<object?>? objGetter, 
            string? commandName = null, 
            string? group = null)
        {
            // todo 支持attribute
            var command = new Command(commandName ?? methodName, null);
            command.AddMethod(type, methodName, objGetter);
            
            self.AddSubcommand(command, group);
            return command;
        }
    }
}