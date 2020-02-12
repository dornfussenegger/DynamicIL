using ILGenerator.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ILGenerator
{
    public abstract class CustomTypeBase
    {

        public virtual void OnBeforeBuild()
        {
            if (this.GetCustomTypeBaseFlag(CustomTypeBaseFlags.AddIInitializeableImplementation))
            {
                AddIInitializeableImplementation();
            }
        }
        public virtual void AddIPropertyDescriptorImplementation()
        {
            if (this.ImplementInterface(typeof(Interfaces.IPropertyDescriptor)) == false)
            {
                return;
            }

            {
                var method = this.TypeBuilder.DefineMethod("GetAllPropertyTypes", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, typeof(Type[]), new Type[] { });
                var il = method.GetILGenerator();

                var overrideMethod = typeof(Interfaces.IPropertyDescriptor).GetMethod("GetAllPropertyTypes");
                if (overrideMethod == null)
                {
                    throw new Exception("Can not find Method 'GetAllPropertyTypes' in interface.");
                }


                var helper = new Helper.ILEmitter(this.TypeBuilder, this, il);
                var allProperties = Properties.Select(s => s.Type).ToArray();
                helper.EmitGetTypeArrayMethodIL(il, allProperties);
                TypeBuilder.DefineMethodOverride(method, overrideMethod);
            }
            {
                var method = this.TypeBuilder.DefineMethod("GetAllPropertyNames", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, typeof(string[]), new Type[] { });
                var il = method.GetILGenerator();

                var overrideMethod = typeof(Interfaces.IPropertyDescriptor).GetMethod("GetAllPropertyNames");
                if (overrideMethod == null)
                {
                    throw new Exception("Can not find Method 'GetAllPropertyNames' in interface.");
                }


                var helper = new Helper.ILEmitter(this.TypeBuilder, this, il);
                var allProperties = Properties.Select(s => s.Name).ToArray();
                helper.EmitGetStringArrayMethodIL(il, allProperties);
                TypeBuilder.DefineMethodOverride(method, overrideMethod);
            }
        }

        public virtual void AddIInitializeableImplementation()
        {

            if (this.ImplementInterface(typeof(Interfaces.IInitializable)) == false)
            {
                return;
            }
            var method = this.TypeBuilder.DefineMethod("Initialize", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig);
            var il = method.GetILGenerator();

            var overrideMethod = typeof(Interfaces.IInitializable).GetMethod("Initialize");
            if (overrideMethod == null)
            {
                throw new Exception("Can not find Method 'Initialize' in interface.");
            }


            il.Emit(OpCodes.Nop);

            foreach (var property in this.Properties.Where(w => w.CustomPropertyFlags.ContainsKey(CustomPropertyFlags.InitializeInInitializeMethod) && w.CustomPropertyFlags[CustomPropertyFlags.InitializeInInitializeMethod] == true && !w.Type.IsValueType))
            {
                //IL_0001: ldarg.0
                il.Emit(OpCodes.Ldarg_0);

                //IL_0002: ldtoken class [mscorlib]System.Collections.Generic.List`1<string>
                il.Emit(OpCodes.Ldtoken, property.Type);

                //IL_0007: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
                var method1 = typeof(Type).GetMethod("GetTypeFromHandle");
                il.Emit(OpCodes.Call, method1);

                //IL_000c: call object [ILGenerator]ILGenerator.CustomTypeBase::CreateInstanceOfType(class [mscorlib]System.Type)
                var method2 = typeof(ILGenerator.CustomTypeBase).GetMethod("CreateInstanceOfType");
                il.Emit(OpCodes.Call, method2);

                //IL_0011: castclass class [mscorlib]System.Collections.Generic.List`1<string>
                il.Emit(OpCodes.Castclass, property.Type);

                //IL_0016: stfld class [mscorlib]System.Collections.Generic.List`1<string> ILGenerator.Extraction.PropertyChangedTest::myList
                il.Emit(OpCodes.Stfld, property.FieldInfo);
            }
            il.Emit(OpCodes.Ret);

            TypeBuilder.DefineMethodOverride(method, overrideMethod);
        }

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
            this.TypeBuilder.SetCustomAttribute(cab);
        }

        public List<Type> ImplementedInterfaces { get; internal set; } = new List<Type>();

        public List<Property> Properties { get; internal set; } = new List<Property>();
        public string Name { get; internal set; }
        public BuildContext BuildContext { get; internal set; }

        public TypeBuilder TypeBuilder { get; internal set; }
        public Type BuildType { get; internal set; }

        public CustomTypeBase(BuildContext bc, string name)
        {
            this.Name = name;
            this.BuildContext = bc;

            var dt = bc.CreateDynamicType(name);
            TypeBuilder = dt;
        }

        public virtual Property CreateNewProperty(string name, Type type)
        {
            return new SimpleProperty(this, name, type);
        }

        public Property AddProperty(string name, Type type)
        {
            var toAdd = CreateNewProperty(name, type);
            this.Properties.Add(toAdd);
            return toAdd;
        }
        public Property AddPropertyOfGenericList(string name, Type type)
        {
            var tg = typeof(GenericAddNewList<>);
            var gl = tg.MakeGenericType(type);
            return AddProperty(name, gl);
        }
        public Property AddPropertyOfGenericDictionary(string name, Type TKey, Type TValue)
        {
            var tg = typeof(System.Collections.Generic.Dictionary<,>);
            var gl = tg.MakeGenericType(TKey, TValue);
            return AddProperty(name, gl);
        }

        public Property AddProperty<TProperty>(string name)
        {
            return this.AddProperty(name, typeof(TProperty));
        }

        public bool ImplementInterface(Type interfaceType)
        {
            if (!ImplementedInterfaces.Contains(interfaceType))
            {
                this.ImplementedInterfaces.Add(interfaceType);
                this.TypeBuilder.AddInterfaceImplementation(interfaceType);
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual void AddPropertyGetAndSet()
        {


            if (ImplementInterface(typeof(Interfaces.IPropertyGetAndSet)) == false)
            {
                return;
            }


            // SET
            var setPropertyValueMethod = TypeBuilder.DefineMethod("SetPropertyValue", System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.ReuseSlot | System.Reflection.MethodAttributes.Virtual | System.Reflection.MethodAttributes.HideBySig,
             typeof(void), new[] { typeof(string), typeof(object) });
            setPropertyValueMethod.DefineParameter(1, System.Reflection.ParameterAttributes.None, "property");
            setPropertyValueMethod.DefineParameter(2, System.Reflection.ParameterAttributes.None, "newValue");

            var ilSet = setPropertyValueMethod.GetILGenerator();
            WriteSetPropertyValueMethod(ilSet);

            var overrideMethodSet = typeof(Interfaces.IPropertyGetAndSet).GetMethod("SetPropertyValue");
            if (overrideMethodSet == null)
            {
                throw new Exception("Can not find Method 'SetPropertyValue' in interface.");
            }

            TypeBuilder.DefineMethodOverride(setPropertyValueMethod, overrideMethodSet);

            // GET
            var getPropertyValueMethod = TypeBuilder.DefineMethod("GetPropertyValue",
             System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.ReuseSlot | System.Reflection.MethodAttributes.Virtual | System.Reflection.MethodAttributes.HideBySig, typeof(object), new[] { typeof(string) });
            getPropertyValueMethod.DefineParameter(1, System.Reflection.ParameterAttributes.None, "property");

            var ilGet = getPropertyValueMethod.GetILGenerator();
            WriteGetPropertyValueMethod(ilGet);

            var overrideMethodGet = typeof(Interfaces.IPropertyGetAndSet).GetMethod("GetPropertyValue");
            if (overrideMethodGet == null)
            {
                throw new Exception("Can not find Method 'GetPropertyValue' in interface.");
            }

        }

        internal void WriteSetPropertyValueMethod(System.Reflection.Emit.ILGenerator il)
        {
            var allProperties = Properties.Where(w => w.PropertySetter != null).ToArray();

            var helper = new Helper.ILEmitter(this.GetType(), this.TypeBuilder, il);
            il.DeclareLocal(typeof(bool));

            for (int i = 0; i < allProperties.Length; i++)
            {
                il.DeclareLocal(typeof(bool));
            }


            helper.Nop();
            helper.Ldarg1();
            helper.Call(typeof(string), "IsNullOrWhiteSpace", typeof(string));

            var label = helper.DefineLabel();
            helper.BrFalse(label);

            var endlabel = helper.DefineLabel();


            helper.Nop();
            helper.Ldarg0();
            helper.Call(typeof(object), "GetType");
            helper.Ldarg1();
            helper.Newobj(typeof(Exceptions.EmptyOrNullPropertyNameException), typeof(Type), typeof(string));
            helper.Throw();

            helper.MarkLabel(label);


            var mcallStringEquals = typeof(string).GetMethod("Equals", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                null, new[] { typeof(string), typeof(string) }, null);


            var plabel = helper.DefineLabel();


            foreach (var p in allProperties)
            {
                helper.MarkLabel(plabel);
                plabel = helper.DefineLabel();

                helper.Ldarg1();
                helper.LdStr(p.Name);
                helper.Call(mcallStringEquals);
                helper.BrFalse(plabel);

                helper.Nop();
                helper.Ldarg0();
                helper.Ldarg2();
                helper.CastOrUnBox(p.Type);
                helper.Call(p.PropertySetter);
                helper.Nop();
                helper.Nop();
                helper.Br(endlabel);
            }

            helper.MarkLabel(plabel);

            helper.Nop();
            helper.Ldarg0();
            helper.Call(typeof(object), "GetType");
            helper.Ldarg1();
            helper.Newobj(typeof(Exceptions.PropertyNotFoundException), typeof(Type), typeof(string));
            helper.Throw();


            helper.MarkLabel(endlabel);
            helper.Ret();
        }

        internal void WriteGetPropertyValueMethod(System.Reflection.Emit.ILGenerator il)
        {
            var helper = new Helper.ILEmitter(this.GetType(), this, il);

            helper.DeclareLocal(typeof(bool));
            helper.DeclareLocal(typeof(bool));
            helper.DeclareLocal(typeof(object));


            //bool flag = string.IsNullOrWhiteSpace(property);
            //if (flag)
            //{
            //    throw new EmptyOrNullPropertyNameException(base.GetType(), property);
            //}

            helper.Nop();
            helper.Ldarg1();
            helper.Call(typeof(string).GetMethod("IsNullOrWhiteSpace"));
            //helper.Stloc0();
            //helper.Ldloc0();

            var label = helper.DefineLabel();
            helper.BrFalse(label);

            Label endlabel = helper.DefineLabel();


            helper.Nop();
            helper.Ldarg0();
            helper.Call(typeof(object), "GetType");
            helper.Ldarg1();
            helper.Newobj(typeof(Exceptions.EmptyOrNullPropertyNameException), typeof(Type), typeof(string));
            helper.Throw();

            helper.MarkLabel(label);

            var mcallStringEquals = typeof(string).GetMethod("Equals", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                null, new[] { typeof(string), typeof(string) }, null);

            var propertiesToImplement = Properties.Where(w => w.PropertyGetter != null).ToArray();

            var pLabel = helper.DefineLabel();

            foreach (var p in propertiesToImplement)
            {
                helper.MarkLabel(pLabel);
                pLabel = helper.DefineLabel();

                helper.Ldarg1();
                helper.LdStr(p.Name);
                helper.Call(mcallStringEquals);
                //helper.Stloc1();
                //helper.Ldloc1();
                helper.BrFalse(pLabel);

                helper.Nop();
                helper.Ldarg0();
                helper.Call(p.PropertyGetter);
                if (p.Type.IsValueType)
                {
                    helper.Box(p.Type);
                }
                //helper.Stloc2();
                helper.Br(endlabel);
            }

            helper.MarkLabel(pLabel);


            helper.Nop();
            helper.Ldarg0();
            helper.Callorvirt(typeof(object), "GetType");
            helper.Ldarg1();
            helper.Newobj(typeof(Exceptions.PropertyNotFoundException), typeof(Type), typeof(string));
            helper.Throw();


            helper.MarkLabel(endlabel);
            helper.Ret();
        }

        public System.Collections.Generic.Dictionary<CustomTypeBaseFlags, bool> CustomTypeFlags { get; set; } = new System.Collections.Generic.Dictionary<CustomTypeBaseFlags, bool>();

        public CustomTypeBase SetCustomCustomTypeFlagsFlags(CustomTypeBaseFlags flag, bool value)
        {
            CustomTypeFlags[flag] = value;
            return this;
        }

        public bool GetCustomTypeBaseFlag(CustomTypeBaseFlags flag)
        {
            if (CustomTypeFlags.ContainsKey(flag))
            {
                return CustomTypeFlags[flag];
            }
            else
            {
                return false;
            }
        }
    }
}
