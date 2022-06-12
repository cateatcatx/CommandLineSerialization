namespace Decoherence.SystemExtensions;
#if HIDE_DECOHERENCE
internal static class CharExtensions
#else
#if HIDE_DECOHERENCE
    internal static class CharExtensions
#else
    public static class CharExtensions
#endif
#endif
{
    /// <summary>
    /// 是否为字母
    /// </summary>
    public static bool IsAlpha(this char self)
    {
        return ('A' <= self && self <= 'Z') || ('a' <= self && self <= 'z');
    }

    /// <summary>
    /// 是否为数字
    /// </summary>
    public static bool IsDigit(this char self)
    {
        return '0' <= self && self <= '9';
    }
}