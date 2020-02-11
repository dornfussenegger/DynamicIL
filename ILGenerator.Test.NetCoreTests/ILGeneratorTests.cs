using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace ILGenerator.Test.NetCoreTests
{
    [TestClass]
    public class ILGeneratorTests
    {

        [TestMethod]
        public void GeneralTest()
        {
            var b = new ClassDefinition.BuildContextDefinition();
            var test = b.AddType("Test", ClassDefinition.DefinitionTypeEnums.SimpleType);
            test.AddProperty("FirstName", typeof(string));
            test.AddProperty("LastName", typeof(string));
            test.AddProperty("Uid", typeof(Guid));

            var t = b.Compile("TestAssembly")[0];
            dynamic instance = t.CreateInstance<dynamic>();
            instance.FirstName = "Lukas";
            instance.LastName = "Dorn-Fussenegger";
            instance.Uid = Guid.NewGuid();

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(instance);
            Console.WriteLine(json);

            dynamic deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject(json, t);
            //Creates: {"FirstName":"Lukas","LastName":"Dorn-Fussenegger","Uid":"7183685b-b314-4fd8-8bc5-7488e52618c4"}

            Assert.IsTrue(deserialized.FirstName == "Lukas");

        }

        [TestMethod]
        public void GetPropertyNamesAndTypes()
        {
            CustomTypeCreator ct = new CustomTypeCreator("testassembly");
            var cPerson = ct.CreateNewNotifyPropertyChangedType("Person", true);
            cPerson.AddProperty("FirstName", typeof(string));
            cPerson.AddProperty("LastName", typeof(string));
            cPerson.AddProperty("Uid", typeof(Guid));
            cPerson.AddIPropertyDescriptorImplementation();

            var types = ct.Build();
            var tPerson = types.FirstOrDefault(w => w.Name == "Person");
            var instance = tPerson.CreateInstance<Interfaces.IPropertyDescriptor>();


            Assert.IsTrue(instance.GetAllPropertyNames().Length == 3);
            Assert.IsTrue(instance.GetAllPropertyNames()[0] == "FirstName");

            Assert.IsTrue(instance.GetAllPropertyTypes().Length == 3);
            Assert.IsTrue(instance.GetAllPropertyTypes()[0] == typeof(string));

        }


        [TestMethod]
        public void NotifyPropertyChangedTestObjectWithChangeTracker()
        {
            CustomTypeCreator ct = new CustomTypeCreator("testassembly");
            var cPerson = ct.CreateNewNotifyPropertyChangedType("Person", true);
            cPerson.AddProperty("FirstName", typeof(string));
            cPerson.AddProperty("LastName", typeof(string));
            cPerson.AddProperty("Uid", typeof(Guid));

            var types = ct.Build();
            var tPerson = types.FirstOrDefault(w => w.Name == "Person");

            var iPerson = tPerson.CreateInstanceWithPropertyGetAndSet() as BaseClasses.NotifyPropertyChangedBaseWithChangeTracker;
            var hadChanges = false;
            iPerson.PropertyChanged += (s, a) => {
                hadChanges = true;
            };

            Assert.IsFalse(iPerson.ChangeTracker.Changes.Any());

            Guid guid = Guid.NewGuid();


            iPerson.SetPropertyValue("FirstName", "Lukas");
            iPerson.SetPropertyValue("LastName", "Dorn-Fussenegger");
            iPerson.SetPropertyValue("Uid", guid);

            Assert.IsTrue(iPerson.ChangeTracker.Changes.Count() == 3);
            Assert.IsTrue(iPerson.ChangeTracker.HasChangedProperty("FirstName"));
            Assert.IsTrue(iPerson.ChangeTracker.HasChangedProperty("LastName"));
            Assert.IsTrue(iPerson.ChangeTracker.HasChangedProperty("Uid"));



            Assert.IsTrue(iPerson.GetPropertyValue<string>("FirstName") == "Lukas");
            Assert.IsTrue(iPerson.GetPropertyValue<string>("LastName") == "Dorn-Fussenegger");
            Assert.IsTrue(iPerson.GetPropertyValue<Guid>("Uid") == guid);

            iPerson.ChangeTracker.ResetChanges();
            Assert.IsTrue(iPerson.ChangeTracker.Changes.Count() == 0);

            Assert.IsTrue(hadChanges);
        }

        [TestMethod]
        public void NotifyPropertyChangedTestObject()
        {
            CustomTypeCreator ct = new CustomTypeCreator("testassembly");
            var cPerson = ct.CreateNewNotifyPropertyChangedType("Person");
            cPerson.AddProperty("FirstName", typeof(string));
            cPerson.AddProperty("LastName", typeof(string));
            cPerson.AddProperty("Uid", typeof(Guid));

            var types = ct.Build();
            var tPerson = types.FirstOrDefault(w => w.Name == "Person");

            var iPerson = tPerson.CreateInstanceWithPropertyGetAndSet() as BaseClasses.NotifyPropertyChangedBase;
            var hadChanges = false;
            iPerson.PropertyChanged += (s, a) => {
                hadChanges = true;
            };

            Guid guid = Guid.NewGuid();


            iPerson.SetPropertyValue("FirstName", "Lukas");
            iPerson.SetPropertyValue("LastName", "Dorn-Fussenegger");
            iPerson.SetPropertyValue("Uid", guid);


            Assert.IsTrue(iPerson.GetPropertyValue<string>("FirstName") == "Lukas");
            Assert.IsTrue(iPerson.GetPropertyValue<string>("LastName") == "Dorn-Fussenegger");
            Assert.IsTrue(iPerson.GetPropertyValue<Guid>("Uid") == guid);

            Assert.IsTrue(hadChanges);
        }

        [TestMethod]
        public void SimpleObject()
        {
            CustomTypeCreator ct = new CustomTypeCreator("testassembly");
            var cPerson = ct.CreateNewSimpleType("Person");
            cPerson.AddProperty("FirstName", typeof(string));
            cPerson.AddProperty("LastName", typeof(string));
            cPerson.AddProperty("Uid", typeof(Guid));
            cPerson.AddPropertyGetAndSet();


            var types = ct.Build();
            var tPerson = types.FirstOrDefault(w => w.Name == "Person");

            var iPerson = tPerson.CreateInstanceWithPropertyGetAndSet();

            Guid guid = Guid.NewGuid();


            iPerson.SetPropertyValue("FirstName", "Lukas");
            iPerson.SetPropertyValue("LastName", "Dorn-Fussenegger");
            iPerson.SetPropertyValue("Uid", guid);


            Assert.IsTrue(iPerson.GetPropertyValue<string>("FirstName") == "Lukas");
            Assert.IsTrue(iPerson.GetPropertyValue<string>("LastName") == "Dorn-Fussenegger");
            Assert.IsTrue(iPerson.GetPropertyValue<Guid>("Uid") == guid);



        }

        [TestMethod]
        public void CollectionObject()
        {
            CustomTypeCreator ct = new CustomTypeCreator("testassembly");
            var cPerson = ct.CreateNewSimpleType("Person");
            cPerson.AddProperty("FirstName", typeof(string));
            cPerson.AddProperty("LastName", typeof(string));
            cPerson.AddProperty("Uid", typeof(Guid));

            var cContract = ct.CreateNewSimpleType("Contract");
            cContract.AddProperty("ContractNumber", typeof(string));
            cContract.AddProperty("Date", typeof(DateTime));
            cPerson.AddProperty("Contract", cContract.TypeBuilder);
            cPerson.AddPropertyOfGenericList("ConnectedPersons", cPerson.TypeBuilder);


            cPerson.AddPropertyGetAndSet();
            cContract.AddPropertyGetAndSet();

            var types = ct.Build();
            var tPerson = types.FirstOrDefault(w => w.Name == "Person");
            var tVertrag = types.FirstOrDefault(w => w.Name == "Contract");
            var iPerson = Activator.CreateInstance(tPerson, Array.Empty<object>());
            var iVertrag = Activator.CreateInstance(tVertrag, Array.Empty<object>());

            var wiPerson = (Interfaces.IPropertyGetAndSet)iPerson;
            var wiVertrag = (Interfaces.IPropertyGetAndSet)iVertrag;
            wiPerson.SetPropertyValue("FirstName", "Lukas");
            wiPerson.SetPropertyValue("LastName", "Dorn-Fussenegger");
            wiPerson.SetPropertyValue("Uid", Guid.NewGuid());
            wiPerson.SetPropertyValue("Contract", wiVertrag);

            wiVertrag.SetPropertyValue("ContractNumber", "1234-5678");
            wiVertrag.SetPropertyValue("Date", DateTime.Now);
            wiPerson.SetPropertyValue("ConnectedPersons", cPerson.BuildType.CreateInstanceOfList());
            for (int i = 0; i < 5; i++)
            {

                var li = wiPerson.GetPropertyValue("ConnectedPersons") as System.Collections.IList;

                var ci = tPerson.CreateInstanceWithPropertyGetAndSet();
                ci.SetPropertyValue("FirstName", $"Person{i}");
                ci.SetPropertyValue("Uid", Guid.NewGuid());
                li.Add(ci);

            }

            var l = wiPerson.GetPropertyValue("ConnectedPersons") as System.Collections.IList;
            Assert.IsTrue(l.Count == 5);
            var last = l[4] as Interfaces.IPropertyGetAndSet;
            Assert.IsTrue(last.GetPropertyValue<string>("FirstName") == "Person4");


        }
    }
}
