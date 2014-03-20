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

namespace PA.View.Pages.TwoTabControl
{
    /// <summary>
    /// Interaction logic for Page_Two_凭证管理.xaml
    /// </summary>
    public partial class Page_Two_凭证管理 : Page
    {
        List<Model_凭证管理> Data_本期凭证 = new List<Model_凭证管理>();
        PA.Helper.XMLHelper.XMLReader xw = new Helper.XMLHelper.XMLReader();
        public Page_Two_凭证管理()
        {
            InitializeComponent();
            InitData();
            ReflashData();
        }
        #region custom event

        private void DoReflashData(object sender, EventArgs e)
        {
            ReflashData();
        }


        #endregion




        private void InitData()
        {
            this.ComboBox_Review.ItemsSource = new PA.Model.ComboBox.ComboBox_Common().GetComboBox_审核();
            this.ComboBox_Review.SelectedIndex = 0;
            Label_账套名称.Content += "\t" + xw.ReadXML("帐套信息");
            Label_操作员.Content += "\t" + CommonInfo.用户权限 + "\t" +CommonInfo.真实姓名;
        }
        private void ReflashData()
        {
            Data_本期凭证 = new PA.ViewModel.ViewModel_凭证管理().GetData();
            this.DataGrid_本期凭证.ItemsSource = Data_本期凭证;
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            PA.View.Windows.Win_记账凭证 win = new PA.View.Windows.Win_记账凭证();
            win.ESubmit += new Windows.Win_记账凭证_Submit(DoReflashData);
            win.ShowDialog();
        }

    }
}
