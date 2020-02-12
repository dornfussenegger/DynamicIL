using System;
using ILGenerator.Interfaces;
using ILGenerator.BaseClasses;

namespace ILGenerator
{
    public static class TypeExtensions
    {
        public static T CreateInstance<T>(this Type t)
        {
            return (T)CreateInstance(t);
        }

        public static object CreateInstance(this Type t)
        {
            var i = Activator.CreateInstance(t, Array.Empty<object>());
            if (i is NotifyPropertyChangedBaseWithChangeTracker)
            {
                ((NotifyPropertyChangedBaseWithChangeTracker)i).ChangeTracker.Enabled = false;
            }
            if (i is IInitializable)
            {
                ((IInitializable)i).Initialize();
            }
            if (i is NotifyPropertyChangedBaseWithChangeTracker)
            {
                ((NotifyPropertyChangedBaseWithChangeTracker)i).ChangeTracker.Enabled = true;
            }
            return i;
        }
        public static IGenericAddNewList CreateInstanceOfList(this Type t)
        {
            var tg = typeof(GenericAddNewList<>);
            var gl = tg.MakeGenericType(new Type[] { t });
            var o = Activator.CreateInstance(gl);
            return (IGenericAddNewList)o;
        }

        public static IPropertyGetAndSet CreateInstanceWithPropertyGetAndSet(this Type t)
        {
            return (IPropertyGetAndSet)CreateInstance(t);
        }
    }
}
