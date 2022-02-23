using System;
using System.Reflection;
using System.Linq;

namespace Decoherence.SystemExtensions
{
#if HIDE_DECOHERENCE
    internal static class TypeExtensions
#else
    public static class TypeExtensions
#endif
    {
        /// <summary>
        /// 类型是否有无参构造函数
        /// </summary>
        public static bool HasParameterlessConstructor(this Type self, BindingFlags bindingFlags = BindingFlags.Public)
        {
            return self.GetConstructors(bindingFlags).Any(info => info.GetParameters().Length == 0);
        }
        
        /// <summary>
        /// 类型是否有不需要输入参数的构造函数（无参或所有参数都有默认值）
        /// </summary>
        public static bool HasNoNeedParameterConstructor(this Type self, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            return self.GetConstructors(bindingFlags).Any(info =>
            {
                var paramInfos = info.GetParameters();
                return paramInfos.Length == 0 || paramInfos.All(paramInfo => paramInfo.DefaultValue != DBNull.Value);
            });
        }
    }
}