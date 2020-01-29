using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;

namespace ILGenerator
{
    public abstract class CustomTypeBase
	{
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


		public void AddProperty(string name, Type type)
		{
			this.Properties.Add(new SimpleProperty(this, name, type));
		}
        public void AddPropertyOfGenericList(string name, Type type)
        {
            var tg = typeof(System.Collections.Generic.List<>);
            var gl = tg.MakeGenericType(type);
            this.Properties.Add(new SimpleProperty(this, name, gl));
        }
        public void AddPropertyOfGenericDictionary(string name, Type TKey, Type TValue)
        {
            var tg = typeof(System.Collections.Generic.Dictionary<,>);
            var gl = tg.MakeGenericType(TKey, TValue);
            this.Properties.Add(new SimpleProperty(this, name, gl));
        }

        public void AddProperty<TProperty>(string name)
		{
			this.AddProperty(name, typeof(TProperty));
		}

		public void AddPropertyGetAndSet()
		{
			if (!ImplementedInterfaces.Contains(typeof(Interfaces.IPropertyGetAndSet)))
			{
				this.ImplementedInterfaces.Add(typeof(Interfaces.IPropertyGetAndSet));
                this.TypeBuilder.AddInterfaceImplementation(typeof(Interfaces.IPropertyGetAndSet));
			}


			// SET
			var setPropertyValueMethod = TypeBuilder.DefineMethod("SetPropertyValue",System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.ReuseSlot |System.Reflection.MethodAttributes.Virtual | System.Reflection.MethodAttributes.HideBySig,
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

        private void WriteSetPropertyValueMethod(System.Reflection.Emit.ILGenerator il)
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

        private void WriteGetPropertyValueMethod(System.Reflection.Emit.ILGenerator il)
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

    }
}
