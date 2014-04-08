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
using PA.Model.DataGrid;
using PA.Helper.DataDefind;
using PA.Model.ComboBox;
using PA.View.ResourceDictionarys.MessageBox;
using PA.Model.CustomEventArgs;

namespace PA.View.Pages.TwoTabControl
{
    public partial class Page_Two_凭证管理 : Page
    {
        List<Model_凭证管理> Data_本期凭证 = new List<Model_凭证管理>();
        PA.Helper.XMLHelper.XMLReader xr = new Helper.XMLHelper.XMLReader();
        private ComboBox_Common cbc = new ComboBox_Common();

        public Page_Two_凭证管理()
        {
            InitializeComponent();
            this.StackPanel_MoreButton.Visibility = System.Windows.Visibility.Collapsed;
            InitData();
            ReflashData();
            SubscribeToEvent();
        }

        #region 订阅事件
        private void SubscribeToEvent()
        {
            PA.View.Windows.Win_记账凭证.ESubmit += new Windows.Win_记账凭证_Submit(DoReflashData);
            PA.View.Pages.TwoTabControl.Page_Two_快捷界面.TabChange += new Page_Two_快捷界面_TabChange(DoTabChange);
        }
        #endregion
        #region 接收后处理
        private void DoReflashData(object sender, MyEventArgs e)
        {
            ReflashData();
        }
        private void DoTabChange(object sender, MyEventArgs e)
        {
            if(e.Y == 1)//x always = 0
            {
                if(e.操作类型 == "凭证审核")
                {
                    this.ComboBox_Review.SelectedIndex = 2;
                    this.ComboBox_Date.SelectedIndex = CommonInfo.当前期;
                }
                else if (e.操作类型 == "本月结账")
                {
                    InitData();
                }
                ReflashData();
            }
        }
        #endregion

        private void InitData()
        {
            this.ComboBox_Date.ItemsSource = cbc.GetComboBox_期数(0);
            this.ComboBox_Date.SelectedIndex = CommonInfo.当前期;
            this.ComboBox_Review.ItemsSource = cbc.GetComboBox_审核();
            this.ComboBox_Review.SelectedIndex = 0;
            Label_账套名称.Content = "当前帐套名称：" + xr.ReadXML("账套信息");
            Label_操作员.Content = "操作员：" + CommonInfo.用户权限 + "\t" +CommonInfo.真实姓名;
            Label_当前期数.Content = "当前期数：" + ComboBox_Date.SelectedValue.ToString();
        }
        private void ReflashData()
        {
            int DateSelectIndex = this.ComboBox_Date.SelectedIndex;
            string DateParm = "";
            int ReviewSelectIndex = this.ComboBox_Review.SelectedIndex;
            string ReviewParm = "";
            switch (ReviewSelectIndex)
            {
                case 1:
                    ReviewParm = " and REVIEW_MARK=1";
                    break;
                case 2:
                    ReviewParm = " and REVIEW_MARK=0";
                    break;
            }
            if (DateSelectIndex != 0)
            {
                DateParm = " and PERIOD=" + DateSelectIndex;
            }
            Data_本期凭证 = new PA.ViewModel.ViewModel_凭证管理().GetData(DateParm + ReviewParm);
            this.DataGrid_本期凭证.ItemsSource = Data_本期凭证;
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            if (CommonInfo.是否初始化年初数)
            {
                PA.View.Windows.Win_记账凭证 win = new PA.View.Windows.Win_记账凭证();
                win.ShowDialog();
            }
            else
            {
                MessageBoxCommon.Show("当前未初始化科目年初数，请设置！");
            }
        }

        private void Button_Review_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataGrid_本期凭证.SelectedCells.Count != 0)
            {
                if (CommonInfo.权限值 >= 2)
                {
                    Model_凭证管理 asd = this.DataGrid_本期凭证.SelectedCells[0].Item as Model_凭证管理;
                    new PA.ViewModel.ViewModel_凭证管理().Review(asd.ID);
                    ReflashData();
                }
                else
                {
                    MessageBoxCommon.Show("您的权限不够，不能进行审核！");
                }
            }
        }

        private void ComboBox_Review_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReflashData();
        }

        private void Button_Del_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataGrid_本期凭证.SelectedCells.Count != 0)
            {
                bool? result = MessageBoxDel.Show("注意", "确认删除数据？");
                if (result == true)
                {
                    Model_凭证管理 asd = this.DataGrid_本期凭证.SelectedCells[0].Item as Model_凭证管理;
                    new PA.ViewModel.ViewModel_凭证管理().Delete(asd.ID);
                    ReflashData();
                }
            }
        }

        private void DataGrid_本期凭证_Row_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (this.DataGrid_本期凭证.SelectedCells.Count != 0)
            {
                Model_凭证管理 asd = this.DataGrid_本期凭证.SelectedCells[0].Item as Model_凭证管理;
                Guid guid = asd.ID;
                PA.View.Windows.Win_记账凭证 win = new PA.View.Windows.Win_记账凭证(guid);
                win.ShowDialog();
            }
        }

        private void Button_More_Click(object sender, RoutedEventArgs e)
        {
            if(this.StackPanel_MoreButton.Visibility == System.Windows.Visibility.Visible)
            {
                this.StackPanel_MoreButton.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.StackPanel_MoreButton.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Button_打印_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataGrid_本期凭证.SelectedCells.Count != 0)
            {
                Model_凭证管理 asd = this.DataGrid_本期凭证.SelectedCells[0].Item as Model_凭证管理;
                new PA.Helper.ExcelHelper.ExcelWriter().ExportVouchers(asd.ID);
                this.StackPanel_MoreButton.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void ComboBox_Date_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReflashData();
        }
    }
}
