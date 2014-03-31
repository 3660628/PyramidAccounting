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
using PA.Model.CustomEventArgs;
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
        private bool initFlag = false;
        private int judge = 0;
        private List<Model_科目管理> lm= new List<Model_科目管理>();
        private ViewModel_科目管理 vm = new ViewModel_科目管理();

        public Win_子细目(string SubjectNum, string SubjectName)
        {
            InitializeComponent();
            this.SubjectNum = SubjectNum;
            this.SubjectName = SubjectName;
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
            List<Model_科目管理> dataList = new List<Model_科目管理>();
            dataList = vm.GetChildSubjectData(SubjectNum);
            if (dataList.Count == 0)
            {
                Model_科目管理 m = new Model_科目管理();
                m.科目编号 = SubjectNum + "01";
                dataList.Add(m);
                initFlag = true;
            }
            DataGrid_子细目.ItemsSource = dataList;
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
        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            judge = 1;
            DataGrid_子细目.CanUserAddRows = true;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            DataGrid_子细目.CanUserAddRows = false;
            if (lm.Count > 0)
            {
                List<Model_科目管理> temp = new List<Model_科目管理>();
                temp = vm.GetChildSubjectData(SubjectNum);
                foreach (Model_科目管理 m in temp)
                {
                    foreach (Model_科目管理 now in lm)
                    {
                        if (string.IsNullOrEmpty(now.科目名称))
                        {
                            MessageBoxCommon.Show("存在科目名称为空，请检查！");
                            return;
                        }
                        if (m.科目名称.Equals(now.科目名称))
                        {
                            MessageBoxCommon.Show("无法添加数据哦,请检查下是否填写了一样的科目名称！");
                            return;
                        }
                    }
                }
                vm.Insert(lm);
                lm.Clear();
                //刷新数据
                this.FreshData();
            }
        }

        private void Button_Del_Click(object sender, RoutedEventArgs e)
        {
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
            this.FreshData();
        }

        private void DataGrid_子细目_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            Model_科目管理 m = new Model_科目管理();
            m = e.Row.Item as Model_科目管理;
            m.父ID = TextBox_科目编号.Text.ToString();
            if (judge == 1 || initFlag)
            {
                lm.Add(m);
                initFlag = false;
            }
            else
            {
                vm.UpdateChildSubject(m);
            }
        }
        #endregion
    }
}
