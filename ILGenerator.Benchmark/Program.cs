using System;
using System.Linq;

namespace ILGenerator.Benchmark
{
    class Program
    {

        public static void GenerateAndTestTypeCreation()
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

            if (iPerson.GetPropertyValue<string>("FirstName") != "Lukas")
            {
                throw new Exception("Check failed");
            }
           
        }


        static void Main(string[] args)
        {
            int interations = 1000;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for (int i = 0; i < interations; i++)
            {
                GenerateAndTestTypeCreation();
                if (i % 100 == 0)
                {
                    Console.Write(".");
                }
            }
            sw.Stop();
            Console.WriteLine($"Created with IL Generator: {sw.ElapsedMilliseconds} ms");
            Console.ReadLine();

        }
    }
}
