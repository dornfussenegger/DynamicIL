namespace ILGenerator.Interfaces
{
    public interface IGenericAddNewList : System.Collections.IList
    {
        object CreateAndAdd();
        object Create();
        object[] CreateAndAdd(int count);
    }
}
