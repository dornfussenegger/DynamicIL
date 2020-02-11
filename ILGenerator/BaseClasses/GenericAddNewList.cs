using ILGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILGenerator.BaseClasses
{
    public class GenericAddNewList<T> : System.Collections.Generic.List<T>, IGenericAddNewList, IGenericAddNewGenericList<T>
    {
        object IGenericAddNewList.Create()
        {
            return typeof(T).CreateInstance();
        }

        T IGenericAddNewGenericList<T>.Create()
        {
            return (T)typeof(T).CreateInstance();
        }

        object IGenericAddNewList.CreateAndAdd()
        {
            var i = (T)typeof(T).CreateInstance();
            this.Add(i);
            return i;
        }

        object[] IGenericAddNewList.CreateAndAdd(int count)
        {
            List<object> results = new List<object>();
            for (int i = 0; i < count; i++)
            {
                var x = (T)typeof(T).CreateInstance();
                this.Add(x);
                results.Add(x);
            }
            return results.ToArray();
        }

        T IGenericAddNewGenericList<T>.CreateAndAdd()
        {
            var i = (T)typeof(T).CreateInstance();
            this.Add(i);
            return i;
        }

        T[] IGenericAddNewGenericList<T>.CreateAndAdd(int count)
        {
            List<T> results = new List<T>();
            for (int i = 0; i < count; i++)
            {
                var x = (T)typeof(T).CreateInstance();
                this.Add(x);
                results.Add(x);
            }
            return results.ToArray();
        }
    }
}
