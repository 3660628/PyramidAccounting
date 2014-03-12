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
using PA.Model.CustomEventArgs;

namespace PA.View.Windows
{
    /// <summary>
    /// Interaction logic for Win_凭证输入.xaml
    /// </summary>
    public partial class Win_记账凭证 : Window
    {
        Model_凭证单 Voucher = new Model_凭证单();
        private int CellId;
        private string CellHeader;

        public Win_记账凭证()
        {
            InitializeComponent();
            InitData();
        }

        #region 自定义事件
        private void doFillData(object sender, StringEventArgs e)
        {
            this.Popup_科目子细目.IsOpen = false;
            this.Window_记账凭证.IsEnabled = true;
            if (CellHeader == "科目")
            {
                Voucher.凭证明细[CellId].科目编号 = e.Str;
            }
            else if (CellHeader == "子细目")
            {
                Voucher.凭证明细[CellId].子细目 = e.Str;
            }
        }
        #endregion

        #region 非事件
        private void InitData()
        {
            Voucher.凭证明细 = new List<Model_凭证明细>();
            for (int i = 0; i < 6; i++)
            {
                Model_凭证明细 a = new Model_凭证明细();
                a.ID = i;
                Voucher.凭证明细.Add(a);
            }
            FillData();
        }
        private void FillData()
        {
            this.DatePicker_Date.SelectedDate = DateTime.Now;
            this.ComboBox_总收付转.SelectedIndex = 0;
            this.TextBox_号.Text = "0";
            this.DataGrid_凭证明细.ItemsSource = Voucher.凭证明细;
        }
        private Model_凭证单 GetData()
        {
            return Voucher;
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
        
        private void DataGrid_凭证明细_Cell_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Model_凭证明细 SelectedRow = (Model_凭证明细)(this.DataGrid_凭证明细 as DataGrid).SelectedItems[0];
            DataGridCellInfo DoubleClickCell = this.DataGrid_凭证明细.CurrentCell;
            CellId = SelectedRow.ID;
            CellHeader = DoubleClickCell.Column.Header.ToString();
            if (DoubleClickCell.Column.Header.ToString() == "科目")
            {
                PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目 page = new PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目();
                page.FillDate += new Pages.Pop.凭证录入.Page_凭证录入_科目_FillDateEventHandle(doFillData);
                this.Frame_科目子细目.Content = page;
                this.Popup_科目子细目.IsOpen = true;
                this.Window_记账凭证.IsEnabled = false;
            }
            else if (DoubleClickCell.Column.Header.ToString() == "子细目")
            {
                PA.View.Pages.Pop.凭证录入.Page_凭证录入_子细目 page = new PA.View.Pages.Pop.凭证录入.Page_凭证录入_子细目();
                page.FillDate += new Pages.Pop.凭证录入.Page_凭证录入_子细目_FillDateEventHandle(doFillData);
                this.Frame_科目子细目.Content = page;
                this.Popup_科目子细目.IsOpen = true;
                this.Window_记账凭证.IsEnabled = false;
            }
        }

        private void Button_PopupClose_Click(object sender, RoutedEventArgs e)
        {
            this.Popup_科目子细目.IsOpen = false;
            this.Window_记账凭证.IsEnabled = true;
        }

        #endregion
    }
}
