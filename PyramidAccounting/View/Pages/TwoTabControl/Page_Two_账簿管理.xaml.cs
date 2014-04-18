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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using PA.Model.DataGrid;
using PA.Model.CustomEventArgs;
using PA.View.ResourceDictionarys.MessageBox;
using PA.ViewModel;
using PA.Helper.DataDefind;
using PA.Model.ComboBox;

namespace PA.View.Pages.TwoTabControl
{
    /// <summary>
    /// Interaction logic for Page_Two_账簿管理.xaml
    /// </summary>
    public partial class Page_Two_账簿管理 : Page
    {
        private ViewModel_账薄管理 vmk = new ViewModel_账薄管理();
        PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目 page = new PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目();
        ComboBox_Common cbc = new ComboBox_Common();

        public Page_Two_账簿管理()
        {
            InitializeComponent();
            SubscribeToEvent();
            FreshComboBox();
        }

        /// <summary>
        /// 刷新日期下拉
        /// </summary>
        private void FreshComboBox()
        {
            Label_账套名称.Content = "当前帐套名称：" + CommonInfo.账套信息;
            Label_操作员.Content = "操作员：" + CommonInfo.用户权限 + "\t" + CommonInfo.真实姓名;
            Label_当前期数.Content = "当前期数：第" + CommonInfo.当前期 + "期";
        }

        #region 事件订阅
        private void SubscribeToEvent()
        {
            PA.View.Pages.TwoTabControl.Page_Two_快捷界面.TabChange += DoTabChange;
            PA.View.Pages.Pop.账簿管理.Page_添加固定资产.ECommit += new Pop.账簿管理.Page_添加固定资产_CommitEventHandle(Do固定资产Commit);
        }
        #endregion
        #region 接受事件后处理
        private void DoTabChange(object sender, MyEventArgs e)
        {
            if(e.Y == 2)
            {
                this.TabControl_账簿管理.SelectedIndex = e.X;
            }
            if (e.操作类型 == "本月结账")
            {
                FreshComboBox();
            }
        }
        private void Do固定资产Commit(object sender, MyEventArgs e)
        {
            this.Grid_账簿管理弹出.Visibility = System.Windows.Visibility.Collapsed;
            this.TabControl_账簿管理.IsEnabled = true;
            if(e.IsCommit)
            {
                Button_固定资产_查询(null,null);
            }
        }
        #endregion

        private void Button_PopupClose_Click(object sender, RoutedEventArgs e)
        {
            this.Popup_科目子细目.IsOpen = false;
            //this.IsEnabled = true;
        }
        #region Mouse事件
        private void FillData总账(object sender, MyEventArgs e)
        {
            this.Popup_科目子细目.IsOpen = false;
            if (typeof(PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目).IsInstanceOfType(sender))
            {
                TextBox_科目及单位名称.Text = e.Str;
            }
        }

        private void FillData多栏明细账_一(object sender, MyEventArgs e)
        {
            this.Popup_科目子细目.IsOpen = false;
            if (typeof(PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目).IsInstanceOfType(sender))
            {
                TextBox_多栏明细账_一.Text = e.Str;
            }
        }
        private void FillData多栏明细账_二(object sender, MyEventArgs e)
        {
            this.Popup_科目子细目.IsOpen = false;
            if (typeof(PA.View.Pages.Pop.凭证录入.Page_凭证录入_子细目).IsInstanceOfType(sender))
            {
                TextBox_多栏明细账_二.Text = e.Str;
            }
        }
        private void FillData多栏明细账_三(object sender, MyEventArgs e)
        {
            this.Popup_科目子细目.IsOpen = false;
            if (typeof(PA.View.Pages.Pop.凭证录入.Page_凭证录入_子细目).IsInstanceOfType(sender))
            {
                TextBox_多栏明细账_三.Text = e.Str;
            }
        }
        private void DoFillData(object sender, MyEventArgs e)
        {
            this.Popup_科目子细目.IsOpen = false;
            //this.IsEnabled = true;
            if (typeof(PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目).IsInstanceOfType(sender))
            {
                TextBox_一级科目.Text = e.Str;
            }
            else if (typeof(PA.View.Pages.Pop.凭证录入.Page_凭证录入_子细目).IsInstanceOfType(sender))
            {
                TextBox_二级科目.Text = e.Str;
            }
        }
        private void TextBox_科目及单位名称_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            page.FillDate += new Pages.Pop.凭证录入.Page_凭证录入_科目_FillDateEventHandle(FillData总账);
            this.Frame_科目子细目.Content = page;
            this.Popup_科目子细目.IsOpen = true;
            //this.IsEnabled = false;
        }


        private void TextBox_多栏明细账_一_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string condition = "4%' or subject_id like '5";
            PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目 page = new PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目(condition);
            page.FillDate += new Pages.Pop.凭证录入.Page_凭证录入_科目_FillDateEventHandle(FillData多栏明细账_一);
            this.Frame_科目子细目.Content = page;
            this.Popup_科目子细目.IsOpen = true;
        }

        private void TextBox_多栏明细账_二_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TextBox_多栏明细账_一.Text == null || TextBox_多栏明细账_一.Text.Equals(""))
            {
                MessageBoxCommon.Show("请选择一级科目编号及名称!");
                TextBox_多栏明细账_一.Focus();
                return;
            }
            string subject_id = TextBox_多栏明细账_一.Text.Split('\t')[0];
            PA.View.Pages.Pop.凭证录入.Page_凭证录入_子细目 page = new PA.View.Pages.Pop.凭证录入.Page_凭证录入_子细目(subject_id, true);
            page.FillDate += new Pages.Pop.凭证录入.Page_凭证录入_子细目_FillDateEventHandle(FillData多栏明细账_二);
            this.Frame_科目子细目.Content = page;
            this.Popup_科目子细目.IsOpen = true;
        }

        private void TextBox_多栏明细账_三_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TextBox_多栏明细账_二.Text == null || TextBox_多栏明细账_二.Text.Equals(""))
            {
                MessageBoxCommon.Show("请选择一级科目编号及名称!");
                TextBox_多栏明细账_二.Focus();
                return;
            }
            string subject_id = TextBox_多栏明细账_二.Text.Split('\t')[0];
            PA.View.Pages.Pop.凭证录入.Page_凭证录入_子细目 page = new PA.View.Pages.Pop.凭证录入.Page_凭证录入_子细目(subject_id, true);
            page.FillDate += new Pages.Pop.凭证录入.Page_凭证录入_子细目_FillDateEventHandle(FillData多栏明细账_三);
            this.Frame_科目子细目.Content = page;
            this.Popup_科目子细目.IsOpen = true;
        }

        private void TextBox_一级科目_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            page.FillDate += new Pages.Pop.凭证录入.Page_凭证录入_科目_FillDateEventHandle(DoFillData);
            this.Frame_科目子细目.Content = page;
            this.Popup_科目子细目.IsOpen = true;
            //this.IsEnabled = false;
        }

        private void TextBox_二级科目_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox_一级科目.Text.Trim()))
            {
                MessageBoxCommon.Show("请选择一级科目后再操作！");
                return;
            }
            PA.View.Pages.Pop.凭证录入.Page_凭证录入_子细目 page = new PA.View.Pages.Pop.凭证录入.Page_凭证录入_子细目(TextBox_一级科目.Text.ToString().Split('\t')[0]);
            page.FillDate += new Pages.Pop.凭证录入.Page_凭证录入_子细目_FillDateEventHandle(DoFillData);
            this.Frame_科目子细目.Content = page;
            this.Popup_科目子细目.IsOpen = true;
            //this.IsEnabled = false;
        }
        #endregion


        #region 查询事件
        private void Button_查询_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox_一级科目.Text))
            {
                MessageBoxCommon.Show("请选择一级科目");
                TextBox_一级科目.Focus();
                return;
            }
            else if (string.IsNullOrEmpty(TextBox_二级科目.Text))
            {
                MessageBoxCommon.Show("请选择二级科目");
                TextBox_二级科目.Focus();
                return;
            }
            else
            {
                string a = TextBox_一级科目.Text.ToString();
                string b = TextBox_二级科目.Text.ToString();
                List<Model_科目明细账> lm = vmk.GetSubjectDetail(a, b);
                if (lm.Count > 0)
                {
                    this.Label_年.Content = lm[0].年 + "年";
                }
                else
                {
                    Model_科目明细账 m = new Model_科目明细账();
                    m.摘要 = "查询不到数据！";
                    lm.Add(m);
                }
                this.DataGrid_科目明细.ItemsSource = lm;
                
            }
        }

        private void Button_总账查询_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox_科目及单位名称.Text))
            {
                MessageBoxCommon.Show("请选择科目");
                TextBox_科目及单位名称.Focus();
                return;
            }
            else
            {
                string a = TextBox_科目及单位名称.Text.ToString();
                List<Model_总账> lm;
                if (a.Substring(0, 1) == "4" || a.Substring(0, 1) == "5")
                {
                    lm = vmk.GetTotalFee(a, true);
                }
                else
                {
                    lm = vmk.GetTotalFee(a);
                }
                this.DataGrid_总账.ItemsSource = lm;
                if (lm.Count > 1)
                {
                    this.Label_总账年.Content = lm[1].年 + "年";
                }
                else
                {
                    Model_总账 m = new Model_总账();
                    m.摘要 = "查询不到数据！";
                    lm.Add(m);
                }
                this.DataGrid_总账.ItemsSource = lm;
            }
        }

        private void Button_费用明细_Click(object sender, RoutedEventArgs e)
        {
            /*if (string.IsNullOrEmpty(TextBox_费用明细.Text))
            {
                MessageBoxCommon.Show("请选择科目");
                this.TextBox_费用明细.Focus();
                return;
            }
            else
            {
                string a = TextBox_费用明细.Text.ToString();
                List<Model_费用明细> lm = vmk.GetFeeDetail(a);
                this.DataGrid_费用明细账.ItemsSource = lm;
                if (lm.Count > 0)
                {
                    this.Label_费用明细年.Content = lm[0].年 + "年";
                    int count = 1;
                    foreach(string s in lm[0].列名)
                    {
                        Label lb = new Label();
                        lb = FindName("Label_" + count) as Label;
                        lb.Content = s.Split('\t')[1];
                        count++;
                    }
                }
                else
                {
                    Model_费用明细 m = new Model_费用明细();
                    m.摘要 = "查询不到数据！";
                    lm.Add(m);
                }
                this.DataGrid_费用明细账.ItemsSource = lm;
            }
             * */
        }
        #endregion

        private void Button_添加(object sender, RoutedEventArgs e)
        {
            this.Grid_账簿管理弹出.Visibility = System.Windows.Visibility.Visible;
            this.Frame_账簿管理弹出.Content = new PA.View.Pages.Pop.账簿管理.Page_添加固定资产();
            this.TabControl_账簿管理.IsEnabled = false;
        }

        private void Popup_科目子细目_Closed(object sender, EventArgs e)
        {
            //this.IsEnabled = true;
        }

        private void Button_总账Print_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox_科目及单位名称.Text))
            {
                MessageBoxCommon.Show("请选择科目");
                TextBox_科目及单位名称.Focus();
                return;
            }
            string Parm = TextBox_科目及单位名称.Text.ToString();
            string result = new PA.Helper.ExcelHelper.ExcelWriter().ExportLedger(Parm);
            if (result != "")
            {
                MessageBoxCommon.Show(result);
            }
        }

        private void Button_经费支出明细Print_Click(object sender, RoutedEventArgs e)
        {
            /*if (string.IsNullOrEmpty(TextBox_费用明细.Text))
            {
                MessageBoxCommon.Show("请选择科目");
                this.TextBox_费用明细.Focus();
                return;
            }
            string Parm = TextBox_费用明细.Text.ToString();
            if(new PA.Helper.ExcelHelper.ExcelWriter().ExportExpenditureDetails(Parm) != "")
            {
                MessageBoxCommon.Show("打印失败，请检查数据。");
            }*/
        }

        private void Button_科目明细Print_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox_一级科目.Text))
            {
                MessageBoxCommon.Show("请选择一级科目");
                TextBox_一级科目.Focus();
                return;
            }
            else if (string.IsNullOrEmpty(TextBox_二级科目.Text))
            {
                MessageBoxCommon.Show("请选择二级科目");
                TextBox_二级科目.Focus();
                return;
            }
            if (new PA.Helper.ExcelHelper.ExcelWriter().ExportSubjectDetails(TextBox_一级科目.Text, TextBox_二级科目.Text) != "")
            {
                MessageBoxCommon.Show("打印失败，请检查数据。");
            }
        }

        private void Button_固定资产_查询(object sender, RoutedEventArgs e)
        {
            ViewModel_固定资产 vm = new ViewModel_固定资产();
            string a = TextBox_科目及单位名称.Text.ToString();
            //List<Model_固定资产> lm = vm.GetAllSource();

            this.DataGrid_固定资产.ItemsSource = vm.GetAllSource();
        }

    }
}
