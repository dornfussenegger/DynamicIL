using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILGenerator.Extraction
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var t = typeof(PropertyChangedTest);
            var m = t.GetMethod("GetPropertyValue");
            var e = m.Attributes;
            Console.WriteLine(e);
            Console.WriteLine("---");
            foreach (System.Reflection.MethodAttributes item in Enum.GetValues(typeof(System.Reflection.MethodAttributes)))
            {
                if (e.HasFlag(item))
                {
                    Console.WriteLine($"System.Reflection.MethodAttributes.{item} |");
                }
            }
            Console.ReadLine();
        }
    }
}
