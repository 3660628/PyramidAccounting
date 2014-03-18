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
using PA.ViewModel;
using PA.Model.Database;
using PA.Helper.DataBase;
using PA.Helper.DataDefind;
using PA.Model.ComboBox;

namespace PA.View.Windows
{
    /// <summary>
    /// Interaction logic for Win_账套页面.xaml
    /// </summary>
    public partial class Win_账套页面 : Window
    {
        private DataBase db = new DataBase();
        private ComboBox_Common cc = new ComboBox_Common();
        private ViewModel_Books vb = new ViewModel_Books();
        public Win_账套页面()
        {
            InitializeComponent();
            this.Top = Properties.Settings.Default.MainWindowRect.Top;
            this.Left = Properties.Settings.Default.MainWindowRect.Left;
            InitComboBox();
        }
        private void InitComboBox()
        {
            ComboBox_制度.ItemsSource = cc.GetComboBox_会计制度();
            ComboBox_制度.SelectedIndex = 0;
            ComboBox_money.ItemsSource = cc.GetComboBox_本位币();
            ComboBox_money.SelectedIndex = 0;
        }
        private bool Validate()
        {
            if (vb.IsBookNameExist(TextBox_账套名称.Text.Trim()))
            {
                MessageBox.Show("当前填写帐套名称已存在数据库中，请核对！");
                return false;
            }
            if (string.IsNullOrEmpty(TextBox_year.Text.Trim()) || string.IsNullOrEmpty(TextBox_期.Text.Trim()))
            {
                MessageBox.Show("当前检测到启用期间未填写完成，请填写后继续！");
                if (string.IsNullOrEmpty(TextBox_year.Text.Trim()))
                {
                    TextBox_year.Focus();
                }
                else if (string.IsNullOrEmpty(TextBox_期.Text.Trim()))
                {
                    TextBox_期.Focus();
                }
                return false;
            }
            return true;
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

        private void Button_创建_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate())  //校验不成功
            {
                return;
            }
            List<Model.DataGrid.Model_帐套> lm = new List<Model.DataGrid.Model_帐套>();
            Model.DataGrid.Model_帐套 m = new Model.DataGrid.Model_帐套();
            m.ID = DateTime.Now.ToString("yyyyMMddHH");
            CommonInfo.账薄号 = m.ID;
            m.帐套名称 = TextBox_账套名称.Text.Trim();
            m.单位名称 = TextBox_公司.Text.Trim();
            m.本位币 = ComboBox_money.Text.Trim();
            string date = TextBox_year.Text + "-" + TextBox_期.Text + "-1";
            m.日期 = Convert.ToDateTime(date);
            m.会计制度 = ComboBox_制度.Text.Trim();
            lm.Add(m);
            //修改下次启动时帐套的显示
            new PA.Helper.XMLHelper.XMLWriter().WriteXML("帐套信息", m.帐套名称);
            new ViewModel_Books().Insert(lm);  //执行插入
            //调整至主页面
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }

        private void Button_取消_Click(object sender, RoutedEventArgs e)
        {
            //此处应该是返回登录窗口
            new PA.Win_SignIn(this.Left, this.Top).Show();
            this.Close();
        }
        #endregion

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
