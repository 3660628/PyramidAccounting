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
using PA.View.ResourceDictionarys.MessageBox;
using PA.Helper.DataDefind;
using PA.Helper.XMLHelper;
using PA.ViewModel;

namespace PA.View.Pages.TwoTabControl
{

    public partial class Page_Two_快捷界面 : Page
    {
        private XMLWriter xw = new XMLWriter();
        private ViewModel_Books vmb = new ViewModel_Books();
        public Page_Two_快捷界面()
        {
            InitializeComponent();
        }

        private void Button_凭证输入_Click(object sender, RoutedEventArgs e)
        {
            PA.View.Windows.Win_记账凭证 win = new PA.View.Windows.Win_记账凭证();

            win.ShowDialog();
        }

        private void Button_凭证审核_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_凭证过账_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_查询修改_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_本月结账_Click(object sender, RoutedEventArgs e)
        {
            bool? result2 = MessageBoxDel.Show("注意", "当前将进行结账操作，是否继续？");
            if (result2 == true)
            {
                bool? result = MessageBoxInput.Show("安全确认，请输入密码");
                if (result == true)
                {
                    if (CommonInfo.验证密码.Equals(CommonInfo.登录密码))
                    {
                        //xw.WriteXML("期", (CommonInfo.当前期 + 1).ToString());
                        //vmb.UpdatePeriod(CommonInfo.当前期 + 1);
                        //CommonInfo.当前期 ++ ;
                        //结账操作
                    }
                    else
                    {
                        MessageBoxCommon.Show("当前输入密码错误！");
                    }
                }
            }
        }

        private void Button_账目查询_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
