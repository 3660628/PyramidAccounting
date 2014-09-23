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
    /// Interaction logic for Page_Two_报表管理_事业.xaml
    /// </summary>
    public partial class Page_Two_报表管理_事业 : Page
    {
        PA.Helper.XMLHelper.XMLReader xr = new Helper.XMLHelper.XMLReader();
        private ComboBox_Common cbc = new ComboBox_Common();
        private ViewModel_ReportManager vmr = new ViewModel_ReportManager();
        private ViewModel_科目管理 vms = new ViewModel_科目管理();
        private ViewModel_操作日志 vm = new ViewModel_操作日志();
        private Model_操作日志 mr = new Model_操作日志();
        public Page_Two_报表管理_事业()
        {
            InitializeComponent();
            SubscribeToEvent();
            for (int i = 1; i <= 3; i++)
            {
                Label lb = FindName("Label_编制单位" + i) as Label;
                lb.Content = "编制单位：" + CommonInfo.制表单位;
            }
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

        private List<Model_报表类> last_资产负债表 = new List<Model_报表类>();
        private void Button_生成1_Click(object sender, RoutedEventArgs e)
        {
            int value = ComboBox_Date.SelectedIndex;
            if (value == CommonInfo.当前期 - 1)
            {
                MessageBoxCommon.Show("结账后方可生成报表!");
                return;
            }
            mr.日志 = "生成" + ComboBox_Date.Text + "资产负债表";
            vm.Insert(mr);

            //清除上一次赋值的值
            foreach (Model_报表类 m in last_资产负债表)
            {
                Label lb = FindName("y" + m.编号) as Label;
                lb.Content = "";
                Label lb2 = FindName("n" + m.编号) as Label;
                lb2.Content = "";
            }

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
                foreach (Model_报表类 m in list) 
                {
                    Label lb = FindName("y" + m.编号) as Label;
                    lb.Content = m.年初数;
                    Label lb2 = FindName("n" + m.编号) as Label;
                    lb2.Content = m.期末数;
                    decimal.TryParse(m.年初数, out dy);
                    decimal.TryParse(m.期末数, out dn);
                    if (m.编号.StartsWith("1"))
                    {
                        sumy1 += dy;
                        sumn1 += dn;
                    }
                    else if (m.编号.StartsWith("2"))
                    {
                        sumy2 += dy;
                        sumn2 += dn;
                    }
                    else if (m.编号.StartsWith("3"))
                    {
                        sumy3 += dy;
                        sumn3 += dn;
                    }
                    else if (m.编号.StartsWith("4"))
                    {
                        sumy4 += dy;
                        sumn4 += dn;
                    }
                    else if (m.编号.StartsWith("5"))
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
                Label_填表人.Content = "填表人：" + CommonInfo.真实姓名;
                Label_填表日期.Content = "填表日期：" + DateTime.Now.ToLongDateString();
                last_资产负债表 = list;
            }
            else
            {
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
                Label_填表日期.Content = "填表日期：";
            }
                
        }

        private List<Model_报表类> lastList_fee1 = new List<Model_报表类>();
        private List<Model_报表类> lastList_fee2 = new List<Model_报表类>();
        
        private void Button_BalanceSheetPrint_Click(object sender, RoutedEventArgs e)
        {
            string result = new PA.Helper.ExcelHelper.InstitutionsExcelWriter().ExportBalanceSheet(ComboBox_Date.SelectedIndex + 1, CommonInfo.真实姓名);
            if (result != "")
            {
                MessageBoxCommon.Show(result);
            }
        }

        private void Button_生成2_Click(object sender, RoutedEventArgs e)
        {
            int value = ComboBox_Date1.SelectedIndex;

            if (value == CommonInfo.当前期 - 1)
            {
                MessageBoxCommon.Show("结账后方可生成报表!");
                return;
            }
            mr.日志 = "生成" + ComboBox_Date.Text + "收入支出总表( 事 业 )";
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
                Total01.Content = "";
                Total02.Content = "";
                Total03.Content = "";
                Total04.Content = "";
                Total05.Content = "";
                Total06.Content = "";
                Total07.Content = "";
                Total08.Content = "";
                Total09.Content = "";
                Total10.Content = "";
                B306.Content = "";
                
            }
            //第一次对一级科目赋值
            List<Model_报表类> list = new List<Model_报表类>();
            list = vmr.GetIncomeAndExpenses(value + 1, vms.GetOneSubjectList());
            decimal dy = 0;
            decimal dn = 0;
            decimal total01 = 0;
            decimal total02 = 0;
            decimal total03 = 0;
            decimal total04 = 0;
            decimal total05 = 0;
            decimal total06 = 0;
            decimal total07 = 0;
            decimal total08 = 0;
            decimal total09 = 0;
            decimal total10 = 0;
            string temp = string.Empty;
            if (list.Count > 0)
            {
                foreach (Model_报表类 m in list)
                {
                    if (m.编号.StartsWith("3"))
                    {
                        continue;
                    }
                    else 
                    {
                        Label lb = FindName("inM" + m.编号) as Label;
                        lb.Content = m.本期数;
                        Label lb2 = FindName("inY" + m.编号) as Label;
                        lb2.Content = m.累计数;
                        decimal.TryParse(m.累计数, out dy);
                        decimal.TryParse(m.本期数, out dn);
                        if (m.编号.StartsWith("4"))
                        {
                            if (m.编号.Equals("404"))
                            {
                                total03 += dn;
                                total04 += dy;
                            }
                            else
                            {
                                total01 += dn;
                                total02 += dy;
                            }
                        }
                        else if (m.编号.StartsWith("5"))
                        {
                            if (m.编号.Equals("512"))
                            {
                                total07 += dn;
                                total08 += dy;
                            }
                            else if (m.编号.Equals("501") || m.编号.Equals("503"))
                            {
                                total09 += dn;
                                total10 += dy;
                            }
                            else
                            {
                                total05 += dn;
                                total06 += dy;
                            }
                        }
                    }
                }
                Total01.Content = total01 == 0 ? "" : "" + total01;
                Total02.Content = total02 == 0 ? "" : "" + total02;
                Total03.Content = total03 == 0 ? "" : "" + total03;
                Total04.Content = total04 == 0 ? "" : "" + total04;
                Total05.Content = total05 == 0 ? "" : "" + total05;
                Total06.Content = total06 == 0 ? "" : "" + total06;
                Total07.Content = total07 == 0 ? "" : "" + total07;
                Total08.Content = total08 == 0 ? "" : "" + total08;
                Total09.Content = total09 == 0 ? "" : "" + total09;
                Total10.Content = total10 == 0 ? "" : "" + total10;

                inSumM1.Content = total01 + total03;
                inSumY1.Content = total02 + total04;
                inSumM2.Content = total05 + total07 + total09;
                inSumY2.Content = total06 + total08 + total10;
                decimal tmp = (total02 + total04) - (total06 + total08 + total10);
                B306.Content = tmp;
                inSumY3.Content = tmp;
                lastList_fee1 = list;
            }

            list.Clear();
            //第一次对二级科目赋值
            list = vmr.GetIncomeAndExpensesForTwoSubject(value + 1, vms.GetIncomeAndOutSubjectList());
            foreach (Model_报表类 m in lastList_fee2)
            {
                Label lb = FindName("inM" + m.编号) as Label;
                lb.Content = "";
                Label lb2 = FindName("inY" + m.编号) as Label;
                lb2.Content = "";
            }
            B30601.Content = "";
            B30602.Content = "";
            foreach (Model_报表类 m in list)
            {
                Label lb = FindName("inM" + m.编号) as Label;
                lb.Content = m.本期数;
                Label lb2 = FindName("inY" + m.编号) as Label;
                lb2.Content = m.累计数;
                if (m.编号.Equals("30601"))
                {
                    B30601.Content = m.累计数;
                }
                else if (m.编号.Equals("30602"))
                {
                    B30602.Content = m.累计数;
                }
            }
            lastList_fee2 = list;    
        }

        private void Button_IncomeAndExpenditurePrint_Click(object sender, RoutedEventArgs e)
        {
            string result = new PA.Helper.ExcelHelper.InstitutionsExcelWriter().ExportIncomeAndExpenditure(ComboBox_Date1.SelectedIndex + 1);
            if (result != "")
            {
                MessageBoxCommon.Show(result);
            }
        }


        private List<Model_报表类> LastList = new List<Model_报表类>();
        private List<Model_报表类> LastList_base = new List<Model_报表类>();
        private List<Model_报表类> LastList_project = new List<Model_报表类>();
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
                Label lb = FindName("Label_A" + LastList[i].编号.Replace("（", "").Replace("(", "").Replace("）", "").Replace(")", "")) as Label;
                lb.Content = "";
                Label lb2 = FindName("Label_B" + LastList[i].编号.Replace("（", "").Replace("(", "").Replace("）", "").Replace(")", "")) as Label;
                lb2.Content = "";
            }
            mr.日志 = "生成" + ComboBox_Date.Text + "事业及经营支出明细表";
            vm.Insert(mr);
            List<Model_报表类> list = new List<Model_报表类>();
            list = vmr.GetAdministrativeExpenseDetail(value + 1, 504);
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
            decimal b501 = 0;
            decimal b502 = 0;
            decimal b601 = 0;
            decimal b602 = 0;
            decimal b701 = 0;
            decimal b702 = 0;

            if (list.Count > 0)
            {
                foreach (Model_报表类 m in list) 
                {
                    Label lb = FindName("Label_A" + m.编号.Replace("（", "").Replace("(", "").Replace("）", "").Replace(")", "")) as Label;
                    lb.Content = m.本期数;
                    Label lb2 = FindName("Label_B" + m.编号.Replace("（", "").Replace("(", "").Replace("）", "").Replace(")", "")) as Label;
                    lb2.Content = m.累计数;
                }
                //2级科目设置 
                #region 本期数赋值
                decimal.TryParse(Label_A基本工资.Content.ToString(), out dn);
                b101 += dn;
                decimal.TryParse(Label_A津贴.Content.ToString(), out dn);
                b101 += dn;            
                decimal.TryParse(Label_A奖金.Content.ToString(), out dn);
                b101 += dn;            
                decimal.TryParse(Label_A社会保障缴费.Content.ToString(), out dn);
                b101 += dn;            
                decimal.TryParse(Label_A伙食费.Content.ToString(), out dn);
                b101 += dn;            
                decimal.TryParse(Label_A伙食补助费.Content.ToString(), out dn);
                b101 += dn;            
                decimal.TryParse(Label_A其他.Content.ToString(), out dn);
                b101 += dn;
                    Label_B101.Content = b101;
                decimal.TryParse(Label_A办公费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_A印刷费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_A咨询费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_A手续费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_A水费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_A电费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_A邮电费.Content.ToString(), out dn);
                b201 += dn;           
                decimal.TryParse(Label_A交通费.Content.ToString(), out dn);
                b201 += dn;           
                decimal.TryParse(Label_A差旅费.Content.ToString(), out dn);
                b201 += dn;           
                decimal.TryParse(Label_A维修护费.Content.ToString(), out dn);
                b201 += dn;          
                decimal.TryParse(Label_A会议费.Content.ToString(), out dn);
                b201 += dn;           
                decimal.TryParse(Label_A培训费.Content.ToString(), out dn);
                b201 += dn;            
                decimal.TryParse(Label_A招待费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_A工程建设费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_A劳务费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_A工会经费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_A福利费.Content.ToString(), out dn);
                b201 += dn;
                decimal.TryParse(Label_A其他商品和服务支出.Content.ToString(), out dn);
                b201 += dn;
                    Label_B201.Content = b201;
                decimal.TryParse(Label_A退休费.Content.ToString(), out dn);
                b301 += dn;            
                decimal.TryParse(Label_A退职役费.Content.ToString(), out dn);
                b301 += dn;            
                decimal.TryParse(Label_A生活补助.Content.ToString(), out dn);
                b301 += dn;            
                decimal.TryParse(Label_A医疗费.Content.ToString(), out dn);
                b301 += dn;            
                decimal.TryParse(Label_A奖励金.Content.ToString(), out dn);
                b301 += dn;            
                decimal.TryParse(Label_A住房公积金.Content.ToString(), out dn);
                b301 += dn;            
                decimal.TryParse(Label_A其他对个人和家庭的补助支出.Content.ToString(), out dn);
                b301 += dn;
                decimal.TryParse(Label_A住房维修补贴.Content.ToString(), out dn);
                b301 += dn;

                    Label_B301.Content = b301;
                decimal.TryParse(Label_A事业单位补贴.Content.ToString(), out dn);
                b401 += dn;
                decimal.TryParse(Label_A其他对企事业单位的补贴支出.Content.ToString(), out dn);
                b401 += dn;
                    Label_B401.Content = b401;

                decimal.TryParse(Label_A向国家银行借款付息.Content.ToString(), out dn);
                b501 += dn;
                    Label_B501.Content = b501;
                decimal.TryParse(Label_A房屋建筑物购建.Content.ToString(), out dn);
                b601 += dn;
                decimal.TryParse(Label_A办公设备购置费.Content.ToString(), out dn);
                b601 += dn;
                decimal.TryParse(Label_A专用设备购置费.Content.ToString(), out dn);
                b601 += dn;
                decimal.TryParse(Label_A交通工具购置费.Content.ToString(), out dn);
                b601 += dn;
                decimal.TryParse(Label_A基础设施建设.Content.ToString(), out dn);
                b601 += dn;
                decimal.TryParse(Label_A大型修缮.Content.ToString(), out dn);
                b601 += dn;
                decimal.TryParse(Label_A信息网络购建.Content.ToString(), out dn);
                b601 += dn;
                decimal.TryParse(Label_A物资储备.Content.ToString(), out dn);
                b601 += dn;
                decimal.TryParse(Label_A其他资本性支出.Content.ToString(), out dn);
                b601 += dn;

                    Label_B601.Content = b601;

                decimal.TryParse(Label_A未划分的项目支出.Content.ToString(), out dn);
                b701 += dn;
                decimal.TryParse(Label_A其他支出.Content.ToString(), out dn);
                b701 += dn;
                    Label_B701.Content = b701;
                #endregion
                #region 本期数赋值
                decimal.TryParse(Label_B基本工资.Content.ToString(), out dn);
                b102 += dn;
                decimal.TryParse(Label_B津贴.Content.ToString(), out dn);
                b102 += dn;
                decimal.TryParse(Label_B奖金.Content.ToString(), out dn);
                b102 += dn;
                decimal.TryParse(Label_B社会保障缴费.Content.ToString(), out dn);
                b102 += dn;
                decimal.TryParse(Label_B伙食费.Content.ToString(), out dn);
                b102 += dn;
                decimal.TryParse(Label_B伙食补助费.Content.ToString(), out dn);
                b102 += dn;
                decimal.TryParse(Label_B其他.Content.ToString(), out dn);
                b102 += dn;
                    Label_B102.Content = b102;

                decimal.TryParse(Label_B办公费.Content.ToString(), out dn);
                b202 += dn;
                decimal.TryParse(Label_B印刷费.Content.ToString(), out dn);
                b202 += dn;
                decimal.TryParse(Label_B咨询费.Content.ToString(), out dn);
                b202 += dn;
                decimal.TryParse(Label_B手续费.Content.ToString(), out dn);
                b202 += dn;
                decimal.TryParse(Label_B水费.Content.ToString(), out dn);
                b202 += dn;
                decimal.TryParse(Label_B电费.Content.ToString(), out dn);
                b202 += dn;
                decimal.TryParse(Label_B邮电费.Content.ToString(), out dn);
                b202 += dn;
                decimal.TryParse(Label_B交通费.Content.ToString(), out dn);
                b202 += dn;
                decimal.TryParse(Label_B差旅费.Content.ToString(), out dn);
                b202 += dn;
                decimal.TryParse(Label_B维修护费.Content.ToString(), out dn);
                b202 += dn;
                decimal.TryParse(Label_B会议费.Content.ToString(), out dn);
                b202 += dn;
                decimal.TryParse(Label_B培训费.Content.ToString(), out dn);
                b202 += dn;
                decimal.TryParse(Label_B招待费.Content.ToString(), out dn);
                b202 += dn;
                decimal.TryParse(Label_B工程建设费.Content.ToString(), out dn);
                b202 += dn;
                decimal.TryParse(Label_B劳务费.Content.ToString(), out dn);
                b202 += dn;
                decimal.TryParse(Label_B工会经费.Content.ToString(), out dn);
                b202 += dn;
                decimal.TryParse(Label_B福利费.Content.ToString(), out dn);
                b202 += dn;
                decimal.TryParse(Label_B其他商品和服务支出.Content.ToString(), out dn);
                b202 += dn;
                    Label_B202.Content = b202;

                decimal.TryParse(Label_B退休费.Content.ToString(), out dn);
                b302 += dn;
                decimal.TryParse(Label_B退职役费.Content.ToString(), out dn);
                b302 += dn;
                decimal.TryParse(Label_B生活补助.Content.ToString(), out dn);
                b302 += dn;
                decimal.TryParse(Label_B医疗费.Content.ToString(), out dn);
                b302 += dn;
                decimal.TryParse(Label_B奖励金.Content.ToString(), out dn);
                b302 += dn;
                decimal.TryParse(Label_B住房公积金.Content.ToString(), out dn);
                b302 += dn;
                decimal.TryParse(Label_B其他对个人和家庭的补助支出.Content.ToString(), out dn);
                b302 += dn;
                decimal.TryParse(Label_B住房维修补贴.Content.ToString(), out dn);
                b302 += dn;

                Label_B302.Content = b302;

                decimal.TryParse(Label_B事业单位补贴.Content.ToString(), out dn);
                b402 += dn;
                decimal.TryParse(Label_B其他对企事业单位的补贴支出.Content.ToString(), out dn);
                b402 += dn;

                Label_B402.Content = b402;

                decimal.TryParse(Label_B向国家银行借款付息.Content.ToString(), out dn);
                b502 += dn;

                Label_B502.Content = b502;

                decimal.TryParse(Label_B房屋建筑物购建.Content.ToString(), out dn);
                b602 += dn;
                decimal.TryParse(Label_B办公设备购置费.Content.ToString(), out dn);
                b602 += dn;
                decimal.TryParse(Label_B专用设备购置费.Content.ToString(), out dn);
                b602 += dn;
                decimal.TryParse(Label_B交通工具购置费.Content.ToString(), out dn);
                b602 += dn;
                decimal.TryParse(Label_B基础设施建设.Content.ToString(), out dn);
                b602 += dn;
                decimal.TryParse(Label_B大型修缮.Content.ToString(), out dn);
                b602 += dn;
                decimal.TryParse(Label_B信息网络购建.Content.ToString(), out dn);
                b602 += dn;
                decimal.TryParse(Label_B物资储备.Content.ToString(), out dn);
                b602 += dn;
                decimal.TryParse(Label_B其他资本性支出.Content.ToString(), out dn);
                b602 += dn;

                Label_B602.Content = b602;

                decimal.TryParse(Label_B未划分的项目支出.Content.ToString(), out dn);
                b702 += dn;
                decimal.TryParse(Label_B其他支出.Content.ToString(), out dn);
                b702 += dn;
                Label_B702.Content = b702;
               

                #endregion
                for (int i = 1; i <= 7; i++)
                {
                    for (int j = 1; j <= 2; j++)
                    {
                        Label lb = FindName("Label_B" + i + "0" + j) as Label;
                        if (lb.Content.ToString().Equals("0"))
                        {
                            lb.Content = "";
                        }
                    }
                }
                Label_A01.Content = (b101 + b201 + b301 + b401 + b501 + b601 + b701);
                Label_A02.Content = (b102 + b202 + b302 + b402 + b502 + b602 + b702);
                LastList = list;
            }
            else
            {
                for (int j = 0; j < 7; j++)
                {
                    Label lb = FindName("Label_B" + (j + 1) + "01") as Label;
                    lb.Content = "";
                    Label lb2 = FindName("Label_B" + (j + 1) + "02") as Label;
                    lb2.Content = "";
                    Label lb3 = FindName("Label_B" + (j + 1) + "03") as Label;
                    lb3.Content = "";
                    Label lb4 = FindName("Label_B" + (j + 1) + "04") as Label;
                    lb4.Content = "";
                    Label lb5 = FindName("Label_B" + (j + 1) + "05") as Label;
                    lb5.Content = "";
                    Label lb6 = FindName("Label_B" + (j + 1) + "06") as Label;
                    lb6.Content = "";
                }
            }
        }

        private void Button_AdministrativeExpensesSchedulePrint_Click(object sender, RoutedEventArgs e)
        {
            string result = new PA.Helper.ExcelHelper.InstitutionsExcelWriter().ExportAdministrativeExpensesSchedule(ComboBox_Date2.SelectedIndex + 1);
            if (result != "")
            {
                MessageBoxCommon.Show(result);
            }
        }
    }
}
