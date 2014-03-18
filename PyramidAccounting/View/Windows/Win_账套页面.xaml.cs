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

namespace PA.View.Windows
{
    /// <summary>
    /// Interaction logic for Win_账套页面.xaml
    /// </summary>
    public partial class Win_账套页面 : Window
    {
        public Win_账套页面()
        {
            InitializeComponent();
        }

        private void Button_Min_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Button_创建_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_取消_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
