using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 用于连接数据库
using System.Data.SqlClient;
// 用于使用ArrayList
using System.Collections;

namespace ConsoleApp4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = CPINFO";
            SqlConnection con = new SqlConnection(sql_link);

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

            // 【流程第一步】
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
            ArrayList BatchId_set = new ArrayList();
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
            Console.WriteLine("已获取全部符合条件的 BatchId !");

            // 【流程第二步】
            // 根据拿到的 BatchID 找到单次流程的四个阶段
            // 该段程序的逻辑为：
            // 1.对于循环的单次而言，查询 BatchId 对应的3到6条数据，并按时间排列。
            // 此时我们拿到了按顺序的4个阶段的始末时间。
            // 2.接下来由于不确定哪个阶段会涉及多段时间，因此采用四个阶段集合，每个集合通过单双号记录始末时间，集合长度不确定。
            // 此时我们拿到了四个阶段的所有始末时间。

            Console.WriteLine("根据 BatchId 获取4个阶段对应的 时间段 ……");
            // 定义第二步查询数据库的字符串
            string second_inquire;
            // 【注意】：接下来将是一个超长且复杂的for循环，然而实际上你只要看懂一次流程就行
            // 每一次操作都是针对一个 BatchId ，如果你看不懂了，就把 i < BatchId_set.Count 改成 i < 1
            for(int i = 0; i < BatchId_set.Count; i++) //BatchId_set.Count
            {
                // 由于12月份有个 BatchId 只有1个阶段，故加一个判断，保证越过阶段数小于3的数据
                string quantity_inquiry = "SELECT COUNT(*) FROM [CPINFO].[dbo].[V_I_WorkOrder]"+
                                            "where BatchId = "+
                                            // "2021122107";// 这个 BatchId 只有一个阶段，会引发错误
                                            Convert.ToString(BatchId_set[i]);
                SqlCommand total_number_of_stages = new SqlCommand(quantity_inquiry, con);
                int Number_of_data_C = Convert.ToInt32(total_number_of_stages.ExecuteScalar());
                // Console.WriteLine(Number_of_data_C);
                if(Number_of_data_C < 3)
                {
                    continue;
                }

                // 定义查询数据库的字符串
                // 这句SQL语句查询的是一个 BatchId 对应的四个阶段 的始末时间
                second_inquire = "SELECT * FROM [CPINFO].[dbo].[V_I_WorkOrder]"+
                                "where outputMaterialName like '黄山（红方印细支）%'"+
                                "and FactStartTime between cast('2021-01-01' as datetime) and cast('2022-01-01' as datetime)"+
                                // "and BatchId = 2021012704"+// 这一条是双B段测试 注：此双B段时间还存在时间重叠
                                // "and BatchId = 2021010208"+// 这一条是双C段测试
                                // "and BatchId = 2021080102"+// 这一条是三B段测试
                                // "and BatchId = 2021090803"+// 这一条是零D段测试
                                "and BatchId = " + Convert.ToString(BatchId_set[i]) +
                                "order by FactStartTime";
                // Console.WriteLine(second_inquire);// 这句用来查看SQL命令
                // 新建连接及查询命令
                SqlCommand sql_cmd_2 = new SqlCommand(second_inquire, con);
                SqlDataReader sql_dr_2 = sql_cmd_2.ExecuteReader();
                // 通过定义 当前UnitName 及 先前UnitName 实现将重复的 UnitName 提取出来
                string current_UnitName, previous_UnitName = "";
                // 定义 阶段编号 及 四个阶段 的时间集合
                int stage_number = 0;
                ArrayList stage_A = new ArrayList();
                ArrayList stage_B = new ArrayList();
                ArrayList stage_C = new ArrayList();
                ArrayList stage_D = new ArrayList();
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
                    switch(stage_number)
                    {
                        case 1:
                            stage_A.Add(Convert.ToString(sql_dr_2["FactStartTime"]));
                            stage_A.Add(Convert.ToString(sql_dr_2["FactEndTime"]));
                            break;
                        case 2:
                            stage_B.Add(Convert.ToString(sql_dr_2["FactStartTime"]));
                            stage_B.Add(Convert.ToString(sql_dr_2["FactEndTime"]));
                            // stage_B.Add(sql_dr_2["FactStartTime"]);// 被注释的这两行这样写是可以的，但是并不能直接进行日期比较
                            // stage_B.Add(sql_dr_2["FactEndTime"]);// 想要比较还是要转成 DateTime ，所以既然上面全都转string了，就不折腾了，与上面保持一致
                            break;
                        case 3:
                            stage_C.Add(Convert.ToString(sql_dr_2["FactStartTime"]));
                            stage_C.Add(Convert.ToString(sql_dr_2["FactEndTime"]));
                            break;
                        case 4:
                            stage_D.Add(Convert.ToString(sql_dr_2["FactStartTime"]));
                            stage_D.Add(Convert.ToString(sql_dr_2["FactEndTime"]));
                            break;
                    }
                }
                // 在创建下一个 SQLDataReader 前，要关闭上一个对象
                sql_dr_2.Close();

                // 【流程第三步】
                // 将各个存有多段时间的阶段进行时间比对，如果上一个结束时间比下一个开始时间要晚，就合并两段时间。
                // 该段程序的逻辑为：假设有三段始末时间[0,1],[2,3],[4,5]
                // 1和2需要比较，如果1<2即1比2时间早，则原数据不作更改；如果1>2即1比2时间晚，那么正确的拼接应变为[0,3]
                // 这里真正的难点在于，如何在不确定1与2早晚的情况下判断3与4，
                // 通过枚举我们知道，无论1与2的比较结果如何，实际上绝不会影响到3，因此3与4一定可以比较，
                // 所以解决方式就是如果a>b，就把a和b直接删了。
                
                // 合并重合的时间段，保证后续拼接的数据 不会重复 也 不会错位
                Stage_Processing(ref stage_A);
                Stage_Processing(ref stage_B);
                Stage_Processing(ref stage_C);
                Stage_Processing(ref stage_D);

                // 【流程第四步】
                // 此时我们拥有4个阶段的无重叠始末时间集合。接下来要做的就是按照各个阶段中的时间，从准备好的数据集中拿出数据。
                // 各阶段所用 列 的集合定义，命名的含义为：阶段_对应数据库中的第几列。例：A_0 表示这是A阶段，所用的是对应数据表中的第0列。
                
                // 创建 各列 的动态集合，准备存储要拼接的数据
                // 三个阶段对应的时间列
                ArrayList A_0 = new ArrayList();
                ArrayList B_0 = new ArrayList();
                ArrayList C_0 = new ArrayList();
                // 第一阶段
                ArrayList A_3 = new ArrayList();
                ArrayList A_5 = new ArrayList();
                ArrayList A_10 = new ArrayList();
                ArrayList A_11 = new ArrayList();
                ArrayList A_2 = new ArrayList();
                // 第二阶段
                ArrayList B_5 = new ArrayList();
                ArrayList B_10 = new ArrayList();
                ArrayList B_7 = new ArrayList();
                ArrayList B_1 = new ArrayList();
                // 第三阶段
                ArrayList C_9 = new ArrayList();
                ArrayList C_6 = new ArrayList();

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
                
                // ————————————————————————————————————————————————————————————————————
                // 根据 stage_A 中的时间判断月份，决定打开的是哪个表
                Data_Table_Name(ref stage_A, ref data_year, ref sql_month);
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
                        A_0.Add(sql_dr_4["时间"]);
                        A_3.Add(sql_dr_4["叶片B线润叶加水流量"]);
                        // A_5.Add(sql_dr_4["B线加料入口蒸汽阀门开度"]);// 最新标准中不用这列
                        A_10.Add(sql_dr_4["叶片B线TBL注入蒸汽流量实际值"]);
                        A_11.Add(sql_dr_4["B线润叶前温度仪实际值"]);
                        A_2.Add(sql_dr_4["叶片B线润叶回风温度"]);
                    }
                    // 在创建下一个 SQLDataReader 前，要关闭上一个对象
                    sql_dr_4.Close();
                }

                // ————————————————————————————————————————————————————————————————————
                // 根据 stage_B 中的时间判断月份，决定打开的是哪个表
                Data_Table_Name(ref stage_B, ref data_year, ref sql_month);
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
                        B_0.Add(sql_dr_4["时间"]);
                        B_5.Add(sql_dr_4["B线加料入口蒸汽阀门开度"]);
                        // B_10.Add(sql_dr_4["叶片B线TBL注入蒸汽流量实际值"]);// 最新标准中不用这列
                        B_7.Add(sql_dr_4["B线加水瞬时流量(流量计)"]);
                        B_1.Add(sql_dr_4["叶片B加料机出口温度 ℃ "]);
                    }
                    // 在创建下一个 SQLDataReader 前，要关闭上一个对象
                    sql_dr_4.Close();
                }

                // ————————————————————————————————————————————————————————————————————
                // 根据 stage_C 中的时间判断月份，决定打开的是哪个表
                Data_Table_Name(ref stage_C, ref data_year, ref sql_month);
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
                        C_0.Add(sql_dr_4["时间"]);
                        C_9.Add(sql_dr_4["燃烧炉出口工艺气温度实际值"]);
                        C_6.Add(sql_dr_4["风选出口水分仪水分实际值"]);
                    }
                    // 在创建下一个 SQLDataReader 前，要关闭上一个对象
                    sql_dr_4.Close();
                }

                // ————————————————————————————————————————————————————————————————————
                // 阶段D，如果日后有需要，再启用

                // ————————————————————————————————————————————————————————————————————
                // 车间温湿度
                // Temperature_and_humidity_stitching(ref A_0, ref B_0, ref C_0,
                //                                     ref A_workshop_time, ref A_temperature, ref A_humidity,
                //                                     ref B_workshop_time, ref B_temperature, ref B_humidity,
                //                                     ref C_workshop_time, ref C_shredded_temperature, ref C_shredded_humidity,
                //                                                             ref C_roasted_temperature, ref C_roasted_humidity);

                //// 连接数据库
                //string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = 芜湖数据_2021";
                //SqlConnection con = new SqlConnection(sql_link);
                // 温湿度数据段提取：
                // 定义第四步查询数据库的字符串
                string Temperature_and_humidity_inquire = "SELECT * FROM [芜湖数据_2021].[dbo].[20210101-20220131制丝车间温湿度导出] order by 时间";
                // Console.WriteLine(Temperature_and_humidity_inquire);// 这句用来查看SQL命令
                // 新建连接及查询命令
                SqlCommand sql_cmd_5 = new SqlCommand(Temperature_and_humidity_inquire, con);
                SqlDataReader sql_dr_5 = sql_cmd_5.ExecuteReader();
                while(sql_dr_5.Read())
                {
                    DateTime workshop_time = Convert.ToDateTime(sql_dr_5["时间"]);
                    for(int a = 0; a < A_0.Count; a++)
                    {
                        DateTime production_time_A = Convert.ToDateTime(A_0[a]);
                        if(production_time_A >= workshop_time && production_time_A < workshop_time.AddMinutes(10))
                        {
                            A_workshop_time.Add(workshop_time);
                            A_temperature.Add(sql_dr_5["K12车间2#(B线润叶)温度"]);
                            A_humidity.Add(sql_dr_5["K12车间2#(B线润叶)湿度"]);
                        }
                    }

                    for(int b = 0; b < B_0.Count; b++)
                    {
                        DateTime production_time_B = Convert.ToDateTime(B_0[b]);
                        if(production_time_B >= workshop_time && production_time_B < workshop_time.AddMinutes(10))
                        {
                            B_workshop_time.Add(workshop_time);
                            B_temperature.Add(sql_dr_5["K13车间5#(B线加料)温度"]);
                            B_humidity.Add(sql_dr_5["K13车间5#(B线加料)湿度"]);
                        }
                    }

                    for(int c = 0; c < C_0.Count; c++)
                    {
                        DateTime production_time_C = Convert.ToDateTime(C_0[c]);
                        if(production_time_C >= workshop_time && production_time_C < workshop_time.AddMinutes(10))
                        {
                            C_workshop_time.Add(workshop_time);
                            C_shredded_temperature.Add(sql_dr_5["K11车间4#(C线切丝)温度"]);
                            C_shredded_humidity.Add(sql_dr_5["K11车间4#(C线切丝)湿度"]);
                            C_roasted_temperature.Add(sql_dr_5["K11车间6#(C线烘丝)湿度"]);
                            C_roasted_humidity.Add(sql_dr_5["K11车间6#(C线烘丝)温度"]);
                        }
                    }
                }
                // 在创建下一个 SQLDataReader 前，要关闭上一个对象
                sql_dr_5.Close();

                // 看一眼长度
                // Console.WriteLine(A_workshop_time.Count);
                // Console.WriteLine(B_workshop_time.Count);
                // Console.WriteLine(C_workshop_time.Count);
                // ————————————————————————————————————————————————————————————————————

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

                // 开始拼接
                for(int j = 0; j < Number_of_stage_Min; j++)
                {
                    // 根据double索引确定int索引
                    s_A_i = Convert.ToInt32(stage_A_index);
                    s_B_i = Convert.ToInt32(stage_B_index);
                    s_C_i = Convert.ToInt32(stage_C_index);
                    // 定义添加数据的字符串
            	    string sql_insert = "insert into import_test.dbo.[2021_done](" +
                                        "阶段A,"+
                                        "阶段B,"+
                                        "阶段C,"+

                                        // 阶段A
                                        "B线润叶加水流量,"+
                                        // "B线润叶回风温度蒸汽阀开度,"+
                                        "B线润叶注入蒸汽流量,"+
                                        "B线润叶前温度仪实际值,"+
                                        "B线润叶回风温度,"+

                                        // 阶段B
                                        "B线加料入口蒸汽阀门开度,"+
                                        // "叶片B线TBL注入蒸汽流量实际值,"+
                                        "B线加料加水瞬时流量,"+
                                        "[叶片B加料机出口温度[℃]]],"+

                                        // 阶段C
                                        "燃烧炉出口工艺气温度实际值,"+
                                        "风选出口水分仪水分实际值,"+

                                        // 温湿度
                                        "阶段A车间时间,"+
                                        "[K12车间2#(B线润叶)温度],"+
                                        "[K12车间2#(B线润叶)湿度],"+

                                        "阶段B车间时间,"+
                                        "[K13车间5#(B线加料)温度],"+
                                        "[K13车间5#(B线加料)湿度],"+

                                        "阶段C车间时间,"+
                                        "[K11车间4#(C线切丝)温度],"+
                                        "[K11车间4#(C线切丝)湿度],"+
                                        "[K11车间6#(C线烘丝)湿度],"+
                                        "[K11车间6#(C线烘丝)温度]"+

                                        ")values("+
                                        "'" + Convert.ToString(A_0[s_A_i]) +"'"+
                                        ","+
                                        "'" + Convert.ToString(B_0[s_B_i]) +"'"+
                                        ","+
                                        "'" + Convert.ToString(C_0[s_C_i]) +"'"+
                                        ","+

                                        Convert.ToString(A_3[s_A_i])+","+
                                        // Convert.ToString(A_5[s_A_i])+","+
                                        Convert.ToString(A_10[s_A_i])+","+
                                        Convert.ToString(A_11[s_A_i])+","+
                                        Convert.ToString(A_2[s_A_i])+","+
                                        
                                        Convert.ToString(B_5[s_B_i])+","+
                                        // Convert.ToString(B_10[s_B_i])+","+
                                        Convert.ToString(B_7[s_B_i])+","+
                                        Convert.ToString(B_1[s_B_i])+","+

                                        Convert.ToString(C_9[s_C_i])+","+
                                        Convert.ToString(C_6[s_C_i])+","+

                                        // 温湿度
                                        "'" + Convert.ToString(A_workshop_time[s_A_i]) +"'"+
                                        ","+
                                        Convert.ToString(A_temperature[s_A_i])+","+
                                        Convert.ToString(A_humidity[s_A_i])+","+

                                        "'" + Convert.ToString(B_workshop_time[s_B_i]) +"'"+
                                        ","+
                                        Convert.ToString(B_temperature[s_B_i])+","+
                                        Convert.ToString(B_humidity[s_B_i])+","+

                                        "'" + Convert.ToString(C_workshop_time[s_C_i]) +"'"+
                                        ","+
                                        Convert.ToString(C_shredded_temperature[s_C_i])+","+
                                        Convert.ToString(C_shredded_humidity[s_C_i])+","+
                                        Convert.ToString(C_roasted_temperature[s_C_i])+","+
                                        Convert.ToString(C_roasted_humidity[s_C_i])+

                                        ")";
                    // Console.WriteLine(sql_insert);// 这句用来查看SQL命令
                    SqlCommand sql_insert_cmd = new SqlCommand(sql_insert, con);
                    sql_insert_cmd.ExecuteNonQuery();

                    // 变更索引
                    stage_A_index += data_increment[0];
                    stage_B_index += data_increment[1];
                    stage_C_index += data_increment[2];

                    // Console.WriteLine("已插入"+ Convert.ToString(j+1) +"条");
                }
                Console.WriteLine("恭喜! BatchId 为 " + Convert.ToString(BatchId_set[i]) + " 的数据均已 拼接 并 插入完成！");
            }
            con.Close();
            Console.WriteLine();
            Console.WriteLine("恭喜! 全部数据均已 拼接 并 插入完成！");
            Console.ReadLine();
        }
        public static void Stage_Processing(ref ArrayList stage_whatever)
        {
            // 这个 阶段处理 方法（函数）所在做的事情就是把重合的时间段删掉
            // 其逻辑为：首先判断出长度大于2的阶段集合，因为这意味着存在多段时间。
            // 然后使用索引对每一个 时间始末对 的结尾时间和下一个 时间始末对 的开始时间作比较，这一部分通过设定步长+=2来实现，
            // 然后通过设定总长度-1来保证索引不会溢出和循环终结
            // 然后把a>b的时间存入待删除集合，最后统一根据 具体时间值 来删除
            // 【特别注意】：之所以不用索引或者索引范围来执行删除操作，是因为删除一个索引后，其余各项索引值都会立即更新，使得删除过程变得复杂。
            ArrayList The_time_set_to_be_deleted = new ArrayList();
            if (stage_whatever.Count > 2)
            {
                for (int index_in_set = 1; index_in_set < stage_whatever.Count - 1; index_in_set += 2)
                {
                    DateTime time_a = Convert.ToDateTime(stage_whatever[index_in_set]);
                    DateTime time_b = Convert.ToDateTime(stage_whatever[index_in_set + 1]);
                    if (time_a > time_b)
                    {
                        The_time_set_to_be_deleted.Add(stage_whatever[index_in_set]);
                        The_time_set_to_be_deleted.Add(stage_whatever[index_in_set + 1]);
                    }
                }
                for (int index_in_set = 0; index_in_set < The_time_set_to_be_deleted.Count; index_in_set++)
                {
                    stage_whatever.Remove(The_time_set_to_be_deleted[index_in_set]);
                }
            }
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
        
        public static void Temperature_and_humidity_stitching(ref ArrayList A_0, ref ArrayList B_0, ref ArrayList C_0,
                                                            ref ArrayList A_workshop_time, ref ArrayList A_temperature, ref ArrayList A_humidity,
                                                            ref ArrayList B_workshop_time, ref ArrayList B_temperature, ref ArrayList B_humidity,
                                                            ref ArrayList C_workshop_time, ref ArrayList C_shredded_temperature, ref ArrayList C_shredded_humidity,
                                                                                            ref ArrayList C_roasted_temperature, ref ArrayList C_roasted_humidity)
        {
            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = 芜湖数据_2021";
            SqlConnection T_H_con = new SqlConnection(sql_link);
            // 温湿度数据段提取：
            // 定义第四步查询数据库的字符串
            string Temperature_and_humidity_inquire = "SELECT * FROM [芜湖数据_2021].[dbo].[20210101-20220131制丝车间温湿度导出] order by 时间";
            Console.WriteLine(Temperature_and_humidity_inquire);// 这句用来查看SQL命令
            // 新建连接及查询命令
            SqlCommand sql_cmd_5 = new SqlCommand(Temperature_and_humidity_inquire, T_H_con);
            SqlDataReader sql_dr_5 = sql_cmd_5.ExecuteReader();
            while(sql_dr_5.Read())
            {
                DateTime workshop_time = Convert.ToDateTime(sql_dr_5["时间"]);
                for(int a = 0; a < A_0.Count; a++)
                {
                    DateTime production_time_A = Convert.ToDateTime(A_0[a]);
                    if(production_time_A >= workshop_time && production_time_A < workshop_time.AddMinutes(10))
                    {
                        A_workshop_time.Add(workshop_time);
                        A_temperature.Add(sql_dr_5["K12车间2#(B线润叶)温度"]);
                        A_humidity.Add(sql_dr_5["K12车间2#(B线润叶)湿度"]);
                    }
                }

                for(int b = 0; b < B_0.Count; b++)
                {
                    DateTime production_time_B = Convert.ToDateTime(B_0[b]);
                    if(production_time_B >= workshop_time && production_time_B < workshop_time.AddMinutes(10))
                    {
                        B_workshop_time.Add(workshop_time);
                        B_temperature.Add(sql_dr_5["K13车间5#(B线加料)温度"]);
                        B_humidity.Add(sql_dr_5["K13车间5#(B线加料)湿度"]);
                    }
                }

                for(int c = 0; c < C_0.Count; c++)
                {
                    DateTime production_time_C = Convert.ToDateTime(C_0[c]);
                    if(production_time_C >= workshop_time && production_time_C < workshop_time.AddMinutes(10))
                    {
                        C_workshop_time.Add(workshop_time);
                        C_shredded_temperature.Add(sql_dr_5["K11车间4#(C线切丝)温度"]);
                        C_shredded_humidity.Add(sql_dr_5["K11车间4#(C线切丝)湿度"]);
                        C_roasted_temperature.Add(sql_dr_5["K11车间6#(C线烘丝)湿度"]);
                        C_roasted_humidity.Add(sql_dr_5["K11车间6#(C线烘丝)温度"]);
                    }
                }
            }
            // 在创建下一个 SQLDataReader 前，要关闭上一个对象
            sql_dr_5.Close();
        }
    }
}