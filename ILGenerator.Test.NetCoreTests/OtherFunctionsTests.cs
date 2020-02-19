using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ILGenerator.Test.NetCoreTests
{
    [TestClass]
    public class OtherFunctionsTests
    {

        [TestMethod]
        public void ToCsNameTests()
        {
            Assert.IsTrue(typeof(string).GetCSTypeName() == "string");
            Assert.IsTrue(typeof(List<string>).GetCSTypeName() == "System.Collections.Generic.List<string>");
            Assert.IsTrue(typeof(Dictionary<string, object>).GetCSTypeName() == "System.Collections.Generic.Dictionary<string, object>");
            Assert.IsTrue(typeof(int[]).GetCSTypeName() == "int[]");
            Assert.IsTrue(typeof(int).GetCSTypeName() == "int");
            Assert.IsTrue(typeof(bool).GetCSTypeName() == "bool");
            Assert.IsTrue(typeof(Guid).GetCSTypeName() == "System.Guid");
            Assert.IsTrue(typeof(double).GetCSTypeName() == "double");
            Assert.IsTrue(typeof(float).GetCSTypeName() == "float");
            Assert.IsTrue(typeof(object[]).GetCSTypeName() == "object[]");
            Assert.IsTrue(typeof(string[]).GetCSTypeName() == "string[]");
            Assert.IsTrue(typeof(Int64).GetCSTypeName() == "long");
        }

        [TestMethod]
        public void ChangeTrackerTest()
        {

            var o = new { FirstName="Lukas", Nachname="Dorn-Fussenegger"};
            var ct = new BaseClasses.ChangeTracker(o);
            bool iNotifyPropertyChangedFiredEvent = false;
            ct.PropertyChanged += (sender, args) => { iNotifyPropertyChangedFiredEvent = true; };

            Assert.IsTrue(iNotifyPropertyChangedFiredEvent == false);

            ct.OnChanged(nameof(o.FirstName), "Lukas", "Lukas2");

            Assert.IsTrue(iNotifyPropertyChangedFiredEvent == true);

            Assert.IsTrue(ct.HasChanges);
            Assert.IsTrue(ct.GetPropertyChanges(nameof(o.FirstName)).Count() == 1);

            ct.ResetChanges();

            Assert.IsFalse(ct.HasChanges);
            Assert.IsTrue(ct.GetPropertyChanges(nameof(o.FirstName)).Count() == 0);



        }

    }

}
