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
            //if (dataList.Count == 0)
            //{
            //    Model_科目管理 m = new Model_科目管理();
            //    m.科目编号 = SubjectNum + "01";
            //    dataList.Add(m);
            //    initFlag = true;
            //}
            //else
            {
                foreach (Model_科目管理 detail in dataList)
                {
                    if (detail.类别 != "1000")
                    {
                        ComboBox_New_ParentsID.Add(detail.科目编号);
                    }
                }
            }
            DataGrid_子细目.ItemsSource = dataList;
            this.ComboBox_New_父ID.ItemsSource = ComboBox_New_ParentsID;
            this.ComboBox_New_父ID.SelectedIndex = 0;
            this.TextBox_New_子细目编号.Text = this.ComboBox_New_父ID.Text;
        }
        /// <summary>
        /// 刷新数据的方法
        /// </summary>
        private void FreshData()
        {
            DataGrid_子细目.ItemsSource = vm.GetChildSubjectData(SubjectNum);
        }
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            OnRerflashData();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        #region Button事件

        //private void Button_Save_Click(object sender, RoutedEventArgs e)
        //{
        //    DataGrid_子细目.CanUserAddRows = false;
        //    if (lm.Count > 0)
        //    {
        //        List<Model_科目管理> temp = new List<Model_科目管理>();
        //        temp = vm.GetChildSubjectData(SubjectNum);
        //        foreach (Model_科目管理 m in temp)
        //        {
        //            foreach (Model_科目管理 now in lm)
        //            {
        //                if (string.IsNullOrEmpty(now.科目名称))
        //                {
        //                    MessageBoxCommon.Show("存在科目名称为空，请检查！");
        //                    return;
        //                }
        //                if (m.科目名称.Equals(now.科目名称))
        //                {
        //                    MessageBoxCommon.Show("无法添加数据哦,请检查下是否填写了一样的科目名称！");
        //                    return;
        //                }
        //                if (m.科目编号.Equals(now.科目编号))
        //                {
        //                    MessageBoxCommon.Show("无法添加数据哦,请检查下是否填写了一样的科目编号！");
        //                    return;
        //                }
        //            }
        //        }
        //        bool flag = vm.Insert(lm);
        //        if (flag)
        //        {
        //            MessageBoxCommon.Show("保存成功！");
        //            lm.Clear();
        //            //刷新数据
        //            this.FreshData();
        //        }
                
        //    }
        //}

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
                    m.父ID = SubjectNum;
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

        //private void DataGrid_子细目_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        //{
        //    Model_科目管理 m = new Model_科目管理();
        //    m = e.Row.Item as Model_科目管理;
        //    m.父ID = SubjectNum;
        //    if (judge == 1 || initFlag)
        //    {
        //        m.类别 = "100";
        //        lm.Add(m);
        //        initFlag = false;
        //    }
        //    else
        //    {
        //        vm.UpdateChildSubject(m);
        //    }
        //}
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
            if (string.IsNullOrEmpty(Number) || string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Money))
            {
                MessageBoxCommon.Show("数据不能为空");
                return;
            }
            string ParentsID = this.ComboBox_New_父ID.Text;
            List<Model_科目管理> details = new List<Model_科目管理>();
            Model_科目管理 detail = new Model_科目管理();
            detail.科目编号 = Number;
            detail.科目名称 = Name;
            detail.年初金额 = Money;
            detail.类别 = (this.ComboBox_New_父ID.SelectedIndex==0)?"100":"1000";
            detail.父ID = ParentsID;
            detail.借贷标记 = BorrowMark;
            details.Add(detail);
            bool flag = vm.Insert(details);
            if (flag)
            {
                details.Clear();
                check();
            }
            else
            {
                MessageBoxCommon.Show("添加不成功！");
            }
        }
        /// <summary>
        /// Lugia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_子细目_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Model_科目管理 detail = this.DataGrid_子细目.SelectedCells[0].Item as Model_科目管理;
            string header = e.Column.Header.ToString();
            string newValue = (e.EditingElement as TextBox).Text.Trim();
            if (header == "年初数")
            {
                vm.UpdateChildSubject(detail.科目编号.ToString(), header, newValue);
            }
            else
            {
                vm.UpdateChildSubject(detail.ID.ToString(), header, newValue);
            }
        }

        private void ComboBox_New_父ID_DropDownClosed(object sender, EventArgs e)
        {
            this.TextBox_New_子细目编号.Text = this.ComboBox_New_父ID.Text;
        }
    }
}
