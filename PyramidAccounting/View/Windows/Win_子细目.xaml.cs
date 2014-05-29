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
using PA.Model.DataGrid;
using PA.Model.CustomEventArgs;
using PA.ViewModel;
using PA.View.ResourceDictionarys.MessageBox;

namespace PA.View.Windows
{
    public delegate void Win_子细目_RerflashData(object sender, MyEventArgs e);
    /// <summary>
    /// Interaction logic for Win_子细目.xaml
    /// </summary>
    public partial class Win_子细目 : Window
    {
        public static event Win_子细目_RerflashData RerflashData;
        private string SubjectNum = string.Empty;
        private string SubjectName = string.Empty;
        private bool BorrowMark = true;
        //private bool initFlag = false;
        //private int judge = 0;
        private List<Model_科目管理> lm= new List<Model_科目管理>();
        private ViewModel_科目管理 vm = new ViewModel_科目管理();
        private string preDataGridValue;

        private string ComboboxText = "";

        public Win_子细目(string SubjectNum, string SubjectName, bool BorrowMark)
        {
            InitializeComponent();
            this.SubjectNum = SubjectNum;
            this.SubjectName = SubjectName;
            this.BorrowMark = BorrowMark;
            this.TextBox_科目编号.Text = this.SubjectNum;
            this.TextBox_科目名称.Text = this.SubjectName;
            check();
        }

        #region 自定义事件
        private void OnRerflashData()
        {
            if(RerflashData != null)
            {
                MyEventArgs e = new Model.CustomEventArgs.MyEventArgs();
                RerflashData(this, e);
            }
        }
        #endregion
        private void check()
        {
            List<string> ComboBox_New_ParentsID = new List<string>();
            ComboBox_New_ParentsID.Add(SubjectNum);
            
            List<Model_科目管理> dataList = new List<Model_科目管理>();
            dataList = vm.GetChildSubjectData(SubjectNum);
                foreach (Model_科目管理 detail in dataList)
                {
                    if (detail.类别 != "10000")
                    {
                        ComboBox_New_ParentsID.Add(detail.科目编号);
                    }
                }
            DataGrid_子细目.ItemsSource = dataList;
            this.ComboBox_New_父ID.ItemsSource = ComboBox_New_ParentsID;
            if (ComboboxText == "")
            {
                this.ComboBox_New_父ID.SelectedIndex = 0;
            }
            else
            {
                this.ComboBox_New_父ID.SelectedValue = ComboboxText;
            }
            this.TextBox_New_子细目编号.Text = vm.GetMaxSubjectID(this.ComboBox_New_父ID.Text);
        }
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid_子细目.Items.Count != 0)
            {
                OnRerflashData();
            }
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        #region Button事件

        private void Button_Del_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid_子细目.SelectedItems.Count == 0)
            {
                return;
            }
            string messageBoxText = "是否删除这些数据？";
            string caption = "注意";
            bool? result = MessageBoxDel.Show(caption, messageBoxText);
            if (result == false)
            {
                return;
            }
            List<Model_科目管理> list = new List<Model_科目管理>();
            for (int i = 0; i < DataGrid_子细目.SelectedItems.Count; i++)
            {
                Model_科目管理 m = new Model_科目管理();
                try
                {
                    m = DataGrid_子细目.SelectedItems[i] as Model_科目管理;
                    //m.父ID = SubjectNum;
                }
                catch (Exception)
                {
                    
                }
                list.Add(m);
            }
            vm.Delete(list);
            list.Clear();
            check();
        }

        #endregion
        /// <summary>
        /// Lugia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_New_Add子细目_Click(object sender, RoutedEventArgs e)
        {
            string Number = this.TextBox_New_子细目编号.Text;
            string Name   = this.TextBox_New_子细目名称.Text;
            string Money  = this.TextBox_New_年初数.Text;
            if (string.IsNullOrEmpty(Number) || string.IsNullOrEmpty(Name))
            {
                MessageBoxCommon.Show("数据不能为空");
                return;
            }
            if (string.IsNullOrEmpty(Money))
            {
                Money = "0";
            }
            string ParentsID = this.ComboBox_New_父ID.Text;
            List<Model_科目管理> details = new List<Model_科目管理>();
            Model_科目管理 detail = new Model_科目管理();
            detail.科目编号 = Number;
            detail.科目名称 = Name;
            detail.年初金额 = Money;
            if (this.ComboBox_New_父ID.Text.Length <= 3)
            {
                detail.类别 = "100";
            }
            else if (this.ComboBox_New_父ID.Text.Length <= 5)
            {
                detail.类别 = "1000";
            }
            else
            {
                detail.类别 = "10000";
            }
            detail.父ID = ParentsID;
            detail.借贷标记 = BorrowMark;
            details.Add(detail);
            bool flag = vm.Insert(details);
            if (flag)
            {
                this.TextBox_New_子细目名称.Text = "";
                this.TextBox_New_年初数.Text = "";
                check();
            }
            else
            {
                MessageBoxCommon.Show("添加不成功,请检查编号是否唯一！");
            }

        }
        /// <summary>
        /// Lugia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_子细目_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            string newValue = (e.EditingElement as TextBox).Text.Trim();
            if(newValue == preDataGridValue)
            {
                return;
            }
            Model_科目管理 detail = this.DataGrid_子细目.SelectedCells[0].Item as Model_科目管理;
            string header = e.Column.Header.ToString();
            
            if (header == "年初数")
            {
                vm.UpdateChildSubject(detail.科目编号.ToString(), header, newValue);
            }
            else
            {
                vm.UpdateChildSubject(detail.ID.ToString(), header, newValue);
            }
            check();
        }

        private void ComboBox_New_父ID_DropDownClosed(object sender, EventArgs e)
        {
            this.TextBox_New_子细目编号.Text = this.ComboBox_New_父ID.Text;
            ComboboxText = this.ComboBox_New_父ID.Text;
        }

        private void DataGrid_子细目_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            preDataGridValue = (e.Column.GetCellContent(e.Row) as TextBlock).Text;
        }
    }
}
