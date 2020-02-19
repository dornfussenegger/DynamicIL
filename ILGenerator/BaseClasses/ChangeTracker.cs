using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ILGenerator.BaseClasses
{
    public class ChangeTracker : System.ComponentModel.INotifyPropertyChanged
    {
        public ChangeTracker(object onObject)
        {
            this.Object = onObject;
        }

        public bool Enabled { get => enabled; set { enabled = value; OnPropertyChanged(nameof(Enabled)); } }
        public object Object { get; private set; }
        public void ResetChanges()
        {
            this.changes.Clear();
            OnPropertyChanged(nameof(HasChanges));
        }

        private readonly System.Collections.ObjectModel.ObservableCollection<Change> changes = new System.Collections.ObjectModel.ObservableCollection<Change>();
        private bool enabled = true;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public System.Collections.ObjectModel.ReadOnlyObservableCollection<Change> Changes => new System.Collections.ObjectModel.ReadOnlyObservableCollection<Change>(changes);

        public void OnChanged(string propertyName, object oldValue, object newValue)
        {
            if (Enabled)
            {
                changes.Add(new Change(propertyName, oldValue, newValue));
            }
            OnPropertyChanged(nameof(HasChanges));
        }

        public bool HasChangedProperty(string propertyName)
        {
            return changes.Any(w => w.PropertyName == propertyName);
        }

        public bool HasChanges { get { return changes.Any(); } }

        public IEnumerable<Change> GetPropertyChanges(string propertyName)
        {
            return Changes.Where(w => w.PropertyName == propertyName);
        }

    }
}
