using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 用于连接数据库
using System.Data.SqlClient;
// 用于使用ArrayList
using System.Collections;

namespace ConsoleApp5
{
    internal class Program
    {
        static void Main(string[] args)
        {
			// 使用C#从数据库中读取三个阶段的数据，并将其中需要的数据进行拼接后，再存入数据库
            // 建立数据库连接
            string sql_str = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = import_test";
            SqlConnection con = new SqlConnection(sql_str);

            // 打开连接
            con.Open();
            // 判断当前连接状态
            if(con.State == System.Data.ConnectionState.Open)
            {
                Console.WriteLine("连接成功");
            }
            else if(con.State == System.Data.ConnectionState.Closed)
            {
                Console.WriteLine("连接失败");
            }

            // 定义 数据条数最小值 和 数据条数列表
            double Number_of_data_Min;
            double[] Number_of_data_list = new double [3];
            // ArrayList Number_of_data_list = new ArrayList();

            // 查询数据库中002,102,202三个表分别有多少条数据的命令
            SqlCommand sql_cmd_count_A = new SqlCommand("SELECT COUNT(*) FROM [import_test].[dbo].[002]", con);
            double Number_of_data_A = Convert.ToDouble(sql_cmd_count_A.ExecuteScalar());
            // Number_of_data_list.Add(Number_of_data_A);
            Number_of_data_list[0] = Number_of_data_A;
            
            SqlCommand sql_cmd_count_B = new SqlCommand("SELECT COUNT(*) FROM [import_test].[dbo].[102]", con);
            double Number_of_data_B = Convert.ToDouble(sql_cmd_count_B.ExecuteScalar());
            // Number_of_data_list.Add(Number_of_data_B);
            Number_of_data_list[1] = Number_of_data_B;
            
            SqlCommand sql_cmd_count_C = new SqlCommand("SELECT COUNT(*) FROM [import_test].[dbo].[202]", con);
            double Number_of_data_C = Convert.ToDouble(sql_cmd_count_C.ExecuteScalar());
            // Number_of_data_list.Add(Number_of_data_C);
            Number_of_data_list[2] = Number_of_data_C;
            
            // 对数组进行排序，第一个为最小值，即增量的除数
            Array.Sort(Number_of_data_list);
            Number_of_data_Min = Number_of_data_list[0];
            
            // 计算三个数据增量
            double[] data_increment = new double [3];
            data_increment[0] = Number_of_data_A / Number_of_data_Min;
            data_increment[1] = Number_of_data_B / Number_of_data_Min;
            data_increment[2] = Number_of_data_C / Number_of_data_Min;

            // 创建 各列 的动态数组，准备存储要拼接的数据
            ArrayList A_0 = new ArrayList();
            ArrayList B_0 = new ArrayList();
            ArrayList C_0 = new ArrayList();
            // 第一阶段取自002
            ArrayList A_3 = new ArrayList();
            ArrayList A_5 = new ArrayList();
            ArrayList A_10 = new ArrayList();
            ArrayList A_11 = new ArrayList();
            ArrayList A_2 = new ArrayList();
            // 第二阶段取自102
            ArrayList B_5 = new ArrayList();
            ArrayList B_10 = new ArrayList();
            ArrayList B_7 = new ArrayList();
            ArrayList B_1 = new ArrayList();
            // 第三阶段取自202
            ArrayList C_9 = new ArrayList();
            ArrayList C_6 = new ArrayList();

            // 查询002中所有数据的SQL语句
            SqlCommand sql_cmd_A = new SqlCommand("SELECT * FROM [import_test].[dbo].[002]", con);
            // 执行SQL语句，并生成一个包含数据的 SqlDataReader 对象的实例
            SqlDataReader sql_dr_A = sql_cmd_A.ExecuteReader();
            
            // 使用 HasRows 属性判断结果中是否有数据
            if (sql_dr_A.HasRows)
            {
                // 使用 Read() 方法读取 SqlDataReader
                // Read() 方法使 SqlDataReader 前进到下一条记录
                while(sql_dr_A.Read())
                {
                    A_0.Add(sql_dr_A["timee"]);
                    A_3.Add(sql_dr_A["叶片B线润叶加水流量"]);
                    A_5.Add(sql_dr_A["B线加料入口蒸汽阀门开度"]);
                    A_10.Add(sql_dr_A["叶片B线TBL注入蒸汽流量实际值"]);
                    A_11.Add(sql_dr_A["B线润叶前温度仪实际值"]);
                    A_2.Add(sql_dr_A["叶片B线润叶回风温度"]);
                }
            }

            // 在创建下一个 SQLDataReader 前，要关闭上一个对象
            sql_dr_A.Close();

            // 查询102中所有数据的SQL语句
            SqlCommand sql_cmd_B = new SqlCommand("SELECT * FROM [import_test].[dbo].[102]", con);
            // 执行SQL语句，并生成一个包含数据的 SqlDataReader 对象的实例
            SqlDataReader sql_dr_B = sql_cmd_B.ExecuteReader();
            
            // 使用 HasRows 属性判断结果中是否有数据
            if (sql_dr_B.HasRows)
            {
                // 使用 Read() 方法读取 SqlDataReader
                // Read() 方法使 SqlDataReader 前进到下一条记录
                while(sql_dr_B.Read())
                {
                    B_0.Add(sql_dr_B["timee"]);
                    B_5.Add(sql_dr_B["B线加料入口蒸汽阀门开度"]);
                    B_10.Add(sql_dr_B["叶片B线TBL注入蒸汽流量实际值"]);
                    B_7.Add(sql_dr_B["B线加水瞬时流量(流量计)"]);
                    B_1.Add(sql_dr_B["叶片B加料机出口温度 ℃ "]);
                }
            }

            // 在创建下一个 SQLDataReader 前，要关闭上一个对象
            sql_dr_B.Close();

            // 查询202中所有数据的SQL语句
            SqlCommand sql_cmd_C = new SqlCommand("SELECT * FROM [import_test].[dbo].[202]", con);
            // 执行SQL语句，并生成一个包含数据的 SqlDataReader 对象的实例
            SqlDataReader sql_dr_C = sql_cmd_C.ExecuteReader();
            
            // 使用 HasRows 属性判断结果中是否有数据
            if (sql_dr_C.HasRows)
            {
                // 使用 Read() 方法读取 SqlDataReader
                // Read() 方法使 SqlDataReader 前进到下一条记录
                while(sql_dr_C.Read())
                {
                    C_0.Add(sql_dr_C["timee"]);
                    C_9.Add(sql_dr_C["燃烧炉出口工艺气温度实际值"]);
                    C_6.Add(sql_dr_C["风选出口水分仪水分实际值"]);
                }
            }

            // 在创建下一个 SQLDataReader 前，要关闭上一个对象
            sql_dr_C.Close();

            // 将数据拼接并存入数据库
			for(int i = 0; i < Number_of_data_Min; i++)
			{
				// 定义添加数据的字符串
            	string sql_insert = "insert into import_test.dbo.[2021_02_done](" +
									"阶段A,"+
									"阶段B,"+
									"阶段C,"+
									"B线润叶加水流量,"+
									"B线润叶回风温度蒸汽阀开度,"+
									"B线润叶注入蒸汽流量,"+
									"B线润叶前温度仪实际值,"+
									"B线润叶回风温度,"+
									"B线加料入口蒸汽阀门开度,"+
									"叶片B线TBL注入蒸汽流量实际值,"+
									"B线加料加水瞬时流量,"+
									"[叶片B加料机出口温度[℃]]],"+
									"燃烧炉出口工艺气温度实际值,"+
									"风选出口水分仪水分实际值"+
									")values("+
									"'" + Convert.ToString(A_0[i]) +"'"+
									","+
									"'" + Convert.ToString(B_0[i]) +"'"+
									","+
									"'" + Convert.ToString(C_0[i]) +"'"+
									","+

									Convert.ToString(A_3[i])+","+
									Convert.ToString(A_5[i])+","+
									Convert.ToString(A_10[i])+","+
									Convert.ToString(A_11[i])+","+
									Convert.ToString(A_2[i])+","+
									
									Convert.ToString(B_5[i])+","+
									Convert.ToString(B_10[i])+","+
									Convert.ToString(B_7[i])+","+
									Convert.ToString(B_1[i])+","+

									Convert.ToString(C_9[i])+","+
									Convert.ToString(C_6[i])+
									")";
				// Console.WriteLine(sql_insert);
				SqlCommand sql_insert_cmd = new SqlCommand(sql_insert, con);
				sql_insert_cmd.ExecuteNonQuery();
				Console.WriteLine("已插入"+ Convert.ToString(i) +"条");
			}
            
			Console.WriteLine();
			Console.WriteLine("恭喜！所有数据均已 拼接 并 插入完成！");

            // // 打印测试
            // int j, k = 0;
            // for(double i = 0; i < Number_of_data_C; i += data_increment[2])
            // {
            //     j = (int)(i);
            //     Console.WriteLine(k);
            //     Console.WriteLine(C_0[j]);
            //     k++;
            // }

            // 关闭数据库连接
            con.Close();
            // cmd停留
            Console.ReadLine();
        }
    }
}
