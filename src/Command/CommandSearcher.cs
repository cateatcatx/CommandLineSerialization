using System;
using System.Collections.Generic;
using System.Reflection;

namespace Decoherence.CommandLineSerialization;

public class CommandSearcher
{
    public IEnumerable<Tuple<ICommand, string?>> SearchType<T>(string? group = null)
        => SearchType(typeof(T), null, group);
        
    public IEnumerable<Tuple<ICommand, string?>> SearchType<T>(object obj, string? group = null)
        => SearchType(typeof(T), () => obj, group);
        
    public IEnumerable<Tuple<ICommand, string?>> SearchType<T>(Func<object?>? objGetter, string? group = null)
        => SearchType(typeof(T), objGetter, group);

    public IEnumerable<Tuple<ICommand, string?>> SearchType(Type type, string? group = null)
        => SearchType(type, null, group);
        
    public IEnumerable<Tuple<ICommand, string?>> SearchType(Type type, Func<object?>? objGetter, string? group = null)
    {
        group ??= type.GetCustomAttribute<CommandGroupAttribute>(true)?.Group;
            
        foreach (Tuple<ICommand, string?>? tuple in _SearchType(type, objGetter, group))
            yield return tuple;
    }

    public IEnumerable<Tuple<ICommand, string?>> SearchAssembly(Assembly assem, Func<Type, Func<object?>>? getObj = null)
    {
        foreach (var type in assem.GetTypes())
        {
            var attr = type.GetCustomAttribute<CommandGroupAttribute>(true);
            if (attr == null)
            {
                continue;
            }

            var objGetter = getObj?.Invoke(type);
            foreach (var tuple in _SearchType(type, objGetter, attr.Group))
            {
                yield return tuple;
            }
        }
    }
        
    protected virtual ICommand OnCreateCommand(Type type, MethodBase method, CommandAttribute attr, Func<object?>? objGetter, string name, string? desc)
    {
        return new Command(name, desc);
    }
        
    private IEnumerable<Tuple<ICommand, string?>> _SearchType(Type type, Func<object?>? objGetter, string? group)
    {
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
            yield return new Tuple<ICommand, string?>(c, group);
        }
    }
}