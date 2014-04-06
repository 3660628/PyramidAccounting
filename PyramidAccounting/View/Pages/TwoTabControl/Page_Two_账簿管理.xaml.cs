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
            this.ComboBox_Date.ItemsSource = cbc.GetComboBox_期数(1);
            this.ComboBox_Date.SelectedIndex = CommonInfo.当前期-1;
        }
        #region 事件订阅
        private void SubscribeToEvent()
        {
            PA.View.Pages.TwoTabControl.Page_Two_快捷界面.TabChange += new Page_Two_快捷界面_TabChange(DoTabChange);
        }
        #endregion
        #region 接受事件后处理
        private void DoTabChange(object sender, MyEventArgs e)
        {
            if(e.Y == 2)
            {
                this.TabControl_账簿管理.SelectedIndex = e.X;
            }
        }
        #endregion

        private void Button_PopupClose_Click(object sender, RoutedEventArgs e)
        {
            this.Popup_科目子细目.IsOpen = false;
            this.IsEnabled = true;
        }
        #region Mouse事件
        private void FillData总账(object sender, MyEventArgs e)
        {
            this.Popup_科目子细目.IsOpen = false;
            this.IsEnabled = true;
            if (typeof(PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目).IsInstanceOfType(sender))
            {
                TextBox_科目及单位名称.Text = e.Str;
            }
        }
        private void FillData费用(object sender, MyEventArgs e)
        {
            this.Popup_科目子细目.IsOpen = false;
            this.IsEnabled = true;
            if (typeof(PA.View.Pages.Pop.凭证录入.Page_凭证录入_子细目).IsInstanceOfType(sender))
            {
                this.TextBox_费用明细.Text = e.Str;
            }
        }
        private void DoFillData(object sender, MyEventArgs e)
        {
            this.Popup_科目子细目.IsOpen = false;
            this.IsEnabled = true;
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
            this.IsEnabled = false;
        }
        private void TextBox_费用明细_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PA.View.Pages.Pop.凭证录入.Page_凭证录入_子细目 page = new PA.View.Pages.Pop.凭证录入.Page_凭证录入_子细目("501");
            page.FillDate += new Pages.Pop.凭证录入.Page_凭证录入_子细目_FillDateEventHandle(FillData费用);
            this.Frame_科目子细目.Content = page;
            this.Popup_科目子细目.IsOpen = true;
            this.IsEnabled = false;
        }

        private void TextBox_一级科目_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            page.FillDate += new Pages.Pop.凭证录入.Page_凭证录入_科目_FillDateEventHandle(DoFillData);
            this.Frame_科目子细目.Content = page;
            this.Popup_科目子细目.IsOpen = true;
            this.IsEnabled = false;
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
            this.IsEnabled = false;
        }
        #endregion
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }


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
                string a = TextBox_一级科目.Text.ToString().Split('\t')[1];
                string b = TextBox_二级科目.Text.ToString();
                List<Model_科目明细账> lm = vmk.GetSubjectDetail(a, b ,ComboBox_Date.SelectedIndex+1);
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
                List<Model_总账> lm = vmk.GetTotalFee(a);
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
            if (string.IsNullOrEmpty(TextBox_费用明细.Text))
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
        }
        #endregion

        private void Button_添加(object sender, RoutedEventArgs e)
        {
            this.Grid_账簿管理弹出.Visibility = System.Windows.Visibility.Visible;
            this.Frame_账簿管理弹出.Content = new PA.View.Pages.Pop.账簿管理.Page_添加固定资产();
        }

        private void Popup_科目子细目_Closed(object sender, EventArgs e)
        {
            this.IsEnabled = true;
        }
    }
}
