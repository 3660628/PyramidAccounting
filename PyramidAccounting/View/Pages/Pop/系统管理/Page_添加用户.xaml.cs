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
using PA.ViewModel;
using PA.Model.DataGrid;
using PA.Helper.DataBase;
using PA.View.ResourceDictionarys.MessageBox;
using PA.Helper.DataDefind;

namespace PA.View.Pages.Pop.系统管理
{
    public delegate void Page_系统管理_CloseEventHandle(object sender, RoutedEventArgs e);
    /// <summary>
    /// Interaction logic for Page_添加用户.xaml
    /// </summary>
    public partial class Page_添加用户 : Page
    {
        public event Page_系统管理_CloseEventHandle CloseEvent;
        private ViewModel_用户 vm = new ViewModel_用户();
        private ViewModel_操作日志 vmr = new ViewModel_操作日志();
        Model_操作日志 _mr = new Model_操作日志();
        public Page_添加用户()
        {
            InitializeComponent();
            InitComboBox();
        }

        private void InitComboBox()
        {
            List<string> list = new List<string>();
            list.Add("请选择...");
            list.Add("记账员");
            list.Add("审核员");
            list.Add("会计主管");
            ComboBox_用户权限.ItemsSource = list;
            ComboBox_用户权限.SelectedIndex = 0;
        }

        
        private Model_用户 SetData()
        {
            Model_用户 m = new Model_用户();
            m.用户名 = TextBox_用户名.Text.Trim();
            m.密码 = Secure.TranslatePassword(TextBox_用户密码.SecurePassword);
            m.真实姓名 = TextBox_真实姓名.Text.Trim();
            m.权限值 = ComboBox_用户权限.SelectedIndex - 1;
            m.创建日期 = DateTime.Now;
            m.用户说明 = TextBox_用户说明.Text.Trim();
            return m;
        }
        private bool Validate()
        {
            string username = TextBox_用户名.Text.Trim();
            if (ComboBox_用户权限.SelectedIndex == 0)
            {
                MessageBoxCommon.Show("请选择用户权限");
                return false;
            }
            if (string.IsNullOrEmpty(username))
            {
                MessageBoxCommon.Show("请填写用户名");
                return false;
            }
            else
            {
                if (vm.ValidateUserName(username)) 
                {
                    MessageBoxCommon.Show("当前用户名已存在，请勿重复添加！");
                    return false;
                }
                if (vm.ValidateAccountOfficer((int)ENUM.EM_AUTHORIY.会计主管))
                {
                    MessageBoxCommon.Show("当前已存在会计主管，请勿重复添加！");
                    return false;
                }
            }
            if (TextBox_用户密码.SecurePassword.Length == 0)
            {
                MessageBoxCommon.Show("请设置初始密码！");
                return false;
            }
            return true;
        }
        private void Button_PopCommit_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate())
            {
                return;
            }
            Model_用户 m = SetData();
            List<Model_用户> lm = new List<Model_用户>();
            lm.Add(m);
            bool flag = vm.Insert(lm);
            if (flag)
            {
                _mr.日志 = "新增用户：" + m.用户名;
                vmr.Insert(_mr);
                NowClose(this, e);
            }
            else
            {
                MessageBoxCommon.Show("添加用户失败！");
            }
        }

        private void Button_PopClose_Click(object sender, RoutedEventArgs e)
        {
            NowClose(this, e);
        }

        private void NowClose(object sender, RoutedEventArgs e)
        {
            CloseEvent(this, e);
        }

        private void TextBox_用户名_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            bool flag = true;
            if (string.IsNullOrEmpty(TextBox_用户名.Text.Trim()))
            {
                TextBlock_用户名.Text = "请填写用户名";
                flag = false;
            }
            else
            {
                if (vm.ValidateUserName(TextBox_用户名.Text))
                {
                    TextBlock_用户名.Text = "当前用户名已存在，请勿重复添加！";
                    flag = false;
                }
            }
            if (flag)
            {
                TextBlock_用户名.Text = "";
                return;
            }
            else
            {
                //TextBox_用户名.Focus();
            }
        }

        private void TextBox_用户密码_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (TextBox_用户密码.SecurePassword.Length == 0)
            {
                TextBlock_密码.Text = "密码不能为空";
                //TextBox_用户密码.Focus();
            }
            else
            {
                TextBlock_密码.Text = "";
            }
        }
    }
}
