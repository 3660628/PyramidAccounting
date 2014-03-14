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

namespace PA.View.Pages.TwoTabControl
{
    /// <summary>
    /// Interaction logic for Page_Two_系统管理.xaml
    /// </summary>
    public partial class Page_Two_系统管理 : Page
    {
        private int i = 1;
        public Page_Two_系统管理()
        {
            InitializeComponent();
            VisibilityButtonSubject();
        }
        #region 科目管理
        /// <summary>
        /// 判断是否已经初始化过年初数据，否则不许修改年初数
        /// </summary>
        private void VisibilityButtonSubject()
        {
            if (new ViewModel_科目管理().CheckSaved())
            {
                this.DataGridTextColumn_fee.IsReadOnly = true;
                this.DataGridTextColumn_mark.IsReadOnly = true;
                this.Button_科目保存.Visibility = Visibility.Hidden;
            }
        }
        private List<Model_科目管理> lm = new List<Model_科目管理>();
        private void Button_资产_Click(object sender, RoutedEventArgs e)
        {
            i = 1;
            this.DataGrid_科目设置.ItemsSource = new ViewModel_科目管理().GetData(i);
        }

        private void Button_负债_Click(object sender, RoutedEventArgs e)
        {
            i = 2;
            this.DataGrid_科目设置.ItemsSource = new ViewModel_科目管理().GetData(i);
        }

        private void Button_净资产_Click(object sender, RoutedEventArgs e)
        {
            i = 3;
            this.DataGrid_科目设置.ItemsSource = new ViewModel_科目管理().GetData(i);
        }

        private void Button_收入_Click(object sender, RoutedEventArgs e)
        {
            i = 4;
            this.DataGrid_科目设置.ItemsSource = new ViewModel_科目管理().GetData(i);
        }

        private void Button_支出_Click(object sender, RoutedEventArgs e)
        {
            i = 5;
            this.DataGrid_科目设置.ItemsSource = new ViewModel_科目管理().GetData(i);
        }

        private void Button_科目保存_Click(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "年初金额初始化不能修改哦，请确认是否填写完整？";
            string caption = "注意";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    break;
                case MessageBoxResult.No:
                    return;
            }
            new ViewModel_科目管理().Update(lm);
            //刷新操作
            Button btn = sender as Button;
            btn.Visibility = Visibility.Hidden;
            this.DataGrid_科目设置.ItemsSource = new ViewModel_科目管理().GetData(i);
        }

        private void DataGrid_科目设置_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            Model_科目管理 m = new Model_科目管理();
            m = e.Row.Item as Model_科目管理;
            m.Used_mark = m.是否启用 == true ? 0 : 1;
            lm.Add(m);
        }

        private void Button_编辑子细目_Click(object sender, RoutedEventArgs e)
        {
            Windows.Win_子细目 w = new Windows.Win_子细目();
            w.ShowDialog();
        }
        #endregion

    }
}
