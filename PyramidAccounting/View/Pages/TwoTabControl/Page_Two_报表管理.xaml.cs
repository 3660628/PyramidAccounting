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
using PA.Model.ComboBox;

namespace PA.View.Pages.TwoTabControl
{

    /// <summary>
    /// Interaction logic for Page_Two_报表管理.xaml
    /// </summary>
    public partial class Page_Two_报表管理 : Page
    {
        PA.Helper.XMLHelper.XMLReader xr = new Helper.XMLHelper.XMLReader();
        private ComboBox_Common cbc = new ComboBox_Common();

        public Page_Two_报表管理()
        {
            InitializeComponent();
            this.Label_编制单位1.Content += "\t" +xr.ReadXML("公司");   //程序启动后加载当前公司名称

            this.ComboBox_Date.ItemsSource = cbc.GetComboBox_期数(1);
            this.ComboBox_Date.SelectedIndex = CommonInfo.当前期;

            this.ComboBox_Date1.ItemsSource = cbc.GetComboBox_期数(1);
            this.ComboBox_Date1.SelectedIndex = CommonInfo.当前期;

            this.ComboBox_Date2.ItemsSource = cbc.GetComboBox_期数(1);
            this.ComboBox_Date2.SelectedIndex = CommonInfo.当前期;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ComboBox_Date_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_Date.SelectedValue == null)
            {
                return;
            }
            else
            {
                Label lb = FindName("y01") as Label;
                Console.WriteLine(lb.Content);
            }
        }
        
    }
}
