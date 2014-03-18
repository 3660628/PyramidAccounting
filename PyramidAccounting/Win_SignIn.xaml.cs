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
using PA.Model.ComboBox;
using PA.Helper.DataBase;
using PA.Helper.DataDefind;

namespace PA
{
    public partial class Win_SignIn : Window
    {
        public Win_SignIn()
        {
            InitializeComponent();
            InitComboBox();
        }
        private void InitComboBox()
        {
            ComboBox_账套.ItemsSource = new ComboBox_Common().GetComboBox_账套();
            ComboBox_账套.DisplayMemberPath = "账套名称";
            ComboBox_账套.SelectedValuePath = "ID";
            ComboBox_账套.Text = new StartUpInit().LoadBookName();
        }
        private void Button_Min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_登陆_Click(object sender, RoutedEventArgs e)
        {
            CommonInfo.账薄号 = ComboBox_账套.SelectedValue.ToString();
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
