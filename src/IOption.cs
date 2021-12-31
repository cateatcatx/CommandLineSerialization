﻿using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public enum OptionValueType
    {
        None,
        Single,
        Multi,
        Sequence,
    }
    
    public class OptionEqualityComparer : IEqualityComparer<IOption>
    {
        public bool Equals(IOption x, IOption y)
        {
            return x.Name.Equals(y.Name);
        }

        public int GetHashCode(IOption obj)
        {
            return obj.Name.GetHashCode();
        }
    }
    
    public interface IOption : ISpec
    {
        string Name { get; }
        OptionValueType ValueType { get; }
    }
}