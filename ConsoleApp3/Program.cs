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
            string a = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            a += "F001";
            a += "0001";
            int b = 10;
            ArrayList c = new ArrayList();
            c.Add(b);
            string d = b.ToString().PadLeft(4,'0');
            Console.WriteLine(d);
            Console.ReadLine();
        }
        public static string test_function()
        {
            return System.Reflection.MethodBase.GetCurrentMethod().Name;
        }
    }
}
