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
using System.Windows.Shapes;
using PA.Model.DataGrid;

namespace PA.View.Windows
{
    /// <summary>
    /// Interaction logic for Win_凭证输入.xaml
    /// </summary>
    public partial class Win_凭证输入 : Window
    {
        public Win_凭证输入()
        {
            InitializeComponent();
            Model_凭证单 m = new Model_凭证单();
            m.凭证明细 = new List<Model_凭证明细>();
            for (int i = 0; i < 6; i++ )
            {
                m.凭证明细.Add(new Model_凭证明细());
            }
            this.DataGrid_凭证明细.ItemsSource = m.凭证明细;
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_凭证输入_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_保存_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_保存并新增_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_打印_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
