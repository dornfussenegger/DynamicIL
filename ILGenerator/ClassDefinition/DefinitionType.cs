using System;
using System.Collections.Generic;

namespace ILGenerator.ClassDefinition
{
    public class DefinitionType
    {
        public DefinitionTypeEnums Type { get; set; }
        public string Name { get; set; }
        public List<Property> Properties { get; set; } = new List<Property>();



        public void AddProperty(string name, Type type)
        {
            Properties.Add(new Property() { Name = name, Type = "SystemType::" + type.FullName, AutoInitialize=false });
        }
        public void AddProperty(string name, DefinitionType type)
        {
            Properties.Add(new Property() { Name = name, Type = "DefinitionType::" + type.Name, AutoInitialize = true });
        }
        public void AddPropertyOfList(string name, Type type)
        {
            Properties.Add(new Property() { Name = name, Type = "CollectionOfSystemType::" + type.FullName, AutoInitialize = true });
        }
        public void AddPropertyOfList(string name, DefinitionType type)
        {
            Properties.Add(new Property() { Name = name, Type = "CollectionOfDefinitionType::" + type.Name, AutoInitialize = true });
        }
    }

}
