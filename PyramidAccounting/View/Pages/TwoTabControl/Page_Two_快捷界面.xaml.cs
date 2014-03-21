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

namespace PA.View.Pages.TwoTabControl
{

    public partial class Page_Two_快捷界面 : Page
    {
        public Page_Two_快捷界面()
        {
            InitializeComponent();
        }

        private void Button_凭证输入_Click(object sender, RoutedEventArgs e)
        {
            PA.View.Windows.Win_记账凭证 win = new PA.View.Windows.Win_记账凭证();

            win.ShowDialog();
        }

        private void Button_凭证审核_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_凭证过账_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_查询修改_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_本月结账_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_账目查询_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
