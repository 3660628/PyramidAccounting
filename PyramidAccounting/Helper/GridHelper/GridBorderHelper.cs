using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;

namespace PA.Helper.GridHelper
{
    public class GridBorderHelper
    {
        ////A0B3C6
        private static SolidColorBrush _BorderBrush = new SolidColorBrush(Colors.Black);

        public static SolidColorBrush BorderBrush
        {
            get { return GridBorderHelper._BorderBrush; }
            set { GridBorderHelper._BorderBrush = value; }
        }

        public static bool GetShowBorder(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowBorderProperty);
        }

        public static void SetShowBorder(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowBorderProperty, value);
        }

        public static readonly DependencyProperty ShowBorderProperty =
        DependencyProperty.RegisterAttached("ShowBorder", typeof(bool), typeof(GridBorderHelper), new PropertyMetadata(OnShowBorderChanged));

        private static void OnShowBorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as Grid;
            if ((bool)e.OldValue)
            {
                grid.Loaded -= (s, arg) => { };
            }

            if ((bool)e.NewValue)
            {
                grid.Loaded += (s, arg) =>
                {

                    //确定行数和列数
                    var rows = grid.RowDefinitions.Count;
                    var columns = grid.ColumnDefinitions.Count;

                    var controls = grid.Children;
                    var count = controls.Count;


                    for (int i = 0; i < count; i++)
                    {
                        var item = controls[i] as FrameworkElement;
                        Border border = new Border();
                        border.BorderBrush = BorderBrush;
                        border.BorderThickness = new Thickness(0, 0, 1, 1);

                        var row = Grid.GetRow(item);
                        var column = Grid.GetColumn(item);
                        var rowspan = Grid.GetRowSpan(item);
                        var columnspan = Grid.GetColumnSpan(item);

                        Grid.SetRow(border, row);
                        Grid.SetColumn(border, column);
                        Grid.SetRowSpan(border, rowspan);
                        Grid.SetColumnSpan(border, columnspan);

                        grid.Children.Add(border);
                    }


                    //画最外面的边框
                    Border bo = new Border();
                    bo.BorderBrush = BorderBrush;
                    bo.BorderThickness = new Thickness(1, 1, 0, 0);
                    bo.SetValue(Grid.ColumnProperty, 0);
                    bo.SetValue(Grid.RowProperty, 0);

                    bo.SetValue(Grid.ColumnSpanProperty, grid.ColumnDefinitions.Count);
                    bo.SetValue(Grid.RowSpanProperty, grid.RowDefinitions.Count);

                    bo.Tag = "autoBorder";
                    grid.Children.Add(bo);
                };

            }
        }

    }
}
