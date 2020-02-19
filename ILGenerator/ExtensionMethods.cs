using ILGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ILGenerator
{
    public static class ExtensionMethods
    {
        public static T GetPropertyValue<T>(this IPropertyGetAndSet obj, string propertyName)
        {
            return (T)obj.GetPropertyValue(propertyName);
        }

        public static bool IsAnonymousType(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                   && type.IsGenericType && type.Name.Contains("AnonymousType")
                   && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                   && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }


        /// <summary>
        ///     Gets the CS Type Code for a type: Credits: https://stackoverflow.com/questions/2448800/given-a-type-instance-how-to-get-generic-type-name-in-c/22824808
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">type</exception>
        public static string GetCSTypeName(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (type == typeof(string))
            {
                return "string";
            }
            if (type == typeof(object))
            {
                return "object";
            }
            if (type == typeof(bool))
            {
                return "bool";
            }
            if (type == typeof(char))
            {
                return "char";
            }
            if (type == typeof(int))
            {
                return "int";
            }
            if (type == typeof(float))
            {
                return "float";
            }
            if (type == typeof(double))
            {
                return "double";
            }
            if (type == typeof(long))
            {
                return "long";
            }
            if (type == typeof(ulong))
            {
                return "ulong";
            }
            if (type == typeof(uint))
            {
                return "uint";
            }
            if (type == typeof(byte))
            {
                return "byte";
            }
            if (type == typeof(long))
            {
                return "Int64";
            }
            if (type == typeof(short))
            {
                return "short";
            }
            if (type == typeof(decimal))
            {
                return "decimal";
            }
            if (type.IsGenericType)
            {
                return $"{ToGenericTypeString(type)}";
            }
            if (type.IsArray)
            {
                List<string> arrayLength = new List<string>();
                for (int i = 0; i < type.GetArrayRank(); i++)
                {
                    arrayLength.Add("[]");
                }
                return GetCSTypeName(type.GetElementType()) + string.Join("", arrayLength).Replace("+", ".");
            }
            return type.FullName.Replace("+", ".");
        }

        /// <summary>
        ///     To the cs reservated word. Credits: https://stackoverflow.com/questions/2448800/given-a-type-instance-how-to-get-generic-type-name-in-c/22824808
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="fullName">if set to <c>true</c> [full name].</param>
        /// <returns></returns>
        public static string ToCSReservatedWord(this Type type, bool fullName)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (type == typeof(string))
            {
                return "string";
            }
            if (type == typeof(object))
            {
                return "object";
            }
            if (type == typeof(bool))
            {
                return "bool";
            }
            if (type == typeof(char))
            {
                return "char";
            }
            if (type == typeof(int))
            {
                return "int";
            }
            if (type == typeof(float))
            {
                return "float";
            }
            if (type == typeof(double))
            {
                return "double";
            }
            if (type == typeof(long))
            {
                return "long";
            }
            if (type == typeof(ulong))
            {
                return "ulong";
            }
            if (type == typeof(uint))
            {
                return "uint";
            }
            if (type == typeof(byte))
            {
                return "byte";
            }
            if (type == typeof(long))
            {
                return "Int64";
            }
            if (type == typeof(short))
            {
                return "short";
            }
            if (type == typeof(decimal))
            {
                return "decimal";
            }
            if (fullName)
            {
                return type.FullName;
            }
            return type.Name;
        }

        /// <summary>
        /// Credits: https://stackoverflow.com/questions/2448800/given-a-type-instance-how-to-get-generic-type-name-in-c/22824808
        /// </summary>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static string ToGenericTypeString(this Type t, params Type[] arg)
        {
            if (t.IsGenericParameter || t.FullName == null)
            {
                return t.FullName; //Generic argument stub
            }

            bool isGeneric = t.IsGenericType || t.FullName.IndexOf('`') >= 0;
            //an array of generic types is not considered a generic type although it still have the genetic notation
            bool isArray = !t.IsGenericType && t.FullName.IndexOf('`') >= 0;
            Type genericType = t;
            while (genericType.IsNested && genericType.DeclaringType.GetGenericArguments().Count() == t.GetGenericArguments().Count())
            //Non generic class in a generic class is also considered in Type as being generic
            {
                genericType = genericType.DeclaringType;
            }
            if (!isGeneric)
            {
                return ToCSReservatedWord(t, true).Replace('+', '.');
            }

            var arguments = arg.Any() ? arg : t.GetGenericArguments();
            //if arg has any then we are in the recursive part, note that we always must take arguments from t, since only t (the last one) will actually have the constructed type arguments and all others will just contain the generic parameters
            string genericTypeName = genericType.ToCSReservatedWord(true);
            if (genericType.IsNested)
            {
                //Debug.Assert(genericType.DeclaringType != null, "genericType.DeclaringType != null");
                var argumentsToPass = arguments.Take(genericType.DeclaringType.GetGenericArguments().Count()).ToArray();
                //Only the innermost will return the actual object and only from the GetGenericArguments directly on the type, not on the on genericDfintion, and only when all parameters including of the innermost are set
                arguments = arguments.Skip(argumentsToPass.Count()).ToArray();
                genericTypeName = $"{genericType.DeclaringType.ToGenericTypeString(argumentsToPass)}.{ToCSReservatedWord(genericType, false)}"; //Recursive
            }
            if (isArray)
            {
                genericTypeName = t.GetElementType().ToGenericTypeString() + "[]"; //this should work even for multidimensional arrays
            }
            if (genericTypeName.IndexOf('`') >= 0)
            {
                genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf('`'));
                string genericArgs = string.Join(", ", arguments.Select(a => a.ToGenericTypeString()).ToArray());
                //Recursive
                genericTypeName = genericTypeName + "<" + genericArgs + ">";
                if (isArray)
                {
                    genericTypeName += "[]";
                }
            }
            if (t != genericType)
            {
                genericTypeName += t.FullName.Replace(genericType.ToCSReservatedWord(true), "").Replace('+', '.');
            }
            if (genericTypeName.IndexOf("[", StringComparison.Ordinal) >= 0 && genericTypeName.IndexOf("]", StringComparison.Ordinal) != genericTypeName.IndexOf("[", StringComparison.Ordinal) + 1)
            {
                genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf("[", StringComparison.Ordinal));
                //For a non generic class nested in a generic class we will still have the type parameters at the end 
            }

            return genericTypeName;
        }


    }  
}
