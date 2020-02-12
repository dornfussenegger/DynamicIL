namespace ILGenerator.Interfaces
{
    public interface IPropertyGetAndSet
    {
        void SetPropertyValue(string propertyName, object value);
        object GetPropertyValue(string propertyName);
    }



}
