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
using System.Data;
using PA.Helper.DataBase;
using PA.Helper.DataDefind;
using PA.Model.ComboBox;
using PA.ViewModel;

namespace PA.View.Pages.TwoTabControl
{

    /// <summary>
    /// Interaction logic for Page_Two_报表管理.xaml
    /// </summary>
    public partial class Page_Two_报表管理 : Page
    {
        PA.Helper.XMLHelper.XMLReader xr = new Helper.XMLHelper.XMLReader();
        private ComboBox_Common cbc = new ComboBox_Common();
        private ViewModel_ReportManager vmr = new ViewModel_ReportManager();
        private ViewModel_操作日志 vm = new ViewModel_操作日志();

        public Page_Two_报表管理()
        {
            InitializeComponent();
            this.Label_编制单位1.Content += "\t" +xr.ReadXML("公司");   //程序启动后加载当前公司名称

            this.ComboBox_Date.ItemsSource = cbc.GetComboBox_期数(1);
            this.ComboBox_Date.SelectedIndex = CommonInfo.当前期-1;

            this.ComboBox_Date1.ItemsSource = cbc.GetComboBox_期数(1);
            this.ComboBox_Date1.SelectedIndex = CommonInfo.当前期-1;

            this.ComboBox_Date2.ItemsSource = cbc.GetComboBox_期数(1);
            this.ComboBox_Date2.SelectedIndex = CommonInfo.当前期-1;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_生成1_Click(object sender, RoutedEventArgs e)
        {
            Model_操作日志 mr = new Model_操作日志();
            mr = vm.GetTOperateLog();
            mr.日志 = "生成" + ComboBox_Date.Text + "资产负债表" ;
            vm.Insert(mr);

            List<Model_报表类> list = new List<Model_报表类>();
            list = vmr.GetBalanceSheet(ComboBox_Date.SelectedIndex + 1);
            decimal dy = 0;
            decimal dn = 0;
            decimal sumy1 = 0;
            decimal sumn1 = 0;
            decimal sumy2 = 0;
            decimal sumn2 = 0;
            decimal sumy3 = 0;
            decimal sumn3 = 0;
            decimal sumy4 = 0;
            decimal sumn4 = 0;
            decimal sumy5 = 0;
            decimal sumn5 = 0;
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    Label lb = FindName("y" + (i + 1)) as Label;
                    lb.Content = list[i].年初数;
                    Label lb2 = FindName("n" + +(i + 1)) as Label;
                    lb2.Content = list[i].期末数;
                    decimal.TryParse(list[i].年初数, out dy);
                    decimal.TryParse(list[i].期末数, out dn);
                    if (i < 10)
                    {
                        sumy1 += dy;
                        sumn1 += dn;
                    }
                    else if (i >= 10 && i < 16)
                    {
                        sumy2 += dy;
                        sumn2 += dn;
                    }
                    else if (i >= 16 && i < 18)
                    {
                        sumy3 += dy;
                        sumn3 += dn;
                    }
                    else if (i >= 18 && i < 21)
                    {
                        sumy4 += dy;
                        sumn4 += dn;
                    }
                    else
                    {
                        sumy5 += dy;
                        sumn5 += dn;
                    }
                }
                //赋值
                sumY1.Content = sumy1;
                sumN1.Content = sumn1;
                sumY2.Content = sumy2;
                sumN2.Content = sumn2;
                sumY3.Content = sumy3;
                sumN3.Content = sumn3;
                sumY4.Content = sumy4;
                sumN4.Content = sumn4;
                sumY5.Content = sumy5;
                sumN5.Content = sumn5;

                totalY1.Content = sumy1 + sumy5;
                totalN1.Content = sumn1 + sumn5;
                totalY2.Content = sumy2 + sumy3 + sumy4;
                totalN2.Content = sumn2 + sumn3 + sumn4;
                Label_填表人.Content = "填表人：" + CommonInfo.真实姓名 ;
                Label_填表日期.Content = "填表日期：" + DateTime.Now.ToLongDateString();
            }
            else
            {
                for (int i = 0; i < 24; i++)
                {
                    Label lb = FindName("y" + (i + 1)) as Label;
                    lb.Content = "";
                    Label lb1 = FindName("n" + (i + 1)) as Label;
                    lb1.Content = "";
                }
                for (int i = 0; i < 5; i++)
                {
                    Label lb = FindName("sumY" + (i + 1)) as Label;
                    lb.Content = "";
                    Label lb1 = FindName("sumN" + (i + 1)) as Label;
                    lb1.Content = "";
                }
                totalN1.Content = "";
                totalN2.Content = "";
                totalY1.Content = "";
                totalY2.Content = "";
            }
        }

        private void Button_生成2_Click(object sender, RoutedEventArgs e)
        {
            Model_操作日志 mr = new Model_操作日志();
            mr = vm.GetTOperateLog();
            mr.日志 = "生成" + ComboBox_Date.Text + "收入支出总表";
            vm.Insert(mr);

            List<Model_报表类> list = new List<Model_报表类>();
            list = vmr.GetIncomeAndExpenses(ComboBox_Date.SelectedIndex + 1);
            decimal dy = 0;
            decimal dn = 0;
            decimal insumm1 = 0;
            decimal insumy1 = 0;
            decimal insumm2 = 0;
            decimal insumy2 = 0;
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    Label lb = FindName("inM" + (i + 1)) as Label;
                    lb.Content = list[i].期末数;
                    Label lb2 = FindName("inY" + +(i + 1)) as Label;
                    lb2.Content = list[i].年初数;
                    decimal.TryParse(list[i].年初数, out dy);
                    decimal.TryParse(list[i].期末数, out dn);
                    if (i < 3)
                    {
                        insumm1 += dn;
                        insumy1 += dy;
                    }
                    else
                    {
                        insumy2 += dy;
                        insumm2 += dn;
                    }
                    inSumM1.Content = insumm1;
                    inSumY1.Content = insumy1;

                    inSumM2.Content = insumm2;
                    inSumY2.Content = insumy2;
                }
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    Label lb = FindName("inM" + (i + 1)) as Label;
                    lb.Content = "";
                    Label lb2 = FindName("inY" + +(i + 1)) as Label;
                    lb2.Content = "";
                }
                inSumM1.Content ="";
                inSumY1.Content ="";

                inSumM2.Content ="";
                inSumY2.Content = "";
            }
        }
        
    }
}
