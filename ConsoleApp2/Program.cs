using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    internal class Program
    {
        static int b;
        static void Main(string[] args)
        {
            int a = 5;
            // int b;
            for(int i = 0; i < 10; i++)
            {
                if(i == a)
                {
                    b = 1;
                    // Console.WriteLine("相等");
                }
            }

            for(int i = 0; i < 10; i++)
            {
                if(i == b)
                {
                    Console.WriteLine("相等");
                    Console.WriteLine("值为{0}", b);
                    Console.WriteLine(b.GetType());
                }
            }
            Console.ReadLine();
        }
    }
}
