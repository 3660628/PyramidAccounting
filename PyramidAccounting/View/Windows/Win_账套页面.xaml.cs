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

namespace PA.View.Windows
{
    /// <summary>
    /// Interaction logic for Win_账套页面.xaml
    /// </summary>
    public partial class Win_账套页面 : Window
    {
        private DataBase db = new DataBase();
        public Win_账套页面()
        {
            InitializeComponent();
        }

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
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Button_创建_Click(object sender, RoutedEventArgs e)
        {
            List<Model.DataGrid.Model_帐套> lm = new List<Model.DataGrid.Model_帐套>();
            Model.DataGrid.Model_帐套 m = new Model.DataGrid.Model_帐套();
            m.ID = DateTime.Now.ToString("yyyyMMddHH");
            Properties.Settings.Default.BookID = m.ID;
            m.账套名称 = TextBox_账套名称.Text.Trim();
            m.单位名称 = TextBox_公司.Text.Trim();
            m.本位币 = ComboBox_money.Text.Trim();
            string date = TextBox_year.Text + "-" + TextBox_期.Text + "-1";
            m.日期 = Convert.ToDateTime(date);
            m.会计制度 = ComboBox_制度.Text.Trim();
            lm.Add(m);
            new ViewModel_Books().Insert(lm);  //执行插入
            //调整至主页面
            Console.WriteLine(CommonInfo.账薄号);
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }

        private void Button_取消_Click(object sender, RoutedEventArgs e)
        {
            //此处应该是返回登录窗口
            this.Close();
        }
    }
}
