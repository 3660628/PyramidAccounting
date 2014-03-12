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
            InitData();
        }

        #region 非事件
        private void InitData()
        {
            this.DatePicker_Date.SelectedDate = DateTime.Now;
            Model_凭证单 InitVoucher = new Model_凭证单();
            InitVoucher.凭证明细 = new List<Model_凭证明细>();
            for (int i = 0; i < 6; i++)
            {
                InitVoucher.凭证明细.Add(new Model_凭证明细());
            }
            this.DataGrid_凭证明细.ItemsSource = InitVoucher.凭证明细;
        }
        private void FillData()
        {

        }
        private Model_凭证单 GetData()
        {
            Model_凭证单 NewVoucher = new Model_凭证单();
            NewVoucher.制表时间 = (DateTime)this.DatePicker_Date.SelectedDate;
            NewVoucher.字 = this.ComboBox_总收付转.SelectedValue.ToString();
            NewVoucher.号 = int.Parse(this.TextBox_号.Text);
            NewVoucher.凭证明细 = this.DataGrid_凭证明细.ItemsSource as List<Model_凭证明细>;
            return NewVoucher;
        }
        #endregion

        #region 控件事件
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
            GetData();
            this.Close();
        }

        private void Button_保存并新增_Click(object sender, RoutedEventArgs e)
        {
            GetData();
            InitData();
        }

        private void Button_打印_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_凭证明细_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }


        #endregion
    }
}
