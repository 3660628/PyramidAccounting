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
    public delegate void Win_记账凭证_Submit(object sender, EventArgs e);

    public partial class Win_记账凭证 : Window
    {
        public event Win_记账凭证_Submit ESubmit;
        Model_凭证单 Voucher = new Model_凭证单();
        List<Model_凭证明细> VoucherDetails = new List<Model_凭证明细>();//所有DataGrid数据集合
        List<Model_凭证明细> VoucherDetailsNow = new List<Model_凭证明细>();//当前DataGrid的数据
        private int CellId;

        private int PageNow = 1;//当前页面
        private int PageAll = 1;//所有页面

        public Win_记账凭证()
        {
            InitializeComponent();
            InitData();
        }

        #region 自定义事件
        private void OnSubmit()
        {
            if(this.ESubmit != null)
            {
                ESubmit(this, new EventArgs());
            }
        }
        private void DoFillData(object sender, StringEventArgs e)
        {
            this.Popup_科目子细目.IsOpen = false;
            this.Window_记账凭证.IsEnabled = true;
            if (typeof(PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目).IsInstanceOfType(sender))
            {
                VoucherDetails[CellId].科目编号 = e.Str;
            }
            else if (typeof(PA.View.Pages.Pop.凭证录入.Page_凭证录入_子细目).IsInstanceOfType(sender))
            {
                VoucherDetails[CellId].子细目 = e.Str;
            }
        }
        #endregion

        #region 非事件
        /// <summary>
        /// 初始化数据(空)
        /// </summary>
        private void InitData()
        {
            VoucherDetails = new List<Model_凭证明细>();
            for (int i = 0; i < 6; i++)
            {
                Model_凭证明细 a = new Model_凭证明细();
                a.序号 = i;
                VoucherDetails.Add(a);
            }
            this.DatePicker_Date.SelectedDate = DateTime.Now;
            this.DataGrid_凭证明细.ItemsSource = VoucherDetails;
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
            Voucher.ID = Guid.NewGuid().ToString();
            Voucher.审核标志 = (this.Label_审核状态.Content.ToString() == "已审核") ? 1 : 0;
            Voucher.制表时间 = (DateTime)this.DatePicker_Date.SelectedDate;
            Voucher.号 = int.Parse(this.TextBox_号.Text.Trim());
            foreach (Model_凭证明细 Detail in VoucherDetails)
            {
                Detail.父节点ID = Voucher.ID;
            }
            Voucher.附属单证数 = int.Parse(this.TextBox_附属单证.Text.Trim());
            Voucher.合计借方金额 = decimal.Parse(this.Label_借方合计.Content.ToString());
            Voucher.合计贷方金额 = decimal.Parse(this.Label_贷方合计.Content.ToString());
            Voucher.会计主管 = this.Label_会计主管.Content.ToString();
            Voucher.制单人 = this.Label_制单人.Content.ToString();
            Voucher.复核 = this.Label_复核.Content.ToString();
            return Voucher;
        }
        private bool CheckData()
        {
            if (this.Label_借方合计.Content.ToString() == this.Label_贷方合计.Content.ToString())
            {
                return true;
            }
            return false;
        }
        private void Count合计()
        {
            decimal count借方 = 0m;
            decimal count贷方 = 0m;
            for (int i = 0; i < 6; i++ )
            {
                count借方 += VoucherDetails[i].借方;
                count贷方 += VoucherDetails[i].贷方;
            }
            this.Label_借方合计.Content = count借方.ToString();
            this.Label_贷方合计.Content = count贷方.ToString();
        }
        #endregion

        #region 控件事件
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            OnSubmit();
            this.Close();
        }

        private void Window_凭证输入_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.Popup_科目子细目.IsOpen && e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Button_保存_Click(object sender, RoutedEventArgs e)
        {
            GetData();
            if (!CheckData())
            {
                return;
            }
            new PA.ViewModel.ViewModel_凭证管理().InsertData(Voucher, VoucherDetails);
            OnSubmit();
            this.Close();
        }

        private void Button_保存并新增_Click(object sender, RoutedEventArgs e)
        {
            GetData();
            if (!CheckData())
            {
                return;
            }
            new PA.ViewModel.ViewModel_凭证管理().InsertData(Voucher, VoucherDetails);
            InitData();
        }

        private void Button_打印_Click(object sender, RoutedEventArgs e)
        {
            foreach (Model_凭证明细 Detail in VoucherDetails)
            {
                Console.WriteLine(Detail.摘要);
            }
        }
        
        private void DataGrid_凭证明细_Cell_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Model_凭证明细 SelectedRow = this.DataGrid_凭证明细.SelectedCells[0].Item as Model_凭证明细;
            DataGridCellInfo DoubleClickCell = this.DataGrid_凭证明细.CurrentCell;
            CellId = SelectedRow.序号;
            if (DoubleClickCell.Column.Header.ToString() == "科目")
            {
                PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目 page = new PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目();
                page.FillDate += new Pages.Pop.凭证录入.Page_凭证录入_科目_FillDateEventHandle(DoFillData);
                this.Frame_科目子细目.Content = page;
                this.Popup_科目子细目.IsOpen = true;
                this.Window_记账凭证.IsEnabled = false;
            }
            else if (DoubleClickCell.Column.Header.ToString() == "子细目")
            {
                //获取科目编号
                if (string.IsNullOrEmpty(SelectedRow.科目编号))
                {
                    MessageBox.Show("请选择科目后再选择子细目");
                    return;
                }
                string str = new PA.ViewModel.ViewModel_科目管理().GetSubjectID(SelectedRow.科目编号);
                PA.View.Pages.Pop.凭证录入.Page_凭证录入_子细目 page = new PA.View.Pages.Pop.凭证录入.Page_凭证录入_子细目(str);
                page.FillDate += new Pages.Pop.凭证录入.Page_凭证录入_子细目_FillDateEventHandle(DoFillData);
                this.Frame_科目子细目.Content = page;
                this.Popup_科目子细目.IsOpen = true;
                this.Window_记账凭证.IsEnabled = false;
            }
            else if (DoubleClickCell.Column.Header.ToString() == "记账")
            {
                if (VoucherDetails[CellId].记账 == 0)
                {
                    VoucherDetails[CellId].记账 = 1;
                }
                else
                {
                    VoucherDetails[CellId].记账 = 0;
                }
            }
        }

        private void Button_PopupClose_Click(object sender, RoutedEventArgs e)
        {
            this.Popup_科目子细目.IsOpen = false;
            this.Window_记账凭证.IsEnabled = true;
        }

        private void DataGrid_凭证明细_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Model_凭证明细 SelectedRow = this.DataGrid_凭证明细.SelectedCells[0].Item as Model_凭证明细;
            string newValue = (e.EditingElement as TextBox).Text.Trim();
            string Header = e.Column.Header.ToString();
            if (Header == "借方金额")
            {
                VoucherDetailsNow[SelectedRow.序号].借方 = decimal.Parse(newValue);
                VoucherDetailsNow[SelectedRow.序号].贷方 = 0m;
            }
            else if (Header == "贷方金额")
            {
                VoucherDetailsNow[SelectedRow.序号].贷方 = decimal.Parse(newValue);
                VoucherDetailsNow[SelectedRow.序号].借方 = 0m;
            }
            Count合计();
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

        private void Button_NewDataGrid_Click(object sender, RoutedEventArgs e)
        {
            if (VoucherDetails.Count < 6 * PageAll)
            {
                List<Model_凭证明细> VoucherDetailNew = this.DataGrid_凭证明细.ItemsSource as List<Model_凭证明细>;
                for (int i = 0; i < 6; i++)
                {
                    VoucherDetailNew[i].凭证号 = this.TextBox_号.Text.Trim();
                    VoucherDetails.Add(VoucherDetailNew[i]);
                }
            }
            VoucherDetailsNow = new List<Model_凭证明细>();
            for (int i = 0; i < 6; i++)
            {
                Model_凭证明细 a = new Model_凭证明细();
                a.序号 = i;
                VoucherDetailsNow.Add(a);
            }
            this.DataGrid_凭证明细.ItemsSource = VoucherDetailsNow;
            PageAll++;
            PageNow = PageAll;
            this.TextBlock_PageNum.Text = PageNow + "/" + PageAll;
        }

        private void Button_Previous_Click(object sender, RoutedEventArgs e)
        {
            if (VoucherDetails.Count < 6*PageAll)
            {
                List<Model_凭证明细> VoucherDetailNew = this.DataGrid_凭证明细.ItemsSource as List<Model_凭证明细>;
                for (int i = 0; i < 6; i++)
                {
                    VoucherDetailNew[i].凭证号 = this.TextBox_号.Text.Trim();
                    VoucherDetails.Add(VoucherDetailNew[i]);
                }
            }
            if(PageNow > 1)
            {
                PageNow--;
                this.TextBlock_PageNum.Text = PageNow + "/" + PageAll;
                VoucherDetailsNow = new List<Model_凭证明细>();
                for (int i = 0; i < 6; i++ )
                {
                    VoucherDetailsNow.Add(VoucherDetails[(PageNow - 1) * 6 + i]);
                }
                this.DataGrid_凭证明细.ItemsSource = VoucherDetailsNow;
            }
        }

        private void Button_Next_Click(object sender, RoutedEventArgs e)
        {
            if (VoucherDetails.Count < 6 * PageAll)
            {
                List<Model_凭证明细> VoucherDetailNew = this.DataGrid_凭证明细.ItemsSource as List<Model_凭证明细>;
                for (int i = 0; i < 6; i++)
                {
                    VoucherDetailNew[i].凭证号 = this.TextBox_号.Text.Trim();
                    VoucherDetails.Add(VoucherDetailNew[i]);
                }
            }
            if (PageNow < PageAll)
            {
                PageNow++;
                this.TextBlock_PageNum.Text = PageNow + "/" + PageAll;
                VoucherDetailsNow = new List<Model_凭证明细>();
                for (int i = 0; i < 6; i++)
                {
                    VoucherDetailsNow.Add(VoucherDetails[(PageNow - 1) * 6 + i]);
                }
                this.DataGrid_凭证明细.ItemsSource = VoucherDetailsNow;
            }
        }
    }
}
