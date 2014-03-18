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
    /// Interaction logic for Win_登录.xaml
    /// </summary>
    public partial class Win_登录 : Window
    {
        public Win_登录()
        {
            InitializeComponent();
        }

        private void Button_Min_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_登陆_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
