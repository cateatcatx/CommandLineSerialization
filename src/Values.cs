using System.Collections.Generic;

namespace Decoherence.CommandLineParsing
{
    public class Values
    {
        private readonly Dictionary<Spec, object?> mValues = new();
        
        public void AddValue(Spec spec, object? value)
        {
            mValues.Add(spec, value);
        }
        
        public bool TryGetValue<T>(Spec spec, out T? value)
        {
            value = default(T);
            
            if (!mValues.TryGetValue(spec, out var obj))
            {
                return false;
            }

            value = (T?)obj;
            return true;
        }
        
        public bool TryGetValue(Spec spec, out object? value)
        {
            return mValues.TryGetValue(spec, out value);
        }
    }
}