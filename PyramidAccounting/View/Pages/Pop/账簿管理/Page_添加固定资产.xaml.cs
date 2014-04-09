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
using PA.View.ResourceDictionarys.MessageBox;
using PA.Model.DataGrid;

namespace PA.View.Pages.Pop.账簿管理
{
    public delegate void Page_添加固定资产_CommitEventHandle(object sender, MyEventArgs e);
    /// <summary>
    /// Interaction logic for Page_添加固定资产.xaml
    /// </summary>
    public partial class Page_添加固定资产 : Page
    {
        public static event Page_添加固定资产_CommitEventHandle ECommit;
        private ViewModel.ViewModel_固定资产 vm = new ViewModel.ViewModel_固定资产();
        Model_操作日志 _mr = new Model_操作日志();
        private ViewModel.ViewModel_操作日志 vmr = new ViewModel.ViewModel_操作日志();

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
            if (!Validate())
                return;
            Model_固定资产 model = SetDate();
            List<Model_固定资产> lm = new List<Model_固定资产>();
            lm.Add(model);
            bool flag = vm.Insert(lm);
            if (flag)
            {
                _mr.日志 = "录入固定资产：" + model.名称及规格;
                vmr.Insert(_mr);
                OnCommit(true);
            }
            else
            {
                MessageBoxCommon.Show("添加固定资产失败，请再添加");
            }
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

        private bool Validate()
        {
            string 编号 = TextBox_编号.Text.Trim();
            if (string.IsNullOrEmpty(编号))
            {
                MessageBoxCommon.Show("请填写编号");
                return false;
            }
            else if(vm.ValidateIndex(TextBox_编号.Text.Trim()))
            {
                MessageBoxCommon.Show("编号已存在");
                return false;
            }

            return true;
        }

        private Model_固定资产 SetDate()
        {
            Model_固定资产 model = new Model_固定资产();
            model.编号 = TextBox_编号.Text.Trim();
            model.名称及规格 = TextBox_名称.Text.Trim();
            model.单位 = TextBox_单位.Text.Trim();
            model.数量 = TextBox_数量.Text.Trim();
            model.价格 = TextBox_原价.Text.Trim();
            model.购置日期 = (DateTime)DatePicker_置购日期.SelectedDate;
            model.使用部门 = TextBox_部门.Text.Trim();
            model.报废日期 = (DateTime)DatePicker_清理日期.SelectedDate;
            model.凭证编号 = TextBox_凭证编号.Text.Trim();
            model.备注 = TextBox_备注.Text.Trim();
            return model;
        }
    }
}
