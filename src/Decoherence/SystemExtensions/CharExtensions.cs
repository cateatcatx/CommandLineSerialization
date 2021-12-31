namespace Decoherence.SystemExtensions
{
    public static class CharExtensions
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
}