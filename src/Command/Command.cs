using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Decoherence.CommandLineSerialization.Attributes;
using Decoherence.SystemExtensions;

namespace Decoherence.CommandLineSerialization;

public delegate void PrintFunc(string? content, bool newLine);
    
public class Command : ICommand
{
    private const string BUILTIN_GROUP = "Default utilities";
        
    public event Action? BeforeAllMethod;
    public event Action? AfterAllMethod;
    public event Action<MethodBase, ParameterInfo[], object?[]>? BeforeOneMethod;
    public event Action<MethodBase, object?>? AfterOneMethod;

    public string Name { get; }
    public string? Desc { get; }

    public IReadOnlyList<IOption> Options
        => mMethods.SelectMany(tuple => tuple.Item1.Options).ToList();
        
    public IReadOnlyList<IArgument> Arguments
        => mMethods.SelectMany(tuple => tuple.Item1.Arguments).ToList();
        
    public IEnumerable<ICommand> Subcommands
    {
        get
        {
            foreach (var commands in mGroup2Subcommands.Values)
            {
                foreach (var (command, _) in commands)
                {
                    yield return command;
                }
            }
        }
    }
        
    public IEnumerable<string> Groups => mGroups;

    public IReadOnlyDictionary<string, IEnumerable<ICommand>> Group2Subcommands
        => mGroup2Subcommands.ToDictionary(kv => kv.Key, kv => kv.Value.Select(tuple => tuple.Item1));

    private readonly PriorityList<Tuple<MethodSpecs, Func<object?>?, int>> mMethods;
    private readonly Dictionary<string, PriorityList<Tuple<ICommand, int>>> mGroup2Subcommands;
    private readonly PriorityList<string> mGroups;
    private readonly CommandLineDrawer mDrawer;

    private PrintFunc mPrintOut;
    private PrintFunc mPrintErr;
    private bool mAddHelp;

    public Command(
        string name, 
        string? desc, 
        int? maxLineLength = null)
    {
        mMethods = new PriorityList<Tuple<MethodSpecs, Func<object?>?, int>>((tuple1, tuple2) => tuple1.Item3 - tuple2.Item3);
        mGroup2Subcommands = new Dictionary<string, PriorityList<Tuple<ICommand, int>>>();
        mGroups = new PriorityList<string>((group1, group2) =>
        {
            // 保证BUILTIN_GROUP在最后
            int value1 = group1 == BUILTIN_GROUP ? 0 : 1;
            int value2 = group2 == BUILTIN_GROUP ? 0 : 1;
            return value1 - value2;
        });

        Name = name;
        Desc = desc;
            
        mPrintOut = _PrintOut;
        mPrintErr = _PrintErr;
        mDrawer = new CommandLineDrawer(new CommandLineWriter(maxLineLength ?? 120, "  ", mPrintOut, mPrintErr));
    }

    public void SetPrintOut(PrintFunc? func)
    {
        mPrintOut = func ?? _PrintOut;
    }
        
    public void SetPrintErr(PrintFunc? func)
    {
        mPrintErr = func ?? _PrintErr;
    }
        
    public void AddMethod(MethodBase method, Func<object?>? objGetter = null)
    {
        ThrowUtil.ThrowIfArgumentNull(method);
            
        mMethods.Add(new Tuple<MethodSpecs, Func<object?>?, int>(new MethodSpecs(method), objGetter, 0));
    }
        
    public void AddSubcommand(ICommand command, string? group = null)
    {
        ThrowUtil.ThrowIfArgumentNull(command);

        if (Subcommands.FindIndex(item => item.Name == command.Name) >= 0)
            throw new ArgumentException($"Command with same name '{command.Name}' already exists.");

        group ??= BUILTIN_GROUP;
        if (mGroups.FindIndex(item => item == group) < 0)
        {
            // 越早注册的group排序越靠前
            mGroups.Add(group);
        }

        var commands = mGroup2Subcommands.AddOrCreateValue(group, () => new PriorityList<Tuple<ICommand, int>>((tuple1, tuple2) => tuple2.Item2 - tuple1.Item2));
        commands.Add(new Tuple<ICommand, int>(command, 0));

        // 有Subcommand才有Help
        if (!mAddHelp)
        {
            mMethods.Add(new Tuple<MethodSpecs, Func<object?>?, int>(new MethodSpecs(typeof(Command).GetMethodThrow(nameof(_HelpMethod))), () => this, -1)); // 保证help在最后
                
            mAddHelp = true;
            var helpCommand = new Command("help", "帮助");
            helpCommand.AddMethod(typeof(Command), nameof(_HelpCommand), this);
            AddSubcommand(helpCommand);
        }
    }

    public int? Run(LinkedList<string> argList)
    {
        CommandLineSerializer commandLineSerializer = new CommandLineSerializer();
            
        BeforeAllMethod?.Invoke();

        bool truncate = false;
        int? ret = null; // 返回值取最后一个（本命令加上所有后面会执行的子命令）返回的int值
        foreach ((MethodSpecs methodSpecs, Func<object?>? objGetter, _) in mMethods)
        {
            var retObj = MethodInvoker.InvokeMethod(commandLineSerializer, methodSpecs, objGetter?.Invoke(), argList, 
                (paramInfos, objects) =>
                {
                    BeforeOneMethod?.Invoke(methodSpecs.Method, paramInfos, objects);
                });
                
            // 处理Task
            if (retObj is Task task)
            {
                task.Wait();
                if (task is Task<BuiltinReturn> taskWithBuiltinReturn)
                {
                    retObj = taskWithBuiltinReturn.Result;
                }
                else if (task is Task<int> taskWithInt)
                {
                    retObj = taskWithInt.Result;
                }
                else
                {
                    // 其他类型返回值一律当null处理
                    retObj = null;
                }
            }
                
            AfterOneMethod?.Invoke(methodSpecs.Method, retObj);
                
            if (retObj is BuiltinReturn builtinReturn)
            {
                if (builtinReturn == BuiltinReturn.Truncate)
                {
                    truncate = true;
                    break;
                }
            }
            else if (retObj is int intRet)
            {
                ret = intRet;
            }
        }
            
        AfterAllMethod?.Invoke();

        if (truncate || !Subcommands.Any())
        {
            return ret;
        }
            
        var commandName = commandLineSerializer.DeserializeObject<string>(argList);
        if (string.IsNullOrWhiteSpace(commandName))
        {
            mPrintErr($"No input command, See '{Name} --help'", true);
            return 1;
        }

        var command = Subcommands.Find(item => item.Name == commandName);
        if (command == null)
        {
            mPrintErr($"No command named '{commandName}', See '{Name} --help'", true);
            return 1;
        }

        return command.Run(argList) ?? ret;
    }

    private BuiltinReturn _HelpMethod(
        [Option(ShortName = "h", LongName = "help", Desc = "Show current command help infos.")] bool showHelp)
    {
        if (showHelp)
        {
            mDrawer.DrawCommand(this);
            return BuiltinReturn.Truncate;
        }

        return BuiltinReturn.Continue;
    }

    private void _HelpCommand(
        [Argument(ValueName = "command", Desc = "待查帮助的命令")] string? commandName = null)
    {
        if (commandName == null)
        {
            mDrawer.DrawCommand(this);
            return;
        }
            
        var command = Subcommands.Find(item => item.Name == commandName);
        if (command == null)
        {
            mPrintErr($"Can not find command {commandName}", true);
            return;
        }
            
        mDrawer.DrawCommand(command);
    }

        
        
    private void _PrintErr(string? content, bool newLine)
    {
        if (newLine)
        {
            Console.Error.WriteLine(content);
        }
        else
        {
            Console.Error.Write(content);
        }
    }

    private void _PrintOut(string? content, bool newLine)
    {
        if (newLine)
        {
            Console.WriteLine(content);
        }
        else
        {
            Console.Write(content);
        }
    }
}