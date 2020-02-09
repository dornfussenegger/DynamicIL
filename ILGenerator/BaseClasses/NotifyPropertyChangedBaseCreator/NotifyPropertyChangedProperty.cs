using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ILGenerator.BaseClasses.NotifyPropertyChangedBaseCreator
{

	public class NotifyPropertyChangedProperty : Property
    {

		public NotifyPropertyChangedProperty(CustomTypeBase ct, string name, Type type) : base(ct, name, type)
		{

			var inType = (NotifyPropertyChangedType)ct;

			var field = ct.TypeBuilder.DefineField(this.FieldName, type, FieldAttributes.Private);

			this.FieldInfo = field;

			MethodAttributes getSetAttr = MethodAttributes.Public |
					   MethodAttributes.SpecialName | MethodAttributes.HideBySig;


			MethodBuilder propertyGetAccessor = ct.TypeBuilder.DefineMethod(
					"get_" + name,
					getSetAttr,
					type,
					Type.EmptyTypes);

			this.PropertyGetter = propertyGetAccessor;

			// GET
			var fieldGetIL = propertyGetAccessor.GetILGenerator();
			var fieldGetIL_loc1 = fieldGetIL.DeclareLocal(type);
			fieldGetIL.Emit(OpCodes.Ldarg_0);
			fieldGetIL.Emit(OpCodes.Ldfld, field);
			fieldGetIL.Emit(OpCodes.Ret);

			MethodBuilder propertySetAccessor = ct.TypeBuilder.DefineMethod(
				"set_" + name,
				getSetAttr,
				null,
				new Type[] { type });

			this.PropertySetter = propertySetAccessor;

			// SET
			var fieldSetIL = propertySetAccessor.GetILGenerator();


			// DECLARE LOCALS
			var loc1 = fieldSetIL.DeclareLocal(type);
			var loc2 = fieldSetIL.DeclareLocal(typeof(bool));

			// DECLARE LABELS
			var IL_003e = fieldSetIL.DefineLabel();

			// IL_0000: nop
			fieldSetIL.Emit(OpCodes.Nop);

			// //  Guid guid = backGuidString;
			// IL_0001: ldarg.0
			fieldSetIL.Emit(OpCodes.Ldarg_0);

			// IL_0002: ldfld valuetype [mscorlib]System.Guid ILGenerator.Extraction.PropertyChangedTest::backGuidString
			fieldSetIL.Emit(OpCodes.Ldfld, field);

			// IL_0007: stloc.0
			fieldSetIL.Emit(OpCodes.Stloc_0);

			// //  if (object.Equals(guid, value))
			// IL_0008: ldloc.0
			fieldSetIL.Emit(OpCodes.Ldloc_0);

			// IL_0009: box [mscorlib]System.Guid
			if (type.IsValueType) fieldSetIL.Emit(OpCodes.Box, type);

			// IL_000e: ldarg.1
			fieldSetIL.Emit(OpCodes.Ldarg_1);

			// IL_000f: box [mscorlib]System.Guid
			if (type.IsValueType) fieldSetIL.Emit(OpCodes.Box, type);

			// IL_0014: call bool [mscorlib]System.Object::Equals(object, object)
			var methodEquals = typeof(System.Object).GetMethod("Equals", new Type[] { typeof(object), typeof(object) });
			if (methodEquals == null)
			{
				throw new Exception("Method not found");
			}
			fieldSetIL.Emit(OpCodes.Call, methodEquals);

			// IL_0019: stloc.1
			fieldSetIL.Emit(OpCodes.Stloc_1);

			// IL_001a: ldloc.1
			fieldSetIL.Emit(OpCodes.Ldloc_1);

			// //  (no C# code)
			// IL_001b: brfalse.s IL_003e
			fieldSetIL.Emit(OpCodes.Brtrue_S, IL_003e);

			// IL_001d: nop
			fieldSetIL.Emit(OpCodes.Nop);

			// //  backGuidString = value;
			// IL_001e: ldarg.0
			fieldSetIL.Emit(OpCodes.Ldarg_0);

			// IL_001f: ldarg.1
			fieldSetIL.Emit(OpCodes.Ldarg_1);

			// IL_0020: stfld valuetype [mscorlib]System.Guid ILGenerator.Extraction.PropertyChangedTest::backGuidString
			fieldSetIL.Emit(OpCodes.Stfld, field);

			// //  OnPropertyChanged("MyGuidProperty", guid, value);
			// IL_0025: ldarg.0
			fieldSetIL.Emit(OpCodes.Ldarg_0);

			// IL_0026: ldstr "MyGuidProperty"
			fieldSetIL.Emit(OpCodes.Ldstr, Name);

			// IL_002b: ldloc.0
			fieldSetIL.Emit(OpCodes.Ldloc_0);

			// IL_002c: box [mscorlib]System.Guid
			if (type.IsValueType) fieldSetIL.Emit(OpCodes.Box, type);

			// IL_0031: ldarg.1
			fieldSetIL.Emit(OpCodes.Ldarg_1);

			// IL_0032: box [mscorlib]System.Guid
			if (type.IsValueType) fieldSetIL.Emit(OpCodes.Box, type);

			// IL_0037: callvirt instance void [ILGenerator]ILGenerator.BaseClasses.NotifyPropertyChangedBase::OnPropertyChanged(string, object, object)
			MethodInfo methodOnPropertyChanged = null;
			var useChangeTracker = ((NotifyPropertyChangedBaseCreator.NotifyPropertyChangedType)this.CustomTypeBase).UseChangeTracker;
			if (useChangeTracker)
			{

				methodOnPropertyChanged = typeof(NotifyPropertyChangedBaseWithChangeTracker).GetMethod("OnPropertyChanged", new Type[] { typeof(string), typeof(object), typeof(object) });
			}
			else
			{
				methodOnPropertyChanged = typeof(NotifyPropertyChangedBase).GetMethod("OnPropertyChanged", new Type[] { typeof(string), typeof(object), typeof(object) });
			}
			if (methodOnPropertyChanged == null)
			{
				throw new Exception("Method not found");
			}

			fieldSetIL.Emit(OpCodes.Callvirt, methodOnPropertyChanged);

			//// //  (no C# code)
			//// IL_003c: nop
			//fieldSetIL.Emit(OpCodes.Nop);



			//// IL_003d: nop
			//fieldSetIL.Emit(OpCodes.Nop);


			fieldSetIL.MarkLabel(IL_003e);
			fieldSetIL.Emit(OpCodes.Ret);
			

			PropertyBuilder.SetGetMethod(propertyGetAccessor);
			PropertyBuilder.SetSetMethod(propertySetAccessor);
		}
	}
}
