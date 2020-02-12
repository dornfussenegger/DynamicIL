using System.Collections.Generic;
namespace ILGenerator.Interfaces
{
    public interface IGenericAddNewGenericList<T> : IList<T>
    {
        T CreateAndAdd();
        T Create();
        T[] CreateAndAdd(int count);
    }
}
