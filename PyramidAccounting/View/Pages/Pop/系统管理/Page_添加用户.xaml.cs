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

        private int ReturnAuthority(string value)
        {
            int i = 0;
            switch (value)
            {
                case "记账员":
                    break;
                case "审核员":
                    i = 1;
                    break;
                case "会计主管":
                    i  =2;
                    break;
            }
            return i;
        }
        private Model_用户 SetData()
        {
            Model_用户 m = new Model_用户();
            m.用户名 = TextBox_用户名.Text.Trim();
            m.密码 = Secure.TranslatePassword(TextBox_用户密码.SecurePassword);
            m.真实姓名 = TextBox_真实姓名.Text.Trim();
            m.权限 = ReturnAuthority(ComboBox_用户权限.Text.ToString());
            m.创建日期 = DateTime.Now;
            m.用户说明 = TextBox_用户说明.Text.Trim();
            return m;
        }
        private void Button_PopCommit_Click(object sender, RoutedEventArgs e)
        {
            Model_用户 m = SetData();
            List<Model_用户> lm = new List<Model_用户>();
            lm.Add(m);
            vm.Insert(lm);
            NowClose(this, e);
        }

        private void Button_PopClose_Click(object sender, RoutedEventArgs e)
        {
            NowClose(this, e);
        }

        private void NowClose(object sender, RoutedEventArgs e)
        {
            CloseEvent(this, e);
        }
    }
}
