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
using PA.ViewModel;

namespace PA.View.Pages.TwoTabControl
{
    /// <summary>
    /// Interaction logic for Page_Two_系统管理.xaml
    /// </summary>
    public partial class Page_Two_系统管理 : Page
    {
        public Page_Two_系统管理()
        {
            InitializeComponent();
        }

        private void Button_资产_Click(object sender, RoutedEventArgs e)
        {
            this.DataGrid_科目设置.ItemsSource = new ViewModel_科目管理().GetData(1);
        }

        private void Button_负债_Click(object sender, RoutedEventArgs e)
        {
            this.DataGrid_科目设置.ItemsSource = new ViewModel_科目管理().GetData(2);
        }

        private void Button_净资产_Click(object sender, RoutedEventArgs e)
        {
            this.DataGrid_科目设置.ItemsSource = new ViewModel_科目管理().GetData(3);
        }

        private void Button_收入_Click(object sender, RoutedEventArgs e)
        {
            this.DataGrid_科目设置.ItemsSource = new ViewModel_科目管理().GetData(4);
        }

        private void Button_支出_Click(object sender, RoutedEventArgs e)
        {
            this.DataGrid_科目设置.ItemsSource = new ViewModel_科目管理().GetData(5);
        }
    }
}
