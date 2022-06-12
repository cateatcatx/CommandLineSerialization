namespace Decoherence.Logging
{
#if HIDE_DECOHERENCE
    internal interface ILogger
#else
    public interface ILogger
#endif
    {
        void Write(LogLevel logLevel, string message);
    }
}