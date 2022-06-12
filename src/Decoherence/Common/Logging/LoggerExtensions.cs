namespace Decoherence.Logging;
#if HIDE_DECOHERENCE
internal static class LoggerExtensions
#else
    public static class LoggerExtensions
#endif
{
    public static void Verbose(this ILogger self, string message)
    {
        self.Write(LogLevel.Verbose, message);
    }

    public static void Debug(this ILogger self, string message)
    {
        self.Write(LogLevel.Debug, message);
    }

    public static void Information(this ILogger self, string message)
    {
        self.Write(LogLevel.Information, message);
    }

    public static void Warning(this ILogger self, string message)
    {
        self.Write(LogLevel.Warning, message);
    }

    public static void Error(this ILogger self, string message)
    {
        self.Write(LogLevel.Error, message);
    }

    public static void Fatal(this ILogger self, string message)
    {
        self.Write(LogLevel.Fatal, message);
    }
}