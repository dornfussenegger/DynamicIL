using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ILGenerator.BaseClasses.NotifyPropertyChangedBaseCreator
{
    public class NotifyPropertyChangedType : CustomTypeBase
    {

        public Type BaseType { get; private set; }

        public override void OnBeforeBuild()
        {
            base.OnBeforeBuild();
            
            var setPropertyValueMethod = TypeBuilder.DefineMethod("SetPropertyValue",
             MethodAttributes.Public |
                MethodAttributes.Virtual |
                MethodAttributes.HideBySig,
             typeof(void), new[] { typeof(string), typeof(object) });
            setPropertyValueMethod.DefineParameter(1, System.Reflection.ParameterAttributes.None, "property");
            setPropertyValueMethod.DefineParameter(2, System.Reflection.ParameterAttributes.None, "newValue");

            var ilSet = setPropertyValueMethod.GetILGenerator();
            WriteSetPropertyValueMethod(ilSet);

            var overrideMethodSet = typeof(NotifyPropertyChangedBase).GetMethod("SetPropertyValue");
            if (overrideMethodSet == null)
            {
                throw new Exception("Can not find Method 'SetPropertyValue' in base class.");
            }

            TypeBuilder.DefineMethodOverride(setPropertyValueMethod, overrideMethodSet);

            // GET
            var getPropertyValueMethod = TypeBuilder.DefineMethod("GetPropertyValue",
              
             MethodAttributes.Public |
                MethodAttributes.Virtual |
                MethodAttributes.HideBySig, 

             typeof(object), new[] { typeof(string) });
            getPropertyValueMethod.DefineParameter(1, System.Reflection.ParameterAttributes.None, "property");

            var ilGet = getPropertyValueMethod.GetILGenerator();
            WriteGetPropertyValueMethod(ilGet);

            var overrideMethodGet = typeof(NotifyPropertyChangedBase).GetMethod("GetPropertyValue");
            if (overrideMethodGet == null)
            {
                throw new Exception("Can not find Method 'GetPropertyValue' in base class.");
            }

            TypeBuilder.DefineMethodOverride(getPropertyValueMethod, overrideMethodGet);
        }

        public override Property CreateNewProperty(string name, Type type)
        {
            return new NotifyPropertyChangedProperty(this, name, type);
        }

        public NotifyPropertyChangedType(BuildContext bc, string name, bool withChangeTracker) : base(bc, name)
        {
            if (withChangeTracker)
            {
                BaseType = typeof(NotifyPropertyChangedBaseWithChangeTracker);
                UseChangeTracker = true;
            }
            else
            {
                BaseType = typeof(NotifyPropertyChangedBase);
                UseChangeTracker = false;
            }
            TypeBuilder.SetParent(BaseType);
        }

        public bool UseChangeTracker { get; internal set; }

        public override void AddPropertyGetAndSet()
        {
            // Ignore
            
        }
    }
}
