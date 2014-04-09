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
using System.Windows.Shapes;
using PA.Model.ComboBox;
using PA.Helper.DataBase;
using PA.Helper.DataDefind;
using PA.ViewModel;
using PA.View.Windows;
using PA.Helper.XMLHelper;
using PA.Model.DataGrid;
using PA.Helper.Tools;

namespace PA
{
    public partial class Win_SignIn : Window
    {
        private XMLWriter xw = new XMLWriter();
        private XMLReader xr = new XMLReader();
        private ViewModel_用户 vm = new ViewModel_用户();
        private ViewModel_操作日志 vmr = new ViewModel_操作日志();
        private ViewModel_系统管理 vsy = new ViewModel_系统管理();

        public Win_SignIn()
        {
            InitializeComponent();
            new PA.Helper.DataBase.StartUpInit().Init();
            InitComboBox();
            this.TextBox_登陆用户名.Focus();
        }

        public Win_SignIn(double Left, double Top)
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            this.Left = Left;
            this.Top = Top;
            new PA.Helper.DataBase.StartUpInit().Init();
            InitComboBox();
            this.TextBox_登陆用户名.Focus();
            
        }
        private void InitComboBox()
        {
            ComboBox_账套.ItemsSource = new ComboBox_Common().GetComboBox_账套();
            ComboBox_账套.DisplayMemberPath = "账套名称";
            ComboBox_账套.SelectedValuePath = "ID";
            ComboBox_账套.Text = new XMLReader().ReadXML("账套信息");

            CommonInfo.U盘设备ID = vsy.GetUsbDeviceID();

        }

        #region Button事件
        private void Button_Min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Button_登陆_Click(object sender, RoutedEventArgs e)
        {
            if (!vsy.GetRunningFlag())
            {
                TextBlock_登陆警告信息.Text = "出于安全考虑，您无法进入系统，详细请联系开发商！";
                return;
            }
            string bookname = ComboBox_账套.Text.ToString();
            string  id = ComboBox_账套.SelectedValue.ToString();
            CommonInfo.账薄号 = id;
            string UserName = TextBox_登陆用户名.Text.Trim();
            string Password = Secure.TranslatePassword(PasswordBox_登陆密码.SecurePassword);

            if (vm.ValidateAccount(UserName,Password))
            {
                Model.DataGrid.Model_用户 m = new Model.DataGrid.Model_用户();
                m = vm.GetUserInfo(UserName);
                CommonInfo.真实姓名 = m.真实姓名;
                CommonInfo.用户名 = UserName;
                CommonInfo.用户权限 = m.用户权限;
                CommonInfo.权限值 = m.权限值;
                CommonInfo.登录密码 = Password;
                CommonInfo.制度索引 = Convert.ToInt32(xr.ReadXML("会计制度"));
                CommonInfo.是否初始化年初数 = new ViewModel_年初金额().IsSaved();
                //先记录一些信息
                Model_操作日志 mr = new Model_操作日志();
                mr = vmr.GetOperateLog();

                if (ComboBox_账套.SelectedValue.ToString().Equals("0"))
                {
                    if (m.权限值 >= 2)
                    {
                        Win_账套页面 w = new Win_账套页面(this.Left, this.Top);
                        w.Show();
                        this.Close();
                    }
                    else
                    {
                        TextBlock_登陆警告信息.Text = "您的权限不够，无法新建账套！";
                        return;
                    }
                }
                else
                {
                    if (xr.ReadXML("期").Equals("0"))
                    {
                        CommonInfo.当前期 = new ViewModel_Books().GetPeriod();
                    }
                    else
                    {
                        CommonInfo.当前期 = Convert.ToInt32(xr.ReadXML("期"));
                    }
                    //这里写日志信息
                    mr.日志 = "登录了账套：" + bookname;
                    vmr.Insert(mr);

                    xw.WriteXML("账套信息", bookname);
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    this.Close();
                }
            }
            else
            {
                TextBlock_登陆警告信息.Text = "账号或密码错误。";
            }

        }

        #endregion
    }
}
