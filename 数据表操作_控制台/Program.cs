using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 用于连接数据库
using System.Data.SqlClient;
// 用于使用ArrayList
using System.Collections;

namespace 数据表操作_控制台
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // // 获取 特征ID 特征名 加工单元ID
            // ArrayList FeatureID_set = new ArrayList();
            // ArrayList featureName_set = new ArrayList();
            // ArrayList UnitID_set = new ArrayList();
            // FeatureID_featureName_UnitID_get(ref FeatureID_set,
            //                                     ref featureName_set,
            //                                     ref UnitID_set);

            truncate_table();
            
            // 获取 BatchId 集合
            ArrayList BatchId_set = new ArrayList();
            BatchID_get(ref BatchId_set);

            for(int BatchId_index = 0; BatchId_index < 2; BatchId_index++) //BatchId_set.Count
            {
                if(stage_all_length(Convert.ToString(BatchId_set[BatchId_index])) < 3)
                {
                    continue;
                }

                // 获取 4个阶段时间 工单号
                ArrayList stage_A = new ArrayList();
                ArrayList stage_B = new ArrayList();
                ArrayList stage_C = new ArrayList();
                ArrayList stage_D = new ArrayList();
                ArrayList WorkOrderID_set_A = new ArrayList();
                ArrayList WorkOrderID_set_B = new ArrayList();
                ArrayList WorkOrderID_set_C = new ArrayList();
                ArrayList WorkOrderID_set_D = new ArrayList();
                string BatchID = Convert.ToString(BatchId_set[BatchId_index]);
                period_and_WorkOrderID_get(BatchID, 
                                            ref stage_A, 
                                            ref stage_B, 
                                            ref stage_C, 
                                            ref stage_D, 
                                            ref WorkOrderID_set_A,
                                            ref WorkOrderID_set_B,
                                            ref WorkOrderID_set_C,
                                            ref WorkOrderID_set_D);
                
                // 获取
                // 三个阶段对应的时间列
                ArrayList A_0 = new ArrayList();
                ArrayList B_0 = new ArrayList();
                ArrayList C_0 = new ArrayList();
                // 第一阶段
                ArrayList A_3 = new ArrayList();
                ArrayList A_10 = new ArrayList();
                ArrayList A_11 = new ArrayList();
                ArrayList A_2 = new ArrayList();
                ArrayList A_WorkOrderID = new ArrayList();
                // 第二阶段
                ArrayList B_5 = new ArrayList();
                ArrayList B_7 = new ArrayList();
                ArrayList B_1 = new ArrayList();
                ArrayList B_WorkOrderID = new ArrayList();
                // 第三阶段
                ArrayList C_9 = new ArrayList();
                ArrayList C_6 = new ArrayList();
                ArrayList C_WorkOrderID = new ArrayList();

                // 温湿度
                // 第一阶段
                ArrayList A_workshop_time = new ArrayList();
                ArrayList A_temperature = new ArrayList();
                ArrayList A_humidity = new ArrayList();
                // 第二阶段
                ArrayList B_workshop_time = new ArrayList();
                ArrayList B_temperature = new ArrayList();
                ArrayList B_humidity = new ArrayList();
                // 第三阶段
                ArrayList C_workshop_time = new ArrayList();
                ArrayList C_shredded_temperature = new ArrayList();
                ArrayList C_shredded_humidity = new ArrayList();
                ArrayList C_roasted_temperature = new ArrayList();
                ArrayList C_roasted_humidity = new ArrayList();

                // 定义数据年份
                int data_year = 2021;
                int sql_month = 0;

                Data_Table_Name(ref stage_A, ref data_year, ref sql_month);
                stage_A_data_get(sql_month, ref stage_A,
                                            ref A_0,
                                            ref A_3,
                                            ref A_10,
                                            ref A_11,
                                            ref A_2,
                                            ref A_WorkOrderID,
                                            WorkOrderID_set_A);
                
                Data_Table_Name(ref stage_B, ref data_year, ref sql_month);
                stage_B_data_get(sql_month, ref stage_B,
                                            ref B_0,
                                            ref B_5,
                                            ref B_7,
                                            ref B_1,
                                            ref B_WorkOrderID,
                                            WorkOrderID_set_B);
                
                Data_Table_Name(ref stage_C, ref data_year, ref sql_month);
                stage_C_data_get(sql_month, ref stage_C,
                                            ref C_0,
                                            ref C_9,
                                            ref C_6,
                                            ref C_WorkOrderID,
                                            WorkOrderID_set_C);
                
                // 定义 数据条数最小值 和 数据条数集合
                double Number_of_stage_Min;
                double[] Number_of_stage_list = new double [3];
                // 记录三个阶段的数据条数
                double Number_of_stage_A = A_0.Count;
                double Number_of_stage_B = B_0.Count;
                double Number_of_stage_C = C_0.Count;
                // 将三个阶段的数据条数添加进 数据条数集合
                Number_of_stage_list[0] = Number_of_stage_A;
                Number_of_stage_list[1] = Number_of_stage_B;
                Number_of_stage_list[2] = Number_of_stage_C;
                // 对集合进行排序后，第一个就是最小值
                Array.Sort(Number_of_stage_list);
                Number_of_stage_Min = Number_of_stage_list[0];
                // 计算三个数据增量
                double[] data_increment = new double [3];
                data_increment[0] = Number_of_stage_A / Number_of_stage_Min;
                data_increment[1] = Number_of_stage_B / Number_of_stage_Min;
                data_increment[2] = Number_of_stage_C / Number_of_stage_Min;
                // 定义三个阶段拼接时会用到的double索引 和 取整后的int索引
                double stage_A_index = 0;
                double stage_B_index = 0;
                double stage_C_index = 0;
                int s_A_i = 0;
                int s_B_i = 0;
                int s_C_i = 0;
                
                workshop_A_data_get(A_0, data_increment[0],
                                    ref A_workshop_time,
                                    ref A_temperature,
                                    ref A_humidity);

                workshop_B_data_get(B_0, data_increment[1],
                                    ref B_workshop_time,
                                    ref B_temperature,
                                    ref B_humidity);

                workshop_C_data_get(C_0, data_increment[2],
                                    ref C_workshop_time,
                                    ref C_shredded_temperature,
                                    ref C_shredded_humidity,
                                    ref C_roasted_temperature,
                                    ref C_roasted_humidity);
                
                int SpecificFlag = 0;
                for(int i = 0; i < Number_of_stage_Min; i++)
                {
                    // 根据double索引确定int索引
                    s_A_i = Convert.ToInt32(Math.Floor(stage_A_index));
                    s_B_i = Convert.ToInt32(Math.Floor(stage_B_index));
                    s_C_i = Convert.ToInt32(Math.Floor(stage_C_index));
                    if(A_temperature[i] != "NULL" && A_humidity[i] != "NULL"&&
                        B_temperature[i] != "NULL" && B_humidity[i] != "NULL" &&
                        C_shredded_temperature[i] != "NULL" && C_shredded_humidity[i] != "NULL" && C_roasted_temperature[i] != "NULL" && C_roasted_humidity[i] != "NULL")
                    {
                        SpecificFlag ++;
                        // 阶段A
                        // B线润叶加水流量
                        string FeatureID = "F001";
                        splicing_scheme_1(Convert.ToString(A_WorkOrderID[s_A_i]),
                                            BatchID,
                                            Convert.ToString(A_0[s_A_i]),
                                            FeatureID,
                                            Convert.ToString(SpecificFlag),
                                            Convert.ToString(A_3[s_A_i]));
                        // B线润叶注入蒸汽流量
                        FeatureID = "F002";
                        splicing_scheme_1(Convert.ToString(A_WorkOrderID[s_A_i]),
                                            BatchID,
                                            Convert.ToString(A_0[s_A_i]),
                                            FeatureID,
                                            Convert.ToString(SpecificFlag),
                                            Convert.ToString(A_10[s_A_i]));
                        // B线润叶前温度仪实际值
                        FeatureID = "F003";
                        splicing_scheme_1(Convert.ToString(A_WorkOrderID[s_A_i]),
                                            BatchID,
                                            Convert.ToString(A_0[s_A_i]),
                                            FeatureID,
                                            Convert.ToString(SpecificFlag),
                                            Convert.ToString(A_11[s_A_i]));
                        // B线润叶回风温度
                        FeatureID = "F004";
                        splicing_scheme_1(Convert.ToString(A_WorkOrderID[s_A_i]),
                                            BatchID,
                                            Convert.ToString(A_0[s_A_i]),
                                            FeatureID,
                                            Convert.ToString(SpecificFlag),
                                            Convert.ToString(A_2[s_A_i]));
                        
                        // 阶段B
                        // B线加料入口蒸汽阀门开度
                        FeatureID = "F005";
                        splicing_scheme_1(Convert.ToString(B_WorkOrderID[s_B_i]),
                                            BatchID,
                                            Convert.ToString(B_0[s_B_i]),
                                            FeatureID,
                                            Convert.ToString(SpecificFlag),
                                            Convert.ToString(B_5[s_B_i]));
                        // B线加料加水瞬时流量
                        FeatureID = "F006";
                        splicing_scheme_1(Convert.ToString(B_WorkOrderID[s_B_i]),
                                            BatchID,
                                            Convert.ToString(B_0[s_B_i]),
                                            FeatureID,
                                            Convert.ToString(SpecificFlag),
                                            Convert.ToString(B_7[s_B_i]));
                        // [叶片B加料机出口温度[℃]]]
                        FeatureID = "F007";
                        splicing_scheme_1(Convert.ToString(B_WorkOrderID[s_B_i]),
                                            BatchID,
                                            Convert.ToString(B_0[s_B_i]),
                                            FeatureID,
                                            Convert.ToString(SpecificFlag),
                                            Convert.ToString(B_1[s_B_i]));
                        
                        // 阶段C
                        // 燃烧炉出口工艺气温度实际值
                        FeatureID = "F008";
                        splicing_scheme_1(Convert.ToString(C_WorkOrderID[s_C_i]),
                                            BatchID,
                                            Convert.ToString(C_0[s_C_i]),
                                            FeatureID,
                                            Convert.ToString(SpecificFlag),
                                            Convert.ToString(C_9[s_C_i]));
                        // 风选出口水分仪水分实际值
                        FeatureID = "F009";
                        splicing_scheme_1(Convert.ToString(C_WorkOrderID[s_C_i]),
                                            BatchID,
                                            Convert.ToString(C_0[s_C_i]),
                                            FeatureID,
                                            Convert.ToString(SpecificFlag),
                                            Convert.ToString(C_6[s_C_i]));

                        // 温湿度
                        // 阶段A
                        // [K12车间2#(B线润叶)温度]
                        FeatureID = "F010";
                        splicing_scheme_1(Convert.ToString(A_WorkOrderID[i]),
                                            BatchID,
                                            Convert.ToString(A_workshop_time[i]),
                                            FeatureID,
                                            Convert.ToString(SpecificFlag),
                                            Convert.ToString(A_temperature[i]));
                        // [K12车间2#(B线润叶)湿度]
                        FeatureID = "F011";
                        splicing_scheme_1(Convert.ToString(A_WorkOrderID[i]),
                                            BatchID,
                                            Convert.ToString(A_workshop_time[i]),
                                            FeatureID,
                                            Convert.ToString(SpecificFlag),
                                            Convert.ToString(A_humidity[i]));
                        
                        // 阶段B
                        // [K13车间5#(B线加料)温度]
                        FeatureID = "F012";
                        splicing_scheme_1(Convert.ToString(B_WorkOrderID[i]),
                                            BatchID,
                                            Convert.ToString(B_workshop_time[i]),
                                            FeatureID,
                                            Convert.ToString(SpecificFlag),
                                            Convert.ToString(B_temperature[i]));
                        // [K13车间5#(B线加料)湿度]
                        FeatureID = "F013";
                        splicing_scheme_1(Convert.ToString(B_WorkOrderID[i]),
                                            BatchID,
                                            Convert.ToString(B_workshop_time[i]),
                                            FeatureID,
                                            Convert.ToString(SpecificFlag),
                                            Convert.ToString(B_humidity[i]));

                        // 阶段C
                        // [K11车间4#(C线切丝)温度]
                        FeatureID = "F014";
                        splicing_scheme_1(Convert.ToString(C_WorkOrderID[i]),
                                            BatchID,
                                            Convert.ToString(C_workshop_time[i]),
                                            FeatureID,
                                            Convert.ToString(SpecificFlag),
                                            Convert.ToString(C_shredded_temperature[i]));
                        // [K11车间4#(C线切丝)湿度]
                        FeatureID = "F015";
                        splicing_scheme_1(Convert.ToString(C_WorkOrderID[i]),
                                            BatchID,
                                            Convert.ToString(C_workshop_time[i]),
                                            FeatureID,
                                            Convert.ToString(SpecificFlag),
                                            Convert.ToString(C_shredded_humidity[i]));
                        // [K11车间6#(C线烘丝)湿度]
                        FeatureID = "F016";
                        splicing_scheme_1(Convert.ToString(C_WorkOrderID[i]),
                                            BatchID,
                                            Convert.ToString(C_workshop_time[i]),
                                            FeatureID,
                                            Convert.ToString(SpecificFlag),
                                            Convert.ToString(C_roasted_temperature[i]));
                        // [K11车间6#(C线烘丝)温度]
                        FeatureID = "F017";
                        splicing_scheme_1(Convert.ToString(C_WorkOrderID[i]),
                                            BatchID,
                                            Convert.ToString(C_workshop_time[i]),
                                            FeatureID,
                                            Convert.ToString(SpecificFlag),
                                            Convert.ToString(C_roasted_humidity[i]));
                    }
                    // 变更索引
                    stage_A_index += data_increment[0];
                    stage_B_index += data_increment[1];
                    stage_C_index += data_increment[2];
                }
                Console.WriteLine(BatchID);
                Console.WriteLine(SpecificFlag + " / " + Number_of_stage_Min);
            }
            Console.WriteLine("fin");
            Console.ReadLine();
        }

        public static void print_test(ArrayList any_set)
        {
            // 该函数用来查看编程中获取的集合的情况

            for(int i = 0; i < any_set.Count; i++)
            {
                Console.WriteLine(any_set[i]);
            }
            Console.WriteLine();
        }

        public static void truncate_table()
        {
            // truncate table [Prediction].[dbo].[T_Pred_Data]
            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = Prediction";
            SqlConnection con = new SqlConnection(sql_link);
            // 打开连接并判断连接状态
            con.Open();
            string sql_truncate = "truncate table [Prediction].[dbo].[T_Pred_Data]";
            SqlCommand sql_truncate_cmd = new SqlCommand(sql_truncate, con);
            sql_truncate_cmd.ExecuteNonQuery();

            // 关闭数据库连接
            con.Close();
        }

        public static void BatchID_get(ref ArrayList BatchId_set)
        {
            // 该函数用来获取符合条件的 BatchID

            // 获取本方法方法名
            string function_name = System.Reflection.MethodBase.GetCurrentMethod().Name;
            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = CPINFO";
            SqlConnection con = new SqlConnection(sql_link);

            // 打开连接并判断连接状态
            con.Open();
            if(con.State == System.Data.ConnectionState.Open)
            {
                Console.WriteLine("函数: "+ function_name + " 连接成功");
            }
            if (con.State == System.Data.ConnectionState.Closed)
            {
                Console.WriteLine("函数: "+ function_name + " 连接失败");
            }

            // 拿到符合条件的所有 BatchID
            // 该段程序的逻辑为：使用SQL语句查询出需要的 outputMaterialName 及相应的生产时间范围，并根据 BatchId 进行排序，保证相同的 BatchId 能够相邻
            Console.WriteLine("开始获取符合条件的 BatchId ……");
            // 定义第一步查询数据库的字符串
            string first_inquire = "SELECT * FROM [CPINFO].[dbo].[V_I_WorkOrder]"+
                                    "where outputMaterialName like '黄山（红方印细支）%'"+
                                    "and FactStartTime between cast('2021-01-01' as datetime) and cast('2022-01-01' as datetime)"+
                                    "order by BatchId";
            // 新建连接及查询命令
            SqlCommand sql_cmd_1 = new SqlCommand(first_inquire, con);
            SqlDataReader sql_dr_1 = sql_cmd_1.ExecuteReader();
            
            // 通过定义 当前数据 及 先前数据 实现将重复的 BatchId 提取出来
            string current_data, previous_data = "";
            
            while(sql_dr_1.Read())
            {
                current_data = Convert.ToString(sql_dr_1["BatchId"]);
                if(previous_data != current_data)
                {
                    // Console.WriteLine(current_data);
                    BatchId_set.Add(current_data);
                    previous_data = current_data;
                }
            }
            //Console.WriteLine(BatchId_set.Count);
            // 在创建下一个 SQLDataReader 前，要关闭上一个对象
            sql_dr_1.Close();
            // 关闭数据库连接
            con.Close();
            Console.WriteLine("已获取全部符合条件的 BatchId !");
        }

        public static int stage_all_length(string BatchId)
        {
            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = CPINFO";
            SqlConnection con = new SqlConnection(sql_link);
            con.Open();
            // 由于12月份有个 BatchId 只有1个阶段，故加一个判断，保证越过阶段数小于3的数据
            string quantity_inquiry = "SELECT COUNT(*) FROM [CPINFO].[dbo].[V_I_WorkOrder]"+
                                        "where BatchId = "+
                                        // "2021122107";// 这个 BatchId 只有一个阶段，会引发错误
                                        BatchId;
            SqlCommand total_number_of_stages = new SqlCommand(quantity_inquiry, con);
            int Number_of_data_C = Convert.ToInt32(total_number_of_stages.ExecuteScalar());
            // 关闭数据库连接
            con.Close();
            return Number_of_data_C;
        }

        public static void period_and_WorkOrderID_get(string BatchId,
                                                        ref ArrayList stage_A,
                                                        ref ArrayList stage_B,
                                                        ref ArrayList stage_C,
                                                        ref ArrayList stage_D,
                                                        ref ArrayList WorkOrderID_set_A,
                                                        ref ArrayList WorkOrderID_set_B,
                                                        ref ArrayList WorkOrderID_set_C,
                                                        ref ArrayList WorkOrderID_set_D)
        {
            // 该函数用来根据 BatchID 获取 4个阶段时间 和 工单号

            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = CPINFO";
            SqlConnection con = new SqlConnection(sql_link);

            // 打开连接并判断连接状态
            con.Open();

            // 定义查询数据库的字符串
            // 这句SQL语句查询的是一个 BatchId 对应的四个阶段 的始末时间
            string second_inquire = "SELECT * FROM [CPINFO].[dbo].[V_I_WorkOrder]"+
                                    "where outputMaterialName like '黄山（红方印细支）%'"+
                                    "and FactStartTime between cast('2021-01-01' as datetime) and cast('2022-01-01' as datetime)"+
                                    // "and BatchId = 2021012704"+// 这一条是双B段测试 注：此双B段时间还存在时间重叠
                                    // "and BatchId = 2021010208"+// 这一条是双C段测试
                                    // "and BatchId = 2021080102"+// 这一条是三B段测试
                                    // "and BatchId = 2021090803"+// 这一条是零D段测试
                                    // "and BatchId = 2021122107"+// 这一条是零D段测试
                                    "and BatchId = " + BatchId +
                                    "order by FactStartTime";
            // Console.WriteLine(second_inquire);// 这句用来查看SQL命令
            // 新建连接及查询命令
            SqlCommand sql_cmd_2 = new SqlCommand(second_inquire, con);
            SqlDataReader sql_dr_2 = sql_cmd_2.ExecuteReader();
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
            while(sql_dr_2.Read())
            {
                current_UnitName = Convert.ToString(sql_dr_2["UnitName"]);
                
                if(previous_UnitName != current_UnitName)
                {
                    stage_number += 1;
                    // Console.WriteLine(current_UnitName);
                    previous_UnitName = current_UnitName;
                }
                string FactStartTime = Convert.ToString(sql_dr_2["FactStartTime"]);
                string FactEndTime = Convert.ToString(sql_dr_2["FactEndTime"]);
                string WorkOrderID = Convert.ToString(sql_dr_2["WorkOrderID"]);
                switch(stage_number)
                {
                    case 1:
                        stage_time_and_WorkOrderID_get(ref stage_A, FactStartTime, FactEndTime,
                                                        ref WorkOrderID_set_A, WorkOrderID);
                        break;
                    case 2:
                        stage_time_and_WorkOrderID_get(ref stage_B, FactStartTime, FactEndTime,
                                                        ref WorkOrderID_set_B, WorkOrderID);
                        break;
                    case 3:
                        stage_time_and_WorkOrderID_get(ref stage_C, FactStartTime, FactEndTime,
                                                        ref WorkOrderID_set_C, WorkOrderID);
                        break;
                    case 4:
                        stage_time_and_WorkOrderID_get(ref stage_D, FactStartTime, FactEndTime,
                                                        ref WorkOrderID_set_D, WorkOrderID);
                        break;
                }
            }
            // 在创建下一个 SQLDataReader 前，要关闭上一个对象
            sql_dr_2.Close();
            // 关闭数据库连接
            con.Close();

            // Console.WriteLine("已获取全部符合条件的 阶段时间 !");
        }

        public static void stage_time_and_WorkOrderID_get(ref ArrayList stage_whatever, string FactStartTime, string FactEndTime,
                                                            ref ArrayList WorkOrderID_set, string WorkOrderID)
        {
            // 该函数用来筛选间隔大于 30min 的始末时间，并将符合条件的时间段及工单号加进相应的集合

            DateTime start_time = Convert.ToDateTime(FactStartTime);
            DateTime end_time = Convert.ToDateTime(FactEndTime);
            TimeSpan interval = end_time.Subtract(start_time);
            if(interval.TotalMinutes > 30)
            {
                stage_whatever.Add(FactStartTime);
                stage_whatever.Add(FactEndTime);
                WorkOrderID_set.Add(WorkOrderID);
            }
        }

        public static void FeatureID_featureName_UnitID_get(ref ArrayList FeatureID_set,
                                                            ref ArrayList featureName_set,
                                                            ref ArrayList UnitID_set)
        {
            // 该函数用来从结构表中拿到 特征ID 特征名 加工单元ID

            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = Prediction";
            SqlConnection con = new SqlConnection(sql_link);
            // 打开连接并判断连接状态
            con.Open();
            
            string sql_inquire = "SELECT * FROM [Prediction].[dbo].[T_Pred_Structure]";
            // 新建连接及查询命令
            SqlCommand sql_cmd = new SqlCommand(sql_inquire, con);
            SqlDataReader sql_dr = sql_cmd.ExecuteReader();
            while(sql_dr.Read())
            {
                FeatureID_set.Add(sql_dr["FeatureID"]);
                featureName_set.Add(sql_dr["featureName"]);
                UnitID_set.Add(sql_dr["UnitID"]);
            }
            // 在创建下一个 SQLDataReader 前，要关闭上一个对象
            sql_dr.Close();
            // 关闭数据库连接
            con.Close();
        }

        public static void Data_Table_Name(ref ArrayList stage_whatever, ref int data_year, ref int sql_month)
        {
            // 这个 数据表名 方法（函数）所在做的事情就是根据阶段中 索引为0的时间 和 阶段中最后一个时间 判断他们属于哪个月份，然后把月份和年份组合成表名。
            
            // 月份初始化
            sql_month = 0;
            // 阶段所属月份判断
            DateTime time_start = Convert.ToDateTime(stage_whatever[0]);
            DateTime time_end = Convert.ToDateTime(stage_whatever[stage_whatever.Count -1]);
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

        public static void stage_A_data_get(int sql_month, ref ArrayList stage_A,
                                                            ref ArrayList A_0,
                                                            ref ArrayList A_3,
                                                            ref ArrayList A_10,
                                                            ref ArrayList A_11,
                                                            ref ArrayList A_2,
                                                            ref ArrayList A_WorkOrderID,
                                                            ArrayList WorkOrderID_set) 
        {
            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = 芜湖数据_2021";
            SqlConnection con = new SqlConnection(sql_link);
            // 打开连接并判断连接状态
            con.Open();
            // 阶段A数据提取
            for(int time_count = 0; time_count < stage_A.Count; time_count +=2)
            {
                // 定义第四步查询数据库的字符串
                string fourth_inquire = "SELECT * FROM [芜湖数据_2021].[dbo].["+
                                        // "202102"+
                                        Convert.ToString(sql_month)+
                                        "]"+
                                        "where 时间 between cast('"+
                                        stage_A[time_count]+
                                        "' as datetime) and cast('"+
                                        stage_A[time_count +1]+
                                        "' as datetime)"+
                                        "order by 时间";
                // Console.WriteLine(fourth_inquire);// 这句用来查看SQL命令
                // 新建连接及查询命令
                SqlCommand sql_cmd_4 = new SqlCommand(fourth_inquire, con);
                SqlDataReader sql_dr_4 = sql_cmd_4.ExecuteReader();

                while(sql_dr_4.Read())
                {
                    string A_3_s = Convert.ToString(sql_dr_4["叶片B线润叶加水流量"]);
                    string A_10_s = Convert.ToString(sql_dr_4["叶片B线TBL注入蒸汽流量实际值"]);
                    string A_11_s = Convert.ToString(sql_dr_4["B线润叶前温度仪实际值"]);
                    string A_2_s = Convert.ToString(sql_dr_4["叶片B线润叶回风温度"]);

                    if(A_3_s != "NULL" && A_10_s != "NULL" && A_11_s != "NULL" && A_2_s != "NULL")
                    {
                        double A_3_v = Convert.ToDouble(A_3_s);
                        double A_10_v = Convert.ToDouble(A_10_s);
                        double A_11_v = Convert.ToDouble(A_11_s);
                        double A_2_v = Convert.ToDouble(A_2_s);

                        if(A_3_v > 1 && A_10_v > 1 && A_11_v > 1 && A_2_v > 1)
                        {
                            A_0.Add(sql_dr_4["时间"]);
                            A_3.Add(A_3_v);
                            A_10.Add(A_10_v);
                            A_11.Add(A_11_v);
                            A_2.Add(A_2_v);
                            A_WorkOrderID.Add(WorkOrderID_set[time_count / 2]);
                        }
                    }
                }
                // 在创建下一个 SQLDataReader 前，要关闭上一个对象
                sql_dr_4.Close();
            }
            // 关闭数据库连接
            con.Close();
        }

        public static void stage_B_data_get(int sql_month, ref ArrayList stage_B,
                                                            ref ArrayList B_0,
                                                            ref ArrayList B_5,
                                                            ref ArrayList B_7,
                                                            ref ArrayList B_1,
                                                            ref ArrayList B_WorkOrderID,
                                                            ArrayList WorkOrderID_set) 
        {
            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = 芜湖数据_2021";
            SqlConnection con = new SqlConnection(sql_link);
            // 打开连接并判断连接状态
            con.Open();
            // 阶段B数据提取
            for(int time_count = 0; time_count < stage_B.Count; time_count +=2)
            {
                // 定义第四步查询数据库的字符串
                string fourth_inquire = "SELECT * FROM [芜湖数据_2021].[dbo].["+
                                        // "202102"+
                                        Convert.ToString(sql_month)+
                                        "]"+
                                        "where 时间 between cast('"+
                                        stage_B[time_count]+
                                        "' as datetime) and cast('"+
                                        stage_B[time_count +1]+
                                        "' as datetime)"+
                                        "order by 时间";
                // Console.WriteLine(fourth_inquire);// 这句用来查看SQL命令
                // 新建连接及查询命令
                SqlCommand sql_cmd_4 = new SqlCommand(fourth_inquire, con);
                SqlDataReader sql_dr_4 = sql_cmd_4.ExecuteReader();

                while(sql_dr_4.Read())
                {
                    string B_5_s = Convert.ToString(sql_dr_4["B线加料入口蒸汽阀门开度"]);
                    string B_7_s = Convert.ToString(sql_dr_4["B线加水瞬时流量(流量计)"]);
                    string B_1_s = Convert.ToString(sql_dr_4["叶片B加料机出口温度 ℃ "]);

                    if(B_5_s != "NULL" && B_7_s != "NULL" && B_1_s != "NULL")
                    {
                        double B_5_v = Convert.ToDouble(B_5_s);
                        double B_7_v = Convert.ToDouble(B_7_s);
                        double B_1_v = Convert.ToDouble(B_1_s);
                        
                        if(B_5_v > 1 && B_7_v > 1 && B_1_v > 1)
                        {
                            B_0.Add(sql_dr_4["时间"]);
                            B_5.Add(B_5_v);
                            B_7.Add(B_7_v);
                            B_1.Add(B_1_v);
                            B_WorkOrderID.Add(WorkOrderID_set[time_count / 2]);
                        }
                    }
                }
                // 在创建下一个 SQLDataReader 前，要关闭上一个对象
                sql_dr_4.Close();
            }
            // 关闭数据库连接
            con.Close();
        }

        public static void stage_C_data_get(int sql_month, ref ArrayList stage_C,
                                                            ref ArrayList C_0,
                                                            ref ArrayList C_9,
                                                            ref ArrayList C_6,
                                                            ref ArrayList C_WorkOrderID,
                                                            ArrayList WorkOrderID_set) 
        {
            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = 芜湖数据_2021";
            SqlConnection con = new SqlConnection(sql_link);
            // 打开连接并判断连接状态
            con.Open();
            // 阶段C数据提取
            for(int time_count = 0; time_count < stage_C.Count; time_count +=2)
            {
                // 定义第四步查询数据库的字符串
                string fourth_inquire = "SELECT * FROM [芜湖数据_2021].[dbo].["+
                                        // "202102"+
                                        Convert.ToString(sql_month)+
                                        "]"+
                                        "where 时间 between cast('"+
                                        stage_C[time_count]+
                                        "' as datetime) and cast('"+
                                        stage_C[time_count +1]+
                                        "' as datetime)"+
                                        "order by 时间";
                // Console.WriteLine(fourth_inquire);// 这句用来查看SQL命令
                // 新建连接及查询命令
                SqlCommand sql_cmd_4 = new SqlCommand(fourth_inquire, con);
                SqlDataReader sql_dr_4 = sql_cmd_4.ExecuteReader();

                while(sql_dr_4.Read())
                {
                    string C_9_s = Convert.ToString(sql_dr_4["燃烧炉出口工艺气温度实际值"]);
                    string C_6_s = Convert.ToString(sql_dr_4["风选出口水分仪水分实际值"]);

                    if(C_9_s != "NULL" && C_6_s != "NULL")
                    {
                        double C_9_v = Convert.ToDouble(C_9_s);
                        double C_6_v = Convert.ToDouble(C_6_s);

                        if(C_9_v > 1 && C_6_v > 1)
                        {
                            C_0.Add(sql_dr_4["时间"]);
                            C_9.Add(C_9_v);
                            C_6.Add(C_6_v);
                            C_WorkOrderID.Add(WorkOrderID_set[time_count / 2]);
                        }
                    }
                }
                // 在创建下一个 SQLDataReader 前，要关闭上一个对象
                sql_dr_4.Close();
            }
            // 关闭数据库连接
            con.Close();
        }

        public static void workshop_A_data_get(ArrayList A_0, double data_increment,
                                                ref ArrayList A_workshop_time,
                                                ref ArrayList A_temperature,
                                                ref ArrayList A_humidity)
        {
            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = 芜湖数据_2021";
            SqlConnection con = new SqlConnection(sql_link);
            // 打开连接并判断连接状态
            con.Open();
            for(double a = 0; a < A_0.Count; a += data_increment)
            {
                // int index_a = Convert.ToInt32(a);
                int index_a = Convert.ToInt32(Math.Floor(a));
                DateTime production_time = Convert.ToDateTime(A_0[index_a]);
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
                SqlCommand sql_cmd_5 = new SqlCommand(Temperature_and_humidity_inquire, con);
                SqlDataReader sql_dr_5 = sql_cmd_5.ExecuteReader();
                while(sql_dr_5.Read())
                {
                    A_workshop_time.Add(sql_dr_5["时间"]);
                    A_temperature.Add(Convert.ToString(sql_dr_5["K12车间2#(B线润叶)温度"]));
                    A_humidity.Add(Convert.ToString(sql_dr_5["K12车间2#(B线润叶)湿度"]));
                }
                // 在创建下一个 SQLDataReader 前，要关闭上一个对象
                sql_dr_5.Close();
            }
            // 关闭数据库连接
            con.Close();
        }

        public static void workshop_B_data_get(ArrayList B_0, double data_increment,
                                                ref ArrayList B_workshop_time,
                                                ref ArrayList B_temperature,
                                                ref ArrayList B_humidity)
        {
            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = 芜湖数据_2021";
            SqlConnection con = new SqlConnection(sql_link);
            // 打开连接并判断连接状态
            con.Open();
            for(double a = 0; a < B_0.Count; a += data_increment)
            {
                // int index_a = Convert.ToInt32(a);
                int index_a = Convert.ToInt32(Math.Floor(a));
                DateTime production_time = Convert.ToDateTime(B_0[index_a]);
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
                SqlCommand sql_cmd_5 = new SqlCommand(Temperature_and_humidity_inquire, con);
                SqlDataReader sql_dr_5 = sql_cmd_5.ExecuteReader();
                while(sql_dr_5.Read())
                {
                    B_workshop_time.Add(sql_dr_5["时间"]);
                    B_temperature.Add(Convert.ToString(sql_dr_5["K13车间5#(B线加料)温度"]));
                    B_humidity.Add(Convert.ToString(sql_dr_5["K13车间5#(B线加料)湿度"]));
                }
                // 在创建下一个 SQLDataReader 前，要关闭上一个对象
                sql_dr_5.Close();
            }
            // 关闭数据库连接
            con.Close();
        }

        public static void workshop_C_data_get(ArrayList C_0, double data_increment,
                                                ref ArrayList C_workshop_time,
                                                ref ArrayList C_shredded_temperature,
                                                ref ArrayList C_shredded_humidity,
                                                ref ArrayList C_roasted_temperature,
                                                ref ArrayList C_roasted_humidity)
        {
            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = 芜湖数据_2021";
            SqlConnection con = new SqlConnection(sql_link);
            // 打开连接并判断连接状态
            con.Open();
            for(double a = 0; a < C_0.Count; a += data_increment)
            {
                // int index_a = Convert.ToInt32(a);
                int index_a = Convert.ToInt32(Math.Floor(a));
                DateTime production_time = Convert.ToDateTime(C_0[index_a]);
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
                SqlCommand sql_cmd_5 = new SqlCommand(Temperature_and_humidity_inquire, con);
                SqlDataReader sql_dr_5 = sql_cmd_5.ExecuteReader();
                while(sql_dr_5.Read())
                {
                    C_workshop_time.Add(sql_dr_5["时间"]);
                    C_shredded_temperature.Add(Convert.ToString(sql_dr_5["K11车间4#(C线切丝)温度"]));
                    C_shredded_humidity.Add(Convert.ToString(sql_dr_5["K11车间4#(C线切丝)湿度"]));
                    C_roasted_temperature.Add(Convert.ToString(sql_dr_5["K11车间6#(C线烘丝)湿度"]));
                    C_roasted_humidity.Add(Convert.ToString(sql_dr_5["K11车间6#(C线烘丝)温度"]));
                }
                // 在创建下一个 SQLDataReader 前，要关闭上一个对象
                sql_dr_5.Close();
            }
            // 关闭数据库连接
            con.Close();
        }

        public static void splicing_scheme_1(string WorkOrderID,
                                             string BatchID,
                                             string X_0,
                                             string FeatureID,
                                             string SpecificFlag,
                                             string Value)
        {
            DateTime RecordTime = Convert.ToDateTime(X_0);
            string InputID = RecordTime.ToString("yyyyMMddHHmmssffff") + 
                                FeatureID + 
                                SpecificFlag.PadLeft(4,'0');
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
    }
}
