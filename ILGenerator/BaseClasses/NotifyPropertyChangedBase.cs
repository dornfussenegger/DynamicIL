using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILGenerator.BaseClasses
{
    public abstract class NotifyPropertyChangedBase : System.ComponentModel.INotifyPropertyChanged, Interfaces.IPropertyGetAndSet
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if (!object.Equals(oldValue,newValue))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public abstract object GetPropertyValue(string propertyName);
        public abstract void SetPropertyValue(string propertyName, object value);
    }
}
