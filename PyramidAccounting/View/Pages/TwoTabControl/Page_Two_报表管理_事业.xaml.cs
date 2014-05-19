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
            this.Label_编制单位1.Content += "\t" + xr.ReadXML("公司");   //程序启动后加载当前公司名称
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
                Label lb = FindName("inM" + m.编号) as Label;
                lb.Content = "";
                Label lb2 = FindName("inY" + m.编号) as Label;
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
            string result = new PA.Helper.ExcelHelper.ExcelWriter().ExportBalanceSheet(ComboBox_Date.SelectedIndex + 1, CommonInfo.真实姓名, Label_填表日期.Content.ToString());
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
                B306.Content = "";
                
            }
            //第一次对一级科目赋值
            List<Model_报表类> list = new List<Model_报表类>();
            list = vmr.GetIncomeAndExpenses(value + 1, vms.GetOneSubjectList());
            decimal dy = 0;
            decimal dn = 0;
            decimal insumm1 = 0;
            decimal insumy1 = 0;
            decimal insumm2 = 0;
            decimal insumy2 = 0;
            string temp = string.Empty;
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
                    if (m.编号.StartsWith("4"))
                    {
                        insumm1 += dn;
                        insumy1 += dy;
                    }
                    else if (m.编号.StartsWith("5"))
                    {
                        insumy2 += dy;
                        insumm2 += dn;
                    }
                    else if (m.编号.StartsWith("3"))
                    {
                        B306.Content = m.累计数;
                    }
                }

                inSumM1.Content = insumm1;
                inSumY1.Content = insumy1;
                inSumM2.Content = insumm2;
                inSumY2.Content = insumy2;
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
                if (m.编号.StartsWith("30601"))
                {
                    B30601.Content = m.累计数;
                }
                else if (m.编号.StartsWith("30602"))
                {
                    B30602.Content = m.累计数;
                }
            }
            
           
            lastList_fee2 = list;    
        }

        private void Button_IncomeAndExpenditurePrint_Click(object sender, RoutedEventArgs e)
        {
            string result = new PA.Helper.ExcelHelper.ExcelWriter().ExportIncomeAndExpenditure(ComboBox_Date1.SelectedIndex + 1);
            if (result != "")
            {
                MessageBoxCommon.Show(result);
            }
        }


        private List<Model_事业报表类> LastList = new List<Model_事业报表类>();
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
                Label lb = FindName("Label_A" + LastList[i].编号.Replace("（", "").Replace("）", "")) as Label;
                lb.Content = "";
                Label lb2 = FindName("Label_B" + LastList[i].编号.Replace("（", "").Replace("）", "")) as Label;
                lb2.Content = "";
                Label lb3 = FindName("Label_C" + LastList[i].编号.Replace("（", "").Replace("）", "")) as Label;
                lb3.Content = "";
                Label lb4 = FindName("Label_D" + LastList[i].编号.Replace("（", "").Replace("）", "")) as Label;
                lb4.Content = "";
                Label lb5 = FindName("Label_E" + LastList[i].编号.Replace("（", "").Replace("）", "")) as Label;
                lb5.Content = "";
                Label lb6 = FindName("Label_F" + LastList[i].编号.Replace("（", "").Replace("）", "")) as Label;
                lb6.Content = "";
            }

            mr.日志 = "生成" + ComboBox_Date.Text + "事业及经营支出明细表";
            vm.Insert(mr);
            List<Model_事业报表类> list = new List<Model_事业报表类>();
            list = vmr.GetExpenditureDetail(value + 1, 504);
            decimal dn = 0;

            //二级科目
            decimal b101 = 0;
            decimal b102 = 0;
            decimal b103 = 0;
            decimal b104 = 0;
            decimal b105 = 0;
            decimal b106 = 0;
            decimal b201 = 0;
            decimal b202 = 0;
            decimal b203 = 0;
            decimal b204 = 0;
            decimal b205 = 0;
            decimal b206 = 0;
            decimal b301 = 0;
            decimal b302 = 0;
            decimal b303 = 0;
            decimal b304 = 0;
            decimal b305 = 0;
            decimal b306 = 0;
            decimal b401 = 0;
            decimal b402 = 0;
            decimal b403 = 0;
            decimal b404 = 0;
            decimal b405 = 0;
            decimal b406 = 0;
            decimal b501 = 0;
            decimal b502 = 0;
            decimal b503 = 0;
            decimal b504 = 0;
            decimal b505 = 0;
            decimal b506 = 0;
            decimal b601 = 0;
            decimal b602 = 0;
            decimal b603 = 0;
            decimal b604 = 0;
            decimal b605 = 0;
            decimal b606 = 0;
            decimal b701 = 0;
            decimal b702 = 0;
            decimal b703 = 0;
            decimal b704 = 0;
            decimal b705 = 0;
            decimal b706 = 0;

            if (list.Count > 0)
            {
                foreach (Model_事业报表类 m in list) 
                {
                    Label lb = FindName("Label_A" + m.编号.Replace("（", "").Replace("）", "")) as Label;
                    lb.Content = m.本期数;
                    Label lb2 = FindName("Label_B" + m.编号.Replace("（", "").Replace("）", "")) as Label;
                    lb2.Content = m.累计数;
                    Label lb3 = FindName("Label_C" + m.编号.Replace("（", "").Replace("）", "")) as Label;
                    lb3.Content = m.本期数1;
                    Label lb4 = FindName("Label_D" + m.编号.Replace("（", "").Replace("）", "")) as Label;
                    lb4.Content = m.累计数1;
                    Label lb5 = FindName("Label_E" + m.编号.Replace("（", "").Replace("）", "")) as Label;
                    lb5.Content = m.本期数2;
                    Label lb6 = FindName("Label_F" + m.编号.Replace("（", "").Replace("）", "")) as Label;
                    lb6.Content = m.累计数2;
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

                Label_B301.Content = b301;

                decimal.TryParse(Label_A事业单位补贴.Content.ToString(), out dn);
                b401 += dn;
                decimal.TryParse(Label_A其他对企事业单位的补贴支出.Content.ToString(), out dn);
                b401 += dn;

                Label_B401.Content = b401;

                decimal.TryParse(Label_A向国家银行借款付息.Content.ToString(), out dn);
                b501 += dn;
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

                Label_B302.Content = b302;

                decimal.TryParse(Label_B事业单位补贴.Content.ToString(), out dn);
                b402 += dn;
                decimal.TryParse(Label_B其他对企事业单位的补贴支出.Content.ToString(), out dn);
                b402 += dn;

                Label_B402.Content = b402;

                decimal.TryParse(Label_B向国家银行借款付息.Content.ToString(), out dn);
                b502 += dn;
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
                #region 本期数赋值
                decimal.TryParse(Label_C基本工资.Content.ToString(), out dn);
                b103 += dn;
                decimal.TryParse(Label_C津贴.Content.ToString(), out dn);
                b103 += dn;
                decimal.TryParse(Label_C奖金.Content.ToString(), out dn);
                b103 += dn;
                decimal.TryParse(Label_C社会保障缴费.Content.ToString(), out dn);
                b103 += dn;
                decimal.TryParse(Label_C伙食费.Content.ToString(), out dn);
                b103 += dn;
                decimal.TryParse(Label_C伙食补助费.Content.ToString(), out dn);
                b103 += dn;
                decimal.TryParse(Label_C其他.Content.ToString(), out dn);
                b103 += dn;

                Label_B103.Content = b103;

                decimal.TryParse(Label_C办公费.Content.ToString(), out dn);
                b203 += dn;
                decimal.TryParse(Label_C印刷费.Content.ToString(), out dn);
                b203 += dn;
                decimal.TryParse(Label_C咨询费.Content.ToString(), out dn);
                b203 += dn;
                decimal.TryParse(Label_C手续费.Content.ToString(), out dn);
                b203 += dn;
                decimal.TryParse(Label_C水费.Content.ToString(), out dn);
                b203 += dn;
                decimal.TryParse(Label_C电费.Content.ToString(), out dn);
                b203 += dn;
                decimal.TryParse(Label_C邮电费.Content.ToString(), out dn);
                b203 += dn;
                decimal.TryParse(Label_C交通费.Content.ToString(), out dn);
                b203 += dn;
                decimal.TryParse(Label_C差旅费.Content.ToString(), out dn);
                b203 += dn;
                decimal.TryParse(Label_C维修护费.Content.ToString(), out dn);
                b203 += dn;
                decimal.TryParse(Label_C会议费.Content.ToString(), out dn);
                b203 += dn;
                decimal.TryParse(Label_C培训费.Content.ToString(), out dn);
                b203 += dn;
                decimal.TryParse(Label_C招待费.Content.ToString(), out dn);
                b203 += dn;
                decimal.TryParse(Label_C工程建设费.Content.ToString(), out dn);
                b203 += dn;
                decimal.TryParse(Label_C劳务费.Content.ToString(), out dn);
                b203 += dn;
                decimal.TryParse(Label_C工会经费.Content.ToString(), out dn);
                b203 += dn;
                decimal.TryParse(Label_C福利费.Content.ToString(), out dn);
                b203 += dn;
                decimal.TryParse(Label_C其他商品和服务支出.Content.ToString(), out dn);
                b203 += dn;

                Label_B203.Content = b203;

                decimal.TryParse(Label_C退休费.Content.ToString(), out dn);
                b303 += dn;
                decimal.TryParse(Label_C退职役费.Content.ToString(), out dn);
                b303 += dn;
                decimal.TryParse(Label_C生活补助.Content.ToString(), out dn);
                b303 += dn;
                decimal.TryParse(Label_C医疗费.Content.ToString(), out dn);
                b303 += dn;
                decimal.TryParse(Label_C奖励金.Content.ToString(), out dn);
                b303 += dn;
                decimal.TryParse(Label_C住房公积金.Content.ToString(), out dn);
                b303 += dn;
                decimal.TryParse(Label_C其他对个人和家庭的补助支出.Content.ToString(), out dn);
                b303 += dn;

                Label_B303.Content = b303;

                decimal.TryParse(Label_C事业单位补贴.Content.ToString(), out dn);
                b403 += dn;
                decimal.TryParse(Label_C其他对企事业单位的补贴支出.Content.ToString(), out dn);
                b403 += dn;

                Label_B403.Content = b403;

                decimal.TryParse(Label_C向国家银行借款付息.Content.ToString(), out dn);
                b503 += dn;
                b503 += dn;

                Label_B503.Content = b503;

                decimal.TryParse(Label_C房屋建筑物购建.Content.ToString(), out dn);
                b603 += dn;
                decimal.TryParse(Label_C办公设备购置费.Content.ToString(), out dn);
                b603 += dn;
                decimal.TryParse(Label_C专用设备购置费.Content.ToString(), out dn);
                b603 += dn;
                decimal.TryParse(Label_C交通工具购置费.Content.ToString(), out dn);
                b603 += dn;
                decimal.TryParse(Label_C基础设施建设.Content.ToString(), out dn);
                b603 += dn;
                decimal.TryParse(Label_C大型修缮.Content.ToString(), out dn);
                b603 += dn;
                decimal.TryParse(Label_C信息网络购建.Content.ToString(), out dn);
                b603 += dn;
                decimal.TryParse(Label_C物资储备.Content.ToString(), out dn);
                b603 += dn;
                decimal.TryParse(Label_C其他资本性支出.Content.ToString(), out dn);
                b603 += dn;

                Label_B603.Content = b603;

                decimal.TryParse(Label_C未划分的项目支出.Content.ToString(), out dn);
                b703 += dn;
                decimal.TryParse(Label_C其他支出.Content.ToString(), out dn);
                b703 += dn;
                Label_B703.Content = b703;
                #endregion
                #region 本期数赋值
                decimal.TryParse(Label_D基本工资.Content.ToString(), out dn);
                b104 += dn;
                decimal.TryParse(Label_D津贴.Content.ToString(), out dn);
                b104 += dn;
                decimal.TryParse(Label_D奖金.Content.ToString(), out dn);
                b104 += dn;
                decimal.TryParse(Label_D社会保障缴费.Content.ToString(), out dn);
                b104 += dn;
                decimal.TryParse(Label_D伙食费.Content.ToString(), out dn);
                b104 += dn;
                decimal.TryParse(Label_D伙食补助费.Content.ToString(), out dn);
                b104 += dn;
                decimal.TryParse(Label_D其他.Content.ToString(), out dn);
                b104 += dn;

                Label_B104.Content = b104;

                decimal.TryParse(Label_D办公费.Content.ToString(), out dn);
                b204 += dn;
                decimal.TryParse(Label_D印刷费.Content.ToString(), out dn);
                b204 += dn;
                decimal.TryParse(Label_D咨询费.Content.ToString(), out dn);
                b204 += dn;
                decimal.TryParse(Label_D手续费.Content.ToString(), out dn);
                b204 += dn;
                decimal.TryParse(Label_D水费.Content.ToString(), out dn);
                b204 += dn;
                decimal.TryParse(Label_D电费.Content.ToString(), out dn);
                b204 += dn;
                decimal.TryParse(Label_D邮电费.Content.ToString(), out dn);
                b204 += dn;
                decimal.TryParse(Label_D交通费.Content.ToString(), out dn);
                b204 += dn;
                decimal.TryParse(Label_D差旅费.Content.ToString(), out dn);
                b204 += dn;
                decimal.TryParse(Label_D维修护费.Content.ToString(), out dn);
                b204 += dn;
                decimal.TryParse(Label_D会议费.Content.ToString(), out dn);
                b204 += dn;
                decimal.TryParse(Label_D培训费.Content.ToString(), out dn);
                b204 += dn;
                decimal.TryParse(Label_D招待费.Content.ToString(), out dn);
                b204 += dn;
                decimal.TryParse(Label_D工程建设费.Content.ToString(), out dn);
                b204 += dn;
                decimal.TryParse(Label_D劳务费.Content.ToString(), out dn);
                b204 += dn;
                decimal.TryParse(Label_D工会经费.Content.ToString(), out dn);
                b204 += dn;
                decimal.TryParse(Label_D福利费.Content.ToString(), out dn);
                b204 += dn;
                decimal.TryParse(Label_D其他商品和服务支出.Content.ToString(), out dn);
                b204 += dn;

                Label_B204.Content = b204;

                decimal.TryParse(Label_D退休费.Content.ToString(), out dn);
                b304 += dn;
                decimal.TryParse(Label_D退职役费.Content.ToString(), out dn);
                b304 += dn;
                decimal.TryParse(Label_D生活补助.Content.ToString(), out dn);
                b304 += dn;
                decimal.TryParse(Label_D医疗费.Content.ToString(), out dn);
                b304 += dn;
                decimal.TryParse(Label_D奖励金.Content.ToString(), out dn);
                b304 += dn;
                decimal.TryParse(Label_D住房公积金.Content.ToString(), out dn);
                b304 += dn;
                decimal.TryParse(Label_D其他对个人和家庭的补助支出.Content.ToString(), out dn);
                b304 += dn;

                Label_B304.Content = b304;

                decimal.TryParse(Label_D事业单位补贴.Content.ToString(), out dn);
                b404 += dn;
                decimal.TryParse(Label_D其他对企事业单位的补贴支出.Content.ToString(), out dn);
                b404 += dn;

                Label_B404.Content = b404;

                decimal.TryParse(Label_D向国家银行借款付息.Content.ToString(), out dn);
                b504 += dn;
                b504 += dn;

                Label_B504.Content = b504;

                decimal.TryParse(Label_D房屋建筑物购建.Content.ToString(), out dn);
                b604 += dn;
                decimal.TryParse(Label_D办公设备购置费.Content.ToString(), out dn);
                b604 += dn;
                decimal.TryParse(Label_D专用设备购置费.Content.ToString(), out dn);
                b604 += dn;
                decimal.TryParse(Label_D交通工具购置费.Content.ToString(), out dn);
                b604 += dn;
                decimal.TryParse(Label_D基础设施建设.Content.ToString(), out dn);
                b604 += dn;
                decimal.TryParse(Label_D大型修缮.Content.ToString(), out dn);
                b604 += dn;
                decimal.TryParse(Label_D信息网络购建.Content.ToString(), out dn);
                b604 += dn;
                decimal.TryParse(Label_D物资储备.Content.ToString(), out dn);
                b604 += dn;
                decimal.TryParse(Label_D其他资本性支出.Content.ToString(), out dn);
                b604 += dn;

                Label_B604.Content = b604;

                decimal.TryParse(Label_D未划分的项目支出.Content.ToString(), out dn);
                b704 += dn;
                decimal.TryParse(Label_D其他支出.Content.ToString(), out dn);
                b704 += dn;
                Label_B704.Content = b704;
                #endregion  
                #region 本期数赋值
                decimal.TryParse(Label_E基本工资.Content.ToString(), out dn);
                b105 += dn;
                decimal.TryParse(Label_E津贴.Content.ToString(), out dn);
                b105 += dn;
                decimal.TryParse(Label_E奖金.Content.ToString(), out dn);
                b105 += dn;
                decimal.TryParse(Label_E社会保障缴费.Content.ToString(), out dn);
                b105 += dn;
                decimal.TryParse(Label_E伙食费.Content.ToString(), out dn);
                b105 += dn;
                decimal.TryParse(Label_E伙食补助费.Content.ToString(), out dn);
                b105 += dn;
                decimal.TryParse(Label_E其他.Content.ToString(), out dn);
                b105 += dn;

                Label_B105.Content = b105;

                decimal.TryParse(Label_E办公费.Content.ToString(), out dn);
                b205 += dn;
                decimal.TryParse(Label_E印刷费.Content.ToString(), out dn);
                b205 += dn;
                decimal.TryParse(Label_E咨询费.Content.ToString(), out dn);
                b205 += dn;
                decimal.TryParse(Label_E手续费.Content.ToString(), out dn);
                b205 += dn;
                decimal.TryParse(Label_E水费.Content.ToString(), out dn);
                b205 += dn;
                decimal.TryParse(Label_E电费.Content.ToString(), out dn);
                b205 += dn;
                decimal.TryParse(Label_E邮电费.Content.ToString(), out dn);
                b205 += dn;
                decimal.TryParse(Label_E交通费.Content.ToString(), out dn);
                b205 += dn;
                decimal.TryParse(Label_E差旅费.Content.ToString(), out dn);
                b205 += dn;
                decimal.TryParse(Label_E维修护费.Content.ToString(), out dn);
                b205 += dn;
                decimal.TryParse(Label_E会议费.Content.ToString(), out dn);
                b205 += dn;
                decimal.TryParse(Label_E培训费.Content.ToString(), out dn);
                b205 += dn;
                decimal.TryParse(Label_E招待费.Content.ToString(), out dn);
                b205 += dn;
                decimal.TryParse(Label_E工程建设费.Content.ToString(), out dn);
                b205 += dn;
                decimal.TryParse(Label_E劳务费.Content.ToString(), out dn);
                b205 += dn;
                decimal.TryParse(Label_E工会经费.Content.ToString(), out dn);
                b205 += dn;
                decimal.TryParse(Label_E福利费.Content.ToString(), out dn);
                b205 += dn;
                decimal.TryParse(Label_E其他商品和服务支出.Content.ToString(), out dn);
                b205 += dn;

                Label_B205.Content = b205;

                decimal.TryParse(Label_E退休费.Content.ToString(), out dn);
                b305 += dn;
                decimal.TryParse(Label_E退职役费.Content.ToString(), out dn);
                b305 += dn;
                decimal.TryParse(Label_E生活补助.Content.ToString(), out dn);
                b305 += dn;
                decimal.TryParse(Label_E医疗费.Content.ToString(), out dn);
                b305 += dn;
                decimal.TryParse(Label_E奖励金.Content.ToString(), out dn);
                b305 += dn;
                decimal.TryParse(Label_E住房公积金.Content.ToString(), out dn);
                b305 += dn;
                decimal.TryParse(Label_E其他对个人和家庭的补助支出.Content.ToString(), out dn);
                b305 += dn;

                Label_B305.Content = b305;

                decimal.TryParse(Label_E事业单位补贴.Content.ToString(), out dn);
                b405 += dn;
                decimal.TryParse(Label_E其他对企事业单位的补贴支出.Content.ToString(), out dn);
                b405 += dn;

                Label_B405.Content = b405;

                decimal.TryParse(Label_E向国家银行借款付息.Content.ToString(), out dn);
                b505 += dn;
                b505 += dn;

                Label_B505.Content = b505;

                decimal.TryParse(Label_E房屋建筑物购建.Content.ToString(), out dn);
                b605 += dn;
                decimal.TryParse(Label_E办公设备购置费.Content.ToString(), out dn);
                b605 += dn;
                decimal.TryParse(Label_E专用设备购置费.Content.ToString(), out dn);
                b605 += dn;
                decimal.TryParse(Label_E交通工具购置费.Content.ToString(), out dn);
                b605 += dn;
                decimal.TryParse(Label_E基础设施建设.Content.ToString(), out dn);
                b605 += dn;
                decimal.TryParse(Label_E大型修缮.Content.ToString(), out dn);
                b605 += dn;
                decimal.TryParse(Label_E信息网络购建.Content.ToString(), out dn);
                b605 += dn;
                decimal.TryParse(Label_E物资储备.Content.ToString(), out dn);
                b605 += dn;
                decimal.TryParse(Label_E其他资本性支出.Content.ToString(), out dn);
                b605 += dn;

                Label_B605.Content = b605;

                decimal.TryParse(Label_E未划分的项目支出.Content.ToString(), out dn);
                b705 += dn;
                decimal.TryParse(Label_E其他支出.Content.ToString(), out dn);
                b705 += dn;
                Label_B705.Content = b705;
                #endregion
                #region 本期数赋值
                decimal.TryParse(Label_F基本工资.Content.ToString(), out dn);
                b106 += dn;
                decimal.TryParse(Label_F津贴.Content.ToString(), out dn);
                b106 += dn;
                decimal.TryParse(Label_F奖金.Content.ToString(), out dn);
                b106 += dn;
                decimal.TryParse(Label_F社会保障缴费.Content.ToString(), out dn);
                b106 += dn;
                decimal.TryParse(Label_F伙食费.Content.ToString(), out dn);
                b106 += dn;
                decimal.TryParse(Label_F伙食补助费.Content.ToString(), out dn);
                b106 += dn;
                decimal.TryParse(Label_F其他.Content.ToString(), out dn);
                b106 += dn;

                Label_B106.Content = b106;

                decimal.TryParse(Label_F办公费.Content.ToString(), out dn);
                b206 += dn;
                decimal.TryParse(Label_F印刷费.Content.ToString(), out dn);
                b206 += dn;
                decimal.TryParse(Label_F咨询费.Content.ToString(), out dn);
                b206 += dn;
                decimal.TryParse(Label_F手续费.Content.ToString(), out dn);
                b206 += dn;
                decimal.TryParse(Label_F水费.Content.ToString(), out dn);
                b206 += dn;
                decimal.TryParse(Label_F电费.Content.ToString(), out dn);
                b206 += dn;
                decimal.TryParse(Label_F邮电费.Content.ToString(), out dn);
                b206 += dn;
                decimal.TryParse(Label_F交通费.Content.ToString(), out dn);
                b206 += dn;
                decimal.TryParse(Label_F差旅费.Content.ToString(), out dn);
                b206 += dn;
                decimal.TryParse(Label_F维修护费.Content.ToString(), out dn);
                b206 += dn;
                decimal.TryParse(Label_F会议费.Content.ToString(), out dn);
                b206 += dn;
                decimal.TryParse(Label_F培训费.Content.ToString(), out dn);
                b206 += dn;
                decimal.TryParse(Label_F招待费.Content.ToString(), out dn);
                b206 += dn;
                decimal.TryParse(Label_F工程建设费.Content.ToString(), out dn);
                b206 += dn;
                decimal.TryParse(Label_F劳务费.Content.ToString(), out dn);
                b206 += dn;
                decimal.TryParse(Label_F工会经费.Content.ToString(), out dn);
                b206 += dn;
                decimal.TryParse(Label_F福利费.Content.ToString(), out dn);
                b206 += dn;
                decimal.TryParse(Label_F其他商品和服务支出.Content.ToString(), out dn);
                b206 += dn;

                Label_B206.Content = b206;

                decimal.TryParse(Label_F退休费.Content.ToString(), out dn);
                b306 += dn;
                decimal.TryParse(Label_F退职役费.Content.ToString(), out dn);
                b306 += dn;
                decimal.TryParse(Label_F生活补助.Content.ToString(), out dn);
                b306 += dn;
                decimal.TryParse(Label_F医疗费.Content.ToString(), out dn);
                b306 += dn;
                decimal.TryParse(Label_F奖励金.Content.ToString(), out dn);
                b306 += dn;
                decimal.TryParse(Label_F住房公积金.Content.ToString(), out dn);
                b306 += dn;
                decimal.TryParse(Label_F其他对个人和家庭的补助支出.Content.ToString(), out dn);
                b306 += dn;

                Label_B306.Content = b306;

                decimal.TryParse(Label_F事业单位补贴.Content.ToString(), out dn);
                b406 += dn;
                decimal.TryParse(Label_F其他对企事业单位的补贴支出.Content.ToString(), out dn);
                b406 += dn;

                Label_B406.Content = b406;

                decimal.TryParse(Label_F向国家银行借款付息.Content.ToString(), out dn);
                b506 += dn;
                b506 += dn;

                Label_B506.Content = b506;

                decimal.TryParse(Label_F房屋建筑物购建.Content.ToString(), out dn);
                b606 += dn;
                decimal.TryParse(Label_F办公设备购置费.Content.ToString(), out dn);
                b606 += dn;
                decimal.TryParse(Label_F专用设备购置费.Content.ToString(), out dn);
                b606 += dn;
                decimal.TryParse(Label_F交通工具购置费.Content.ToString(), out dn);
                b606 += dn;
                decimal.TryParse(Label_F基础设施建设.Content.ToString(), out dn);
                b606 += dn;
                decimal.TryParse(Label_F大型修缮.Content.ToString(), out dn);
                b606 += dn;
                decimal.TryParse(Label_F信息网络购建.Content.ToString(), out dn);
                b606 += dn;
                decimal.TryParse(Label_F物资储备.Content.ToString(), out dn);
                b606 += dn;
                decimal.TryParse(Label_F其他资本性支出.Content.ToString(), out dn);
                b606 += dn;

                Label_B606.Content = b606;

                decimal.TryParse(Label_F未划分的项目支出.Content.ToString(), out dn);
                b706 += dn;
                decimal.TryParse(Label_F其他支出.Content.ToString(), out dn);
                b706 += dn;
                Label_B706.Content = b706;
                #endregion
                Label_A01.Content = (b101 + b201 + b301 + b401 + b501 + b601 + b701);
                Label_A02.Content = (b102 + b202 + b302 + b402 + b502 + b602 + b702);
                Label_A03.Content = (b103 + b203 + b303 + b403 + b503 + b603 + b703);
                Label_A04.Content = (b104 + b204 + b304 + b404 + b504 + b604 + b704);
                Label_A05.Content = (b105 + b205 + b305 + b405 + b505 + b605 + b705);
                Label_A06.Content = (b106 + b206 + b306 + b406 + b506 + b606 + b706);
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
            string result = new PA.Helper.ExcelHelper.ExcelWriter().ExportAdministrativeExpensesSchedule(ComboBox_Date2.SelectedIndex + 1);
            if (result != "")
            {
                MessageBoxCommon.Show(result);
            }
        }
    }
}
