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
using PA.Helper.DataDefind;

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
            TextBlock_编制单位.Text = new PA.Helper.XMLHelper.XMLReader().ReadXML("公司");   //程序启动后加载当前公司名称
            this.DataGrid_资产负债表.ItemsSource = new ViewModel.ViewModel_ReportManager().GetAccountDebt();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }
        
    }
}
