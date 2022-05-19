using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// 用于连接数据库
using System.Data.SqlClient;
// 用于使用ArrayList
using System.Collections;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 1000;
            progressBar1.Step = 1;
            // 连接数据库
            string sql_link = "Server = localhost; User ID = sa; Pwd = 2013cj1055; DataBase = CPINFO";
            SqlConnection con = new SqlConnection(sql_link);

            // 打开连接并判断连接状态
            con.Open();
            if (con.State == System.Data.ConnectionState.Open)
            {
                // Console.WriteLine("连接成功");
                richTextBox1.Text = "连接成功";
            }
            if (con.State == System.Data.ConnectionState.Closed)
            {
                // Console.WriteLine("连接失败");
                richTextBox1.Text = "连接失败";
            }

            for(int i = 0; i < 1000; i++)
            {
                richTextBox1.AppendText("\n");
                richTextBox1.AppendText(Convert.ToString(i));
                progressBar1.PerformStep();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.AcceptButton = button1;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            //设置滚动条滚动显示最后一条新数据
            richTextBox1.SelectionStart = int.MaxValue;
            richTextBox1.SelectionLength = 1;

            this.richTextBox1.HideSelection = false;
            richTextBox1.ScrollToCaret();
        }
    }
}
