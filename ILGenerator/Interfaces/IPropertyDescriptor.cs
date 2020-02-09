using System;

namespace ILGenerator.Interfaces
{
    public interface IPropertyDescriptor
    {
        string[] GetAllPropertyNames();
        Type[] GetAllPropertyTypes();

    }
}
