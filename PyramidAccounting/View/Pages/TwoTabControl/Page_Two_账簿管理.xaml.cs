﻿using System;
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

namespace PA.View.Pages.TwoTabControl
{
    /// <summary>
    /// Interaction logic for Page_Two_账簿管理.xaml
    /// </summary>
    public partial class Page_Two_账簿管理 : Page
    {
        private ViewModel_账薄管理 vmk = new ViewModel_账薄管理();
        public Page_Two_账簿管理()
        {
            InitializeComponent();
        }
        
       
        private void Button_PopupClose_Click(object sender, RoutedEventArgs e)
        {
            this.Popup_科目子细目.IsOpen = false;
            this.IsEnabled = true;
        }
        #region Mouse事件
        private void FillData总账(object sender, StringEventArgs e)
        {
            this.Popup_科目子细目.IsOpen = false;
            this.IsEnabled = true;
            if (typeof(PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目).IsInstanceOfType(sender))
            {
                TextBox_科目及单位名称.Text = e.Str;
            }
        }
        private void DoFillData(object sender, StringEventArgs e)
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
            PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目 page = new PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目();
            page.FillDate += new Pages.Pop.凭证录入.Page_凭证录入_科目_FillDateEventHandle(FillData总账);
            this.Frame_科目子细目.Content = page;
            this.Popup_科目子细目.IsOpen = true;
            this.IsEnabled = false;
        }
        private void TextBox_一级科目_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目 page = new PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目();
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
                this.DataGrid_科目明细.ColumnHeaderHeight = 0;
                this.DataGrid_科目明细.RowHeaderWidth = 0;
                List<Model_科目明细账> lm = vmk.GetData(a, b);
                this.DataGrid_科目明细.ItemsSource = lm;
                this.Label_年.Content = lm[0].年 + "年";
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
                List<Model_总账> lm = vmk.GetData(a);
                this.DataGrid_总账.ItemsSource = lm;
                this.Label_总账年.Content = lm[0].年 + "年";
            }
        }

        private void Button_费用明细_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
