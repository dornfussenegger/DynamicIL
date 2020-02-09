
# DynamicIL

With the Package DynamicIL (MIT Licence) can classes be created during runtime using the Reflection Emit Library. Works with .net Full Framework and also with .net Core.

Important in this context is, that you have to know, how you can create dynamic code in .net. There are different approaches:

* Creating and Compiling Code
* Emit Objects with Code with IL Emit
* Use the .net Scripting Engine (ex. Ironpython)

With creating and compiling code at runtime, you can get results very fast. But, the compile process is not the fastest. If you use the .net scripting engine, you can change the code during runtime, because it is only interpreted. 

This Library uses a complete diffent way: it builds objects with .net Emit, where the IL Code is generated during runtime, and, in full framework enviruments it can be saved to disk for reusing the results.

The advantage of generating the code with IL: it is fast. And the result can be taken from any library via reflection. If you want to build a Webservice on the fly, generating or parsing json, this is an interessting option.
An interessting possible use case is, to save the structure of objects in a database, add a bit of logic and create by this way a complete Business Layer.


Example of creating a simple Type with the INotifyPropertyChanged Interface:

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

Example of creating simple objects:

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

This library works with .net Full Framework AND .net Core. Saving Types to a DLL is only possible with the full framework. With .net Core it is only possible with an inmemory-creation.

