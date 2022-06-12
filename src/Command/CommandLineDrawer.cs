using System.Collections.Generic;
using System.Linq;

namespace Decoherence.CommandLineSerialization;

public class CommandLineDrawer
{
    public static int CalculateShowLength(string str)
    {
        int l = 0;
        foreach (var ch in str)
        {
            l += IsAscii(ch) ? 1 : 2;
        }

        return l;
    }
    
    public static bool IsAscii(char c) => (uint)c <= '\x007f';
    
    private readonly CommandLineWriter mWriter;

    public CommandLineDrawer(CommandLineWriter writer)
    {
        mWriter = writer;
    }
        
    public void DrawCommand(ICommand command)
    {
        _DrawUseage(command);
            
        if (command.Desc != null)
        {
            mWriter.WriteLine();
            mWriter.WriteLine(command.Desc);
        }

        var drawGroup = command.Groups.Count() >= 2;
        int minLength = 0;
        List<string> optionExplainHeads = new();
        List<string> argumentExplainHeads = new();
        foreach (var option in command.Options)
        {
            var head = option.GetDrawExplainHead();
            var l = head.Length + mWriter.IndentStr.Length;
            if (l > minLength)
            {
                minLength = l;
            }
            optionExplainHeads.Add(head);
        }
        foreach (var argument in command.Arguments)
        {
            var head = argument.GetDrawExplainHead();
            var l = head.Length + mWriter.IndentStr.Length;
            if (l > minLength)
            {
                minLength = l;
            }
            argumentExplainHeads.Add(head);
        }
        foreach (var subcommand in command.Subcommands)
        {
            // 如果画Group则会缩进2次
            // todo 这里的算法要重构一下，在获取Head的时候要考虑缩进，后面真正画的时候就不用考虑了，这样可以只在前面考虑一次
            var l = drawGroup ? subcommand.Name.Length + 2 * mWriter.IndentStr.Length : subcommand.Name.Length + mWriter.IndentStr.Length;
            if (l > minLength)
            {
                minLength = l;
            }
        }

        _DrawOptions(command.Options, optionExplainHeads, minLength);
        _DrawArguments(command.Arguments, argumentExplainHeads, minLength);
        _DrawSubcommands(command, minLength, drawGroup);
            
        mWriter.WriteLine();
        mWriter.WriteLine($"See '{command.Name} help <subcommand>' to read about a specific subcommand.");
    }
        
    private void _DrawUseage(ICommand command)
    {
        var head = $"usage: {command.Name}";
        mWriter.Write(head);
        mWriter.IndentLevel = $"{head} ".Length;

        // draw options
        foreach (var option in command.Options)
        {
            mWriter.Write($" {option.GetDrawUsageHead()}");
        }

        var anySubcommand = command.Subcommands.Any();
        if (command.Arguments.Any() || anySubcommand)
        {
            mWriter.Write(" [--]");
        }

        // draw arguments
        foreach (var argument in command.Arguments)
        {
            mWriter.Write($" {argument.GetDrawUsageHead()}");
        }

        if (anySubcommand)
        {
            mWriter.Write($" <subcommand> [sub-options] [sub-args]");
        }
            
        mWriter.IndentLevel = 0;
        mWriter.WriteLine();
    }

    private void _DrawOptions(IReadOnlyList<IOption> options, IReadOnlyList<string> explainHeads, int minLength)
    {
        if (options.Count <= 0)
            return;
            
        mWriter.WriteLine();
        mWriter.WriteLine("Available options");
        mWriter.IncreaseIndent();

        for (int i = 0; i < options.Count; ++i)
        {
            var option = options[i];
            var head = explainHeads[i];

            head = _GetHead(minLength, head);
                
            mWriter.Write(head);
            _WriteDesc(mWriter, option.Desc);
                
            mWriter.WriteLine();
        }
            
        mWriter.DecreaseIndent();
    }

    private void _DrawArguments(IReadOnlyList<IArgument> arguments, IReadOnlyList<string> explainHeads, int minLength)
    {
        if (arguments.Count <= 0)
            return;
            
        mWriter.WriteLine();
        mWriter.WriteLine("Available args");
        mWriter.IncreaseIndent();

        for (var i = 0; i < arguments.Count; ++i)
        {
            var argument = arguments[i];
            var head = explainHeads[i];
                
            head = _GetHead(minLength, head);
                
            mWriter.Write(head);
            _WriteDesc(mWriter, argument.Desc);
                
            mWriter.WriteLine();
        }
            
        mWriter.DecreaseIndent();
    }

    private void _DrawSubcommands(ICommand command, int minLength, bool anyGroup)
    {
        if (!command.Subcommands.Any())
            return;

        mWriter.WriteLine();
        mWriter.WriteLine("Available subcommands");
        mWriter.IncreaseIndent();

        var groups = command.Groups;
        foreach (var group in groups)
        {
            if (anyGroup)
            {
                mWriter.WriteLine(group);
                mWriter.IncreaseIndent();
            }

            var subcommands = command.Group2Subcommands[group];
            foreach (var subcommand in subcommands)
            {
                var head = _GetHead(minLength, subcommand.Name);
                
                mWriter.Write(head);
                _WriteDesc(mWriter, subcommand.Desc);
                
                mWriter.WriteLine();
            }
                
            if (anyGroup)
            {
                mWriter.DecreaseIndent();
            }
        }

        mWriter.DecreaseIndent();
    }
        
    private void _WriteDesc(CommandLineWriter writer, string? desc)
    {
        if (desc != null)
        {
            var oldLevel = writer.IndentLevel;
            writer.IndentLevel = writer.CurCharCount;

            writer.Write(desc, true);

            writer.IndentLevel = oldLevel;
        }
    }

    private string _GetHead(int minLength, string head)
    {
        minLength -= mWriter.IndentLevel;
        
        return minLength >= 0 && head.Length < minLength
            ? $"{head}{new string(' ', minLength - head.Length)} : "
            : $"{head} : ";
    }
}