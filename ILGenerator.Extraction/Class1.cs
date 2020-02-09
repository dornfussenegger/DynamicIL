using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILGenerator.Extraction
{
    public class PropertyChangedTest : BaseClasses.NotifyPropertyChangedBase
    {

        public PropertyChangedTest()
        {
            webClient = new System.Net.WebClient();
            myList = new List<string>();
            myList2 = new List<int>();
        }

        public void Initialize()
        {
            //myList = (List<string>)CustomTypeBase(typeof(List<string>));
            //myList2 = (List<int>)CustomTypeBase.CreateInstanceOfType(typeof(List<int>));
            //webClient = (System.Net.WebClient)CustomTypeBase.CreateInstanceOfType(typeof(System.Net.WebClient));

        }

        private System.Net.WebClient webClient;

        public System.Net.WebClient WebClient
        {
            get { return webClient; }
            set { webClient = value; }
        }

        private List<string> myList;

        public List<string> MyList
        {
            get { return myList; }
            set { myList = value; }
        }

        private List<int> myList2;

        public List<int> MyList2
        {
            get { return myList2; }
            set { myList2 = value; }
        }



        private string backFieldString;

        public string MyStringProperty
        {
            get
            {
                return backFieldString;
            }
            set
            {
                var oldValue = backFieldString;
                if (Object.Equals(oldValue, value))
                {
                    backFieldString = value;
                    OnPropertyChanged("MyProperty", oldValue, value);
                }
            }
        }

        private Guid backGuidString;

        public Guid MyGuidProperty
        {
            get
            {
                return backGuidString;
            }
            set
            {
                var oldValue = backGuidString;
                if (Object.Equals(oldValue, value))
                {
                    backGuidString = value;
                    OnPropertyChanged("MyGuidProperty", oldValue, value);
                }
            }
        }




        public override object GetPropertyValue(string propertyName)
        {
            throw new NotImplementedException();
        }

        public override void SetPropertyValue(string propertyName, object value)
        {
            throw new NotImplementedException();
        }
    }

    public class PropertyChangedTestWithChangeTracker : BaseClasses.NotifyPropertyChangedBaseWithChangeTracker
    {
        public override object GetPropertyValue(string propertyName)
        {
            throw new NotImplementedException();
        }

        public override void SetPropertyValue(string propertyName, object value)
        {
            throw new NotImplementedException();
        }
    }
}
