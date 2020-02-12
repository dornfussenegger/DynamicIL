using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ILGenerator
{
    public abstract class Property
    {

        public PropertyBuilder PropertyBuilder { get; internal set; }

        public void AddAttribute(Type attributeType, params object[] attributeParameters)
        {
            if (attributeType == null)
            {
                throw new ArgumentNullException(nameof(attributeType));
            }

            ConstructorInfo classCtorInfo = attributeType.GetConstructor(attributeParameters.Select(s => s.GetType()).ToArray());
            if (classCtorInfo == null)
            {
                throw new Exception(
                    $"Can not find constructor without parameters for attribute type {attributeType.FullName}");
            }
            CustomAttributeBuilder cab = new CustomAttributeBuilder(classCtorInfo, attributeParameters);
            this.PropertyBuilder.SetCustomAttribute(cab);
        }

        public CustomTypeBase CustomTypeBase { get; internal set; }
        public string Name { get; set; }
        public Type Type { get; internal set; }
        public string FieldName { get; set; }
        public MethodInfo PropertyGetter { get; internal set; }
        public MethodInfo PropertySetter { get; internal set; }
        public FieldInfo FieldInfo { get; internal set; }
        public Property(CustomTypeBase ct, string name, Type type)
        {
            this.CustomTypeBase = ct;
            this.Name = name;
            this.Type = type;
            this.FieldName = "_" + name;

            PropertyBuilder propertyBuilder = ct.TypeBuilder.DefineProperty(
                name,
                PropertyAttributes.HasDefault,
                type,
                null);

            PropertyBuilder = propertyBuilder;
        }

        public System.Collections.Generic.Dictionary<CustomPropertyFlags, bool> CustomPropertyFlags { get; set; } = new System.Collections.Generic.Dictionary<CustomPropertyFlags, bool>();

        public Property SetCustomPropertyFlags(CustomPropertyFlags flag, bool value)
        {
            CustomPropertyFlags[flag] = value;
            return this;
        }

        public bool GetCustomPropertyFlag(CustomPropertyFlags flag)
        {
            if (CustomPropertyFlags.ContainsKey(flag))
            {
                return CustomPropertyFlags[flag];
            }
            else
            {
                return false;
            }
        }

    }
}
