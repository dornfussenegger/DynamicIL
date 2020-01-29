using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILGenerator.Interfaces
{
    public interface IPropertyGetAndSet
    {
        void SetPropertyValue(string propertyName, object value);
        object GetPropertyValue(string propertyName);

    }
}
