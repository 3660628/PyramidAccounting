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
    /// Interaction logic for Win_子细目.xaml
    /// </summary>
    public partial class Win_子细目 : Window
    {
        private int judge = 0;
        public Win_子细目()
        {
            InitializeComponent();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Button_Min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            judge = 1;
            DataGrid_子细目.CanUserAddRows = true;
        }
    }
}
