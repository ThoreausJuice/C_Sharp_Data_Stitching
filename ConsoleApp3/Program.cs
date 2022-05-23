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
            List<ArrayList> a = new List<ArrayList>();
            List<ArrayList> b = new List<ArrayList>();
            List<ArrayList> c = new List<ArrayList>();
            a.Add(1);a.Add(2);a.Add(3);
            b.Add(4);b.Add(5);b.Add(6);
            c.Add(a);c.Add(b);
            foreach(var i in c)
            {
                foreach(var j in i)
                {
                    Console.WriteLine(j);
                }
            }
            // for(int i = 0; i < c.Count; i++)
            // {
            //     for(int j = 0; j < c[i].Count; j++)
            //     {
            //         Console.WriteLine(c[i][j]);
            //     }
            // }
            Console.ReadLine();
        }
        public static string test_function()
        {
            return System.Reflection.MethodBase.GetCurrentMethod().Name;
        }
    }
}
