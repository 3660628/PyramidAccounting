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
using PA.Model.DataGrid;

namespace PA.View.Pages.TwoTabControl
{
    public delegate void Page_Two_快捷界面_TabChange(object sender, MyEventArgs e);

    public partial class Page_Two_快捷界面 : Page
    {
        public static event Page_Two_快捷界面_TabChange TabChange;
        private XMLWriter xw = new XMLWriter();
        private ViewModel_Books vmb = new ViewModel_Books();
        private ViewModel_账薄管理 vm = new ViewModel_账薄管理();
        private ViewModel_凭证管理 vmp = new ViewModel_凭证管理();
        Model_操作日志 mr = new Model_操作日志();
        ViewModel_操作日志 vmr = new ViewModel_操作日志();
           
        public Page_Two_快捷界面()
        {
            InitializeComponent();
            mr = vmr.GetOperateLog();
        }
        private void OnTabChange(int y, int x)
        {
            OnTabChange(y, x, "null");
        }
        private void OnTabChange(int y, int x, string type)
        {
            if (TabChange != null)
            {
                MyEventArgs e = new MyEventArgs();
                e.Y = y;
                e.X = x;
                e.操作类型 = type;
                TabChange(this, e);
            }
        }

        private void Button_凭证输入_Click(object sender, RoutedEventArgs e)
        {
            if (CommonInfo.是否初始化年初数)
            {
                new PA.View.Windows.Win_记账凭证().ShowDialog();
                //OnTabChange(1, 0);
            }
            else
            {
                MessageBoxCommon.Show("当前未初始化年初金额，请先初始化年初数！");
            }
        }

        private void Button_凭证审核_Click(object sender, RoutedEventArgs e)
        {
            mr.日志 = "进入凭证审核模块！";
            vmr.Insert(mr);
            OnTabChange(1, 0, "凭证审核");

        }

        private void Button_凭证过账_Click(object sender, RoutedEventArgs e)
        {

        }



        private void Button_本月结账_Click(object sender, RoutedEventArgs e)
        {
            this.Button_隐藏.Focus();
            bool mark = false;
            if (CommonInfo.是否初始化年初数)
            {
                mark = true;
            }
            else
            {
                MessageBoxCommon.Show("当前未初始化年初金额，请先初始化年初数！");
                return;
            }
            if(!vmp.IsReview(CommonInfo.当前期))
            {
                Button_凭证审核_Click(this,null);
                mark = false;
                MessageBoxCommon.Show("检测到还有当前期未审核的凭证单，请先做审核！");
            }
            int temp = 0;
            if (mark)
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
                                if (CommonInfo.当前期 < 12)
                                {
                                    temp = CommonInfo.当前期 - 1;
                                    vmb.UpdatePeriod(CommonInfo.当前期);
                                }
                                else
                                {
                                    temp = CommonInfo.当前期;
                                }
                                mr.日志 = "进行了结账操作，结算第：" + temp + "期账";
                                vmr.Insert(mr);
                                xw.WriteXML("期", (CommonInfo.当前期).ToString());
                                OnTabChange(1, 0, "本月结账");
                            }
                        }
                        else
                        {
                            MessageBoxCommon.Show("当前输入密码错误！");
                        }
                    }
                }
            }
        }

        private void Button_账目查询_Click(object sender, RoutedEventArgs e)
        {
            OnTabChange(2, 1);
            mr.日志 = "进入账目查询模块！";
            vmr.Insert(mr);
        }

        private void Button_账簿查询_Click(object sender, RoutedEventArgs e)
        {
            OnTabChange(2, 0);
            mr.日志 = "进入账簿查询模块！";
            vmr.Insert(mr);
        }

        private void Button_科目查询_Click(object sender, RoutedEventArgs e)
        {
            OnTabChange(2, 2);
            mr.日志 = "进入科目查询模块！";
            vmr.Insert(mr);
        }

        private void Button_报表查询_Click(object sender, RoutedEventArgs e)
        {
            OnTabChange(3, 0);
            mr.日志 = "进入报表查询模块！";
            vmr.Insert(mr);
        }
    }
}
