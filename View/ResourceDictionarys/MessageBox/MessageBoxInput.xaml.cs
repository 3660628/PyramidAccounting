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
using PA.Helper.DataBase;
using PA.Helper.DataDefind;

namespace PA.View.ResourceDictionarys.MessageBox
{
    /// <summary>
    /// Interaction logic for MessageBox_Input.xaml
    /// </summary>
    public partial class MessageBoxInput : Window
    {
        public MessageBoxInput()
        {
            InitializeComponent();
            PasswordBox_User.Focus();
        }
        public new string Title
        {
            get { return this.lblTitle.Text; }
            set { this.lblTitle.Text = value; }
        }
        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        /// <summary>
        /// 静态方法 模拟MESSAGEBOX.Show方法
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public static bool? Show(string title)
        {

            var msgBox = new MessageBoxInput();
            msgBox.Title = title;
            return msgBox.ShowDialog();
        }
        private void Yes_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DialogResult = true;
            CommonInfo.验证密码 = Secure.TranslatePassword(PasswordBox_User.SecurePassword);
            this.Close();
        }

        private void No_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void main_Loaded(object sender, RoutedEventArgs e)
        {
            this.PreviewKeyDown += new KeyEventHandler(main_PreviewKeyDown);
        }

        private void main_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Yes_MouseLeftButtonDown(this, null);
            }
        }
    }
}
