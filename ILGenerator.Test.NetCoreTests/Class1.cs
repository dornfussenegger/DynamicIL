using System;
using System.Collections.Generic;
using System.Text;

namespace ILGenerator.Test.NetCoreTests
{
    public class DefinitionTests
    {

        public void GeneralTest()
        {
            var b = new ClassDefinition.BuildContextDefinition();
            var test = b.AddType("Test", ClassDefinition.DefinitionTypeEnums.SimpleType);
            test.AddProperty("FirstName", typeof(string));
            test.AddProperty("LastName", typeof(string));
            test.AddProperty("Uid", typeof(Guid));

        }

    }
}
