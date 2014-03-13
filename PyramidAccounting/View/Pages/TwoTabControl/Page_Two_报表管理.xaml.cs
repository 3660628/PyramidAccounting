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
            TextBlock_编制单位.Text = Properties.Settings.Default.Company;   //程序启动后加载当前公司名称
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
            string sql = "SELECT SID,SUBJECT_ID,SUBJECT_NAME FROM T_SUBJECT WHERE USED_MARK <>- 1 AND PARENT_ID = 0 ORDER BY ID";
            dt = new DataBase().Query(sql).Tables[0];
            for (int i = 0; i < 40; i++)
            {
                Model_资产负债表 m = new Model_资产负债表();
                DataRow dr;
                DataRow dr2;
                if (i < 19)
                {
                    dr = dt.Rows[i];
                    m.Col_00 = dr[0].ToString();
                    m.Col_01 = dr[1].ToString();
                    m.Col_02 = dr[2].ToString();
                }
                else if(i > 20 && i < 27)
                {
                    dr = dt.Rows[i+29];
                    m.Col_00 = dr[0].ToString();
                    m.Col_01 = dr[1].ToString();
                    m.Col_02 = dr[2].ToString();
                }
                
                if (i < 11)
                {
                    dr2 = dt.Rows[i + 19];
                    m.Col_05 = dr2[0].ToString();
                    m.Col_06 = dr2[1].ToString();
                    m.Col_07 = dr2[2].ToString();
                }
                if (i == 11)
                {
                    m.Col_07 = "三、净资产类";
                }
                else if (i > 11 && i != 19 && i != 21 && i < 27)
                {
                    dr2 = dt.Rows[i + 18];
                    m.Col_05 = dr2[0].ToString();
                    m.Col_06 = dr2[1].ToString();
                    m.Col_07 = dr2[2].ToString();
                }
                else if (i > 29 && i <35)
                {
                    dr2 = dt.Rows[i + 16];
                    m.Col_05 = dr2[0].ToString();
                    m.Col_06 = dr2[1].ToString();
                    m.Col_07 = dr2[2].ToString();
                }
                if (i == 19)
                {
                    m.Col_00 = "";
                    m.Col_01 = "";
                    m.Col_02 = "资产合计";
                }
                if (i == 21)
                {
                    m.Col_00 = "";
                    m.Col_01 = "";
                    m.Col_02 = "五、支出类";
                }
                if (i == 27)
                {
                    m.Col_02 = "支出合计";
                    m.Col_07 = "负债合计";
                }
                if (i == 28)
                {
                    m.Col_07 = "四、收入类";
                }
                if (i == 36)
                {
                    m.Col_07 = "收入合计";
                }
                if (i ==  38)
                {
                    m.Col_02 = "资产部合计";
                    m.Col_07 = "负债部合计";
                }
                list.Add(m);
            }
            return list;
        }
    }
}
