using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization;

namespace ILGenerator.Test.NetCoreTests
{

    [TestClass]
    public class DefinitionTests
    {

        public void GeneralTest()
        {
            var b = new ClassDefinition.BuildContextDefinition();

            var links = b.AddType("Url", ClassDefinition.DefinitionTypeEnums.SimpleType);
            links.AddProperty("Url", typeof(string));
            links.AddProperty("Title", typeof(string));

            var address = b.AddType("Address", ClassDefinition.DefinitionTypeEnums.SimpleType);
            address.AddProperty("Street", typeof(string));
            address.AddProperty("Housenumber", typeof(string));
            address.AddProperty("PostcalCode", typeof(string));
            address.AddProperty("Country", typeof(string));

            var test = b.AddType("Test", ClassDefinition.DefinitionTypeEnums.SimpleType);
            test.AddProperty("FirstName", typeof(string));
            test.AddProperty("LastName", typeof(string));
            test.AddProperty("Uid", typeof(Guid));
            test.AddProperty("Address", address);
            test.AddPropertyOfList("Links", links);

            var defJson = Newtonsoft.Json.JsonConvert.SerializeObject(b, Newtonsoft.Json.Formatting.Indented);
            {
                var t = b.Compile("test").FirstOrDefault(w => w.Name == "Test");
                var i = t.CreateInstance() as Interfaces.IPropertyGetAndSet;
                var linkCollection = i.GetPropertyValue<BaseClasses.IGenericAddNewList>("Links");

                int x = 0;
                foreach (var item in linkCollection.CreateAndAdd(3))
                {
                    x += 1;
                    var l = item as Interfaces.IPropertyGetAndSet;
                    l.SetPropertyValue("Url", $"{x}");
                    l.SetPropertyValue("Title", $"{x}");
                }

                var serializer = new SerializerBuilder().Build();

                var yaml = serializer.Serialize(b);

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(i, Newtonsoft.Json.Formatting.Indented);

                Console.WriteLine(yaml);
                Console.WriteLine(
                    json
                );
            }




            {
                var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<ClassDefinition.BuildContextDefinition>(defJson);
                var b2 = deserialized;
                var t = b2.Compile("test").FirstOrDefault(w => w.Name == "Test");
                var i = t.CreateInstance() as Interfaces.IPropertyGetAndSet;
                var linkCollection = i.GetPropertyValue<BaseClasses.IGenericAddNewList>("Links");

                int x = 0;
                foreach (var item in linkCollection.CreateAndAdd(3))
                {
                    x += 1;
                    var l = item as Interfaces.IPropertyGetAndSet;
                    l.SetPropertyValue("Url", $"{x}");
                    l.SetPropertyValue("Title", $"{x}");
                }

                var serializer = new SerializerBuilder().Build();

                var yaml = serializer.Serialize(b2);

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(i, Newtonsoft.Json.Formatting.Indented);

                Console.WriteLine(yaml);
                Console.WriteLine(
                    json
                );
            }
        }

    }
}
