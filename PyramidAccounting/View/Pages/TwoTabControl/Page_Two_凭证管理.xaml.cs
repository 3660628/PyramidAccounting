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

namespace PA.View.Pages.TwoTabControl
{
    /// <summary>
    /// Interaction logic for Page_Two_凭证管理.xaml
    /// </summary>
    public partial class Page_Two_凭证管理 : Page
    {
        List<Model_凭证管理> Data_本期凭证 = new List<Model_凭证管理>();
        PA.Helper.XMLHelper.XMLReader xw = new Helper.XMLHelper.XMLReader();
        private ComboBox_Common cbc = new ComboBox_Common();

        public Page_Two_凭证管理()
        {
            InitializeComponent();
            InitData();
            ReflashData();
        }

        private void DoReflashData(object sender, EventArgs e)
        {
            ReflashData();
        }
        private void InitData()
        {
            this.ComboBox_Date.ItemsSource = cbc.GetComboBox_期数();
            this.ComboBox_Date.SelectedIndex = CommonInfo.当前期 - 1;
            this.ComboBox_Review.ItemsSource = cbc.GetComboBox_审核();
            this.ComboBox_Review.SelectedIndex = 0;
            Label_账套名称.Content += "：" + xw.ReadXML("帐套信息");
            Label_操作员.Content += "：" + CommonInfo.用户权限 + "\t" +CommonInfo.真实姓名;
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
            switch (DateSelectIndex)
            {
                case 0:

                    break;
            }
            ReflashData(DateParm + ReviewParm);
        }
        private void ReflashData(string parm)
        {
            Data_本期凭证 = new PA.ViewModel.ViewModel_凭证管理().GetData(parm);
            this.DataGrid_本期凭证.ItemsSource = Data_本期凭证;
        }
        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            PA.View.Windows.Win_记账凭证 win = new PA.View.Windows.Win_记账凭证();
            win.ESubmit += new Windows.Win_记账凭证_Submit(DoReflashData);
            win.ShowDialog();
        }

        private void Button_Review_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataGrid_本期凭证.SelectedCells.Count != 0)
            {
                Model_凭证管理 asd = this.DataGrid_本期凭证.SelectedCells[0].Item as Model_凭证管理;
                new PA.ViewModel.ViewModel_凭证管理().Review(asd.ID);
                ReflashData();
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
                Model_凭证管理 asd = this.DataGrid_本期凭证.SelectedCells[0].Item as Model_凭证管理;
                new PA.ViewModel.ViewModel_凭证管理().Delete(asd.ID);
                ReflashData();
            }
        }

        private void DataGrid_本期凭证_Row_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (this.DataGrid_本期凭证.SelectedCells.Count != 0)
            {
                Model_凭证管理 asd = this.DataGrid_本期凭证.SelectedCells[0].Item as Model_凭证管理;
                Guid guid = asd.ID;
                PA.View.Windows.Win_记账凭证 win = new PA.View.Windows.Win_记账凭证(guid);
                win.ESubmit += new Windows.Win_记账凭证_Submit(DoReflashData);
                win.ShowDialog();
            }
        }
    }
}
