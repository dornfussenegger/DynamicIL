using System;

namespace ILGenerator
{
    public abstract class Property
	{
		public CustomTypeBase CustomTypeBase { get; internal set; }
		public string Name { get; set; }
		public Type Type { get; internal set; }
		public string FieldName { get; set; }
		public System.Reflection.MethodInfo PropertyGetter { get; internal set; }
		public System.Reflection.MethodInfo PropertySetter { get; internal set; }
		public System.Reflection.FieldInfo FieldInfo { get; internal set; }
		public Property(CustomTypeBase ct, string name, Type type)
		{
			this.CustomTypeBase = ct;
			this.Name = name;
			this.Type = type;
			this.FieldName = "_" + name;
		}
	}
}
