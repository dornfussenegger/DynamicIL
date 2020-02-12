using System.Collections.Generic;
using System.Linq;

namespace ILGenerator.BaseClasses
{
    public class ChangeTracker
    {

        public bool Enabled { get; set; } = true;

        public object Object { get; set; }
        public void ResetChanges()
        {
            this.changes.Clear();
        }

        private readonly List<Change> changes = new List<Change>();

        public IReadOnlyCollection<Change> Changes => changes;

        public void Changed(string propertyName, object oldValue, object newValue)
        {
            if (Enabled)
            {
                changes.Add(new Change(propertyName, oldValue, newValue));
            }
        }

        public bool HasChangedProperty(string propertyName)
        {
            return changes.Any(w => w.PropertyName == propertyName);
        }
        public bool HasChanges()
        {
            return changes.Any();
        }
        public IEnumerable<Change> GetPropertyChanges(string propertyName)
        {
            return Changes.Where(w => w.PropertyName == propertyName);
        }

    }
}
