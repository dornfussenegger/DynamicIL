using ILGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILGenerator
{
    public static class ExtensionMethods
    {
        public static T GetPropertyValue<T>(this IPropertyGetAndSet obj, string propertyName)
        {
            return (T)obj.GetPropertyValue(propertyName);
        }
    }
}
