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
using PA.Model.DataGrid;
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
        private Helper.XMLHelper.XMLReader xr = new Helper.XMLHelper.XMLReader();
        private PA.Helper.XMLHelper.XMLWriter xw = new Helper.XMLHelper.XMLWriter();
        private string date = DateTime.Now.ToString("yyyy-M-d");
        public Win_账套页面()
        {
            InitializeComponent();
            this.Top = Properties.Settings.Default.MainWindowRect.Top;
            this.Left = Properties.Settings.Default.MainWindowRect.Left;
            InitData();
            this.TextBox_公司.Focus();
        }
        public Win_账套页面(double Left, double Top)
        {
            InitializeComponent();
            this.Left = Left;
            this.Top = Top;
            InitData();
            this.TextBox_公司.Focus();
        }
        private void InitData()
        {
            TextBox_公司.Text = xr.ReadXML("公司");
            TextBox_year.Text = date.Split('-')[0];
            TextBox_期.Text = date.Split('-')[1];
            ComboBox_制度.ItemsSource = cc.GetComboBox_会计制度();
            ComboBox_制度.SelectedIndex = Convert.ToInt32(xr.ReadXML("会计制度"));
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
            m.ID = DateTime.Now.ToString("yyyyMMddHHmmss");
            CommonInfo.账薄号 = m.ID;
            CommonInfo.制度索引 = ComboBox_制度.SelectedIndex;
            m.帐套名称 = TextBox_账套名称.Text.Trim();
            m.单位名称 = TextBox_公司.Text.Trim();
            string date = TextBox_year.Text + "-" + TextBox_期.Text + "-1";
            m.日期 = Convert.ToDateTime(date);
            m.会计制度 = ComboBox_制度.Text.Trim();
            lm.Add(m);

            //修改下次启动时帐套的显示
            xw.WriteXML("帐套信息", m.帐套名称);
            xw.WriteXML("公司",TextBox_公司.Text.Trim());
            xw.WriteXML("会计制度", ComboBox_制度.SelectedIndex.ToString());

            //数据创建步骤
            //1.创建账套
            new ViewModel_Books().Insert(lm);
            //2.为账套新建初始年初数
            new ViewModel_年初金额().Insert(m.ID);

            Model_操作日志 mr = new Model_操作日志();
            ViewModel_操作日志 vmr = new ViewModel_操作日志();
            mr = vmr.GetTOperateLog();
            mr.日志 = "创建了账套：" + m.帐套名称;
            vmr.Insert(mr);

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

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        #endregion
        #region 页面验证
        private void TextBox_账套名称_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            string bookName = TextBox_账套名称.Text.Trim();
            if (string.IsNullOrEmpty(bookName))
            {
                TextBlock_账套错误信息.Text = "当前填写账套名称为空，请填写！";
                TextBox_账套名称.Focus();
                return;
            }
            else
            {
                if (vb.IsBookNameExist(TextBox_账套名称.Text.Trim()))
                {
                    TextBlock_账套错误信息.Text = "当前填写帐套名称已存在数据库中，请修正！";
                    TextBox_账套名称.Focus();
                    return;
                }
                else
                {
                    TextBlock_账套错误信息.Text = "";
                }
            }
        }
        private void TextBox_公司_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox_公司.Text.Trim()))
            {
                TextBlock_公司错误信息.Text = "单位名称将作为报表必填项，请填写！";
                TextBox_公司.Focus();
                return;
            }
            else
            {
                TextBlock_公司错误信息.Text = "";
            }
        }
        /// <summary>
        /// 校验方法
        /// </summary>
        /// <returns>校验结果</returns>
        private bool Validate()
        {
            string bookName = TextBox_账套名称.Text.Trim();
            if (string.IsNullOrEmpty(bookName))
            {
                MessageBox.Show("当前账套名称为空，请核对！");
                TextBox_账套名称.Focus();
                return false;
            }
            else
            {
                if (vb.IsBookNameExist(TextBox_账套名称.Text.Trim()))
                {
                    MessageBox.Show("当前填写帐套名称已存在数据库中，请核对！");
                    TextBox_账套名称.Focus();
                    return false;
                }
            }
            if (string.IsNullOrEmpty(TextBox_公司.Text.Trim()))
            {
                MessageBox.Show("单位名称将作为报表必填项，请填写！");
                TextBox_公司.Focus();
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
        #endregion

        private void TextBox_公司_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextBox_year.Text.ToString().Equals(""))
            {
                TextBox_year.Text = date.Split('-')[0];
            }
            this.TextBox_账套名称.Text = TextBox_year.Text.ToString() + TextBox_公司.Text.ToString() + "财务账";
        }
    }
}
