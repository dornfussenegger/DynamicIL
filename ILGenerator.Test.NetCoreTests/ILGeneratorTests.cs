using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace ILGenerator.Test.NetCoreTests
{
    [TestClass]
    public class ILGeneratorTests
    {

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
