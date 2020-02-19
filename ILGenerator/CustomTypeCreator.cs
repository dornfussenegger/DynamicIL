using System;
using System.Collections.Generic;
using System.Linq;
using ILGenerator.BaseClasses;
using ILGenerator.Interfaces;

namespace ILGenerator
{

    public class CustomTypeCreator
    {
#if NETFRAMEWORK
        public string OutputDll { get; set; }
#endif
        public CustomTypeCreator(string assemblyName = "dynamicGeneratedAssembly"
#if NETFRAMEWORK
            , string outputDll = null
#endif
            )
        {
#if NETFRAMEWORK
            this.OutputDll = outputDll;
            this.BuildContext = new BuildContext(assemblyName, this, this.OutputDll);
#else
			this.BuildContext = new BuildContext(assemblyName, this, null);
#endif

        }

        public List<CustomTypeBase> CustomTypes { get; set; } = new List<CustomTypeBase>();
        public BuildContext BuildContext { get; set; }

        public SimpleType CreateNewSimpleType(string name)
        {
            var t = new SimpleType(this.BuildContext, name);
            this.CustomTypes.Add(t);
            return t;
        }

        public BaseClasses.NotifyPropertyChangedBaseCreator.NotifyPropertyChangedType CreateNewNotifyPropertyChangedType(string name, bool withChangeTracker = false)
        {
            var t = new BaseClasses.NotifyPropertyChangedBaseCreator.NotifyPropertyChangedType(this.BuildContext, name, withChangeTracker);
            this.CustomTypes.Add(t);
            return t;
        }

        public Type[] Build()
        {
            foreach (var item in this.CustomTypes)
            {
                item.OnBeforeBuild();
            }
            foreach (var element in BuildContext.TypesToCompile)
            {
                var t = element.CreateType();
                BuildContext.ResultTypes.Add(t);
            }

            var resultTypes = BuildContext.AssemblyBuilder.GetTypes();

#if NETFRAMEWORK
            if (OutputDll != null)
            {
                BuildContext.AssemblyBuilder.Save(OutputDll);
            }
#endif
            foreach (var element in CustomTypes)
            {
                element.BuildType = resultTypes.FirstOrDefault(w => w.Name == element.Name);
            }
            return resultTypes;
        }
    }
}
