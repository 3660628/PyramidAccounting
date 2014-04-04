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
using PA.View.ResourceDictionarys.MessageBox;

namespace PA.View.Pages.Pop.系统管理
{
    /// <summary>
    /// Interaction logic for Page_修改用户.xaml
    /// </summary>
    public partial class Page_修改用户 : Page
    {
        public event Page_系统管理_CloseEventHandle CloseEvent;
        private int id = 0;
        private ViewModel_用户 vm = new ViewModel_用户();
        public Page_修改用户(int id)
        {
            InitializeComponent();
            this.id = id;
            InitComboBox();
            InitData(id);
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
        private void InitData(int i)
        {
            Model_用户 m = new Model_用户();
            m = vm.GetUserInfo(i);
            TextBox_用户名.Text = m.用户名;
            TextBox_真实姓名.Text = m.真实姓名;
            ComboBox_用户权限.Text = m.用户权限;
            TextBox_用户说明.Text = m.用户说明;
        }
        private Model_用户 SetData()
        {
            Model_用户 m = new Model_用户();
            m.ID = id;
            m.真实姓名 = TextBox_真实姓名.Text.Trim();
            m.权限值 = ComboBox_用户权限.SelectedIndex - 1;
            m.用户说明 = TextBox_用户说明.Text.Trim();
            return m;
        }

        private void Button_PopCommit_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBox_用户权限.SelectedIndex == 0)
            {
                MessageBoxCommon.Show("请选择用户权限");
                ComboBox_用户权限.Focus();
                return;
            }
            Model_用户 m = SetData();
            bool flag =vm.Update(m);
            if (flag)
            {
                NowClose(this, e);
            }
            else
            {
                MessageBoxCommon.Show("修改用户失败，请联系管理员！");
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
    }
}
