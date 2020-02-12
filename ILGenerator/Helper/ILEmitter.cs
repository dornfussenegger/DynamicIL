using System;
using System.Linq;
using System.Reflection.Emit;

namespace ILGenerator.Helper
{

    public class ILEmitter
    {

        private readonly System.Reflection.Emit.ILGenerator _il;
        public ILEmitter(Type callerType, object buildingType, System.Reflection.Emit.ILGenerator il,
                         [System.Runtime.CompilerServices.CallerMemberName] string callerMethod = "")
        {
            if (callerType == null)
            {
                throw new ArgumentNullException(nameof(callerType));
            }
            if (buildingType == null)
            {
                throw new ArgumentNullException(nameof(buildingType));
            }
            if (il == null)
            {
                throw new ArgumentNullException(nameof(il));
            }
            if (string.IsNullOrWhiteSpace(callerMethod))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(callerMethod));
            }
            _il = il;
            Emitter = il;
            CallerMethod = callerMethod;

            BuildingType = buildingType;
            CallerType = callerType;


        }

        public void EmitGetStringArrayMethodIL(System.Reflection.Emit.ILGenerator emit, string[] array)
        {
            var e = emit;
            e.DeclareLocal(typeof(string[]));
            e.Emit(OpCodes.Ldc_I4, array.Length);
            e.Emit(OpCodes.Newarr, typeof(string));

            var i = 0;
            foreach (var p in array)
            {
                e.Emit(OpCodes.Dup);
                e.Emit(OpCodes.Ldc_I4, i);
                e.Emit(OpCodes.Ldstr, p);
                e.Emit(OpCodes.Stelem_Ref);
                i += 1;
            }
            e.Emit(OpCodes.Ret);
        }

        public void EmitGetTypeArrayMethodIL(System.Reflection.Emit.ILGenerator emit, Type[] array)
        {
            var e = emit;
            e.DeclareLocal(typeof(Type[]));
            e.Emit(OpCodes.Ldc_I4, array.Length);
            e.Emit(OpCodes.Newarr, typeof(Type));

            var mGetTypeFromHandle = typeof(Type).GetMethod("GetTypeFromHandle",
                new[] { typeof(RuntimeTypeHandle) });

            var i = 0;
            foreach (var p in array)
            {
                e.Emit(OpCodes.Dup);
                e.Emit(OpCodes.Ldc_I4, i);
                e.Emit(OpCodes.Ldtoken, p);
                e.Emit(OpCodes.Call, mGetTypeFromHandle);
                e.Emit(OpCodes.Stelem_Ref);

                i += 1;
            }
            e.Emit(OpCodes.Ret);
        }

        public string CallerMethod { get; private set; }

        public Type CallerType { get; private set; }

        public object BuildingType { get; private set; }

        public System.Reflection.Emit.ILGenerator Emitter { get; private set; }

        public System.Reflection.Emit.ILGenerator Il => _il;

        public void DeclareLocal(Type type)
        {
            Il.DeclareLocal(type);
        }

        public void DeclareLocal<T>()
        {
            Il.DeclareLocal(typeof(T));
        }

        public ILEmitter Ret()
        {
            Il.Emit(OpCodes.Ret);
            return this;
        }

        public ILEmitter Cast(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            Il.Emit(OpCodes.Castclass, type);
            return this;
        }

        public ILEmitter CastOrUnBox(Type type)
        {
            if (type.IsValueType)
            {
                Il.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                Il.Emit(OpCodes.Castclass, type);
            }

            return this;
        }

        public ILEmitter Box(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            Il.Emit(OpCodes.Box, type);
            return this;
        }

        public ILEmitter Unbox_any(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            Il.Emit(OpCodes.Unbox_Any, type);
            return this;
        }

        public ILEmitter Unbox(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            Il.Emit(OpCodes.Unbox, type);
            return this;
        }

        public ILEmitter Call(System.Reflection.MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            Il.Emit(OpCodes.Call, method);
            return this;
        }

        public ILEmitter Callvirt(System.Reflection.MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            Il.Emit(OpCodes.Callvirt, method);
            return this;
        }

        public ILEmitter Call(Type type, string methodName)
        {
            var methodInfo = type.GetMethod(methodName);
            if (methodInfo == null)
            {
                throw new Exception($"Can not find Method {methodName} on Type {type.FullName}");
            }
            Il.Emit(OpCodes.Call, type.GetMethod(methodName));
            return this;
        }

        public ILEmitter Call(Type type, string methodName, params Type[] parameterTypes)
        {
            var methodInfo = type.GetMethod(methodName, parameterTypes);
            if (methodInfo == null)
            {
                throw new Exception($"Can not find Method {methodName} on Type {type.FullName} with Parameters {string.Join(", ", parameterTypes.Select(s => s.FullName))}");
            }
            Il.Emit(OpCodes.Call, type.GetMethod(methodName, parameterTypes));
            return this;
        }

        public ILEmitter Callvirt(Type type, string methodName, params Type[] parameterTypes)
        {
            var methodInfo = type.GetMethod(methodName, parameterTypes);
            if (methodInfo == null)
            {
                throw new Exception($"Can not find Method {methodName} on Type {type.FullName} with Parameters {string.Join(", ", parameterTypes.Select(s => s.FullName))}");
            }
            Il.Emit(OpCodes.Callvirt, type.GetMethod(methodName, parameterTypes));
            return this;
        }

        public ILEmitter Callvirt(Type type, string methodName)
        {
            var methodInfo = type.GetMethod(methodName);
            if (methodInfo == null)
            {
                throw new Exception($"Can not find Method {methodName} on Type {type.FullName}");
            }
            Il.Emit(OpCodes.Callvirt, type.GetMethod(methodName));
            return this;
        }

        public ILEmitter Ldnull()
        {
            Il.Emit(OpCodes.Ldnull);
            return this;
        }

        public ILEmitter Bne_un(Label target)
        {
            Il.Emit(OpCodes.Bne_Un, target);
            return this;
        }

        public ILEmitter Beq(Label target)
        {
            Il.Emit(OpCodes.Beq, target);
            return this;
        }

        public ILEmitter Ldc_i4_0()
        {
            Il.Emit(OpCodes.Ldc_I4_0);
            return this;
        }

        public ILEmitter Ldc_i4_1()
        {
            Il.Emit(OpCodes.Ldc_I4_1);
            return this;
        }

        public ILEmitter Ldc_i4(int c)
        {
            Il.Emit(OpCodes.Ldc_I4, c);
            return this;
        }

        public ILEmitter Ldarg0()
        {
            Il.Emit(OpCodes.Ldarg_0);
            return this;
        }

        public ILEmitter Ldarg1()
        {
            Il.Emit(OpCodes.Ldarg_1);
            return this;
        }

        public ILEmitter Ldarg2()
        {
            Il.Emit(OpCodes.Ldarg_2);
            return this;
        }

        public ILEmitter Ldarga(int idx)
        {
            Il.Emit(OpCodes.Ldarga, idx);
            return this;
        }

        public ILEmitter Ldarga_s(int idx)
        {
            Il.Emit(OpCodes.Ldarga_S, idx);
            return this;
        }

        public ILEmitter Ldarg(int idx)
        {
            Il.Emit(OpCodes.Ldarg, idx);
            return this;
        }

        public ILEmitter Ldarg_s(int idx)
        {
            Il.Emit(OpCodes.Ldarg_S, idx);
            return this;
        }

        public ILEmitter Ifclass_ldind_ref(Type type)
        {
            if (!type.IsValueType)
            {
                Il.Emit(OpCodes.Ldind_Ref);
            }
            return this;
        }

        public ILEmitter Ldloc0()
        {
            Il.Emit(OpCodes.Ldloc_0);
            return this;
        }

        public ILEmitter Ldloc1()
        {
            Il.Emit(OpCodes.Ldloc_1);
            return this;
        }

        public ILEmitter Ldloc2()
        {
            Il.Emit(OpCodes.Ldloc_2);
            return this;
        }

        public ILEmitter Ldloca_s(int idx)
        {
            Il.Emit(OpCodes.Ldloca_S, idx);
            return this;
        }

        public ILEmitter Ldloca_s(LocalBuilder local)
        {
            Il.Emit(OpCodes.Ldloca_S, local);
            return this;
        }

        public ILEmitter Ldloc_s(int idx)
        {
            Il.Emit(OpCodes.Ldloc_S, idx);
            return this;
        }

        public ILEmitter Ldloc_s(LocalBuilder local)
        {
            Il.Emit(OpCodes.Ldloc_S, local);
            return this;
        }

        public ILEmitter Ldloca(int idx)
        {
            Il.Emit(OpCodes.Ldloca, idx);
            return this;
        }

        public ILEmitter Ldloca(LocalBuilder local)
        {
            Il.Emit(OpCodes.Ldloca, local);
            return this;
        }

        public ILEmitter Ldloc(int idx)
        {
            Il.Emit(OpCodes.Ldloc, idx);
            return this;
        }

        public ILEmitter Ldloc(LocalBuilder local)
        {
            Il.Emit(OpCodes.Ldloc, local);
            return this;
        }

        public ILEmitter Initobj(Type type)
        {
            Il.Emit(OpCodes.Initobj, type);
            return this;
        }

        public ILEmitter Newobj(System.Reflection.ConstructorInfo ctor)
        {
            Il.Emit(OpCodes.Newobj, ctor);
            return this;
        }

        public ILEmitter Newobj(Type type, params Type[] parameters)
        {
            var ctr = type.GetConstructor(parameters);
            Il.Emit(OpCodes.Newobj, ctr);
            return this;
        }

        public ILEmitter Throw()
        {
            Il.Emit(OpCodes.Throw);
            return this;
        }

        public ILEmitter Throw_new(Type type)
        {
            var exp = type.GetConstructor(Type.EmptyTypes);
            Newobj(exp).Throw();
            return this;
        }

        public ILEmitter Stelem_ref()
        {
            Il.Emit(OpCodes.Stelem_Ref);
            return this;
        }

        public ILEmitter Ldelem_ref()
        {
            Il.Emit(OpCodes.Ldelem_Ref);
            return this;
        }

        public ILEmitter Ldlen()
        {
            Il.Emit(OpCodes.Ldlen);
            return this;
        }

        public ILEmitter Stloc(int idx)
        {
            Il.Emit(OpCodes.Stloc, idx);
            return this;
        }

        public ILEmitter Stloc_s(int idx)
        {
            Il.Emit(OpCodes.Stloc_S, idx);
            return this;
        }

        public ILEmitter Stloc(LocalBuilder local)
        {
            Il.Emit(OpCodes.Stloc, local);
            return this;
        }

        public ILEmitter Stloc_s(LocalBuilder local)
        {
            Il.Emit(OpCodes.Stloc_S, local);
            return this;
        }

        public ILEmitter Stloc0()
        {
            Il.Emit(OpCodes.Stloc_0);
            return this;
        }

        public ILEmitter Stloc1()
        {
            Il.Emit(OpCodes.Stloc_1);
            return this;
        }

        public ILEmitter Mark(Label label)
        {
            Il.MarkLabel(label);
            return this;
        }

        public ILEmitter Ldfld(System.Reflection.FieldInfo field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }
            Il.Emit(OpCodes.Ldfld, field);
            return this;
        }

        public ILEmitter Ldsfld(System.Reflection.FieldInfo field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }
            Il.Emit(OpCodes.Ldsfld, field);
            return this;
        }

        public ILEmitter Lodfld(System.Reflection.FieldInfo field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }
            if (field.IsStatic)
            {
                Ldsfld(field);
            }
            else
            {
                Ldfld(field);
            }
            return this;
        }

        public ILEmitter IfValueType_Box(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (type.IsValueType)
            {
                Il.Emit(OpCodes.Box, type);
            }
            return this;
        }

        public ILEmitter Stfld(System.Reflection.FieldInfo field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }
            Il.Emit(OpCodes.Stfld, field);
            return this;
        }

        public ILEmitter Stsfld(System.Reflection.FieldInfo field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }
            Il.Emit(OpCodes.Stsfld, field);
            return this;
        }

        public ILEmitter Setfld(System.Reflection.FieldInfo field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }
            if (field.IsStatic)
            {
                Stsfld(field);
            }
            else
            {
                Stfld(field);
            }
            return this;
        }

        public ILEmitter UnboxOrCast(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (type.IsValueType)
            {
                Unbox(type);
            }
            else
            {
                Cast(type);
            }
            return this;
        }

        public ILEmitter Callorvirt(Type onType, string methodName, params Type[] parameter)
        {
            if (onType == null)
            {
                throw new ArgumentNullException(nameof(onType));
            }
            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(methodName));
            }
            var mi = onType.GetMethod(methodName, parameter);
            if (mi == null)
            {
                throw new Exception($"Can not find Method {methodName} on Type {onType.FullName} with Parameters {string.Join(", ", parameter.Select(s => s.FullName))}");
            }
            return Callorvirt(mi);
        }

        public ILEmitter Callorvirt(System.Reflection.MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (method.IsVirtual)
            {
                Il.Emit(OpCodes.Callvirt, method);
            }
            else
            {
                Il.Emit(OpCodes.Call, method);
            }
            return this;
        }

        public ILEmitter Stind_ref()
        {
            Il.Emit(OpCodes.Stind_Ref);
            return this;
        }

        public ILEmitter Ldind_ref()
        {
            Il.Emit(OpCodes.Ldind_Ref);
            return this;
        }

        public LocalBuilder Declocal(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return Il.DeclareLocal(type);
        }

        public void Nop()
        {
            Il.Emit(OpCodes.Nop);
        }

        public LocalBuilder Declocal<T>()
        {
            return Il.DeclareLocal(typeof(T));
        }

        public Label Deflabel()
        {
            return Il.DefineLabel();
        }

        public ILEmitter Ifclass_ldarg_else_ldarga(int idx, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (type.IsValueType)
            {
                Ldarga(idx);
            }
            else
            {
                Ldarg(idx);
            }
            return this;
        }

        public ILEmitter Ifclass_ldloc_else_ldloca(int idx, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (type.IsValueType)
            {
                Ldloca(idx);
            }
            else
            {
                Ldloc(idx);
            }
            return this;
        }

        public ILEmitter Perform(Action<ILEmitter, System.Reflection.MemberInfo> action, System.Reflection.MemberInfo member)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }
            action(this, member);
            return this;
        }

        public ILEmitter Ifbyref_ldloca_else_ldloc(LocalBuilder local, Type type)
        {
            if (local == null)
            {
                throw new ArgumentNullException(nameof(local));
            }
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (type.IsByRef)
            {
                Ldloca(local);
            }
            else
            {
                Ldloc(local);
            }
            return this;
        }

        public void LdStr(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }
            Emitter.Emit(OpCodes.Ldstr, str);
        }

        public void Br(Label label)
        {
            Emitter.Emit(OpCodes.Br, label);
        }


        public void BrFalse(Label label)
        {
            Emitter.Emit(OpCodes.Brfalse, label);
        }

        public void MarkLabel(Label label)
        {
            Emitter.MarkLabel(label);
        }

        public Label DefineLabel()
        {
            return Emitter.DefineLabel();
        }

        public ILEmitter Stloc2()
        {
            Il.Emit(OpCodes.Stloc_2);
            return this;
        }

        public ILEmitter CallPropertyGet(Type onType, string propertyName)
        {
            var prop = onType.GetProperty(propertyName);
            var getter = prop.GetGetMethod();
            Call(getter);
            return this;
        }

        public ILEmitter CallPropertySet(Type onType, string propertyName)
        {
            var prop = onType.GetProperty(propertyName);
            var getter = prop.GetSetMethod();
            Call(getter);
            return this;
        }

        public ILEmitter Dup()
        {
            Il.Emit(OpCodes.Dup);
            return this;
        }

        public ILEmitter BrTrue(Label label)
        {
            Il.Emit(OpCodes.Brtrue, label);
            return this;
        }

        public ILEmitter Pop()
        {
            Il.Emit(OpCodes.Pop);
            return this;
        }

        public ILEmitter Ceq()
        {
            Il.Emit(OpCodes.Ceq);
            return this;
        }
    }

    public class Brfalse
    {
        public Brfalse(ILEmitter ILEmitter, Action<ILEmitter> doDuringIf)
        {
            if (doDuringIf == null)
            {
                throw new ArgumentNullException(nameof(doDuringIf));
            }
            this.ILEmitter = ILEmitter;
            ILEmitter.Emitter.DefineLabel();
            ILEmitter.Emitter.Emit(OpCodes.Brfalse, Label);
            doDuringIf(ILEmitter);
            ILEmitter.Emitter.MarkLabel(Label);
        }

        public ILEmitter ILEmitter { get; private set; }
        public Label Label { get; private set; }
    }
}

