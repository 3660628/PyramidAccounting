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

        public Win_记账凭证()
        {
            InitializeComponent();
            InitData();
        }

        #region 自定义事件
        private void ThisFillData(object sender, StringEventArgs e)
        {
            this.Popup_科目子细目.IsOpen = false;
            this.Window_记账凭证.IsEnabled = true;
            if (typeof(PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目).IsInstanceOfType(sender))
            {
                Voucher.凭证明细[CellId].科目编号 = e.Str;
            }
            else if (typeof(PA.View.Pages.Pop.凭证录入.Page_凭证录入_子细目).IsInstanceOfType(sender))
            {
                Voucher.凭证明细[CellId].子细目 = e.Str;
            }
        }
        #endregion

        #region 非事件
        /// <summary>
        /// 初始化数据(空)
        /// </summary>
        private void InitData()
        {
            Voucher.凭证明细 = new List<Model_凭证明细>();
            for (int i = 0; i < 6; i++)
            {
                Model_凭证明细 a = new Model_凭证明细();
                a.ID = i;
                Voucher.凭证明细.Add(a);
            }
            this.DatePicker_Date.SelectedDate = DateTime.Now;
            this.DataGrid_凭证明细.ItemsSource = Voucher.凭证明细;
        }
        /// <summary>
        /// 填充数据(查看修改)
        /// </summary>
        private void FillData()
        {

        }
        /// <summary>
        /// 获取全部数据
        /// </summary>
        /// <returns></returns>
        private Model_凭证单 GetData()
        {
            Voucher.审核标志 = 0;
            Voucher.制表时间 = (DateTime)this.DatePicker_Date.SelectedDate;
            Voucher.字 = this.ComboBox_总收付转.Text;
            Voucher.号 = int.Parse(this.TextBox_号.Text.Trim());
            Voucher.凭证明细 = this.DataGrid_凭证明细.ItemsSource as List<Model_凭证明细>;
            Voucher.附属单证数 = int.Parse(this.TextBox_附属单证.Text.Trim());
            Voucher.合计借方金额 = decimal.Parse(this.Label_借方合计.Content.ToString());
            Voucher.合计贷方金额 = decimal.Parse(this.Label_贷方合计.Content.ToString());
            Voucher.会计主管 = this.Label_会计主管.Content.ToString();
            Voucher.记账 = this.Label_制单人.Content.ToString();
            Voucher.审核 = this.Label_复核.Content.ToString();
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
            GetData();
            Console.WriteLine(Voucher.制表时间);
            Console.WriteLine(Voucher.字);
            Console.WriteLine(Voucher.号);
            //Console.WriteLine(Voucher.制表时间);
        }
        
        private void DataGrid_凭证明细_Cell_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Model_凭证明细 SelectedRow = (Model_凭证明细)(this.DataGrid_凭证明细 as DataGrid).SelectedItems[0];
            DataGridCellInfo DoubleClickCell = this.DataGrid_凭证明细.CurrentCell;
            CellId = SelectedRow.ID;
            if (DoubleClickCell.Column.Header.ToString() == "科目")
            {
                PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目 page = new PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目();
                page.FillDate += new Pages.Pop.凭证录入.Page_凭证录入_科目_FillDateEventHandle(ThisFillData);
                this.Frame_科目子细目.Content = page;
                this.Popup_科目子细目.IsOpen = true;
                this.Window_记账凭证.IsEnabled = false;
            }
            else if (DoubleClickCell.Column.Header.ToString() == "子细目")
            {
                PA.View.Pages.Pop.凭证录入.Page_凭证录入_子细目 page = new PA.View.Pages.Pop.凭证录入.Page_凭证录入_子细目();
                page.FillDate += new Pages.Pop.凭证录入.Page_凭证录入_子细目_FillDateEventHandle(ThisFillData);
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

        private void DataGrid_凭证明细_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {

        }

        private void DataGrid_凭证明细_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

        }

        private void TextBox_附属单证_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                this.TextBox_附属单证.Text = (int.Parse(this.TextBox_附属单证.Text.Trim()) + 1).ToString();
            }
            else if (e.Delta < 0)
            {
                if (this.TextBox_附属单证.Text != "0")
                {
                    this.TextBox_附属单证.Text = (int.Parse(this.TextBox_附属单证.Text.Trim()) - 1).ToString();
                }
            }
        }

        private void TextBox_号_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                this.TextBox_号.Text = (int.Parse(this.TextBox_号.Text.Trim()) + 1).ToString();
            }
            else if (e.Delta < 0)
            {
                if (this.TextBox_号.Text != "0")
                {
                    this.TextBox_号.Text = (int.Parse(this.TextBox_号.Text.Trim()) - 1).ToString();
                }
            }
        }

        #endregion

        

        
    }
}
