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

namespace PA
{
    public partial class MainWindow : Window
    {
        Rect WorkRect = SystemParameters.WorkArea;

        public MainWindow()
        {
            InitializeComponent();
            this.Frame_MainTabControl.Content = new PA.View.Pages.Page_MainTabControl();
        }

        #region 非事件方法

        private void MaxWindow()
        {
            this.Top = 0;
            this.Left = 0;
            this.Width = WorkRect.Width;
            this.Height = WorkRect.Height;
            Properties.Settings.Default.isMainWindowMax = true;
        }
        private void NormalWindowRect()
        {
            this.Height = Properties.Settings.Default.MainWindowRect.Height;
            this.Width = Properties.Settings.Default.MainWindowRect.Width;
            this.Left = Properties.Settings.Default.MainWindowRect.Left;
            this.Top = Properties.Settings.Default.MainWindowRect.Top;
            Properties.Settings.Default.isMainWindowMax = false;
        }

        #endregion

        private void Window_MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && Properties.Settings.Default.isMainWindowMax == false)
            {
                this.DragMove();
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Max_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.isMainWindowMax == true)
            {
                this.NormalWindowRect();
            }
            else
            {
                Properties.Settings.Default.MainWindowRect = new Rect(this.Left, this.Top, this.Width, this.Height);
                this.MaxWindow();
            }
        }

        private void Button_Min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void Window_MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualHeight > WorkRect.Height || this.ActualWidth > WorkRect.Width)
            {
                this.WindowState = System.Windows.WindowState.Normal;
                Button_Max_Click(null, null);
            }
        }
    }
}
