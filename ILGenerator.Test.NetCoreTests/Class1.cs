using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace ILGenerator.Test.NetCoreTests
{

    [TestClass]
    public class DefinitionTests
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

    }
}
