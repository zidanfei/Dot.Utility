using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;

namespace Dot.DBTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void A_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUser.Text;
            string pwd = txtPwd.Text;
            string serverIp = txtServerIP.Text;
            string db = txtDB.Text;
            string conStr = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};", serverIp, db, user, pwd);
            try
            {
                using (SqlConnection connection = new SqlConnection(conStr))
                {
                    connection.Open();
                    labMsg.Content = "连接成功";
                }
            }
            catch (Exception ex)
            {
                labMsg.Content = ex.Message;
            }
        }


    }
}
