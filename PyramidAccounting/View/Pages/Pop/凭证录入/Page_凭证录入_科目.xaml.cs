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

namespace PA.View.Pages.Pop.凭证录入
{
    public delegate void Page_凭证录入_科目_FillDateEventHandle(object sender, StringEventArgs e);

    public partial class Page_凭证录入_科目 : Page
    {
        public event Page_凭证录入_科目_FillDateEventHandle FillDate;
        public Page_凭证录入_科目()
        {
            InitializeComponent();
            List<string> li = new List<string>();
            li.Add("1");
            li.Add("2");
            li.Add("3");
            li.Add("4");
            li.Add("5");
            li.Add("6");
            li.Add("7");
            li.Add("8");
            li.Add("9");
            li.Add("10");
            li.Add("11");
            li.Add("12");
            this.ListBox_1.ItemsSource = li;
        }

        private void DoFillDate(string str)
        {
            if(FillDate != null)
            {
                StringEventArgs e = new StringEventArgs();
                e.Str = str;
                FillDate(this, e);
            }
        }

        private void ListBox_1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DoFillDate(this.ListBox_1.SelectedValue.ToString());
        }
    }
}
