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
using PA.ViewModel;

namespace PA.View.Windows
{
    /// <summary>
    /// Interaction logic for Win_子细目.xaml
    /// </summary>
    public partial class Win_子细目 : Window
    {
        private int judge = 0;
        private List<Model_科目管理> lm= new List<Model_科目管理>();
        private ViewModel_科目管理 vm = new ViewModel_科目管理();
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

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            DataGrid_子细目.CanUserAddRows = false;
            vm.Insert(lm);
            lm.Clear();
        }

        private void Button_Del_Click(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "确认删除数据？";
            string caption = "注意";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    break;
                case MessageBoxResult.No:
                    return;
            }
            List<int> list = new List<int>();
            for (int i = 0; i < DataGrid_子细目.SelectedItems.Count; i++)
            {
                Model_科目管理 m = new Model_科目管理();
                try
                {
                    m = DataGrid_子细目.SelectedItems[i] as Model_科目管理;
                }
                catch (Exception)
                {
                    
                }
                list.Add(m.ID);
            }
            vm.Delete(list);
            list.Clear();
        }

        private void DataGrid_子细目_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            Model_科目管理 m = new Model_科目管理();
            m = e.Row.Item as Model_科目管理;
            if (judge == 1)
            {
                m.父ID = TextBox_科目编号.Text.ToString();
                lm.Add(m);
            }
            else
            {
                vm.UpdateChildSubject(m);
            }
        }

    }
}
