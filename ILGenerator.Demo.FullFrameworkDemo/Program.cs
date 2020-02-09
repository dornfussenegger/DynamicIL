using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace ILGenerator.Demo.FullFrameworkDemo
{

    public class TestAttribute : Attribute
    {

        public string Name { get; set; }

        public TestAttribute(string name)
        {
            this.Name = name;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Test1();
            //Test2();
            //Test3();

            Console.ReadKey();
        }

        private static void Test2()
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

        private static void Test3()
        {
            CustomTypeCreator ct = new CustomTypeCreator("testassembly2", "test2.dll");
            var cPerson = ct.CreateNewSimpleType("Person");
            cPerson.AddProperty("FirstName", typeof(string));
            cPerson.AddProperty("LastName", typeof(string));
            cPerson.AddProperty("Uid", typeof(Guid));
            cPerson.AddProperty("WebClient", typeof(System.Net.WebClient));

            var cContract = ct.CreateNewSimpleType("Contract");
            cContract.AddProperty("ContractNumber", typeof(string));
            cContract.AddProperty("Date", typeof(DateTime));
            cPerson.AddProperty("Contract", cContract.TypeBuilder);
            

            cPerson.AddPropertyGetAndSet();
            cContract.AddPropertyGetAndSet();

            //cPerson.AddIInitializeableImplementation();
            //cContract.AddIInitializeableImplementation();


            var types = ct.Build();
            var tPerson = types.FirstOrDefault(w => w.Name == "Person");
            var tVertrag = types.FirstOrDefault(w => w.Name == "Contract");
            var iPerson = tPerson.CreateInstance();
            var iVertrag = tVertrag.CreateInstance();
        }

            private static void Test1()
        {
            CustomTypeCreator ct = new CustomTypeCreator("testassembly", "test.dll");
            var cPerson = ct.CreateNewNotifyPropertyChangedType("Person", true).SetCustomCustomTypeFlagsFlags(CustomTypeBaseFlags.AddIInitializeableImplementation, true);
            cPerson.AddProperty("FirstName", typeof(string));
            cPerson.AddProperty("LastName", typeof(string));
            cPerson.AddProperty("Uid", typeof(Guid));
            cPerson.AddProperty("WebClient", typeof(System.Net.WebClient)).SetCustomPropertyFlags(CustomPropertyFlags.InitializeInInitializeMethod, true);

            var cContract = ct.CreateNewNotifyPropertyChangedType("Contract", true).SetCustomCustomTypeFlagsFlags(CustomTypeBaseFlags.AddIInitializeableImplementation, true);
            cContract.AddProperty("ContractNumber", typeof(string));
            cContract.AddProperty("Date", typeof(DateTime));
            cPerson.AddProperty("Contract", cContract.TypeBuilder).SetCustomPropertyFlags(CustomPropertyFlags.InitializeInInitializeMethod, true);
            cPerson.AddPropertyOfGenericList("ConnectedPersons", cPerson.TypeBuilder).SetCustomPropertyFlags(CustomPropertyFlags.InitializeInInitializeMethod, true);

            cPerson.AddPropertyGetAndSet();
            cContract.AddPropertyGetAndSet();

            //cPerson.AddIInitializeableImplementation();
            //cContract.AddIInitializeableImplementation();
            cPerson.AddIPropertyDescriptorImplementation();
            cContract.AddIPropertyDescriptorImplementation();


            var types = ct.Build();
            var tPerson = types.FirstOrDefault(w => w.Name == "Person");
            var tVertrag = types.FirstOrDefault(w => w.Name == "Contract");
            var iPerson = tPerson.CreateInstance();
            var iVertrag = tVertrag.CreateInstance<Interfaces.IPropertyDescriptor>();
            foreach (var item in iVertrag.GetAllPropertyNames())
            {
                Console.WriteLine(item);
            }
            foreach (var item in iVertrag.GetAllPropertyTypes())
            {
                Console.WriteLine(item.FullName);
            }

            //var propChangedOnPerson = (System.ComponentModel.INotifyPropertyChanged)iPerson;
            //propChangedOnPerson.PropertyChanged += (s, a) =>
            //{
            //    Console.WriteLine($"PropertyChanged: {a.PropertyName}");
            //};

            //var wiPerson = (Interfaces.IPropertyGetAndSet)iPerson;
            //var wiVertrag = (Interfaces.IPropertyGetAndSet)iVertrag;
            //wiPerson.SetPropertyValue("FirstName", "Lukas");
            //wiPerson.SetPropertyValue("LastName", "Dorn-Fussenegger");
            //wiPerson.SetPropertyValue("Uid", Guid.NewGuid());
            ////wiPerson.SetPropertyValue("Contract", wiVertrag);

            //wiVertrag.SetPropertyValue("ContractNumber", "1234-5678");
            //wiVertrag.SetPropertyValue("Date", DateTime.Now);
            ////wiPerson.SetPropertyValue("ConnectedPersons", cPerson.BuildType.CreateInstanceOfList());
            //for (int i = 0; i < 5; i++)
            //{

            //    var li = wiPerson.GetPropertyValue("ConnectedPersons") as System.Collections.IList;

            //    var ci = tPerson.CreateInstanceWithPropertyGetAndSet();
            //    ci.SetPropertyValue("FirstName", $"Person{i}");
            //    ci.SetPropertyValue("Uid", Guid.NewGuid());
            //    li.Add(ci);

            //}


            //Console.WriteLine(
            //    Newtonsoft.Json.JsonConvert.SerializeObject(wiPerson, Newtonsoft.Json.Formatting.Indented)
            //    );
        }
    }
}
