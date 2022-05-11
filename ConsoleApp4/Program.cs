using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Collections;


namespace ConsoleApp4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ArrayList a = new ArrayList();
            // 连接数据库
            string SQL_str = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = new";
            SqlConnection con = new SqlConnection(SQL_str);

            // 打开连接并判断连接状态
            con.Open();
            if(con.State == System.Data.ConnectionState.Open)
            {
                Console.WriteLine("连接成功");
            }
            if (con.State == System.Data.ConnectionState.Closed)
            {
                Console.WriteLine("连接失败");
            }

            // // 定义查询数据库的字符串
            // SqlCommand sql_cmd = new SqlCommand("select * from Class", con);
            // SqlDataReader sql_dr = sql_cmd.ExecuteReader();

            // 定义添加数据的字符串
            string sql_insert = "insert into [new].[dbo].[Class](class_id, class_section, class_grade, class_no, class_room_no) values(124, 'Middle_School', 'Grade_2', 4, '1-206')";
            SqlCommand sql_insert_cmd = new SqlCommand(sql_insert, con);
            sql_insert_cmd.ExecuteNonQuery();

            // if(sql_dr.HasRows)
            // {
            //     while(sql_dr.Read())
            //     {
            //         Console.Write(sql_dr["class_id"]);
            //         a.Add(sql_dr["class_id"]);
            //         Console.Write(" ");
            //         Console.Write(sql_dr["class_section"]);
            //         a.Add(sql_dr["class_section"]);
            //         Console.Write(" ");
            //         Console.Write(sql_dr["class_grade"]);
            //         a.Add(sql_dr["class_grade"]);
            //         Console.Write(" ");
            //         Console.Write(sql_dr["class_no"]);
            //         a.Add(sql_dr["class_no"]);
            //         Console.Write(" ");
            //         Console.Write(sql_dr["class_room_no"]);
            //         a.Add(sql_dr["class_room_no"]);
            //         Console.WriteLine();
            //     }
            // }
            // foreach(object b in a)
            // {
            //     Console.WriteLine(b.ToString());
            // }
            // Console.WriteLine(a);
            Console.ReadLine();
            // sql_dr.Close();
            con.Close();
        }
    }
}
