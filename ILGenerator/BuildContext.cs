using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ILGenerator
{
    public class BuildContext
    {

        public AssemblyName AssemblyName { get; internal set; }
        public System.Reflection.Emit.AssemblyBuilder AssemblyBuilder { get; internal set; }
        public System.Reflection.Emit.ModuleBuilder ModuleBuilder { get; internal set; }
        public CustomTypeCreator CustomTypeCreator { get; internal set; }

        public List<System.Reflection.Emit.TypeBuilder> TypesToCompile { get; internal set; } = new List<System.Reflection.Emit.TypeBuilder>();
        public List<Type> ResultTypes { get; internal set; } = new List<Type>();

        public TypeBuilder CreateDynamicType(string dynamicTypeName)
        {
            var tb = ModuleBuilder.DefineType(dynamicTypeName, TypeAttributes.Public);
            TypesToCompile.Add(tb);
            return tb;
        }

        public BuildContext(string assemblyName, CustomTypeCreator ctc, string outputDllPath)
        {
            CustomTypeCreator = ctc;
#if NETFRAMEWORK
			AssemblyName = new AssemblyName(assemblyName);
			AssemblyBuilder =
				AppDomain.CurrentDomain.DefineDynamicAssembly(
					AssemblyName,
					AssemblyBuilderAccess.RunAndSave);
			if (outputDllPath != null)
			{
				ModuleBuilder = AssemblyBuilder.DefineDynamicModule(assemblyName, outputDllPath);
			}
			else
			{
				ModuleBuilder = AssemblyBuilder.DefineDynamicModule(assemblyName, assemblyName + ".dll");
			}
			
			
#else
            var name = new AssemblyName(assemblyName);
            AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            ModuleBuilder = AssemblyBuilder.DefineDynamicModule(assemblyName);
#endif



        }
    }
}
