using System;

namespace ILGenerator.BaseClasses
{
    public class Change
    {
        public string PropertyName { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
        public DateTime Timestamp { get; set; }

        public Change(string propertyName, object oldValue, object newValue)
        {
            this.PropertyName = propertyName;
            this.OldValue = oldValue;
            this.NewValue = newValue;
            this.Timestamp = DateTime.Now;
        }
    }
}
