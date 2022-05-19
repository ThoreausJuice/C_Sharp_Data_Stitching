using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

// 用于使用ArrayList
using System.Collections;

namespace ConsoleApp3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ArrayList a = new ArrayList();
            a.Add(1);
            a.Add("NULL");
            a.Add(3);
            for(int i = 0; i < a.Count; i++)
            {
                Console.WriteLine(a[i]);
            }
            Console.ReadLine();
        }

        public static int add_two_number(int x, int y)
        {
            return x + y;
        }
    }
}
