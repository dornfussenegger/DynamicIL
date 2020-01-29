using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ILGenerator
{
    public class SimpleProperty : Property
	{
		public SimpleProperty(CustomTypeBase ct, string name, Type type) : base(ct, name, type)
		{
			var field = ct.TypeBuilder.DefineField(this.FieldName, type, FieldAttributes.Private);

			this.FieldInfo = field;

			PropertyBuilder propertyBuilder = ct.TypeBuilder.DefineProperty(
				name,
				System.Reflection.PropertyAttributes.HasDefault,
				type,
				null);

			MethodAttributes getSetAttr = MethodAttributes.Public |
					   MethodAttributes.SpecialName | MethodAttributes.HideBySig;


			MethodBuilder propertyGetAccessor = ct.TypeBuilder.DefineMethod(
					"get_" + name,
					getSetAttr,
					type,
					Type.EmptyTypes);

			this.PropertyGetter = propertyGetAccessor;

			var fieldGetIL = propertyGetAccessor.GetILGenerator();
			fieldGetIL.Emit(OpCodes.Ldarg_0);
			fieldGetIL.Emit(OpCodes.Ldfld, field);
			fieldGetIL.Emit(OpCodes.Ret);

			MethodBuilder propertySetAccessor = ct.TypeBuilder.DefineMethod(
				"set_" + name,
				getSetAttr,
				null,
				new Type[] { type });

			this.PropertySetter = propertySetAccessor;

		var fieldSetIL = propertySetAccessor.GetILGenerator();
			fieldSetIL.Emit(OpCodes.Ldarg_0);
			fieldSetIL.Emit(OpCodes.Ldarg_1);
			fieldSetIL.Emit(OpCodes.Stfld, field);
			fieldSetIL.Emit(OpCodes.Ret);

			propertyBuilder.SetGetMethod(propertyGetAccessor);
			propertyBuilder.SetSetMethod(propertySetAccessor);
		}
	}
}
