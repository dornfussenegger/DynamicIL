using System.Collections;
namespace ILGenerator.Interfaces
{
    public interface IGenericAddNewList : IList
    {
        object CreateAndAdd();
        object Create();
        object[] CreateAndAdd(int count);
    }
}
