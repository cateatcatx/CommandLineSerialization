using System;
using System.Collections.Generic;
using System.Reflection;

namespace Decoherence.CommandLineSerialization
{
    public class CommandSearcher
    {
        public IEnumerable<Tuple<ICommand, string?>> SearchCommands<T>()
            => SearchCommands(typeof(T), null);
        
        public IEnumerable<Tuple<ICommand, string?>> SearchCommands<T>(object obj)
            => SearchCommands(typeof(T), () => obj);
        
        public IEnumerable<Tuple<ICommand, string?>> SearchCommands<T>(Func<object?>? objGetter)
            => SearchCommands(typeof(T), objGetter);

        public IEnumerable<Tuple<ICommand, string?>> SearchCommands(Type type)
            => SearchCommands(type, null);
        
        public IEnumerable<Tuple<ICommand, string?>> SearchCommands(Type type, Func<object?>? objGetter)
        {
            var commandGroupAttr = type.GetCustomAttribute<CommandGroupAttribute>(true);

            foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var commandAttr = method.GetCustomAttribute<CommandAttribute>(true);
                if (commandAttr == null)
                {
                    continue;
                }

                var name = commandAttr.Name ?? method.Name;
                var desc = commandAttr.Desc;
                var c = OnCreateCommand(type, method, commandAttr, objGetter, name, desc);
                yield return new Tuple<ICommand, string?>(c, commandGroupAttr?.Group);
            }
        }
        
        protected virtual ICommand OnCreateCommand(Type type, MethodBase method, CommandAttribute attr, Func<object?>? objGetter, string name, string? desc)
        {
            return new Command(name, desc);
        }
    }
}