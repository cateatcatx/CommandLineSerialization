using System.Diagnostics.CodeAnalysis;
using Decoherence.SystemExtensions;

namespace Decoherence.CommandLineSerialization
{
    public static class SpecsExtensions
    {
        /// <summary>
        /// 通过name获取Option。LongName长度至少为2所以不用担心LongName和ShortName相同。
        /// </summary>
        /// <param name="specs"></param>
        /// <param name="name"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static bool TryGetOption(this ISpecs specs, string name, [NotNullWhen(true)] out IOption? option)
        {
            option = specs.Options.Find(item => item.ShortName?.ToString() == name || item.LongName == name);
            return option != null;
        }
    }
}