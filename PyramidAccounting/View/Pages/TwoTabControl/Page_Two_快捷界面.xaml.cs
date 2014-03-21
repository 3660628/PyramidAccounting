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
using PA.View.ResourceDictionarys.MessageBox;
using PA.Helper.DataDefind;

namespace PA.View.Pages.TwoTabControl
{

    public partial class Page_Two_快捷界面 : Page
    {
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
            bool? result = MessageBox_Input.Show("安全确认，请输入密码");
            if (result == true)
            {
                if (CommonInfo.验证密码.Equals(CommonInfo.登录密码))
                {
                    //do sth
                    bool? result2 = MessageBox_Del.Show("注意", "当前将进行结账操作，是否继续？");
                }
                else
                {
                    MessageBox_Common.Show("当前输入密码错误！");
                }
            }
        }

        private void Button_账目查询_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
