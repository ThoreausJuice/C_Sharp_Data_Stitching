using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 用于连接数据库
using System.Data.SqlClient;
// 用于使用ArrayList
using System.Collections;

namespace 结构表操作_控制台
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // T_Pred_Structure表
            ArrayList FeatureID = new ArrayList();
            ArrayList featureName = new ArrayList();
            ArrayList UnitID = new ArrayList();
            ArrayList UnitName = new ArrayList();
            // 阶段A
            FeatureID.Add("F001");featureName.Add("B线润叶加水流量");UnitID.Add("45");UnitName.Add("叶片B进预混柜B");
            FeatureID.Add("F002");featureName.Add("B线润叶注入蒸汽流量");UnitID.Add("45");UnitName.Add("叶片B进预混柜B");
            FeatureID.Add("F003");featureName.Add("B线润叶前温度仪实际值");UnitID.Add("45");UnitName.Add("叶片B进预混柜B");
            FeatureID.Add("F004");featureName.Add("B线润叶回风温度");UnitID.Add("45");UnitName.Add("叶片B进预混柜B");
            // 阶段B
            FeatureID.Add("F005");featureName.Add("B线加料入口蒸汽阀门开度");UnitID.Add("46");UnitName.Add("预混柜B出料进储叶柜");
            FeatureID.Add("F006");featureName.Add("B线加料加水瞬时流量");UnitID.Add("46");UnitName.Add("预混柜B出料进储叶柜");
            FeatureID.Add("F007");featureName.Add("[叶片B加料机出口温度[℃]]]");UnitID.Add("46");UnitName.Add("预混柜B出料进储叶柜");
            // 阶段C
            FeatureID.Add("F008");featureName.Add("燃烧炉出口工艺气温度实际值"); UnitID.Add("19");UnitName.Add("制丝掺配CA");
            FeatureID.Add("F009");featureName.Add("风选出口水分仪水分实际值");UnitID.Add("19");UnitName.Add("制丝掺配CA");
            // 阶段A 温湿度
            FeatureID.Add("F010");featureName.Add("[K12车间2#(B线润叶)温度]");UnitID.Add("P45");UnitName.Add("A段温度");
            FeatureID.Add("F011");featureName.Add("[K12车间2#(B线润叶)湿度]");UnitID.Add("P45");UnitName.Add("A段湿度");
            // 阶段B 温湿度
            FeatureID.Add("F012");featureName.Add("[K13车间5#(B线加料)温度]");UnitID.Add("P46");UnitName.Add("B段温度");
            FeatureID.Add("F013");featureName.Add("[K13车间5#(B线加料)湿度]");UnitID.Add("P46");UnitName.Add("B段湿度");
            // 阶段C 温湿度
            FeatureID.Add("F014");featureName.Add("[K11车间4#(C线切丝)温度]");UnitID.Add("P19");UnitName.Add("C段温度");
            FeatureID.Add("F015");featureName.Add("[K11车间4#(C线切丝)湿度]");UnitID.Add("P19");UnitName.Add("C段湿度");
            FeatureID.Add("F016");featureName.Add("[K11车间6#(C线烘丝)湿度]");UnitID.Add("P19");UnitName.Add("C段温度");
            FeatureID.Add("F017");featureName.Add("[K11车间6#(C线烘丝)温度]");UnitID.Add("P19");UnitName.Add("C段湿度");

            truncate_data("Prediction", "T_Pred_Structure");
            insert_data(ref FeatureID, ref featureName, ref UnitID, ref UnitName);

            Console.ReadLine();
        }

        public static void insert_data(ref ArrayList FeatureID,
                                        ref ArrayList featureName,
                                        ref ArrayList UnitID,
                                        ref ArrayList UnitName)
        {
            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = Prediction";
            SqlConnection con = new SqlConnection(sql_link);
            // 打开数据库连接并判断连接状态
            con.Open();
            if(con.State == System.Data.ConnectionState.Open)
            {
                Console.WriteLine("insert_data 连接成功");
            }
            if (con.State == System.Data.ConnectionState.Closed)
            {
                Console.WriteLine("insert_data 连接失败");
            }

            for(int i = 0; i < featureName.Count; i++)
            {
                string sql_insert = "insert into Prediction.dbo.T_Pred_Structure("+
                                    "FeatureID,"+
                                    "featureName,"+
                                    "UnitID,"+
                                    "UnitName"+
                                    ")values("+
                                    "'F" + zero_pad(i+1) +"',"+
                                    // "'" + Convert.ToString(FeatureID[i]) +"',"+
                                    "'" + Convert.ToString(featureName[i]) +"',"+
                                    "'" + Convert.ToString(UnitID[i]) +"',"+
                                    "'" + Convert.ToString(UnitName[i]) +"'"+
                                    ")";
                // Console.WriteLine(sql_insert);// 这句用来查看SQL命令
                SqlCommand sql_insert_cmd = new SqlCommand(sql_insert, con);
                sql_insert_cmd.ExecuteNonQuery();
                Console.WriteLine("已插入：" + (i+1) + " 条，共 " + featureName.Count + " 条");
            }

            // 关闭数据库连接
            con.Close();

            Console.WriteLine("插入已完成");
        }

        public static void truncate_data(string DataBase_name, string table_name)
        {
            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = " + DataBase_name;
            SqlConnection con = new SqlConnection(sql_link);
            // 打开数据库连接并判断连接状态
            con.Open();
            if(con.State == System.Data.ConnectionState.Open)
            {
                Console.WriteLine("truncate_data 连接成功");
            }
            if (con.State == System.Data.ConnectionState.Closed)
            {
                Console.WriteLine("truncate_data 连接失败");
            }

            string sql_truncate = "truncate table Prediction.dbo." + table_name;
            // Console.WriteLine(sql_truncate);// 这句用来查看SQL命令
            SqlCommand sql_truncate_cmd = new SqlCommand(sql_truncate, con);
            sql_truncate_cmd.ExecuteNonQuery();

            // 关闭数据库连接
            con.Close();

            Console.WriteLine("数据表 "+ table_name +" 已清空");
        }

        public static string zero_pad(int number)
        {
            if(number>100)
            {
                return number.ToString();
            }
            else
            {
                return number.ToString().PadLeft(3,'0'); //一共4位,位数不够时从左边开始用0补
            }
        }
    }
}
