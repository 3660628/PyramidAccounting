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
using System.Threading;
using PA.Helper.Tools;
using System.IO;

namespace PA.View.Pages.TwoTabControl
{
    /// <summary>
    /// Interaction logic for Page_Two_系统管理.xaml
    /// </summary>
    public partial class Page_Two_系统管理 : Page
    {
        private string dbfilepath = DBInitialize.dataSource;

        private XMLWriter xw = new XMLWriter();
        private XMLReader xr = new XMLReader();

        //更改备份标识
        private bool alterBackupTag = false;
        //后台备份标识
        private bool backgroundBackupTag = false;

        private Model_操作日志 _mr = new Model_操作日志();

        private List<Model_科目管理> lm = new List<Model_科目管理>();

        private ViewModel_操作日志 vmr = new ViewModel_操作日志();
        private ViewModel_系统管理 vmm = new ViewModel_系统管理();
        private ViewModel_用户 vm = new ViewModel_用户();
        private ViewModel_Books vmb = new ViewModel_Books();

        Register rg = new Register();

        private string DoubleClickSubject = "";
        private string ModifySubjectCellOldValue = "";

        public Page_Two_系统管理()
        {
            InitializeComponent();
            SubscribeToEvent();
            LoadPage();
            VisibilityData();
            _mr = vmr.GetOperateLog();
        }

        private void LoadPage()
        {
            #region 3.数据管理
            string text = xr.ReadXML("自动备份标志");
            is_auto_backup.IsChecked = bool.Parse(xr.ReadXML("自动备份标志"));
            TextBox_备份天数.Text = xr.ReadXML("备份时间");
            backup_filePath.Text = xr.ReadXML("备份路径");
            Recover_filepath.Text = xr.ReadXML("还原路径");
            if (vmm.IsBackupNow())
            {
                backgroundBackupTag = true;
                this.Button_备份_Click(this, null);
            }
            alterBackupTag = true;
            #endregion

            #region 4.操作记录
            this.DatePicker_操作记录.Text = DateTime.Now.ToShortDateString();
            this.DatePicker_操作记录End.Text = DateTime.Now.ToShortDateString();

            #endregion
            #region 5.关于我们
            this.Label_状态.Content = rg.GetVersionMessage();
            this.Laber_Version.Content = "版本V" + Application.ResourceAssembly.GetName().Version.ToString();
            #endregion
        }

        #region 事件订阅
        private void SubscribeToEvent()
        {
            this.Button_Brower.Click += new System.Windows.RoutedEventHandler(Button_Brower_Click);
            PA.View.Windows.Win_子细目.RerflashData += new Windows.Win_子细目_RerflashData(DoRerflashData);
        }
        #endregion
        #region 接受事件后处理
        private void DoRerflashData(object sender, MyEventArgs e)
        {
            new PA.ViewModel.ViewModel_科目管理().UpdateMainSubjectsFee(DoubleClickSubject);
            TabControl_五大科目_SelectionChanged(null, null);
        }
        private void CloseGrid(object sender, RoutedEventArgs e)
        {
            this.Grid_Pop弹出.Visibility = Visibility.Collapsed;
            FreshData();
        }
        #endregion
        #region 自定义事件
        
        #endregion

        #region 1.用户安全
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
                    _mr.日志 = "修改密码";
                    vmr.Insert(_mr);
                    CommonInfo.登录密码 = NewPassword;
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
                bool flag = vm.StopUse(m.ID);
                if (flag)
                {
                    _mr.日志 = "进行停用了用户名：" + m.用户名;
                    vmr.Insert(_mr);
                    FreshData();
                }
                else
                {
                    MessageBoxCommon.Show("操作失败！请联系管理员");
                }
            }
            else
            {
                MessageBoxCommon.Show("请选择需要停用的用户");
            }
        }
        private void Button_账套修改_Click(object sender, RoutedEventArgs e)
        {
            Model_账套 m = new Model_账套();
            m.账套名称 = TextBox_账套名称.Text.Trim();

            bool flag = vmb.UpdateBookName(m, 0);
            if (flag)
            {
                _mr.日志 = "账套名称由" + xr.ReadXML("账套信息") + "修改为" + m.账套名称;
                vmr.Insert(_mr);
                MessageBoxCommon.Show("修改账套名称成功,重启程序生效！");
                xw.WriteXML("账套信息", m.账套名称);
            }
            else
            {
                MessageBoxCommon.Show("修改账套名称失败！");
            }
        }
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
        private void Expander_修改密码_Expanded(object sender, RoutedEventArgs e)
        {
            if(this.IsLoaded)
            {
                this.Expander_权限.IsExpanded = false;
                this.Expander_账套管理.IsExpanded = false;
            }
        }
        #endregion

        #region 2.科目设置
        private void TabControl_五大科目_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int SelectedIndex = this.TabControl_五大科目.SelectedIndex;
            this.DataGrid_科目设置.ItemsSource = new ViewModel_科目管理().GetSujectData(SelectedIndex + 1);
        }
        private void DataGrid_科目设置_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            Model_科目管理 m = new Model_科目管理();
            m = e.Row.Item as Model_科目管理;
            m.Used_mark = m.是否启用 == true ? 1 : 0;
            lm.Add(m);
        }
        /// <summary>
        /// 判断是否已经初始化过年初数据，否则不许修改年初数
        /// </summary>
        private void VisibilityData()
        {
            if (CommonInfo.权限值 != 4)
            {
                Button_解密.Visibility = Visibility.Collapsed;
            }
            if (CommonInfo.权限值 < 3)
            {
                Expander_权限.Visibility = Visibility.Hidden;
            }
            if (CommonInfo.权限值 < 2)
            {
                Expander_账套管理.Visibility = Visibility.Hidden;
            }
            if (CommonInfo.SoftwareState == (int)ENUM.EM_SOFTWARESTATE.已注册)
            {
                Grid_Register.Visibility = Visibility.Collapsed;
            }
        }
        private void Button_科目保存_Click(object sender, RoutedEventArgs e)
        {
            if (CommonInfo.权限值 < 2)
            {
                MessageBoxCommon.Show("需要会计主管确认后再进行保存！");
                return;
            }
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
                _mr.日志 = "保存了年初金额初始化！";
                vmr.Insert(_mr);
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
                Windows.Win_子细目 w = new Windows.Win_子细目(m.科目编号, m.科目名称, m.借贷标记);
                w.ShowDialog();
            }
            else
            {
                MessageBoxCommon.Show("请选择科目！");
            }
        }
        private void CheckBox_启用_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid_科目设置.SelectedCells.Count > 0)
            {
                CheckBox b = sender as CheckBox;
                Model_科目管理 m = new Model_科目管理();
                m = DataGrid_科目设置.SelectedCells[0].Item as Model_科目管理;
                m.Used_mark = b.IsChecked == true ? 0 : 1;
                new ViewModel_科目管理().UpdateUsedMark(m);
            }
        }
        private void CheckBox_借贷方向_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid_科目设置.SelectedCells.Count > 0)
            {
                CheckBox cb = sender as CheckBox;
                Model_科目管理 m = new Model_科目管理();
                m = DataGrid_科目设置.SelectedCells[0].Item as Model_科目管理;
                m.借贷标记 = (bool)cb.IsChecked;
                new ViewModel_科目管理().UpdateBorrowMark(m);
            }
        }
        private void DataGrid_科目设置_Row_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            string header = DataGrid_科目设置.CurrentCell.Column.Header.ToString();
            if (header == "是否启用" || header == "借贷方向" || header == "年初数")
            {
                return;
            }
            if (DataGrid_科目设置.SelectedCells.Count > 0)
            {
                Model_科目管理 m = new Model_科目管理();
                m = DataGrid_科目设置.SelectedCells[0].Item as Model_科目管理;
                DoubleClickSubject = m.科目编号;
                Windows.Win_子细目 w = new Windows.Win_子细目(m.科目编号, m.科目名称, m.借贷标记);
                w.ShowDialog();
            }
        }
        private void DataGrid_科目设置_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            ModifySubjectCellOldValue = (e.Column.GetCellContent(e.Row) as TextBlock).Text;
        }
        private void DataGrid_科目设置_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            string newValue = (e.EditingElement as TextBox).Text.Trim();
            if (ModifySubjectCellOldValue == newValue)
            {
                return;
            }
            Model_科目管理 detail = this.DataGrid_科目设置.SelectedCells[0].Item as Model_科目管理;
            string header = e.Column.Header.ToString();
            if(header == "年初数")
            {
                new PA.ViewModel.ViewModel_科目管理().UpdateMainSubjectsFeeManual(detail.科目编号, newValue);
            }
        }
        #endregion

        #region 3.数据管理
        private string defaultfilePath = "";
        private string fname = string.Empty;
        /// <summary>
        /// 显示自动备份开启状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void is_auto_backup_Checked(object sender, RoutedEventArgs e)
        {
            this.is_auto_mark.Text = "开";
            if (alterBackupTag)
            {
                vmm.UpdateAutoBackTag(TextBox_备份天数.Text);
            }
        }
        /// <summary>
        /// 显示自动备份关闭状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void is_auto_backup_Unchecked(object sender, RoutedEventArgs e)
        {
            this.is_auto_mark.Text = "关";
            vmm.DeleteAutoBackTag();
        }
        /// <summary>
        /// 浏览备份文件位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Brower_Click(object sender, RoutedEventArgs e)
        {
            Thread app = new Thread(new ThreadStart(openFile));
            app.SetApartmentState(ApartmentState.STA);
            app.Start();
            app.Join();
            this.Recover_filepath.Text = fname;
        }
        /// <summary>
        /// 打开文件
        /// </summary>
        private void openFile()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Title = "请选择需要恢复的数据文件";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            openFileDialog.Filter = "备份文件(*.bak)|*.bak|所有文件(*.*)|*.*";
            openFileDialog.DefaultExt = "bak";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fname = openFileDialog.FileName;
            }
        }
        /// <summary>
        /// 选择备份目录按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            if (defaultfilePath != "")
            {
                folderBrowserDialog1.SelectedPath = defaultfilePath;
            }
            folderBrowserDialog1.Description = "请选择数据库备份存放的文件夹";
            folderBrowserDialog1.ShowNewFolderButton = true;
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
            System.Windows.Forms.DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                defaultfilePath = folderBrowserDialog1.SelectedPath;
                string folderName = folderBrowserDialog1.SelectedPath;
                if (folderName != "")
                {
                    this.backup_filePath.Text = folderName;
                }
            }
        }
        /// <summary>
        /// 备份功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_备份_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string folderpath = this.backup_filePath.Text;
                string newfilepath = folderpath + "\\PyramidAccounting" + DateTime.Now.ToString("yyyyMMdd") + ".bak";
                if (!System.IO.Directory.Exists(folderpath))
                {
                    System.IO.Directory.CreateDirectory(folderpath);
                }
                System.IO.File.Copy(dbfilepath, newfilepath, true);     //做数据库文件复制

                vmm.UpdateAutoBackTag(TextBox_备份天数.Text);
                if (!backgroundBackupTag)
                {
                    MessageBoxCommon.Show("数据备份操作成功！");
                }
                _mr.日志 = "进行备份操作，备份路径为：" + newfilepath;
                vmr.Insert(_mr);
            }
            catch (Exception ee)
            {
                Log.Write(ee.Message);
            }
            
        }
        /// <summary>
        /// 输入空格无效
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_备份天数_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
        /// <summary>
        /// 只允许输入数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_备份天数_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!new PA.Helper.Tools.Util().IsNumber(e.Text))
            {
                e.Handled = true;
            }
            else
                e.Handled = false;
        }

        private void Button_Recover_Click(object sender, RoutedEventArgs e)
        {
            string recover_path = Recover_filepath.Text.ToString();
            if (!System.IO.File.Exists(recover_path))
            {
                MessageBoxCommon.Show("当前路径找不到数据文件，请检查路径！", "不好意思");
            }
            else if (!string.IsNullOrEmpty(recover_path))
            {
                string lastname = recover_path.Substring(recover_path.LastIndexOf(".") + 1, (recover_path.Length - recover_path.LastIndexOf(".") - 1));
                if (lastname.Equals("db") || lastname.Equals("bak"))
                {
                    string newDBname = System.IO.Path.GetFileNameWithoutExtension(recover_path) + ".db";
                    string newpath = "Data\\" + newDBname;
                    System.IO.File.Copy(recover_path, newpath, true);  //复制回原来的目录
                    xw.WriteXML("数据库", newDBname);
                    MessageBoxCommon.Show("成功", "恢复数据成功，重启软件生效哦！");
                    _mr.日志 = "进行了恢复数据";
                    vmr.Insert(_mr);
                }
                else
                {
                    MessageBoxCommon.Show("选择的数据库文件格式不正确！");
                }
            }
        }
       

        private void Button_save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                xw.WriteXML("自动备份标志",is_auto_backup.IsChecked.ToString());
                xw.WriteXML("备份时间",TextBox_备份天数.Text);
                xw.WriteXML("备份路径", backup_filePath.Text);
                xw.WriteXML("还原路径", Recover_filepath.Text);
                _mr.日志 = "保存当前配置信息！";
                vmr.Insert(_mr);
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message);
            }
            MessageBoxCommon.Show("设置成功！");
        }
        #endregion

        #region 4.操作记录

        private void Button_操作记录查询_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(DatePicker_操作记录.Text) || string.IsNullOrEmpty(DatePicker_操作记录End.Text))
            {
                MessageBoxCommon.Show("请选择查询时间的范围！");
                return;
            }
            string dateStart = Convert.ToDateTime(DatePicker_操作记录.Text).ToString("yyyy-MM-dd HH:mm:ss");
            string dateEnd = Convert.ToDateTime(DatePicker_操作记录End.Text).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");

            ViewModel_操作日志 v = new ViewModel_操作日志();
            this.DataGrid_操作记录.ItemsSource = v.GetData(dateStart, dateEnd);
        }
        #endregion

        #region 5.关于我们

        private void Button_注册_Click(object sender, RoutedEventArgs e)
        {
            string registerCode = TextBox_注册.Text.Trim().ToUpper();
            UsbController usb = new Helper.Tools.UsbController();
            string date = DateTime.Now.ToString("yyyyMMdd");
            string orginCode = "StoneAnt.PA" + date + usb.getSerialNumberFromDriveLetter();
            string validateCode = Secure.GetMD5_32(orginCode).ToUpper();
            if (registerCode.Equals(validateCode))
            {
                rg.UpdateSoftwareVersionStatus((int)ENUM.EM_SOFTWARESTATE.已注册);
                rg.UpdateSoftwareRegisterCode(registerCode);
                //注册后加密数据库
                DBInitialize.ChangeDBPassword();
                xw.WriteXML("注册", "true");

                MessageBoxCommon.Show("注册成功！");
                this.LoadPage();
                Grid_Register.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBoxCommon.Show("注册码不准确，请联系开发商！");
            }
        }

        #endregion
        #region 拷贝数据库
        private void Button_解密_Click(object sender, RoutedEventArgs e)
        {
            DBInitialize.ClearPassword();
            File.Copy(DBInitialize.dataSource, "Data\\DataWithoutPassword.db", true);
            new XMLWriter().WriteXML("注册", "false");
            DBInitialize.ChangeDBPassword();
            new XMLWriter().WriteXML("注册", "true");
            Log.Write("成功拷贝机密文件");
            MessageBoxCommon.Show("拷贝成功!");    
        }
        #endregion
    }
}
