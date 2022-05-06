using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace ConsoleApp1
{
    internal class Program
    {
        static string A_time_a;
        static void Main(string[] args)
        {
            //从A文件中找出特定字符串A_string_a对应的时间A_time_a
            string A_string_a = "s002";
            // string A_time_a;
            string A_text = File.ReadAllText(@"测试文件\A.csv");

            char[] first_separator = {'\n'};
            char[] second_separator = {','};
            
            string[] A_first_split_strings = new string[100];
            A_first_split_strings = A_text.Split(first_separator);
            
            for(int i = 0; i < A_first_split_strings.Length; i++)
            {
                String[] A_second_split_strings = new String[100];
                A_second_split_strings = A_first_split_strings[i].Split(second_separator);
                
                for(int j = 0; j < A_second_split_strings.Length; j++)
                {
                    if (A_second_split_strings[0] == A_string_a)
                    {
                        A_time_a = A_second_split_strings[1];
                        // Console.WriteLine("time:{0}", A_time_a);
                    } 
                    // Console.WriteLine(A_second_split_strings[j]);
                }
            }
            
            // 从B文件中找出A_time_a对应的数据B_data_a()
            string B_text = File.ReadAllText(@"测试文件\B.csv");
            string[] B_first_split_strings = new string[100];
            B_first_split_strings = B_text.Split(first_separator);
            for(int i = 0; i < B_first_split_strings.Length; i++)
            {
                String[] B_second_split_strings = new String[100];
                B_second_split_strings = B_first_split_strings[i].Split(second_separator);
                
                for(int j = 0; j < B_second_split_strings.Length; j++)
                {
                    // Console.WriteLine(B_second_split_strings[0]);
                    // Console.WriteLine(B_second_split_strings[0].GetType());
                    // Console.WriteLine(A_time_a);
                    // Console.WriteLine(A_time_a.GetType());
                    string a = B_second_split_strings[0];
                    string b = Program.A_time_a;
                    if (a == b)
                    {
                        Console.WriteLine(B_second_split_strings[0]);
                        Console.WriteLine(A_time_a);
                        Console.WriteLine("相等");
                    }
                }
            }
            Console.ReadLine();
        }
    }
}
