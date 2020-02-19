using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ILGenerator
{
    /// <summary>
    ///     ReflectorUtil
    /// </summary>
    public static class ReflectorUtil
    {
        /// <summary>
        ///     Follows the property path.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        ///     value
        ///     or
        ///     path
        /// </exception>
        public static object FollowPropertyPath(object value, string path)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            Type currentType = value.GetType();

            object obj = value;
            foreach (string propertyName in path.Split('.'))
            {
                if (currentType != null)
                {
                    int brackStart = propertyName.IndexOf("[", StringComparison.Ordinal);
                    int brackEnd = propertyName.IndexOf("]", StringComparison.Ordinal);

                    var property = currentType.GetProperty(brackStart > 0 ? propertyName.Substring(0, brackStart) : propertyName);
                    if (property != null)
                    {
                        obj = property.GetValue(obj, null);
                    }

                    if (brackStart > 0)
                    {
                        string index = propertyName.Substring(brackStart + 1, brackEnd - brackStart - 1);
                        foreach (Type iType in obj.GetType().GetInterfaces())
                        {
                            if (iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                            {
                                obj = typeof(ReflectorUtil).GetMethod("GetDictionaryElement")
                                    .MakeGenericMethod(iType.GetGenericArguments())
                                    .Invoke(null, new[] { obj, index });
                                break;
                            }
                            if (iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IList<>))
                            {
                                obj = typeof(ReflectorUtil).GetMethod("GetListElement")
                                    .MakeGenericMethod(iType.GetGenericArguments())
                                    .Invoke(null, new[] { obj, index });
                                break;
                            }
                        }
                    }

                    currentType = obj?.GetType(); //property.PropertyType;
                }
                else
                {
                    return null;
                }
            }
            return obj;
        }

        /// <summary>
        ///     Follows the property information by path.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        ///     value
        ///     or
        ///     path
        /// </exception>
        public static PropertyInfoReference FollowPropertyInfoByPath(object value, string path)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            Type currentType = value.GetType();
            Type parentType = value.GetType();

            object obj = value;
            PropertyInfo property = null;
            foreach (string propertyName in path.Split('.'))
            {
                if (currentType != null)
                {
                    int brackStart = propertyName.IndexOf("[", StringComparison.Ordinal);
                    int brackEnd = propertyName.IndexOf("]", StringComparison.Ordinal);

                    property = currentType.GetProperty(brackStart > 0 ? propertyName.Substring(0, brackStart) : propertyName);
                    if (property != null)
                    {
                        obj = property.GetValue(obj, null);
                    }

                    try
                    {
                        if (brackStart > 0)
                        {
                            string index = propertyName.Substring(brackStart + 1, brackEnd - brackStart - 1);
                            foreach (Type iType in obj.GetType().GetInterfaces())
                            {
                                if (iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                                {
                                    obj = typeof(ReflectorUtil).GetMethod("GetDictionaryElement")
                                        .MakeGenericMethod(iType.GetGenericArguments())
                                        .Invoke(null, new[] { obj, index });
                                    break;
                                }
                                if (iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IList<>))
                                {
                                    obj = typeof(ReflectorUtil).GetMethod("GetListElement")
                                        .MakeGenericMethod(iType.GetGenericArguments())
                                        .Invoke(null, new[] { obj, index });
                                    break;
                                }
                            }
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        return null;
                    }
                    parentType = currentType;
                    currentType = obj?.GetType(); //property.PropertyType;
                }
                else
                {
                    return null;
                }
            }
            return new PropertyInfoReference { PropertyInfo = property, Path = path, PropertyHolderType = parentType, Value = obj };
        }

        /// <summary>
        ///     Gets the dictionary element.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dict">The dictionary.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public static TValue GetDictionaryElement<TKey, TValue>(IDictionary<TKey, TValue> dict, object index)
        {
            TKey key = (TKey)Convert.ChangeType(index, typeof(TKey), null);
            return dict[key];
        }

        /// <summary>
        ///     Gets the list element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public static T GetListElement<T>(IList<T> list, object index)
        {
            if (list.Count - 1 >= Convert.ToInt32(index))
            {
                return list[Convert.ToInt32(index)];
            }
            return default(T);
        }


        /// <summary>
        ///     PropertyInfoReference
        /// </summary>
        public class PropertyInfoReference
        {
            /// <summary>
            ///     Gets the property information.
            /// </summary>
            /// <value>
            ///     The property information.
            /// </value>
            public PropertyInfo PropertyInfo { get; internal set; }

            /// <summary>
            ///     Gets the type of the property holder.
            /// </summary>
            /// <value>
            ///     The type of the property holder.
            /// </value>
            public Type PropertyHolderType { get; internal set; }

            /// <summary>
            ///     Gets the path.
            /// </summary>
            /// <value>
            ///     The path.
            /// </value>
            public string Path { get; internal set; }

            /// <summary>
            ///     Gets the value.
            /// </summary>
            /// <value>
            ///     The value.
            /// </value>
            public object Value { get; internal set; }
        }

        /// <summary>
        ///     PropertyInfoReference
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class PropertyInfoReference<T>
        {
            /// <summary>
            ///     Gets the property information.
            /// </summary>
            /// <value>
            ///     The property information.
            /// </value>
            public PropertyInfo PropertyInfo { get; internal set; }

            /// <summary>
            ///     Gets the type of the property holder.
            /// </summary>
            /// <value>
            ///     The type of the property holder.
            /// </value>
            public Type PropertyHolderType { get; internal set; }

            /// <summary>
            ///     Gets the path.
            /// </summary>
            /// <value>
            ///     The path.
            /// </value>
            public string Path { get; internal set; }

            /// <summary>
            ///     Gets the value.
            /// </summary>
            /// <value>
            ///     The value.
            /// </value>
            public T Value { get; internal set; }
        }
    }
}
