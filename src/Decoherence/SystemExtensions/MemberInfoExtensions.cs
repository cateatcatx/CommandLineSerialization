using System;
using System.Reflection;

namespace Decoherence.SystemExtensions;
#if HIDE_DECOHERENCE
internal static class MemberInfoExtensions
#else
    public static class MemberInfoExtensions
#endif
{
    public static bool CanWrite(this MemberInfo memberInfo)
    {
        if (memberInfo is PropertyInfo propertyInfo)
        {
            return propertyInfo.CanWrite;
        }

        return memberInfo is FieldInfo;
    }

    public static bool CanRead(this MemberInfo memberInfo)
    {
        if (memberInfo is PropertyInfo propertyInfo)
        {
            return propertyInfo.CanRead;
        }

        return memberInfo is FieldInfo;
    }

    public static void SetValue(this MemberInfo memberInfo, object? obj, object? value)
    {
        if (memberInfo is PropertyInfo propertyInfo)
        {
            propertyInfo.SetValue(obj, value);
        }
        else if (memberInfo is FieldInfo fieldInfo)
        {
            fieldInfo.SetValue(obj, value);
        }
        else
        {
            throw new InvalidOperationException($"{memberInfo} can not set value.");
        }
    }

    public static object? GetValue(this MemberInfo memberInfo, object? obj)
    {
        if (memberInfo is PropertyInfo propertyInfo)
        {
            return propertyInfo.GetValue(obj);
        }

        if (memberInfo is FieldInfo fieldInfo)
        {
            return fieldInfo.GetValue(obj);
        }

        throw new InvalidOperationException($"{memberInfo} can not get value.");
    }
}