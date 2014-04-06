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

namespace PA.View.Pages.Pop.账簿管理
{
    public delegate void Page_添加固定资产_CommitEventHandle(object sender, MyEventArgs e);
    /// <summary>
    /// Interaction logic for Page_添加固定资产.xaml
    /// </summary>
    public partial class Page_添加固定资产 : Page
    {
        public static event Page_添加固定资产_CommitEventHandle ECommit;
        public Page_添加固定资产()
        {
            InitializeComponent();
        }

        #region 自定义事件
        private void OnCommit(bool isCommit)
        {
            if (ECommit != null)
            {
                MyEventArgs e = new MyEventArgs();
                e.IsCommit = isCommit;
                ECommit(this, e);
            }
        }
        #endregion


        /// <summary>
        /// 添加固定资产
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_PopCommit_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_PopClose_Click(object sender, RoutedEventArgs e)
        {
            OnCommit(false);
        }
    }
}
