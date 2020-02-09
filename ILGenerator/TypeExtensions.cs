using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (i is BaseClasses.NotifyPropertyChangedBaseWithChangeTracker)
            {
                ((BaseClasses.NotifyPropertyChangedBaseWithChangeTracker)i).ChangeTracker.Enabled = false;
            }
            if (i is Interfaces.IInitializable)
            {
                ((Interfaces.IInitializable)i).Initialize();
            }
            if (i is BaseClasses.NotifyPropertyChangedBaseWithChangeTracker)
            {
                ((BaseClasses.NotifyPropertyChangedBaseWithChangeTracker)i).ChangeTracker.Enabled = true;
            }
            return i;
        }
        public static BaseClasses.IGenericAddNewList CreateInstanceOfList(this Type t)
        {
            var tg = typeof(BaseClasses.GenericAddNewList<>);
            var gl = tg.MakeGenericType(new Type[] { t });
            var o = Activator.CreateInstance(gl);
            return (BaseClasses.IGenericAddNewList)o;
        }

        public static Interfaces.IPropertyGetAndSet CreateInstanceWithPropertyGetAndSet(this Type t)
        {
            return (Interfaces.IPropertyGetAndSet)CreateInstance(t);
        }
    }
}
