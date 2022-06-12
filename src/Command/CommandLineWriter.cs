using System;
using System.Threading;

namespace Decoherence.CommandLineSerialization;

public class CommandLineWriter
{
    public int IndentLevel
    {
        get => mLinePrefix.Length;
        set => mLinePrefix = new string(' ', value);
    }

    /// <summary>
    /// 当前行的字符数，包括prefix
    /// </summary>
    public int CurCharCount { get; private set; }

    /// <summary>
    /// 当前行剩余可写的字符数
    /// </summary>
    public int LeftCountCanWrite => CurCharCount == 0 ? mLineMaxLength - mLinePrefix.Length : mLineMaxLength - CurCharCount;
        
    public readonly string IndentStr;

    /// <summary>
    /// 包括prefix
    /// </summary>
    private readonly int mLineMaxLength;
    private readonly PrintFunc mPrintOut;
    private readonly PrintFunc mPrintErr;
    private string mLinePrefix;

    public CommandLineWriter(int lineMaxLength, string indentStr, PrintFunc printOut, PrintFunc printErr)
    {
        mLineMaxLength = lineMaxLength;
        IndentStr = indentStr;
        mPrintOut = printOut;
        mPrintErr = printErr;
        mLinePrefix = string.Empty;
    }

    public void IncreaseIndent()
    {
        mLinePrefix += IndentStr;
    }

    public void DecreaseIndent()
    {
        if (mLinePrefix.Length >= IndentStr.Length)
        {
            mLinePrefix = mLinePrefix.Remove(0, IndentStr.Length);
        }
    }

    public void Write(string content) => Write(content, false);
        
    public void Write(string content, bool canTruncate)
    {
        if (!canTruncate)
        {
            _Write(content);
            return;
        }

        while (content.Length > LeftCountCanWrite)
        {
            var tmp = content.Substring(LeftCountCanWrite);
                
            _Write(content.Substring(0, LeftCountCanWrite));

            content = tmp;
            if (content.Length > 0)
            {
                WriteLine();
            }
        }

        if (content.Length > 0)
        {
            _Write(content);
        }
    }

    public void WriteLine()
    {
        mPrintOut(null, true);
        CurCharCount = 0;
    }

    public void WriteLine(string content)
    {
        Write(content);
        WriteLine();
    }

    private void _Write(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return;
            
        if (LeftCountCanWrite <= 0)
        {
            WriteLine();
        }

        if (CurCharCount == 0)
        {
            // 如果是第一个字符，则去掉开头的空格
            content = content.TrimStart(' ');

            // 加上前缀
            if (mLinePrefix.Length > 0)
            {
                content = $"{mLinePrefix}{content}";
            }
        }

        mPrintOut(content, false);
        CurCharCount += content.Length;
    }
}