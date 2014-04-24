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
using PA.Model.CustomEventArgs;
using PA.View.ResourceDictionarys.MessageBox;

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
        private Model_操作日志 mr = new Model_操作日志();
        public Page_Two_报表管理()
        {
            InitializeComponent();
            SubscribeToEvent();
            this.Label_编制单位1.Content += "\t" +xr.ReadXML("公司");   //程序启动后加载当前公司名称
            mr = vm.GetOperateLog();
            FreshComboBox();
        }

        /// <summary>
        /// 刷新日期下拉
        /// </summary>
        private void FreshComboBox()
        {
            this.ComboBox_Date.ItemsSource = cbc.GetComboBox_期数(1);
            

            this.ComboBox_Date1.ItemsSource = cbc.GetComboBox_期数(1);

            this.ComboBox_Date2.ItemsSource = cbc.GetComboBox_期数(1);
            if (CommonInfo.当前期 == 1)
            {
                this.ComboBox_Date.SelectedIndex = CommonInfo.当前期 - 1;
                this.ComboBox_Date1.SelectedIndex = CommonInfo.当前期 - 1;
                this.ComboBox_Date2.SelectedIndex = CommonInfo.当前期 - 1;
            }
            else
            {
                this.ComboBox_Date.SelectedIndex = CommonInfo.当前期 - 2;
                this.ComboBox_Date1.SelectedIndex = CommonInfo.当前期 - 2;
                this.ComboBox_Date2.SelectedIndex = CommonInfo.当前期 - 2;
            }
            Label_账套名称.Content = "当前帐套名称：" + CommonInfo.账套信息;
            Label_操作员.Content = "操作员：" + CommonInfo.用户权限 + "\t" + CommonInfo.真实姓名;
            Label_当前期数.Content = "当前期数：第" + CommonInfo.当前期 + "期";
        }
        #region 事件订阅
        private void SubscribeToEvent()
        {
            PA.View.Pages.TwoTabControl.Page_Two_快捷界面.TabChange += DoTabChange;
        }
        #endregion
        #region 接受事件后处理
        private void DoTabChange(object sender, MyEventArgs e)
        {
            if (e.操作类型 == "本月结账")
            {
                FreshComboBox();
            }
        }
        #endregion

        private void Button_生成1_Click(object sender, RoutedEventArgs e)
        {
            int value = ComboBox_Date.SelectedIndex;

            if (value == CommonInfo.当前期 - 1)
            {
                MessageBoxCommon.Show("结账后方可生成报表!");
                return;
            }
            mr.日志 = "生成" + ComboBox_Date.Text + "资产负债表" ;
            vm.Insert(mr);

            List<Model_报表类> list = new List<Model_报表类>();
            list = vmr.GetBalanceSheet(value + 1);
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

        private List<Model_报表类> lastList_fee1 = new List<Model_报表类>();
        private List<Model_报表类> lastList_fee2 = new List<Model_报表类>();
        private void Button_生成2_Click(object sender, RoutedEventArgs e)
        {
            int value = ComboBox_Date1.SelectedIndex;

            if (value == CommonInfo.当前期 - 1)
            {
                MessageBoxCommon.Show("结账后方可生成报表!");
                return;
            }
            mr.日志 = "生成" + ComboBox_Date.Text + "收入支出总表";
            vm.Insert(mr);

            //清除上一次赋值的值
            foreach (Model_报表类 m in lastList_fee1)
            {
                Label lb = FindName("inM" + m.编号) as Label;
                lb.Content = "";
                Label lb2 = FindName("inY" + m.编号) as Label;
                lb2.Content = "";
                inSumM1.Content = "";
                inSumY1.Content = "";
                inSumM2.Content = "";
                inSumY2.Content = "";
            }

            List<Model_报表类> list = new List<Model_报表类>();
            list = vmr.GetIncomeAndExpenses(value+1);
            decimal dy = 0;
            decimal dn = 0;
            decimal insumm1 = 0;
            decimal insumy1 = 0;
            decimal insumm2 = 0;
            decimal insumy2 = 0;
            string temp = string.Empty ;
            if (list.Count > 0)
            {
                foreach (Model_报表类 m in list)
                {
                    Label lb = FindName("inM" + m.编号) as Label;
                    lb.Content = m.本期数;
                    Label lb2 = FindName("inY" + m.编号) as Label;
                    lb2.Content = m.累计数;
                    decimal.TryParse(m.累计数, out dy);
                    decimal.TryParse(m.本期数, out dn);
                    if (m.编号.StartsWith("40"))
                    {
                        insumm1 += dn;
                        insumy1 += dy;
                    }
                    else if (m.编号.StartsWith("50"))
                    {
                        insumy2 += dy;
                        insumm2 += dn;
                    }
                }

                    inSumM1.Content = insumm1;
                    inSumY1.Content = insumy1;
                    inSumM2.Content = insumm2;
                    inSumY2.Content = insumy2;
                lastList_fee1 = list;
            }

            list.Clear();
            list = vmr.GetIncomeAndExpensesForTwoSubject(value + 1);
            //清除上一次赋值的值
            foreach (Model_报表类 m in lastList_fee2)
            {
                Label lb = FindName("inM" + m.编号) as Label;
                lb.Content = "";
                Label lb2 = FindName("inY" + m.编号) as Label;
                lb2.Content = "";
            }
            foreach (Model_报表类 m in list)
            {
                Label lb = FindName("inM" + m.编号) as Label;
                lb.Content = m.本期数;
                Label lb2 = FindName("inY" + m.编号) as Label;
                lb2.Content = m.累计数;
            }
            lastList_fee2 = list;    
        }

        private List<Model_报表类> LastList = new List<Model_报表类>();
        /// <summary>
        /// 行政费用明细表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_生成3_Click(object sender, RoutedEventArgs e)
        {
            int value = ComboBox_Date2.SelectedIndex;

            if (value == CommonInfo.当前期 - 1)
            {
                MessageBoxCommon.Show("结账后方可生成报表!");
                return;
            }
            //清除上一次赋值的值
            for (int i = 0; i < LastList.Count; i++)
            {
                Label lb = FindName("Label_B" + LastList[i].编号.Replace("（", "").Replace("）", "")) as Label;
                lb.Content = "";
                Label lb2 = FindName("Label_L" + LastList[i].编号.Replace("（", "").Replace("）", "")) as Label;
                lb2.Content = "";
            }

            mr.日志 = "生成" + ComboBox_Date.Text + "行政费用支出明细表";
            vm.Insert(mr);
            List<Model_报表类> list = new List<Model_报表类>();
            list = vmr.GetAdministrativeExpenseDetail(value + 1);
            decimal dy = 0;
            decimal dn = 0;

            //二级科目
            decimal b101 = 0;
            decimal b102 = 0;
            decimal b201 = 0;
            decimal b202 = 0;
            decimal b301 = 0;
            decimal b302 = 0;
            decimal b401 = 0;
            decimal b402 = 0;

            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    Label lb = FindName("Label_B" + list[i].编号.Replace("（", "").Replace("）", "")) as Label;
                    lb.Content = list[i].本期数;
                    Label lb2 = FindName("Label_L" + list[i].编号.Replace("（", "").Replace("）", "")) as Label;
                    lb2.Content = list[i].累计数;
                    decimal.TryParse(list[i].本期数, out dn);
                    decimal.TryParse(list[i].累计数, out dy);
                }
                //2级科目设置
                #region 本期数赋值
                decimal.TryParse(Label_B基本工资.Content.ToString(), out dn);
                b101 += dn;
                decimal.TryParse(Label_B津贴.Content.ToString(), out dn);
                b101 += dn;
                decimal.TryParse(Label_B奖金.Content.ToString(), out dn);
                b101 += dn;
                decimal.TryParse(Label_B社会保障缴费.Content.ToString(), out dn);
                b101 += dn;
                decimal.TryParse(Label_B伙食费.Content.ToString(), out dn);
                b101 += dn;
                decimal.TryParse(Label_B伙食补助费.Content.ToString(), out dn);
                b101 += dn;
                decimal.TryParse(Label_B其他.Content.ToString(), out dn);
                b101 += dn;

                Label_B101.Content = b101;

                decimal.TryParse(Label_B办公费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_B印刷费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_B咨询费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_B手续费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_B水电费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_B邮电费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_B交通费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_B差旅费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_B维修护费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_B租赁费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_B会议费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_B培训费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_B招待费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_B劳务费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_B委托业务费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_B工会经费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_B福利费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_B其他商品和服务支出.Content.ToString(), out dn);
                b201 += dn;

                Label_B201.Content = b201;

                decimal.TryParse(Label_B离休费.Content.ToString(), out dn);
                b301 += dn;
                decimal.TryParse(Label_B退休费.Content.ToString(), out dn);
                b301 += dn;
                decimal.TryParse(Label_B退职役费.Content.ToString(), out dn);
                b301 += dn;
                decimal.TryParse(Label_B抚恤金.Content.ToString(), out dn);
                b301 += dn;
                decimal.TryParse(Label_B生活补助.Content.ToString(), out dn);
                b301 += dn;
                decimal.TryParse(Label_B救济费.Content.ToString(), out dn);
                b301 += dn;
                decimal.TryParse(Label_B医疗费.Content.ToString(), out dn);
                b301 += dn;
                decimal.TryParse(Label_B助学金.Content.ToString(), out dn);
                b301 += dn;
                decimal.TryParse(Label_B奖励金.Content.ToString(), out dn);
                b301 += dn;
                decimal.TryParse(Label_B生产补助.Content.ToString(), out dn);
                b301 += dn;
                decimal.TryParse(Label_B住房公积金.Content.ToString(), out dn);
                b301 += dn;
                decimal.TryParse(Label_B提租补贴.Content.ToString(), out dn);
                b301 += dn;
                decimal.TryParse(Label_B购房补贴.Content.ToString(), out dn);
                b301 += dn;
                decimal.TryParse(Label_B其他对个人和家庭的补助支出.Content.ToString(), out dn);
                b301 += dn;

                Label_B301.Content = b301;

                decimal.TryParse(Label_B房屋建筑物购建.Content.ToString(), out dn);
                b401 += dn;
                decimal.TryParse(Label_B办公设备购置费.Content.ToString(), out dn);
                b401 += dn;
                decimal.TryParse(Label_B交通工具购置费.Content.ToString(), out dn);
                b401 += dn;
                decimal.TryParse(Label_B大型修缮.Content.ToString(), out dn);
                b401 += dn;
                decimal.TryParse(Label_B信息网络购建.Content.ToString(), out dn);
                b401 += dn;
                decimal.TryParse(Label_B其他资本性支出.Content.ToString(), out dn);
                b401 += dn;

                Label_B401.Content = b401;
                #endregion

                #region 年累计
                decimal.TryParse(Label_L基本工资.Content.ToString(), out dy);
                b102 += dy;
                decimal.TryParse(Label_L津贴.Content.ToString(), out dy);
                b102 += dy;
                decimal.TryParse(Label_L奖金.Content.ToString(), out dy);
                b102 += dy;
                decimal.TryParse(Label_L社会保障缴费.Content.ToString(), out dy);
                b102 += dy;
                decimal.TryParse(Label_L伙食费.Content.ToString(), out dy);
                b102 += dy;
                decimal.TryParse(Label_L伙食补助费.Content.ToString(), out dy);
                b102 += dy;
                decimal.TryParse(Label_L其他.Content.ToString(), out dy);
                b102 += dy;

                Label_B102.Content = b102;

                decimal.TryParse(Label_L办公费.Content.ToString(), out dy);
                b202 += dy;
                decimal.TryParse(Label_L印刷费.Content.ToString(), out dy);
                b202 += dy;
                decimal.TryParse(Label_L咨询费.Content.ToString(), out dy);
                b202 += dy;
                decimal.TryParse(Label_L手续费.Content.ToString(), out dy);
                b202 += dy;
                decimal.TryParse(Label_L水电费.Content.ToString(), out dy);
                b202 += dy;
                decimal.TryParse(Label_L邮电费.Content.ToString(), out dy);
                b202 += dy;
                decimal.TryParse(Label_L交通费.Content.ToString(), out dy);
                b202 += dy;
                decimal.TryParse(Label_L差旅费.Content.ToString(), out dy);
                b202 += dy;
                decimal.TryParse(Label_L维修护费.Content.ToString(), out dy);
                b202 += dy;
                decimal.TryParse(Label_L租赁费.Content.ToString(), out dy);
                b202 += dy;
                decimal.TryParse(Label_L会议费.Content.ToString(), out dy);
                b202 += dy;
                decimal.TryParse(Label_L培训费.Content.ToString(), out dy);
                b202 += dy;
                decimal.TryParse(Label_L招待费.Content.ToString(), out dy);
                b202 += dy;
                decimal.TryParse(Label_L劳务费.Content.ToString(), out dy);
                b202 += dy;
                decimal.TryParse(Label_L委托业务费.Content.ToString(), out dy);
                b202 += dy;
                decimal.TryParse(Label_L工会经费.Content.ToString(), out dy);
                b202 += dy;
                decimal.TryParse(Label_L福利费.Content.ToString(), out dy);
                b202 += dy;
                decimal.TryParse(Label_L其他商品和服务支出.Content.ToString(), out dy);
                b202 += dy;

                Label_B202.Content = b202;
                decimal.TryParse(Label_L离休费.Content.ToString(), out dy);
                b302 += dy;
                decimal.TryParse(Label_L退休费.Content.ToString(), out dy);
                b302 += dy;
                decimal.TryParse(Label_L退职役费.Content.ToString(), out dy);
                b302 += dy;
                decimal.TryParse(Label_L抚恤金.Content.ToString(), out dy);
                b302 += dy;
                decimal.TryParse(Label_L生活补助.Content.ToString(), out dy);
                b302 += dy;
                decimal.TryParse(Label_L救济费.Content.ToString(), out dy);
                b302 += dy;
                decimal.TryParse(Label_L医疗费.Content.ToString(), out dy);
                b302 += dy;
                decimal.TryParse(Label_L助学金.Content.ToString(), out dy);
                b302 += dy;
                decimal.TryParse(Label_L奖励金.Content.ToString(), out dy);
                b302 += dy;
                decimal.TryParse(Label_L生产补助.Content.ToString(), out dy);
                b302 += dy;
                decimal.TryParse(Label_L住房公积金.Content.ToString(), out dy);
                b302 += dy;
                decimal.TryParse(Label_L提租补贴.Content.ToString(), out dy);
                b302 += dy;
                decimal.TryParse(Label_L购房补贴.Content.ToString(), out dy);
                b302 += dy;
                decimal.TryParse(Label_L其他对个人和家庭的补助支出.Content.ToString(), out dy);
                b302 += dy;

                Label_B302.Content = b302;

                decimal.TryParse(Label_L房屋建筑物购建.Content.ToString(), out dy);
                b402 += dy;
                decimal.TryParse(Label_L办公设备购置费.Content.ToString(), out dy);
                b402 += dy;
                decimal.TryParse(Label_L交通工具购置费.Content.ToString(), out dy);
                b402 += dy;
                decimal.TryParse(Label_L大型修缮.Content.ToString(), out dy);
                b402 += dy;
                decimal.TryParse(Label_L信息网络购建.Content.ToString(), out dy);
                b402 += dy;
                decimal.TryParse(Label_L其他资本性支出.Content.ToString(), out dy);
                b402 += dy;

                Label_B402.Content = b402;

                #endregion
                Label_B102.Content = b102;
                Label_B202.Content = b202;
                Label_B302.Content = b302;
                Label_B402.Content = b402;

                Label_A01.Content = (b101 + b201 + b301 + b401);
                Label_A02.Content = (b102 + b202 + b302 + b402);
                LastList = list;
            }
            else
            {
                for (int j = 0; j < 4; j++)
                {
                    Label lb = FindName("Label_B" + (j + 1) + "01") as Label;
                    lb.Content = "";
                    Label lb2 = FindName("Label_B" + (j + 1) + "02") as Label;
                    lb2.Content = "";
                }
            }
        }

        private void Button_BalanceSheetPrint_Click(object sender, RoutedEventArgs e)
        {
            string result = new PA.Helper.ExcelHelper.ExcelWriter().ExportBalanceSheet(ComboBox_Date.SelectedIndex + 1, CommonInfo.真实姓名, Label_填表日期.Content.ToString());
            if(result != "")
            {
                MessageBoxCommon.Show(result);
            }
        }

        private void Button_IncomeAndExpenditurePrint_Click(object sender, RoutedEventArgs e)
        {
            string result = new PA.Helper.ExcelHelper.ExcelWriter().ExportIncomeAndExpenditure(ComboBox_Date1.SelectedIndex + 1);
            if (result != "")
            {
                MessageBoxCommon.Show(result);
            }
        }

        private void Button_AdministrativeExpensesSchedulePrint_Click(object sender, RoutedEventArgs e)
        {
            string result = new PA.Helper.ExcelHelper.ExcelWriter().ExportAdministrativeExpensesSchedule(ComboBox_Date2.SelectedIndex + 1);
            if (result != "")
            {
                MessageBoxCommon.Show(result);
            }
        }
    }
}
