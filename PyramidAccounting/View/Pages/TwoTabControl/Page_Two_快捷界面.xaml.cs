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
using PA.Model.CustomEventArgs;

namespace PA.View.Pages.TwoTabControl
{
    public delegate void Page_Two_快捷界面_TabChange(object sender, MyEventArgs e);
    public delegate void Page_Two_快捷界面_FilterData(object sender, MyEventArgs e);

    public partial class Page_Two_快捷界面 : Page
    {
        public static event Page_Two_快捷界面_TabChange TabChange;
        public static event Page_Two_快捷界面_FilterData FilterData;
        private XMLWriter xw = new XMLWriter();
        private ViewModel_Books vmb = new ViewModel_Books();
        private ViewModel_账薄管理 vm = new ViewModel_账薄管理();


        public Page_Two_快捷界面()
        {
            InitializeComponent();
        }

        private void OnTabChange(int y, int x)
        {
            if (TabChange != null)
            {
                MyEventArgs e = new MyEventArgs();
                e.Y = y;
                e.X = x;
                TabChange(this, e);
            }
        }
        private void OnFilterData(int y, int x, string type)
        {
            if (FilterData != null)
            {
                MyEventArgs e = new MyEventArgs();
                e.Y = y;
                e.X = x;
                e.操作类型 = type;
                FilterData(this, e);
            }
        }


        private void Button_凭证输入_Click(object sender, RoutedEventArgs e)
        {
            if (CommonInfo.是否初始化年初数)
            {
                new PA.View.Windows.Win_记账凭证().ShowDialog();
            }
            else
            {
                MessageBoxCommon.Show("当前未初始化年初金额，请先初始化年初数！");
            }
        }

        private void Button_凭证审核_Click(object sender, RoutedEventArgs e)
        {
            OnTabChange(1,0);
            OnFilterData(1, 0, "凭证审核");
        }

        private void Button_凭证过账_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_查询修改_Click(object sender, RoutedEventArgs e)
        {
            OnTabChange(1, 0);
        }

        private void Button_本月结账_Click(object sender, RoutedEventArgs e)
        {
            if (CommonInfo.是否初始化年初数)
            {
                bool? result2 = MessageBoxDel.Show("注意", "当前将进行结账操作，是否继续？");
                if (result2 == true)
                {
                    bool? result = MessageBoxInput.Show("安全确认，请输入密码");
                    if (result == true)
                    {
                        if (CommonInfo.验证密码.Equals(CommonInfo.登录密码))
                        {
                            bool flag = vm.CheckOut();
                            if (flag)
                            {
                                MessageBoxCommon.Show("结账完毕！");
                                xw.WriteXML("期", (CommonInfo.当前期 + 1).ToString());
                                vmb.UpdatePeriod(CommonInfo.当前期 + 1);
                            }
                        }
                        else
                        {
                            MessageBoxCommon.Show("当前输入密码错误！");
                        }
                    }
                }
            }
            else
            {
                MessageBoxCommon.Show("当前未初始化年初金额，请先初始化年初数！");
            }
        }

        private void Button_账目查询_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
