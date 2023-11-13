using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT_APP.Services
{
    public interface IPrintInterface
    {
        public void Print();
    }

    class Person : IPrintInterface
    {
     
        //implementation
        public void Print()
        {
            Console.WriteLine("Person");
        }
    }

    class Employee : IPrintInterface
    {

        //implementation
        public void Print ()
        {
            Console.WriteLine("Company");
        }
    }
}
