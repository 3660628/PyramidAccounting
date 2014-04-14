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

namespace PA.View.Pages
{
    /// <summary>
    /// Interaction logic for Page_MainTabControl.xaml
    /// </summary>
    public partial class Page_MainTabControl : Page
    {
        public Page_MainTabControl()
        {
            InitializeComponent();
            this.Frame_快捷界面.Content = new PA.View.Pages.TwoTabControl.Page_Two_快捷界面();
            this.Frame_凭证管理.Content = new PA.View.Pages.TwoTabControl.Page_Two_凭证管理();
            this.Frame_账簿管理.Content = new PA.View.Pages.TwoTabControl.Page_Two_账簿管理();
            this.Frame_报表管理.Content = new PA.View.Pages.TwoTabControl.Page_Two_报表管理();
            this.Frame_系统管理.Content = new PA.View.Pages.TwoTabControl.Page_Two_系统管理();
            SubscribeToEvent();
        }

        private void SubscribeToEvent()
        {
            PA.View.Pages.TwoTabControl.Page_Two_快捷界面.TabChange += DoTabChange;
        }

        private void DoTabChange(object sender, MyEventArgs e)
        {
            this.TabControl_Main.SelectedIndex = e.Y;
        }
    }
}
