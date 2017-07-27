using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dot.TestDB
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            cmbType.SelectedIndex = 0;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            string constr = txtCon.Text;
            if (string.IsNullOrEmpty(constr))
            {

            }


            try
            {
                if (cmbType.Text.Equals("MsSql", StringComparison.OrdinalIgnoreCase))
                {
                    SqlConnection connection = new SqlConnection(constr);
                    connection.Open();
                    //var count= SqlServerHelper.ExecuteScalar(constr, CommandType.Text, sql);
                    MessageBox.Show("连接成功！");
                    return;

                }
                else if (cmbType.Text.Equals("Oracle", StringComparison.OrdinalIgnoreCase))
                {
                    OracleConnection connection = new OracleConnection(constr);
                    connection.Open();

                    //var count = OracleHelper.ExecuteScalar(constr, CommandType.Text, sql);
                    MessageBox.Show("连接成功！");
                    return;

                }
                else if (cmbType.Text.Equals("MySql", StringComparison.OrdinalIgnoreCase))
                {
                    return;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接失败！" + ex.Message);
                return;
            }
            MessageBox.Show("没有可执行项！");
        }
    }
}
