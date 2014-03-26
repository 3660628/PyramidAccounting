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

namespace PA.View.Pages.TwoTabControl
{
    /// <summary>
    /// Interaction logic for Page_Two_账簿管理.xaml
    /// </summary>
    public partial class Page_Two_账簿管理 : Page
    {
        public Page_Two_账簿管理()
        {
            InitializeComponent();
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
        private void TextBox_一级科目_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目 page = new PA.View.Pages.Pop.凭证录入.Page_凭证录入_科目();
            page.FillDate += new Pages.Pop.凭证录入.Page_凭证录入_科目_FillDateEventHandle(DoFillData);
            this.Frame_科目子细目.Content = page;
            this.Popup_科目子细目.IsOpen = true;
            this.IsEnabled = false;
        }

        private void Button_PopupClose_Click(object sender, RoutedEventArgs e)
        {
            this.Popup_科目子细目.IsOpen = false;
            this.IsEnabled = true;
        }

        private void TextBox_二级科目_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox_一级科目.Text.Trim()))
            {
                MessageBoxCommon.Show("请选择一级科目后再操作！");
                return;
            }
            PA.View.Pages.Pop.凭证录入.Page_凭证录入_子细目 page = new PA.View.Pages.Pop.凭证录入.Page_凭证录入_子细目(TextBox_一级科目.Text.ToString().Split('\t')[1]);
            page.FillDate += new Pages.Pop.凭证录入.Page_凭证录入_子细目_FillDateEventHandle(DoFillData);
            this.Frame_科目子细目.Content = page;
            this.Popup_科目子细目.IsOpen = true;
            this.IsEnabled = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
