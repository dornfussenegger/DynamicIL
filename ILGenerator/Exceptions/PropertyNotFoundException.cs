using System;

namespace ILGenerator.Exceptions
{
    public class PropertyNotFoundException : Exception
    {
        public PropertyNotFoundException(Type type, string propertyName)
            : base($"Did not find Property '{propertyName}' on the Type '{type.FullName}'.")
        {
            Type = type;
            PropertyName = propertyName;
        }
        public Type Type { get; private set; }
        public string PropertyName { get; private set; }
    }
}
