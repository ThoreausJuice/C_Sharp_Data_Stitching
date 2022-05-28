using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 用于连接数据库
using System.Data.SqlClient;

namespace ConsoleApp6
{
    internal class Program
    {
        // 第5版
        // 该版本全面采用泛型List<T>替代之前版本的ArrayList，使得程序因二维集合的存在而更加灵活
        static void Main(string[] args)
        {
            // 计时开始
            DateTime program_start_time = System.DateTime.Now;
            Print_Line("开始时间：" + program_start_time);
            // 初始化数据表
            Truncate_Table("[T_Pred_Data]");
            Truncate_Table("[T_Pred_Test]");
            // ---------------------------------------------------------------------------------------

            // 【第一步】获取 T_Pred_Structure 二维集合
            // 表中的所有列，及其列中的值，存入一个二维List集合
            // 以2022/05/24数据库的情况，将会是一个包含4个小集合的大集合，每个小集合含有该列的所有值
            List<List<string>> T_Pred_Structure_2D_collection = new List<List<string>>();
            T_Pred_Structure_Get(ref T_Pred_Structure_2D_collection);

            // ---------------------------------------------------------------------------------------

            // 【第二步】获取 BatchId 集合
            List<string> BatchId_collection = new List<string>();
            BatchID_Get(ref BatchId_collection);

            // ---------------------------------------------------------------------------------------

            // 设定阶段总数
            int total_number_of_stages = 4;
            // 设定实际用到的阶段总数
            int real_number_of_stages = 3;
            // 数出每个阶段需要的一维集合数量
            // 这里用数组不用集合的原因可以看 Quantity_Get() 函数，因为更方便 
            int[] stage_data_count = new int[3]{0,0,0};
            int[] workshop_data_count = new int[3]{0,0,0};
            Quantity_Get(T_Pred_Structure_2D_collection,
                         ref stage_data_count,
                         ref workshop_data_count);

            // 【第三步】针对每一个 BatchId 进行操作
            for(int BatchId_index = 0; BatchId_index < BatchId_collection.Count; BatchId_index++) //BatchId_collection.Count
            {
                // 越过阶段少于3的 BatchId
                string BatchID = BatchId_collection[BatchId_index];
                if(Stage_All_Length(BatchID) < 3)
                {
                    continue;
                }

                // 【第四步】获取 阶段时间的二维集合 和 工单号的二维集合
                List<List<string>> stage_whole_time_2D_collection = new List<List<string>>();
                Create_a_2D_Collection(total_number_of_stages, ref stage_whole_time_2D_collection);
                List<List<string>> WorkOrderID_whole_stage_2D_collection = new List<List<string>>();
                Create_a_2D_Collection(total_number_of_stages, ref WorkOrderID_whole_stage_2D_collection);
                Period_and_WorkOrderID_Get(BatchID,
                                           ref stage_whole_time_2D_collection,
                                           ref WorkOrderID_whole_stage_2D_collection);

                // 【第五步】获取 各个阶段的数据
                
                // ------------------------------------------------------------------------------------------
                
                // 非温湿度时间集合
                List<List<string>> stage_time_point_2D_collection = new List<List<string>>();
                Create_a_2D_Collection(real_number_of_stages, ref stage_time_point_2D_collection);

                // 工单号集合
                List<List<string>> WorkOrderID_Amplify_2D_collection = new List<List<string>>();
                Create_a_2D_Collection(real_number_of_stages, ref WorkOrderID_Amplify_2D_collection);
                
                // 创建一个三维集合，存储所有需要拼接的数据
                // 第一维：有多少个阶段；第二维：每个阶段有多少种数据；第三维：每种数据有多少条
                List<List<List<string>>> all_stage_data_3D_collection = new List<List<List<string>>>();
                Create_a_3D_Collection(stage_data_count, ref all_stage_data_3D_collection);

                // ------------------------------------------------------------------------------------------
                // 温湿度这里后续到现场如果不需要时间对比就能直接拿到数据，就可以省掉下面那个三维集合，将其与上面的三维集合整合到一起

                // 温湿度时间集合
                List<List<string>> workshop_time_2D_collection = new List<List<string>>();
                Create_a_2D_Collection(real_number_of_stages, ref workshop_time_2D_collection);

                // 创建一个三维集合，存储所有需要拼接的数据
                // 第一维：有多少个阶段；第二维：每个阶段有多少种数据；第三维：每种数据有多少条
                List<List<List<string>>> all_workshop_data_3D_collection = new List<List<List<string>>>();
                Create_a_3D_Collection(workshop_data_count, ref all_workshop_data_3D_collection);

                // ------------------------------------------------------------------------------------------
                
                // 定义数据年份
                int data_year = 2021;
                int sql_month = 0;
                // 定义数据标志
                string DataType;

                // 获取各阶段非温湿度的数据，数据存在 all_stage_data_3D_collection 里
                // 非温湿度数据的标志
                DataType = "0";
                for(int which_stage = 0; which_stage < real_number_of_stages; which_stage++)
                {
                    Data_Table_Name(which_stage, ref stage_whole_time_2D_collection, ref data_year, ref sql_month);
                    List<string> featureName_need_now = new List<string>();
                    featureName_Need_Now_Get(which_stage, DataType,
                                             ref featureName_need_now, 
                                             T_Pred_Structure_2D_collection);
                    Stage_Whatever_Data_Get(which_stage, sql_month, 
                                            stage_whole_time_2D_collection,
                                            featureName_need_now,
                                            ref stage_time_point_2D_collection,
                                            ref WorkOrderID_Amplify_2D_collection,
                                            WorkOrderID_whole_stage_2D_collection,
                                            ref all_stage_data_3D_collection);
                }
                
                // 定义 数据条数最小值 和 数据条数集合
                double number_of_stage_Min;
                List<double> number_of_stage_collection = new List<double>();
                // 记录三个阶段的数据条数
                for(int i = 0; i < stage_time_point_2D_collection.Count; i++)
                {
                    number_of_stage_collection.Add(stage_time_point_2D_collection[i].Count);
                }
                // 记录集合中的最小值
                number_of_stage_Min = number_of_stage_collection.Min();
                // 计算三个数据增量
                List<double> data_increment_collection = new List<double>();
                for(int i = 0; i < number_of_stage_collection.Count; i++)
                {
                    data_increment_collection.Add(number_of_stage_collection[i] / number_of_stage_Min);
                }
                // 定义三个阶段拼接时会用到的double索引 和 取整后的int索引
                List<double> stage_whatever_index_collection = new List<double>();
                List<int> s_w_i_collection = new List<int>();
                for(int i = 0; i < data_increment_collection.Count; i++)
                {
                    stage_whatever_index_collection.Add(0);
                    s_w_i_collection.Add(0);
                }
                
                // 获取各阶段温湿度的数据，数据存在 all_workshop_data_3D_collection 里
                // 温湿度数据的标志
                DataType = "1";
                for(int which_stage = 0; which_stage < real_number_of_stages; which_stage++)
                {
                    List<string> featureName_need_now = new List<string>();
                    featureName_Need_Now_Get(which_stage, DataType,
                                             ref featureName_need_now, 
                                             T_Pred_Structure_2D_collection);
                    Workshop_Data_Get(which_stage,
                                      stage_time_point_2D_collection,
                                      data_increment_collection,
                                      featureName_need_now,
                                      ref workshop_time_2D_collection,
                                      ref all_workshop_data_3D_collection);
                }

                // 将两种特征ID分别存放
                List<string> non_environment_FeatureID_collection = new List<string>();
                List<string> environment_FeatureID_collection = new List<string>();
                for(int column_num = 0; column_num < T_Pred_Structure_2D_collection[0].Count; column_num ++)
                {
                    if (T_Pred_Structure_2D_collection[4][column_num] == "0")
                    {
                        non_environment_FeatureID_collection.Add(T_Pred_Structure_2D_collection[0][column_num]);
                    }
                    else if(T_Pred_Structure_2D_collection[4][column_num] == "1")
                    {
                        environment_FeatureID_collection.Add(T_Pred_Structure_2D_collection[0][column_num]);
                    }
                }

                // 拼接标志初始化
                int specific_flag = 0;
                // 数据插入开始
                for(int i = 0; i < number_of_stage_Min; i++)
                {
                    // 根据double索引确定int索引
                    for(int j = 0; j < s_w_i_collection.Count; j++)
                    {
                        s_w_i_collection[j] = Convert.ToInt32(Math.Floor(stage_whatever_index_collection[j]));
                    }
                    
                    // 判断温湿度是否为空，非空才进行插入
                    int abandoned_sign = 0;
                    for(int level_one = 0; level_one < all_workshop_data_3D_collection.Count; level_one ++)
                    {
                        for(int level_two = 0; level_two < all_workshop_data_3D_collection[level_one].Count; level_two ++)
                        {
                            if(Convert.ToString(all_workshop_data_3D_collection[level_one][level_two][i]) == "NULL")
                            {
                                abandoned_sign += 1;
                            }
                        }
                    }
                    if(abandoned_sign == 0)
                    {
                        specific_flag ++;
                        int column_num = 0;
                        // 非环境数据插入
                        for(int which_stage = 0; which_stage < all_stage_data_3D_collection.Count; which_stage ++)
                        {
                            for(int which_column = 0; which_column < all_stage_data_3D_collection[which_stage].Count; which_column ++)
                            {
                                Splicing_Scheme_1(WorkOrderID_Amplify_2D_collection[which_stage][s_w_i_collection[which_stage]],
                                                  BatchID,
                                                  stage_time_point_2D_collection[which_stage][s_w_i_collection[which_stage]],
                                                  non_environment_FeatureID_collection[column_num],
                                                  Convert.ToString(specific_flag),
                                                  all_stage_data_3D_collection[which_stage][which_column][s_w_i_collection[which_stage]]);
                                column_num += 1;
                            }
                        }
                        column_num = 0;
                        // 环境数据插入
                        for(int which_stage = 0; which_stage < all_workshop_data_3D_collection.Count; which_stage ++)
                        {
                            for(int which_column = 0; which_column < all_workshop_data_3D_collection[which_stage].Count; which_column ++)
                            {
                                Splicing_Scheme_1(WorkOrderID_Amplify_2D_collection[which_stage][s_w_i_collection[which_stage]],
                                                  BatchID,
                                                  workshop_time_2D_collection[which_stage][i],
                                                  environment_FeatureID_collection[column_num],
                                                  Convert.ToString(specific_flag),
                                                  all_workshop_data_3D_collection[which_stage][which_column][i]);
                                column_num += 1;
                            }
                        }

                        // (可选)小张函数
                        Splicing_Scheme_for_Miss_Zhang(non_environment_FeatureID_collection,
                                                       environment_FeatureID_collection,
                                                       all_stage_data_3D_collection,
                                                       all_workshop_data_3D_collection,
                                                       i);
                    }
                    // 变更索引
                    for(int j = 0; j < stage_whatever_index_collection.Count; j++)
                    {
                        stage_whatever_index_collection[j] += data_increment_collection[j];
                    }
                }
                Console.WriteLine(BatchId_index + " : " + BatchID);
                Console.WriteLine(specific_flag + " / " + number_of_stage_Min);
            }
            Console.WriteLine("fin");
            // 计时结束
            DateTime program_end_time = System.DateTime.Now;
            Print_Line("结束时间：" + program_end_time);
            TimeSpan ts = program_end_time.Subtract(program_start_time);
            Print_Line("用时：" + ts);
            // 用时：01:24:44.4947234
            Console.ReadLine();
        }

        // 以下皆是自造函数
        // 按首字母排序，功能在函数第一行注释皆有说明

        public static void BatchID_Get(ref List<string> BatchId_collection)
        {
            // 该函数用来获取符合条件的 BatchID

            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = CPINFO";
            SqlConnection con = new SqlConnection(sql_link);
            // 打开连接
            con.Open();

            // 拿到符合条件的所有 BatchID
            // 该段程序的逻辑为：使用SQL语句查询出需要的 outputMaterialName 及相应的生产时间范围，并根据 BatchId 进行排序，保证相同的 BatchId 能够相邻
            // 定义查询数据库的字符串
            string sql_inquire = "SELECT * FROM [CPINFO].[dbo].[V_I_WorkOrder]"+
                                    "where outputMaterialName like '黄山（红方印细支）%'"+
                                    "and FactStartTime between cast('2021-01-01' as datetime) and cast('2022-01-01' as datetime)"+
                                    "order by BatchId";
            // 新建连接及查询命令
            SqlCommand sql_cmd = new SqlCommand(sql_inquire, con);
            SqlDataReader sql_dr = sql_cmd.ExecuteReader();
            
            // 通过定义 当前数据 及 先前数据 实现将重复的 BatchId 提取出来
            string current_data, previous_data = "";
            
            while(sql_dr.Read())
            {
                current_data = Convert.ToString(sql_dr["BatchId"]);
                if(previous_data != current_data)
                {
                    BatchId_collection.Add(current_data);
                    previous_data = current_data;
                }
            }
            // 在创建下一个 SQLDataReader 前，要关闭上一个对象
            sql_dr.Close();
            // 关闭数据库连接
            con.Close();
        }

        public static void Create_a_2D_Collection(int any_number, ref List<List<string>> any_collection)
        {
            // 创造一个 给定数字长度 的二维集合

            for(int i = 0; i < any_number; i++)
            {
                any_collection.Add(new List<string>());
            }
        }

        public static void Create_a_3D_Collection(int[] any_array, ref List<List<List<string>>> any_collection)
        {
            // 创造一个 给定数字长度 的三维集合

            for(int i = 0; i < any_array.Length; i++)
            {
                any_collection.Add(new List<List<string>>());
                for(int j = 0; j < any_array[i]; j++)
                {
                    any_collection[i].Add(new List<string>());
                }
            }
        }

        public static void Data_Table_Name(int which_stage, ref List<List<string>> stage_whole_time_2D_collection, ref int data_year, ref int sql_month)
        {
            // 这个 数据表名 方法（函数）所在做的事情就是根据阶段中 索引为0的时间 和 阶段中最后一个时间 判断他们属于哪个月份，然后把月份和年份组合成表名。
            
            // 月份初始化
            sql_month = 0;
            // 阶段所属月份判断
            DateTime time_start = Convert.ToDateTime(stage_whole_time_2D_collection[which_stage][0]);
            DateTime time_end = Convert.ToDateTime(stage_whole_time_2D_collection[which_stage][stage_whole_time_2D_collection[which_stage].Count -1]);
            DateTime beginning_of_the_year = new DateTime(data_year,1,1);
            int stage_month = 0;
            // 这一段非常巧妙，设定一个年初的日子（例：2021,1,1），然后每次加一个月，使用月初和月末构成的范围 判断始末两个时间点是否在这个范围内
            // 这一步如果只通过月末来判断的话，会出现1月的数据小于2月，也小于3月4月，导致最后1月的数据会被定位在错误的月份
            for(int month_increment = 0; month_increment < 12; month_increment++)
            {
                if(time_start >= beginning_of_the_year.AddMonths(month_increment) &&
                    time_start < beginning_of_the_year.AddMonths(month_increment +1) && 
                    time_end >= beginning_of_the_year.AddMonths(month_increment) &&
                    time_end < beginning_of_the_year.AddMonths(month_increment +1))
                {
                    stage_month = month_increment +1;
                }
            }
            // 将月份和年份组合成 数据表名 需要的格式。例：202101
            sql_month = data_year * 100 + stage_month;
        }

        public static void featureName_Need_Now_Get(int which_stage, string DataType,
                                                    ref List<string> featureName_need_now,
                                                    List<List<string>> T_Pred_Structure_2D_collection)
        {
            switch(which_stage)
            {
                case 0:
                {
                    for(int i = 0; i < T_Pred_Structure_2D_collection[1].Count; i++)
                    {
                        if(T_Pred_Structure_2D_collection[2][i] == "45" && T_Pred_Structure_2D_collection[4][i] == DataType)
                        {
                            featureName_need_now.Add(T_Pred_Structure_2D_collection[1][i]);
                        }
                    }
                    break;
                }
                case 1:
                {
                    for(int i = 0; i < T_Pred_Structure_2D_collection[1].Count; i++)
                    {
                        if(T_Pred_Structure_2D_collection[2][i] == "46" && T_Pred_Structure_2D_collection[4][i] == DataType)
                        {
                            featureName_need_now.Add(T_Pred_Structure_2D_collection[1][i]);
                        }
                    }
                    break;
                }
                case 2:
                {
                    for(int i = 0; i < T_Pred_Structure_2D_collection[1].Count; i++)
                    {
                        if(T_Pred_Structure_2D_collection[2][i] == "19" && T_Pred_Structure_2D_collection[4][i] == DataType)
                        {
                            featureName_need_now.Add(T_Pred_Structure_2D_collection[1][i]);
                        }
                    }
                    break;
                }
            }
        }

        public static void Name_of_Columns_Get(string table_name,
                                              ref List<string> name_of_columns_collection)
        {
            // 该函数用来获取 待查询表 中列的名字

            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = Prediction";
            SqlConnection con = new SqlConnection(sql_link);
            // 打开连接
            con.Open();
            // 查询 T_Pred_Structure 表中有多少列
            string sql_inquire = "Select Name FROM SysColumns Where id = Object_Id('" + table_name + "') order by colid asc";
            SqlCommand sql_cmd = new SqlCommand(sql_inquire, con);
            SqlDataReader sql_dr = sql_cmd.ExecuteReader();
            while(sql_dr.Read())
            {
                name_of_columns_collection.Add(Convert.ToString(sql_dr["Name"]));
            }
            // 在创建下一个 SQLDataReader 前，要关闭上一个对象
            sql_dr.Close();
            // 关闭数据库连接
            con.Close();
        }

        public static int Number_of_Columns_Get(string table_name)
        {
            // 该函数用来获取 待查询表 中列的个数

            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = Prediction";
            SqlConnection con = new SqlConnection(sql_link);
            // 打开连接
            con.Open();
            // 查询 T_Pred_Structure 表中有多少列
            string sql_inquire = "select count(*) from syscolumns s  where s.id = object_id('" + table_name + "')";
            SqlCommand sql_cmd = new SqlCommand(sql_inquire, con);
            int number_of_columns = Convert.ToInt32(sql_cmd.ExecuteScalar());
            // 关闭数据库连接
            con.Close();
            return number_of_columns;
        }

        public static void Period_and_WorkOrderID_Get(string BatchId,
                                                      ref List<List<string>> stage_whole_time_2D_collection,
                                                      ref List<List<string>> WorkOrderID_whole_stage_2D_collection)
        {
            // 该函数用来根据 BatchID 获取 4个阶段时间 和 工单号

            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = CPINFO";
            SqlConnection con = new SqlConnection(sql_link);

            // 打开连接并判断连接状态
            con.Open();

            // 定义查询数据库的字符串
            // 这句SQL语句查询的是一个 BatchId 对应的四个阶段 的始末时间
            string sql_inquire = "SELECT * FROM [CPINFO].[dbo].[V_I_WorkOrder]"+
                                    "where outputMaterialName like '黄山（红方印细支）%'"+
                                    "and FactStartTime between cast('2021-01-01' as datetime) and cast('2022-01-01' as datetime)"+
                                    // "and BatchId = 2021012704"+// 这一条是双B段测试 注：此双B段时间还存在时间重叠
                                    // "and BatchId = 2021010208"+// 这一条是双C段测试
                                    // "and BatchId = 2021080102"+// 这一条是三B段测试
                                    // "and BatchId = 2021090803"+// 这一条是零D段测试
                                    // "and BatchId = 2021122107"+// 这一条是单A段测试
                                    "and BatchId = " + BatchId +
                                    "order by FactStartTime";
            // Console.WriteLine(sql_inquire);// 这句用来查看SQL命令
            // 新建连接及查询命令
            SqlCommand sql_cmd = new SqlCommand(sql_inquire, con);
            SqlDataReader sql_dr = sql_cmd.ExecuteReader();
            // 通过定义 当前UnitName 及 先前UnitName 实现将重复的 UnitName 提取出来
            string current_UnitName, previous_UnitName = "";
            // 定义 阶段编号 及 四个阶段 的时间集合
            int stage_number = 0;
            
            // 接下来的while循环所做的事情为：
            // 在数据已经根据 FactStartTime 排序完毕的情况下，
            // 通过对比 UnitName 字段是否相同来判断是否出现了阶段变化
            // 经此处理，即便一个阶段有多个时间段也可以存入相同的阶段集合
            // 该段程序的逻辑基础为：一套烟草处理流程必然会按顺序经历：
            // 叶片B进预混柜B → 预混柜B出料进储叶柜 → 制丝掺配CA → 加香A
            // 上一个阶段未执行完，没理由执行下一个阶段。这是按 FactStartTime 进行排列的根据。
            // 也因此，如果 UnitName 不变，代表当前阶段未执行完，而数据库中保存的时间段必然是 时间始末对 ，
            // 因此在 当前阶段 的集合中，成双往后Add就行，读取的时候也是成双读取。
            while(sql_dr.Read())
            {
                current_UnitName = Convert.ToString(sql_dr["UnitName"]);
                
                if(previous_UnitName != current_UnitName)
                {
                    stage_number += 1;
                    previous_UnitName = current_UnitName;
                }
                string FactStartTime = Convert.ToString(sql_dr["FactStartTime"]);
                string FactEndTime = Convert.ToString(sql_dr["FactEndTime"]);
                string WorkOrderID = Convert.ToString(sql_dr["WorkOrderID"]);
                switch(stage_number)
                {
                    case 1:
                        Stage_Time_and_WorkOrderID_Get(stage_number,
                                                        ref stage_whole_time_2D_collection, FactStartTime, FactEndTime,
                                                        ref WorkOrderID_whole_stage_2D_collection, WorkOrderID);
                        break;
                    case 2:
                        Stage_Time_and_WorkOrderID_Get(stage_number,
                                                        ref stage_whole_time_2D_collection, FactStartTime, FactEndTime,
                                                        ref WorkOrderID_whole_stage_2D_collection, WorkOrderID);
                        break;
                    case 3:
                        Stage_Time_and_WorkOrderID_Get(stage_number,
                                                        ref stage_whole_time_2D_collection, FactStartTime, FactEndTime,
                                                        ref WorkOrderID_whole_stage_2D_collection, WorkOrderID);
                        break;
                    case 4:
                        Stage_Time_and_WorkOrderID_Get(stage_number,
                                                        ref stage_whole_time_2D_collection, FactStartTime, FactEndTime,
                                                        ref WorkOrderID_whole_stage_2D_collection, WorkOrderID);
                        break;
                }
            }
            // 在创建下一个 SQLDataReader 前，要关闭上一个对象
            sql_dr.Close();
            // 关闭数据库连接
            con.Close();
        }

        public static void Quantity_Get(List<List<string>> T_Pred_Structure_2D_collection,
                                        ref int[] stage_data_count,
                                        ref int[] workshop_data_count)
        {
            for(int i = 0; i < T_Pred_Structure_2D_collection[2].Count; i++)
            {
                if(T_Pred_Structure_2D_collection[2][i] == "45")
                {
                    if(T_Pred_Structure_2D_collection[4][i] == "0")
                    {
                        stage_data_count[0] += 1;
                    }
                    else if(T_Pred_Structure_2D_collection[4][i] == "1")
                    {
                        workshop_data_count[0] += 1;
                    }
                }
                else if(T_Pred_Structure_2D_collection[2][i] == "46")
                {
                    if(T_Pred_Structure_2D_collection[4][i] == "0")
                    {
                        stage_data_count[1] += 1;
                    }
                    else if(T_Pred_Structure_2D_collection[4][i] == "1")
                    {
                        workshop_data_count[1] += 1;
                    }
                }
                else if(T_Pred_Structure_2D_collection[2][i] == "19")
                {
                    if(T_Pred_Structure_2D_collection[4][i] == "0")
                    {
                        stage_data_count[2] += 1;
                    }
                    else if(T_Pred_Structure_2D_collection[4][i] == "1")
                    {
                        workshop_data_count[2] += 1;
                    }
                }
            }
        }

        public static void Splicing_Scheme_1(string WorkOrderID,
                                             string BatchID,
                                             string X_0,
                                             string FeatureID,
                                             string SpecificFlag,
                                             string Value)
        {
            DateTime RecordTime = Convert.ToDateTime(X_0);
            string InputID = RecordTime.ToString("yyyyMMddHHmmssffff") + //16位
                                BatchID + //10位
                                FeatureID + //3位
                                SpecificFlag.PadLeft(4,'0'); //4位
            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = Prediction";
            SqlConnection con = new SqlConnection(sql_link);
            // 打开连接并判断连接状态
            con.Open();
            // 定义添加数据的字符串
            string sql_insert = "insert into [Prediction].[dbo].[T_Pred_Data]("+
                                "InputID,"+
                                "WorkOrderID,"+
                                "BatchID,"+
                                "RecordTime,"+
                                "FeatureID,"+
                                "SpecificFlag,"+
                                "Value"+

                                ")values("+

                                "'" + InputID +"'"+
                                ","+
                                "'" + WorkOrderID +"'"+
                                ","+
                                BatchID+","+
                                "'" + RecordTime +"'"+
                                ","+
                                "'" + FeatureID +"'"+
                                ","+
                                SpecificFlag+","+
                                Value+
                                ")";
            // Console.WriteLine(sql_insert);// 这句用来查看SQL命令
            SqlCommand sql_insert_cmd = new SqlCommand(sql_insert, con);
            sql_insert_cmd.ExecuteNonQuery();

            // 关闭数据库连接
            con.Close();
        }

        public static void Splicing_Scheme_for_Miss_Zhang(List<string> non_environment_FeatureID_collection,
                                                          List<string> environment_FeatureID_collection,
                                                          List<List<List<string>>> all_stage_data_3D_collection,
                                                          List<List<List<string>>> all_workshop_data_3D_collection,
                                                          int which_num_insert)
        {
            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = Prediction";
            SqlConnection con = new SqlConnection(sql_link);
            // 打开连接并判断连接状态
            con.Open();
            // 定义添加数据的字符串
            string sql_insert = "insert into [Prediction].[dbo].[T_Pred_Test](";
            for(int i = 0; i < non_environment_FeatureID_collection.Count; i++)
            {
                sql_insert += non_environment_FeatureID_collection[i];
                sql_insert += ",";
            }
            for(int i = 0; i < environment_FeatureID_collection.Count; i++)
            {
                sql_insert += environment_FeatureID_collection[i];
                if(i != environment_FeatureID_collection.Count -1)
                {
                    sql_insert += ",";
                }
            }
            sql_insert += ")values(";
            // 非环境数据插入
            for(int which_stage = 0; which_stage < all_stage_data_3D_collection.Count; which_stage ++)
            {
                for(int which_column = 0; which_column < all_stage_data_3D_collection[which_stage].Count; which_column ++)
                {
                    sql_insert += all_stage_data_3D_collection[which_stage][which_column][which_num_insert];
                    sql_insert += ",";
                }
            }
            // 环境数据插入
            for(int which_stage = 0; which_stage < all_workshop_data_3D_collection.Count; which_stage ++)
            {
                for(int which_column = 0; which_column < all_workshop_data_3D_collection[which_stage].Count; which_column ++)
                {
                    sql_insert += all_workshop_data_3D_collection[which_stage][which_column][which_num_insert];
                    sql_insert += ",";
                }
            }
            sql_insert = sql_insert.Remove(sql_insert.LastIndexOf(","), 1);
            sql_insert += ")";
            // Console.WriteLine(sql_insert);// 这句用来查看SQL命令
            SqlCommand sql_insert_cmd = new SqlCommand(sql_insert, con);
            sql_insert_cmd.ExecuteNonQuery();

            // 关闭数据库连接
            con.Close();
        }

        public static int Stage_All_Length(string BatchId)
        {
            // 该函数用来查询阶段一个 BatchId 对应多少条数据

            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = CPINFO";
            SqlConnection con = new SqlConnection(sql_link);
            con.Open();
            // 由于12月份有个 BatchId 只有1个阶段，故加一个判断，保证越过阶段数小于3的数据
            string quantity_inquiry = "SELECT COUNT(*) FROM [CPINFO].[dbo].[V_I_WorkOrder]"+
                                        "where BatchId = "+
                                        // "2021122107";// 这个 BatchId 只有一个阶段，会引发错误
                                        BatchId;
            SqlCommand sql_cmd = new SqlCommand(quantity_inquiry, con);
            int total_number_of_stages = Convert.ToInt32(sql_cmd.ExecuteScalar());
            // 关闭数据库连接
            con.Close();
            return total_number_of_stages;
        }

        public static void Stage_Time_and_WorkOrderID_Get(int stage_number,
                                                          ref List<List<string>> collection_whatever, string FactStartTime, string FactEndTime,
                                                          ref List<List<string>> WorkOrderID_whatever, string WorkOrderID)
        {
            // 该函数用来筛选间隔大于 30min 的始末时间，并将符合条件的时间段及工单号加进相应的集合

            DateTime start_time = Convert.ToDateTime(FactStartTime);
            DateTime end_time = Convert.ToDateTime(FactEndTime);
            TimeSpan interval = end_time.Subtract(start_time);
            if(interval.TotalMinutes > 30)
            {
                collection_whatever[stage_number -1].Add(FactStartTime);
                collection_whatever[stage_number -1].Add(FactEndTime);
                WorkOrderID_whatever[stage_number -1].Add(WorkOrderID);
            }
        }

        public static void Stage_Whatever_Data_Get(int which_stage, int sql_month, 
                                                   List<List<string>> stage_whole_time_2D_collection,
                                                   List<string> featureName_need_now,
                                                   ref List<List<string>> stage_time_point_2D_collection,
                                                   ref List<List<string>> WorkOrderID_Amplify_2D_collection,
                                                   List<List<string>> WorkOrderID_whole_stage_2D_collection,
                                                   ref List<List<List<string>>> all_stage_data_3D_collection)
        {
            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = 芜湖数据_2021";
            SqlConnection con = new SqlConnection(sql_link);
            // 打开连接并判断连接状态
            con.Open();
            // 提取该阶段的数据
            for(int time_count = 0; time_count < stage_whole_time_2D_collection[which_stage].Count; time_count +=2)
            {
                // 定义第四步查询数据库的字符串
                string sql_inquire = "SELECT * FROM [芜湖数据_2021].[dbo].["+
                                        // "202102"+
                                        Convert.ToString(sql_month)+
                                        "]"+
                                        "where 时间 between cast('"+
                                        stage_whole_time_2D_collection[which_stage][time_count]+
                                        "' as datetime) and cast('"+
                                        stage_whole_time_2D_collection[which_stage][time_count +1]+
                                        "' as datetime)"+
                                        "order by 时间";
                // Console.WriteLine(sql_inquire);// 这句用来查看SQL命令
                // 新建连接及查询命令
                SqlCommand sql_cmd = new SqlCommand(sql_inquire, con);
                SqlDataReader sql_dr = sql_cmd.ExecuteReader();

                int featureName_count = all_stage_data_3D_collection[which_stage].Count;

                while(sql_dr.Read())
                {
                    List<string> specific_values_collection = new List<string>();
                    for(int i = 0; i < featureName_count; i++)
                    {
                        specific_values_collection.Add(Convert.ToString(sql_dr[featureName_need_now[i]]));
                    }

                    if(specific_values_collection.Contains("NULL") == false &&
                       specific_values_collection.Exists(ele_value => Convert.ToDouble(ele_value) < 1) == false)
                    {
                        stage_time_point_2D_collection[which_stage].Add(Convert.ToString(sql_dr["时间"]));
                        WorkOrderID_Amplify_2D_collection[which_stage].Add(WorkOrderID_whole_stage_2D_collection[which_stage][time_count / 2]);
                        for(int i = 0; i < featureName_count; i++)
                        {
                            all_stage_data_3D_collection[which_stage][i].Add(specific_values_collection[i]);
                        }
                    }
                }
                // 在创建下一个 SQLDataReader 前，要关闭上一个对象
                sql_dr.Close();
            }
            // 关闭数据库连接
            con.Close();
        }

        public static void T_Pred_Structure_Get(ref List<List<string>> T_Pred_Structure_2D_collection)
        {
            // 该函数用来获取 T_Pred_Structure 表中的所有列，及其列中的值

            // int number_of_columns = Number_of_Columns_Get("T_Pred_Structure");// 获取 目标表 中 列的个数

            // 获取 目标表 中 各列的名字
            List<string> name_of_columns_collection = new List<string>();
            Name_of_Columns_Get("T_Pred_Structure",
                                ref name_of_columns_collection);
            int number_of_columns = name_of_columns_collection.Count;
            // 根据 列的个数 向 T_Pred_Structure_2D_collection 中添加小集合
            Create_a_2D_Collection(number_of_columns, ref T_Pred_Structure_2D_collection);
            
            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = Prediction";
            SqlConnection con = new SqlConnection(sql_link);
            // 打开连接
            con.Open();
            // 查询 T_Pred_Structure 表
            string sql_inquire = "SELECT * FROM [Prediction].[dbo].[T_Pred_Structure] order by FeatureID";
            SqlCommand sql_cmd = new SqlCommand(sql_inquire, con);
            SqlDataReader sql_dr = sql_cmd.ExecuteReader();
            while(sql_dr.Read())
            {
                for(int i = 0; i < number_of_columns; i++)
                {
                    T_Pred_Structure_2D_collection[i].Add(Convert.ToString(sql_dr[name_of_columns_collection[i]]));
                }
            }
            // 在创建下一个 SQLDataReader 前，要关闭上一个对象
            sql_dr.Close();
            // 关闭数据库连接
            con.Close();
        }

        public static void Truncate_Table(string table_name)
        {
            // truncate table [Prediction].[dbo].[T_Pred_Data]
            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = Prediction";
            SqlConnection con = new SqlConnection(sql_link);
            // 打开连接并判断连接状态
            con.Open();
            string sql_truncate = "truncate table [Prediction].[dbo]." + table_name;
            SqlCommand sql_truncate_cmd = new SqlCommand(sql_truncate, con);
            sql_truncate_cmd.ExecuteNonQuery();
            // 关闭数据库连接
            con.Close();
            Console.WriteLine(table_name + " 已清空");
        }

        public static void Workshop_Data_Get(int which_stage,
                                             List<List<string>> stage_time_point_2D_collection,
                                             List<double> data_increment_collection,
                                             List<string> featureName_need_now,
                                             ref List<List<string>> workshop_time_2D_collection,
                                             ref List<List<List<string>>> all_workshop_data_3D_collection)
        {
            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = 芜湖数据_2021";
            SqlConnection con = new SqlConnection(sql_link);
            // 打开连接并判断连接状态
            con.Open();
            for(double i = 0; i < stage_time_point_2D_collection[which_stage].Count; i += data_increment_collection[which_stage])
            {
                // int index_a = Convert.ToInt32(a);
                int index_i = Convert.ToInt32(Math.Floor(i));
                DateTime production_time = Convert.ToDateTime(stage_time_point_2D_collection[which_stage][index_i]);
                // 定义第四步查询数据库的字符串
                string Temperature_and_humidity_inquire = "SELECT * FROM [芜湖数据_2021].[dbo].[20210101-20220131制丝车间温湿度导出] " +
                                                            "where 时间 >='"+
                                                            production_time.AddMinutes(-10)+
                                                            "' and 时间 <'"+
                                                            production_time +
                                                            "'"+
                                                            "order by 时间";
                // Console.WriteLine(Temperature_and_humidity_inquire);// 这句用来查看SQL命令
                // 新建连接及查询命令
                SqlCommand sql_cmd = new SqlCommand(Temperature_and_humidity_inquire, con);
                SqlDataReader sql_dr = sql_cmd.ExecuteReader();

                int featureName_count = all_workshop_data_3D_collection[which_stage].Count;

                while(sql_dr.Read())
                {
                    workshop_time_2D_collection[which_stage].Add(Convert.ToString(sql_dr["时间"]));
                    for(int j = 0; j < featureName_count; j++)
                    {
                        all_workshop_data_3D_collection[which_stage][j].Add(Convert.ToString(sql_dr[featureName_need_now[j]]));
                    }                   
                }
                // 在创建下一个 SQLDataReader 前，要关闭上一个对象
                sql_dr.Close();
            }
            // 关闭数据库连接
            con.Close();
        }

        // ------------------------------------------------------------------------------------------------

        // 编程及测试时用到的函数
        public static void Print(object any_value)
        {
            // 该函数用来简化打印流程

            Console.Write(any_value);
            Console.Write(" ");
        }

        public static void Print_Line(object any_value)
        {
            // 该函数用来简化打印流程

            Console.WriteLine(any_value);
        }

        public static void Print_1D(List<string> any_collection)
        {
            // 该函数用来查看编程中获取的一维 string 集合的情况

            for(int i = 0; i < any_collection.Count; i++)
            {
                Console.WriteLine(i + " : " + any_collection[i]);
            }
            Console.WriteLine();
        }

        public static void Print_1D(List<double> any_collection)
        {
            // 该函数用来查看编程中获取的一维 double 集合的情况

            for(int i = 0; i < any_collection.Count; i++)
            {
                Console.WriteLine(i + " : " + any_collection[i]);
            }
            Console.WriteLine();
        }

        public static void Print_1D(List<int> any_collection)
        {
            // 该函数用来查看编程中获取的一维 int 集合的情况

            for(int i = 0; i < any_collection.Count; i++)
            {
                Console.WriteLine(i + " : " + any_collection[i]);
            }
            Console.WriteLine();
        }

        public static void Print_2D(List<List<string>> any_collection)
        {
            // 该函数用来查看编程中获取的二维集合的情况

            for(int i = 0; i < any_collection.Count; i++)
            {
                Print_1D(any_collection[i]);
            }
            Console.WriteLine();
        }

        public static void Print_3D(List<List<List<string>>> any_collection)
        {
            // 该函数用来查看编程中获取的三维集合的情况

            for(int a = 0; a < any_collection.Count; a++)
            {
                for(int b = 0; b < any_collection[a].Count; b++)
                {
                    for(int c = 0; c < any_collection[a][b].Count; c++)
                    {
                        Console.Write(any_collection[a][b][c]);
                        Console.Write(" ");
                    }
                    Console.WriteLine(b + " :以上是1维的");
                }
                Console.WriteLine(a + " :以上是2维的");
            }
            Console.WriteLine("以上是3维的");
        }

        public static void Check_3D(List<List<List<string>>> any_collection)
        {
            // 该函数用来查看编程中获取的三维集合的长度情况

            // 打印第一层数量
            int level_one_length = any_collection.Count;
            Console.WriteLine(level_one_length);

            // 打印第二层数量
            for(int i = 0; i < level_one_length; i++)
            {
                int level_two_length = any_collection[i].Count;
                Console.Write(level_two_length);
                Console.Write(" ");
            }
            Console.WriteLine();

            // 打印第三层数量
            for(int a = 0; a < any_collection.Count; a++)
            {
                for(int b = 0; b < any_collection[a].Count; b++)
                {
                    Console.Write(any_collection[a][b].Count);
                    Console.Write(" ");
                }
            }
            Console.WriteLine();
        }
    }
}
