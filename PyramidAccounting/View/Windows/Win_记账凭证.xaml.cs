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
using System.Data;
using PA.Model.DataGrid;

namespace PA.View.Windows
{
    /// <summary>
    /// Interaction logic for Win_凭证输入.xaml
    /// </summary>
    public partial class Win_记账凭证 : Window
    {
        Model_凭证单 Voucher = new Model_凭证单();
        public Win_记账凭证()
        {
            InitializeComponent();
            InitData();
        }

        #region 非事件
        private void InitData()
        {
            this.DatePicker_Date.SelectedDate = DateTime.Now;
            //Model_凭证单 InitVoucher = new Model_凭证单();
            Voucher.凭证明细 = new List<Model_凭证明细>();
            for (int i = 0; i < 6; i++)
            {
                Model_凭证明细 a = new Model_凭证明细();
                a.ID = i;
                Voucher.凭证明细.Add(a);
            }
            this.DataGrid_凭证明细.ItemsSource = Voucher.凭证明细;
        }
        private void FillData()
        {

        }
        private Model_凭证单 GetData()
        {
            Model_凭证单 NewVoucher = new Model_凭证单();
            NewVoucher.制表时间 = (DateTime)this.DatePicker_Date.SelectedDate;
            NewVoucher.字 = this.ComboBox_总收付转.Text;
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
            if (!this.Popup_科目子细目.IsOpen)
            {
                this.DragMove();
            }
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
        private Model_凭证明细 SelectedRow;
        private int id;
        private void DataGrid_凭证明细_Cell_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            SelectedRow = (Model_凭证明细)(this.DataGrid_凭证明细 as DataGrid).SelectedItems[0];
            DataGridCellInfo DoubleClickCell = this.DataGrid_凭证明细.CurrentCell;
            id = SelectedRow.ID;
            if (DoubleClickCell.Column.Header.ToString() == "科目")
            {
                this.Popup_科目子细目.IsOpen = true;
                this.Window_记账凭证.IsEnabled = false;
            }
            else if (DoubleClickCell.Column.Header.ToString() == "子细目")
            {
                this.Popup_科目子细目.IsOpen = true;
                this.Window_记账凭证.IsEnabled = false;
            }
        }

        private void Button_PopupClose_Click(object sender, RoutedEventArgs e)
        {
            this.Popup_科目子细目.IsOpen = false;
            this.Window_记账凭证.IsEnabled = true;
            Voucher.凭证明细[id].科目编号 = "asd";
        }

        #endregion
    }
}
