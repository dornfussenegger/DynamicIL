# DynamicIL

With the Package DynamicIL (MIT Licence) can classes be created during runtime using the Reflection Emit Library. Works with .net Full Framework and also with .net Core.

Important in this context is, that you have to know, how you can create dynamic code in .net. There are different approaches:

* Creating and Compiling Code
* Emit Objects with Code with IL Emit
* Use the .net Scripting Engine (ex. Ironpython)

With creating and compiling code at runtime, you can get results very fast. But, the compile process is not the fastest. If you use the .net scripting engine, you can change the code during runtime, because it is only interpreted. 

This Library uses a complete diffent way: it builds objects with .net Emit, where the IL Code is generated during runtime, and, in full framework enviruments it can be saved to disk for reusing the results.

The advantage of generating the code with IL: it is fast. And the result can be taken from any library via reflection. If you want to build a Webservice on the fly, generating or parsing json, this is an interessting option.


Example:

            CustomTypeCreator ct = new CustomTypeCreator("testassembly", "test.dll");
            var cPerson = ct.CreateNewSimpleType("Person");
            cPerson.AddProperty("Vorname", typeof(string));
            cPerson.AddProperty("Nachname", typeof(string));
            cPerson.AddProperty("Uid", typeof(Guid));

            var cContract = ct.CreateNewSimpleType("Vertrag");
            cContract.AddProperty("Vertragsnummer", typeof(string));
            cContract.AddProperty("Datum", typeof(DateTime));
            cPerson.AddProperty("Vertrag", cContract.TypeBuilder);
            cPerson.AddPropertyOfGenericList("VerbundenePersonen", cPerson.TypeBuilder);


            cPerson.AddPropertyGetAndSet();
            cContract.AddPropertyGetAndSet();

            var types = ct.Build();
            var tPerson = types.FirstOrDefault( w => w.Name == "Person");
            var tContract = types.FirstOrDefault(w => w.Name == "Vertrag");
            var iPerson = Activator.CreateInstance(tPerson, Array.Empty<object>());
            var iContract = Activator.CreateInstance(tContract, Array.Empty<object>());

            var wiPerson = (Interfaces.IPropertyGetAndSet)iPerson;
            var wiVertrag = (Interfaces.IPropertyGetAndSet)iContract;
            wiPerson.SetPropertyValue("Vorname", "Lukas");
            wiPerson.SetPropertyValue("Nachname", "Dorn-Fussenegger");
            wiPerson.SetPropertyValue("Uid", Guid.NewGuid());
            wiPerson.SetPropertyValue("Vertrag", wiVertrag);
            
            wiVertrag.SetPropertyValue("Vertragsnummer", "1234-5678");
            wiVertrag.SetPropertyValue("Datum", DateTime.Now);
            wiPerson.SetPropertyValue("VerbundenePersonen", cPerson.BuildType.CreateInstanceOfList());
            for (int i = 0; i < 5; i++)
            {

               var li = wiPerson.GetPropertyValue("VerbundenePersonen") as System.Collections.IList;

                var ci = tPerson.CreateInstanceWithPropertyGetAndSet();
                ci.SetPropertyValue("Vorname", $"Person{i}");
                ci.SetPropertyValue("Uid", Guid.NewGuid());
                li.Add(ci);

            }


            Console.WriteLine(
                Newtonsoft.Json.JsonConvert.SerializeObject(wiPerson, Newtonsoft.Json.Formatting.Indented)
                );

            Console.ReadKey();
