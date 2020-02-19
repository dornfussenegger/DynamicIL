using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILGenerator
{
    public abstract class Method
    {
        public Type ReturnType { get; set; }
        public Type[] ParameterTypes { get; set; }
        public System.Reflection.MethodInfo OverwriteMethod { get; set; }
        public abstract void ImplementMethodBody(System.Reflection.Emit.ILGenerator generator);
        public CustomTypeBase CustomType { get; private set; }
        public Method(CustomTypeBase onCustomType)
        {
            this.CustomType = onCustomType;
        }
    }
}
