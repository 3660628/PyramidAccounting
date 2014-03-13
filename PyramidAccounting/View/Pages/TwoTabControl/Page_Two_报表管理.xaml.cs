using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PA.Model.DataGrid;
using System.Data;
using PA.Helper.DataBase;

namespace PA.View.Pages.TwoTabControl
{
    /// <summary>
    /// Interaction logic for Page_Two_报表管理.xaml
    /// </summary>
    public partial class Page_Two_报表管理 : Page
    {
        public Page_Two_报表管理()
        {
            InitializeComponent();
            TextBlock_制表单位.Text = Properties.Settings.Default.Company;   //程序启动后加载当前公司名称
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DataGrid_资产负债表.ItemsSource = null;
            this.DataGrid_资产负债表.ItemsSource = test();
        }
        /// <summary>
        /// 模型 后改数据库查询获取 并填充到页面
        /// </summary>
        /// <returns></returns>
        private List<Model_资产负债表> test()
        {
            List<Model_资产负债表> list = new List<Model_资产负债表>();
            Model_资产负债表 m_0 = new Model_资产负债表();
            m_0.Col_02 = "一、资产类";
            m_0.Col_07 = "二、负债类";
            list.Add(m_0);
            DataTable dt = new DataTable();
            string sql = "SELECT ID,SUBJECT_ID,SUBJECT_NAME FROM T_SUBJECT WHERE USED_MARK <>- 1 AND PARENT_ID = 0 ORDER BY ID";
            dt = new DataBase().Query(sql).Tables[0];
            int count = 0;
            for (int i = 0; i < 20; i++)
            {
                DataRow dr = dt.Rows[i];
                Model_资产负债表 m = new Model_资产负债表();
                if (i == 5 || i == 6)
                {
                    m.Col_00="";
                }
                else
                {
                    count++;
                    m.Col_00 = count.ToString();
                }
                m.Col_01 = dr[1].ToString();
                m.Col_02 = dr[2].ToString();

                DataRow dr2 = dt.Rows[i+20];
                if (i == 11)
                {
                    m.Col_05 = "";
                    m.Col_07 = "三、净资产类";
                    m.Col_08 = "";
                }
                else if(i>11)
                {
                    m.Col_07 = dr2[1].ToString();
                    m.Col_08 = dr2[2].ToString();
                }
                list.Add(m);
            }
            return list;
        }
    }
}
