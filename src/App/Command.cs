using System;
using System.Collections.Generic;
using System.Reflection;
using Decoherence.CommandLineSerialization.Attributes;
using Decoherence.SystemExtensions;

namespace Decoherence.CommandLineSerialization
{
    public class Command
    {
        private const string BUILTIN_GROUP = "Default utilities";
        
        public event Action? BeforeMethod;
        public event Action? AfterMethod;
        
        public readonly string Name;
        public readonly string? Desc;
        public IEnumerable<Command> Subcommands
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

        private readonly PriorityList<Tuple<MethodSpecs, Func<object?>?, int>> mMethods;
        private readonly Dictionary<string, PriorityList<Tuple<Command, int>>> mGroup2Subcommands;
        private readonly PriorityList<string> mGroups;

        private bool mAddHelp;

        public Command(string name, string? desc)
        {
            mMethods = new PriorityList<Tuple<MethodSpecs, Func<object?>?, int>>((tuple1, tuple2) => tuple1.Item3 - tuple2.Item3);
            mGroup2Subcommands = new Dictionary<string, PriorityList<Tuple<Command, int>>>();
            mGroups = new PriorityList<string>((group1, group2) =>
            {
                // 保证BUILTIN_GROUP在最后
                int value1 = group1 == BUILTIN_GROUP ? 0 : 1;
                int value2 = group2 == BUILTIN_GROUP ? 0 : 1;
                return value1 - value2;
            });

            Name = name;
            Desc = desc;
        }
        
        public void AddMethod(MethodBase method, Func<object?>? objGetter)
        {
            ThrowUtil.ThrowIfArgumentNull(method);
            
            mMethods.Add(new Tuple<MethodSpecs, Func<object?>?, int>(new MethodSpecs(method), objGetter, 0));
        }
        
        public void AddSubcommand(Command command)
        {
            AddSubcommand(command, BUILTIN_GROUP);
        }
        
        public void AddSubcommand(Command command, string? group)
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

            var commands = mGroup2Subcommands.AddOrCreateValue(group, () => new PriorityList<Tuple<Command, int>>((tuple1, tuple2) => tuple2.Item2 - tuple1.Item2));
            commands.Add(new Tuple<Command, int>(command, 0));

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

        public int? Run(IEnumerable<string> args, out LinkedList<string> remainArgs)
        {
            remainArgs = new LinkedList<string>(args);
            return Run(remainArgs);
        }
        
        public int? Run(LinkedList<string> argList)
        {
            CommandLineSerializer commandLineSerializer = new CommandLineSerializer();
            
            BeforeMethod?.Invoke();

            bool truncate = false;
            int? ret = null; // 返回值取最后一个（本命令加上所有后面会执行的子命令）返回的int值
            foreach ((MethodSpecs methodSpecs, Func<object?>? objGetter, _) in mMethods)
            {
                var retObj = MethodInvoker.InvokeMethod(commandLineSerializer, methodSpecs, objGetter?.Invoke(), argList);
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
            
            AfterMethod?.Invoke();

            if (truncate || mGroup2Subcommands.Count <= 0)
            {
                return ret;
            }
            
            var commandName = commandLineSerializer.DeserializeObject<string>(argList);
            if (string.IsNullOrWhiteSpace(commandName))
            {
                throw new Exception($"No input command");
            }

            var command = Subcommands.Find(item => item.Name == commandName);
            if (command == null)
            {
                throw new Exception($"No command named {commandName}");
            }

            return command.Run(argList) ?? ret;
        }

        public void Draw(CommandLineWriter writer)
        {
            List<IOption> options = new List<IOption>();
            List<IArgument> arguments = new List<IArgument>();
            foreach (var (methodSpecs, _, _) in mMethods)
            {
                options.AddRange(methodSpecs.Options);
                arguments.AddRange(methodSpecs.Arguments);
            }
            
            _DrawUseage(options, arguments, writer);
            
            if (Desc != null)
            {
                writer.WriteLine();
                writer.WriteLine(Desc);
            }

            int minLength = 0;
            List<string> optionExplainHeads = new();
            List<string> argumentExplainHeads = new();
            foreach (var option in options)
            {
                var head = option.GetDrawExplainHead();
                if (head.Length > minLength)
                {
                    minLength = head.Length;
                }
                optionExplainHeads.Add(head);
            }
            foreach (var argument in arguments)
            {
                var head = argument.GetDrawExplainHead();
                if (head.Length > minLength)
                {
                    minLength = head.Length;
                }
                argumentExplainHeads.Add(head);
            }
            foreach (var command in Subcommands)
            {
                if (command.Name.Length > minLength)
                {
                    minLength = command.Name.Length;
                }
            }

            _DrawOptions(options, optionExplainHeads, writer, minLength);
            _DrawArguments(arguments, argumentExplainHeads, writer, minLength);
            _DrawSubcommands(writer, minLength);
            
            writer.WriteLine();
            writer.WriteLine($"See '{Name} help <subcommand>' to read about a specific subcommand.");
        }

        private BuiltinReturn _HelpMethod(
            [Option(ShortName = "h", LongName = "help", Desc = "Show current command help infos.")] bool showHelp)
        {
            if (showHelp)
            {
                CommandLineWriter writer = new CommandLineWriter(120, "  ");
                Draw(writer);
                return BuiltinReturn.Truncate;
            }

            return BuiltinReturn.Continue;
        }

        private void _HelpCommand(
            [Argument(ValueName = "command", Desc = "待查帮助的命令")] string? commandName)
        {
            if (commandName == null)
            {
                _HelpMethod(true);
                return;
            }
            
            var command = Subcommands.Find(item => item.Name == commandName);
            if (command == null)
            {
                Console.Error.WriteLine($"Can not find command {commandName}");
                return;
            }
            
            CommandLineWriter writer = new CommandLineWriter(120, "  ");
            command.Draw(writer);
        }

        private void _DrawUseage(IEnumerable<IOption> options, IEnumerable<IArgument> arguments, CommandLineWriter writer)
        {
            var head = $"usage: {Name}";
            writer.Write(head);
            writer.IndentLevel = $"{head} ".Length;

            // draw options
            foreach (var option in options)
            {
                writer.Write($" {option.GetDrawUsageHead()}");
            }

            // draw arguments
            foreach (var argument in arguments)
            {
                writer.Write($" {argument.GetDrawUsageHead()}");
            }

            if (mGroup2Subcommands.Count > 0)
            {
                writer.Write($" <subcommand> [sub-options] [sub-args]");
            }
            
            writer.IndentLevel = 0;
            writer.WriteLine();
        }

        private void _DrawOptions(IReadOnlyList<IOption> options, IReadOnlyList<string> explainHeads, CommandLineWriter writer, int minLength)
        {
            if (options.Count <= 0)
                return;
            
            writer.WriteLine();
            writer.WriteLine("Available options");
            writer.IncreaseIndent();

            for (int i = 0; i < options.Count; ++i)
            {
                var option = options[i];
                var head = explainHeads[i];

                head = _FillHead(minLength, head);
                
                writer.Write(head);
                _WriteDesc(writer, option.Desc);
                
                writer.WriteLine();
            }
            
            writer.DecreaseIndent();
        }

        private void _DrawArguments(IReadOnlyList<IArgument> arguments, IReadOnlyList<string> explainHeads, CommandLineWriter writer, int minLength)
        {
            if (arguments.Count <= 0)
                return;
            
            writer.WriteLine();
            writer.WriteLine("Available args");
            writer.IncreaseIndent();

            for (var i = 0; i < arguments.Count; ++i)
            {
                var argument = arguments[i];
                var head = explainHeads[i];
                
                head = _FillHead(minLength, head);
                
                writer.Write(head);
                _WriteDesc(writer, argument.Desc);
                
                writer.WriteLine();
            }
            
            writer.DecreaseIndent();
        }

        private void _DrawSubcommands(CommandLineWriter writer, int minLength)
        {
            if (mGroup2Subcommands.Count <= 0)
                return;

            writer.WriteLine();
            writer.WriteLine("Available subcommands");
            writer.IncreaseIndent();

            foreach (var group in mGroups)
            {
                if (mGroups.Count > 1)
                {
                    writer.WriteLine(group);
                    writer.IncreaseIndent();
                }

                var commands = mGroup2Subcommands[group];
                foreach (var (command, _) in commands)
                {
                    var head = _FillHead(minLength, command.Name);
                
                    writer.Write(head);
                    _WriteDesc(writer, command.Desc);
                
                    writer.WriteLine();
                }
                
                if (mGroups.Count > 1)
                {
                    writer.DecreaseIndent();
                }
            }

            writer.DecreaseIndent();
        }
        
        private static void _WriteDesc(CommandLineWriter writer, string? desc)
        {
            if (desc != null)
            {
                var oldLevel = writer.IndentLevel;
                writer.IndentLevel = writer.CurCharCount;

                writer.Write(desc, true);

                writer.IndentLevel = oldLevel;
            }
        }

        private static string _FillHead(int minLength, string head)
        {
            return minLength >= 0 && head.Length < minLength
                ? $"{head}{new string(' ', minLength - head.Length)} : "
                : $"{head} : ";
        }
    }
}