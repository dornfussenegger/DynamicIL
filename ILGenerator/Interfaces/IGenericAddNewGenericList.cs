namespace ILGenerator.Interfaces
{
    public interface IGenericAddNewGenericList<T> : System.Collections.Generic.IList<T>
    {
        T CreateAndAdd();
        T Create();
        T[] CreateAndAdd(int count);
    }
}
