using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            string sql = txtSql.Text;
            if (string.IsNullOrEmpty(constr))
            {

            }

            if (string.IsNullOrEmpty(sql))
            {

            }
            sql = string.Format( @"select count(*) from ({0}) m", sql);
            try
            {
                if (cmbType.Text.Equals("MsSql", StringComparison.OrdinalIgnoreCase))
                {
                    var count= SqlServerHelper.ExecuteScalar(constr, CommandType.Text, sql);
                    MessageBox.Show("连接成功！数据有 " + count+" 条");
                    return;

                }
                else if (cmbType.Text.Equals("Oracle", StringComparison.OrdinalIgnoreCase))
                {
                    var count = OracleHelper.ExecuteScalar(constr, CommandType.Text, sql);
                    MessageBox.Show("连接成功！数据有 " + count+" 条");
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
