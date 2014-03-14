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
using PA.Model.CustomEventArgs;
using PA.Model.ComboBox;

namespace PA.View.Pages.Pop.凭证录入
{
    public delegate void Page_凭证录入_科目_FillDateEventHandle(object sender, StringEventArgs e);

    public partial class Page_凭证录入_科目 : Page
    {
        public event Page_凭证录入_科目_FillDateEventHandle FillDate;
        public Page_凭证录入_科目()
        {
            InitializeComponent();
            this.ListBox_科目.ItemsSource = new ListCommon().GetSubjectList("");
            this.TextBox_科目搜索.Focus();
        }

        private void OnFillDate(string str)
        {
            if(FillDate != null)
            {
                StringEventArgs e = new StringEventArgs();
                e.Str = str;
                FillDate(this, e);
            }
        }

        private void ListBox_科目_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OnFillDate(this.ListBox_科目.SelectedValue.ToString().Split('\t')[1]);
        }

        private void TextBox_科目搜索_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            this.ListBox_科目.ItemsSource = new ListCommon().GetSubjectList(tb.Text.Trim());
        }

        private void Button_确定_Click(object sender, RoutedEventArgs e)
        {
            if (this.ListBox_科目.SelectedValue != null)
            {
                OnFillDate(this.ListBox_科目.SelectedValue.ToString().Split('\t')[1]);
            }
        }
    }
}
