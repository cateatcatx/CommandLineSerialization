using System;
using System.Collections.Generic;
using System.Text;

namespace Decoherence.CommandLineSerialization;

public abstract class Spec : ISpec
{
    private const string InvalidOperationError = "No specified ValueSerializer.";

    public string ValueName { get; }
    public ValueType ValueType { get; }
    public string? Desc { get; }
    public Type ObjType { get; }

    protected readonly IValueSerializer? mValueSerializer;
    private readonly HashSet<string>? mLimitValues;

    protected Spec(ValueType valueType, Type objType, string? valueName, string? desc, IValueSerializer? valueSerializer)
    {
        ValueName = valueName ?? "ARG";
        ValueType = valueType;
        ObjType = objType;
        Desc = desc;
        mValueSerializer = valueSerializer;
            
        mLimitValues = _CalculateLimitValues();
    }

    public bool CanHandleType(Type objType)
    {
        return mValueSerializer != null && objType == ObjType;
    }

    public ObjectSpecs? GetObjectSpecs(Type objType)
    {
        if (mValueSerializer == null)
            throw new InvalidOperationException(InvalidOperationError);

        return mValueSerializer.GetObjectSpecs(objType);
    }

    public object? DeserializeNonValue(CommandLineSerializer serializer, Type objType, bool matched)
    {
        if (mValueSerializer == null)
            throw new InvalidOperationException(InvalidOperationError);
            
        return mValueSerializer.DeserializeNonValue(serializer, objType, matched);
    }

    public object? DeserializeSingleValue(CommandLineSerializer serializer, Type objType, string? value)
    {
        if (mValueSerializer == null)
            throw new InvalidOperationException(InvalidOperationError);
            
        return mValueSerializer.DeserializeSingleValue(serializer, objType, value);
    }

    public object? DeserializeMultiValue(CommandLineSerializer serializer, Type objType, List<string> values)
    {
        if (mValueSerializer == null)
            throw new InvalidOperationException(InvalidOperationError);

        return mValueSerializer.DeserializeMultiValue(serializer, objType, values);
    }

    public bool SerializeNonValue(CommandLineSerializer serializer, Type objType, object? obj)
    {
        if (mValueSerializer == null)
            throw new InvalidOperationException(InvalidOperationError);
            
        return mValueSerializer.SerializeNonValue(serializer, objType, obj);
    }

    public string SerializeSingleValue(CommandLineSerializer serializer, Type objType, object? obj)
    {
        if (mValueSerializer == null)
            throw new InvalidOperationException(InvalidOperationError);
            
        return mValueSerializer.SerializeSingleValue(serializer, objType, obj);
    }

    public IEnumerable<string> SerializeMultiValue(CommandLineSerializer serializer, Type objType, object? obj)
    {
        if (mValueSerializer == null)
            throw new InvalidOperationException(InvalidOperationError);
            
        return mValueSerializer.SerializeMultiValue(serializer, objType, obj);
    }
        
    public HashSet<string>? GetLimitValues()
    {
        return mLimitValues;
    }
        
    public abstract string GetDrawUsageHead();
    public abstract string GetDrawExplainHead();

    protected string DrawExplainValue(StringBuilder? sb)
    {
        sb ??= new StringBuilder();
            
        sb.Append($"{ValueName}");
        var limitValues = GetLimitValues();
        if (limitValues != null && limitValues.Count > 0)
        {
            sb.Append(": ");
            foreach (var limitValue in limitValues)
            {
                sb.Append($"{limitValue}|");
            }
            sb.Remove(sb.Length - 1, 1);
        }

        return sb.ToString();
    }

    private HashSet<string>? _CalculateLimitValues()
    {
        var t = ObjType;
        if (t.Name == "Nullable`1")
        {
            t = t.GenericTypeArguments[0];
        }
            
        if (t.IsEnum)
        {
            var names = Enum.GetNames(t);
            return new HashSet<string>(names);
        }
        else
        {
            return null;
        }
    }
}