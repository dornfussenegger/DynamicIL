using ILGenerator.Interfaces;

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
