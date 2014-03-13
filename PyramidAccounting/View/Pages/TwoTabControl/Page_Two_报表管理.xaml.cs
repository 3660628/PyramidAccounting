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
        private List<Model_资产负债表> test()
        {
            List<Model_资产负债表> list = new List<Model_资产负债表>();
            Model_资产负债表 m_0 = new Model_资产负债表();
            m_0.Col_02 = "一、资产类";
            m_0.Col_06 = "二、负债类";
            list.Add(m_0);
            Model_资产负债表 m_1 = new Model_资产负债表();
            m_1.Col_01 = "1001";
            m_1.Col_02 = "库存现金";
            m_1.Col_05 = "";
            return list;
        }
    }
}
