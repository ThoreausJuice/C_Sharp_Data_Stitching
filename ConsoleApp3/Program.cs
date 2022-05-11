using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double b, c;
            b = 2049;
            c = 1863;
            double a = b / c;
            Console.WriteLine(a);
            Console.ReadLine();
        }

        public static int add_two_number(int x, int y)
        {
            return x + y;
        }
    }
}
