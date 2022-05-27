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
            List<string> a = new List<string>();
            // 向集合中填充数据
            for (int i = 0; i < 10; i++)
            {
                // a.Add(i);
                a.Add(Convert.ToString(i));
            }

            // 方法一：
            if (a.Exists(x => Convert.ToInt32(x) < 1))
            {
                Console.WriteLine("yes1");
            }
            else
            {
                Console.WriteLine("no1");
            }

            // // 方法二：
            // if (a.Contains("5") == false)
            // {
            //     Console.WriteLine("yes2");
            // }
            // else
            // {
            //     Console.WriteLine("no2");
            // }

            Console.ReadLine();
        }
        public static void Print_1D(List<string> any_collection)
        {
            // 该函数用来查看编程中获取的一维集合的情况

            for (int i = 0; i < any_collection.Count; i++)
            {
                Console.WriteLine(i + " : " + any_collection[i]);
            }
            Console.WriteLine();
        }
    }
}
