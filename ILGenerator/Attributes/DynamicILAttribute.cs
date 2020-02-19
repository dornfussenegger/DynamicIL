using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILGenerator.Attributes
{
    public abstract class DynamicILAttribute : Attribute
    {
    }

    public class DataMapping : DynamicILAttribute
    {
        public string Map { get; private set; }
        public DataMapping(string map)
        {
            this.Map = map;
        }
    }

    public class Identifier : DynamicILAttribute
    {
        public string Id { get; private set; }
        public Identifier(string id)
        {
            this.Id = id;
        }
    }
}
