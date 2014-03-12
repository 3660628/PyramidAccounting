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

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DoFillDate("科目1");
        }
    }
}
