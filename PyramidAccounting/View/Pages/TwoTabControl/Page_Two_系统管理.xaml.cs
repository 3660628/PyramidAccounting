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
using System.Data;
using PA.Model.CustomEventArgs;
using PA.Helper.DataBase;
using PA.Helper.DataDefind;
using PA.View.ResourceDictionarys.MessageBox;
using PA.Helper.XMLHelper;

namespace PA.View.Pages.TwoTabControl
{
    /// <summary>
    /// Interaction logic for Page_Two_系统管理.xaml
    /// </summary>
    public partial class Page_Two_系统管理 : Page
    {
        private ViewModel_用户 vm = new ViewModel_用户();
        private ViewModel_Books vmb = new ViewModel_Books();
        private XMLWriter xw = new XMLWriter();
        private XMLReader xr = new XMLReader();
        private List<Model_科目管理> lm = new List<Model_科目管理>();

        public Page_Two_系统管理()
        {
            InitializeComponent();
            SubscribeToEvent();
            VisibilityData();
        }

        #region 事件订阅
        private void SubscribeToEvent()
        {
            PA.View.Windows.Win_子细目.RerflashData += new Windows.Win_子细目_RerflashData(DoRerflashData);
        }
        #endregion
        #region 接受事件后处理
        private void DoRerflashData(object sender, MyEventArgs e)
        {
            TabControl_五大科目_SelectionChanged(null, null);
        }
        #endregion

        #region 修改密码

        private void Button_ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            string OldPassword = Secure.TranslatePassword(this.PasswordBox_Old.SecurePassword);
            string NewPassword = Secure.TranslatePassword(this.PasswordBox_New.SecurePassword);
            string NewPasswordRepeat = Secure.TranslatePassword(this.PasswordBox_NewRepeat.SecurePassword);
            string username = PA.Helper.DataDefind.CommonInfo.用户名;
            bool flag = vm.ValidateAccount(username, OldPassword);   //检验旧密码是否一致
            if (!flag)
            {
                this.Label_密码错误.Visibility = System.Windows.Visibility.Visible;
                return;
            }
            if (NewPasswordRepeat.Equals(NewPassword))
            {
                if (vm.UpdatePassword(username, NewPassword))
                {
                    this.Label_密码修改成功.Visibility = System.Windows.Visibility.Visible;
                    this.PasswordBox_Old.Clear();
                    this.PasswordBox_New.Clear();
                    this.PasswordBox_NewRepeat.Clear();
                }
                else
                {
                    MessageBoxCommon.Show("当前尝试修改密码失败，请联系软件开发商！");
                }
            }
            else
            {
                this.Label_新密码不一致.Visibility = System.Windows.Visibility.Visible;
                return;
            }
        }
        #endregion
        #region 科目管理
        /// <summary>
        /// 判断是否已经初始化过年初数据，否则不许修改年初数
        /// </summary>
        private void VisibilityData()
        {
            /*if (new ViewModel_年初金额().IsSaved())
            {
                this.DataGridTextColumn_fee.IsReadOnly = true;
                this.Button_科目保存.Visibility = Visibility.Hidden;
            }*/
            if (CommonInfo.权限值 < 3)
            {
                Expander_权限.Visibility = Visibility.Hidden;
            }
            if (CommonInfo.权限值 < 2)
            {
                Expander_账套管理.Visibility = Visibility.Hidden;
            }
        }

        private void Button_科目保存_Click(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "年初金额初始化不能修改哦，请确认是否填写完整？";
            string caption = "注意";
            bool? result = MessageBoxDel.Show(caption, messageBoxText);
            if (result == false)
            {
                return;
            }
            bool flag = new ViewModel_年初金额().Update();
            if (flag)
            {
                MessageBoxCommon.Show("保存成功！");
            }
            //刷新操作
            Button btn = sender as Button;
            //btn.Visibility = Visibility.Hidden;
            CommonInfo.是否初始化年初数 = true;
        }

        private void Button_编辑子细目_Click(object sender, RoutedEventArgs e)
        {
            Model_科目管理 m = new Model_科目管理();
            try
            {
                m = DataGrid_科目设置.SelectedCells[0].Item as Model_科目管理;
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message);
            }
            if (m.科目编号 != null)
            {
                Windows.Win_子细目 w = new Windows.Win_子细目(m.科目编号, m.科目名称);
                w.ShowDialog();
            }
            else
            {
                MessageBoxCommon.Show("请选择科目！");
            }
        }

        private void CheckBox_启用_Click(object sender, RoutedEventArgs e)
        {
            CheckBox b = sender as CheckBox;
            Model_科目管理 m = new Model_科目管理();
            try
            {
                m = DataGrid_科目设置.SelectedCells[0].Item as Model_科目管理;
                m.Used_mark = b.IsChecked == true? 0 : 1;
                new ViewModel_科目管理().UpdateUsedMark(m);
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message);   
            }
        }
        private void CheckBox_借贷方向_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            Model_科目管理 m = new Model_科目管理();
            try
            {
                m = DataGrid_科目设置.SelectedCells[0].Item as Model_科目管理;
                m.借贷标记 = (bool)cb.IsChecked;
                new ViewModel_科目管理().UpdateBorrowMark(m);
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message);
            }
        }

        private void DataGrid_科目设置_Row_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            string header = DataGrid_科目设置.CurrentCell.Column.Header.ToString();
            if (header == "是否启用" || header == "借贷方向")
            {
                return;
            }
            Model_科目管理 m = new Model_科目管理();
            try
            {
                m = DataGrid_科目设置.SelectedCells[0].Item as Model_科目管理;
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message);
            }
            Windows.Win_子细目 w = new Windows.Win_子细目(m.科目编号, m.科目名称);
            w.ShowDialog();
        }

        #endregion
        #region 自定义事件
        private void CloseGrid(object sender, RoutedEventArgs e)
        {
            this.Grid_Pop弹出.Visibility = Visibility.Collapsed;
            FreshData();
        }
        #endregion
        #region Button 用户安全
        private void Button_新增_Click(object sender, RoutedEventArgs e)
        {
            Pop.系统管理.Page_添加用户 p = new Pop.系统管理.Page_添加用户();
            this.Grid_Pop弹出.Visibility = Visibility;
            this.Frame_系统管理_Pop.Content = p;
            p.CloseEvent += new Pop.系统管理.Page_系统管理_CloseEventHandle(CloseGrid);
        }

        private void Button_修改_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid_权限设置.SelectedItem != null)
            {
                Model_用户 m = DataGrid_权限设置.SelectedItem as Model_用户;
                Pop.系统管理.Page_修改用户 p = new Pop.系统管理.Page_修改用户(m.ID);
                this.Grid_Pop弹出.Visibility = Visibility;
                this.Frame_系统管理_Pop.Content = p;
                p.CloseEvent += new Pop.系统管理.Page_系统管理_CloseEventHandle(CloseGrid);
            }
            else
            {
                MessageBoxCommon.Show("请选择需要修改的用户");
            }
        }

        private void Button_停用_Click(object sender, RoutedEventArgs e)
        {

            if (DataGrid_权限设置.SelectedItem != null)
            {
                Model_用户 m = DataGrid_权限设置.SelectedItem as Model_用户;
                if (m.是否使用.Equals("停用"))
                {
                    MessageBoxCommon.Show("当前用户已经停用，请勿重复操作！");
                    return;
                }
                string messageBoxText = "用户停用后，将不能登录！";
                string caption = "注意";
                bool? result = false;
                result = MessageBoxDel.Show(caption, messageBoxText);
                if (result == false)
                {
                    return;
                }
                vm.StopUse(m.ID);
                FreshData();
            }
            else
            {
                MessageBoxCommon.Show("请选择需要停用的用户");
            }
        }

        #endregion
        #region Expander事件
        private void Expander_权限_Expanded(object sender, RoutedEventArgs e)
        {
            FreshData();
            this.Expander_修改密码.IsExpanded = false;
            this.Expander_账套管理.IsExpanded = false;
        }
        /// <summary>
        /// 刷新数据
        /// </summary>
        private void FreshData()
        {
            List<Model_用户> u = new List<Model_用户>();
            u = vm.GetAllUser();
            if (u != null)
            {
                DataGrid_权限设置.ItemsSource = u;
            }
        }

        private void Expander_账套管理_Expanded(object sender, RoutedEventArgs e)
        {
            this.Expander_修改密码.IsExpanded = false;
            this.Expander_权限.IsExpanded = false;
            Model_账套 m = new Model_账套();
            m = vmb.GetData();
            TextBox_账套名称.Text = m.账套名称;
            TextBox_制度.Text = m.会计制度;
            TextBox_启用期间.Text = m.启用期间;
            TextBox_创建时间.Text = m.创建日期字符串;
        }
        
        #endregion

        private void Button_账套修改_Click(object sender, RoutedEventArgs e)
        {
            Model_账套 m = new Model_账套();
            m.账套名称 = TextBox_账套名称.Text.Trim();

            bool flag = vmb.Update(m, 0);
            if (flag)
            {
                MessageBoxCommon.Show("修改账套名称成功,重启程序生效！");
                xw.WriteXML("账套信息", m.账套名称);
            }
            else
            {
                MessageBoxCommon.Show("修改账套名称失败！");
            }    
        }
        #region 行编辑
       
        private void DataGrid_科目设置_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            Model_科目管理 m = new Model_科目管理();
            m = e.Row.Item as Model_科目管理;
            m.Used_mark = m.是否启用 == true ? 1 : 0;
            lm.Add(m);
        }

        #endregion

        private void TabControl_五大科目_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int SelectedIndex = this.TabControl_五大科目.SelectedIndex;
            this.DataGrid_科目设置.ItemsSource = new ViewModel_科目管理().GetSujectData(SelectedIndex+1);
        }


    }
}
