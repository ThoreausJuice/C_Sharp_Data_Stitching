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
            string A_string_a = "s002";
            string A_time_a;
            string A_text = File.ReadAllText(@"测试文件\A.csv");
            string B_text = File.ReadAllText(@"测试文件\B.csv");
            
            char[] first_separator = {'\n'};
            char[] second_separator = {','};

            string[] A_first_split_strings = new string[100];
            A_first_split_strings = A_text.Split(first_separator);
            
            for(int i = 0; i < A_first_split_strings.Length; i++)
            {
                string[] A_second_split_strings = new string[100];
                A_second_split_strings = A_first_split_strings[i].Split(second_separator);
                for(int j = 0; j < A_second_split_strings.Length; j++)
                {
                    if (A_second_split_strings[0] == A_string_a)
                    {
                        A_time_a = A_second_split_strings[1];
                        Console.WriteLine(j);
                    } 
                    // Console.WriteLine(A_second_split_strings[j]);
                }
            }
            Console.WriteLine(1.2+'A');
            Console.Read();
        }
    }
}
