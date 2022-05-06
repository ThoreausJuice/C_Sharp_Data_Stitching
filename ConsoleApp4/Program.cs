using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int i = 5, j = 5;
            // Console.WriteLine(i += i++);
            // Console.WriteLine(i);
            // int result = (i += i++) > j ? i : j;

            if(i++ > j)
            {
                Console.WriteLine(1);
                Console.WriteLine(i);
            }
            else
            {
                Console.WriteLine(2);
                Console.WriteLine(i);
            }
            // Console.WriteLine(result);
            Console.ReadLine();
        }
    }
}
