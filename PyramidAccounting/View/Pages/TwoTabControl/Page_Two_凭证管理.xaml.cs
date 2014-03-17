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
    /// <summary>
    /// Interaction logic for Page_Two_凭证管理.xaml
    /// </summary>
    public partial class Page_Two_凭证管理 : Page
    {
        public Page_Two_凭证管理()
        {
            InitializeComponent();
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            PA.View.Windows.Win_记账凭证 win = new PA.View.Windows.Win_记账凭证();
            win.ShowDialog();
        }

    }
}
