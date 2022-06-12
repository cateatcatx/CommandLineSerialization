namespace Decoherence.SystemExtensions;
#if HIDE_DECOHERENCE
internal static class StringExtensions
#else
    public static class StringExtensions
#endif
{
    /// <summary>
    /// 判断字符串是否全为字母
    /// </summary>
    public static bool IsAlpha(this string self)
    {
        if (string.IsNullOrWhiteSpace(self))
            return false;

        foreach (var ch in self)
        {
            if (!ch.IsAlpha())
                return false;
        }

        return true;
    }
        
    /// <summary>
    /// 判断字符串是否全为数字
    /// </summary>
    public static bool IsDigit(this string self)
    {
        if (string.IsNullOrWhiteSpace(self))
            return false;

        foreach (var ch in self)
        {
            if (!ch.IsDigit())
                return false;
        }

        return true;
    }
}