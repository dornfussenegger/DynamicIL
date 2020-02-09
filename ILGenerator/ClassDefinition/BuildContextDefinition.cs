using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILGenerator.ClassDefinition
{
    public class BuildContextDefinition
    {
        public List<DefinitionType> Types { get; set; } = new List<DefinitionType>();

        public DefinitionType AddType(string name, DefinitionTypeEnums type)
        {
            DefinitionType t = new DefinitionType();
            t.Name = name;
            t.Type = type;
            Types.Add(t);
            return t;
        }

        private class ToCompile
        {
            public DefinitionType DefinitionType { get; set; }
            public ILGenerator.CustomTypeBase CompileType { get; set; }
           
        }
        public Type[] Compile(string assemblyName)
        {

            CustomTypeCreator ct = new CustomTypeCreator(assemblyName);
            List<ToCompile> l = new List<ToCompile>();

            foreach (var t in Types)
            {
                switch (t.Type)
                {
                    case DefinitionTypeEnums.SimpleType:
                        l.Add(new ToCompile() { CompileType= ct.CreateNewSimpleType(t.Name).SetCustomCustomTypeFlagsFlags(CustomTypeBaseFlags.AddIInitializeableImplementation,true), DefinitionType=t });
                        break;
                    case DefinitionTypeEnums.NotifyPropertyChanged:
                        l.Add(new ToCompile() { CompileType = ct.CreateNewNotifyPropertyChangedType(t.Name, false).SetCustomCustomTypeFlagsFlags(CustomTypeBaseFlags.AddIInitializeableImplementation, true), DefinitionType = t });
                        break;
                    case DefinitionTypeEnums.NotifyPropertyChangedWithChangeTracker:
                        l.Add(new ToCompile() { CompileType = ct.CreateNewNotifyPropertyChangedType(t.Name, true).SetCustomCustomTypeFlagsFlags(CustomTypeBaseFlags.AddIInitializeableImplementation, true), DefinitionType = t });
                        break;
                    default:
                        break;
                }
            }

            foreach (var t in l)
            {
                foreach (var p in t.DefinitionType.Properties)
                {
                    t.CompileType.AddProperty(p.Name, LocateType(p.Type, l)).SetCustomPropertyFlags(CustomPropertyFlags.InitializeInInitializeMethod, p.AutoInitialize == true);
                }

                t.CompileType.AddPropertyGetAndSet();
            }

            return ct.Build();
        }

        private Type LocateType(string identifier, List<ToCompile> inTypes)
        {
            var parts = identifier.Split(new string[] { "::" }, StringSplitOptions.None);
            if (parts.Length != 2)
            {
                throw new Exception("Can not find identifer: " + identifier);
            }

            if (string.Equals(parts[0], "SystemType", StringComparison.InvariantCultureIgnoreCase))
            {
                return Type.GetType(parts[1]);
            }
            else if (string.Equals(parts[0], "DefinitionType", StringComparison.InvariantCultureIgnoreCase))
            {
                return inTypes.FirstOrDefault(w => w.DefinitionType.Name == parts[1]).CompileType.TypeBuilder;
            }
            else if (string.Equals(parts[0], "CollectionOfSystemType", StringComparison.InvariantCultureIgnoreCase))
            {
                var lType = typeof(BaseClasses.GenericAddNewList<>);
                var gType = lType.MakeGenericType(new Type[] { Type.GetType(parts[1]) });
                return gType;
            }
            else if (string.Equals(parts[0], "CollectionOfDefinitionType", StringComparison.InvariantCultureIgnoreCase))
            {
                var lType = typeof(BaseClasses.GenericAddNewList<>);
                var gType = lType.MakeGenericType(new Type[] { inTypes.FirstOrDefault(w => w.DefinitionType.Name == parts[1]).CompileType.TypeBuilder });
                return gType;
            }
            else
            {
                throw new Exception("Can not find identifer: " + identifier);
            }
        }
    }

    public enum DefinitionTypeEnums
    {
        SimpleType,
        NotifyPropertyChanged,
        NotifyPropertyChangedWithChangeTracker
    }

    public class DefinitionType
    {
        public DefinitionTypeEnums Type { get; set; }
        public string Name { get; set; }
        public List<Property> Properties { get; set; } = new List<Property>();



        public void AddProperty(string name, Type type)
        {
            Properties.Add(new Property() { Name = name, Type = "SystemType::" + type.FullName });
        }
        public void AddProperty(string name, DefinitionType type)
        {
            Properties.Add(new Property() { Name = name, Type = "DefinitionType::" + type.Name, AutoInitialize=true });
        }
        public void AddPropertyOfList(string name, Type type)
        {
            Properties.Add(new Property() { Name = name, Type = "CollectionOfSystemType::" + type.FullName, AutoInitialize=true });
        }
        public void AddPropertyOfList(string name, DefinitionType type)
        {
            Properties.Add(new Property() { Name = name, Type = "CollectionOfDefinitionType::" + type.Name, AutoInitialize=true });
        }
    }

    public class Property
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool AutoInitialize { get; set; }

    }

    public enum PropertyType
    {
        SystemType,
        DefinitionType,
        CollectionOfSystemType,
        CollectionOfDefinitionType        
    }

}
