using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ProcessProxifier.Behaviors
{
    public class AutoSizeListViewColumns : DependencyObject
    {
        public static readonly DependencyProperty EnableProperty =
                        DependencyProperty.RegisterAttached(
                            "Enable",
                            typeof(bool),
                            typeof(AutoSizeListViewColumns),
                            new FrameworkPropertyMetadata(OnEnableChanged));

        public static bool GetEnable(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableProperty);
        }

        public static void SetEnable(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableProperty, value);
        }

        public static readonly DependencyProperty AutoSizeColumnProperty =
            DependencyProperty.RegisterAttached(
                "AutoSizeColumn",
                typeof(bool),
                typeof(AutoSizeListViewColumns),
                new UIPropertyMetadata(false)
            );

        public static bool GetAutoSizeColumn(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoSizeColumnProperty);
        }

        public static void SetAutoSizeColumn(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoSizeColumnProperty, value);
        }

        public static void OnEnableChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var val = (bool)e.NewValue;
            var listView = obj as ListView;
            if (listView == null)
                throw new InvalidOperationException("This behavior can only be attached to a ListView.");

            if (val)
            {
                listView.SizeChanged += listViewSizeChanged;
            }
            else
            {
                listView.SizeChanged -= listViewSizeChanged;
            }
        }

        private static void listViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (listView == null)
                throw new InvalidOperationException("This behavior can only be attached to a ListView.");

            // Only relevant to grid views
            if (!(listView.View is GridView))
                return;

            var grid = listView.View as GridView;

            // Only relevant for width
            if (!e.WidthChanged)
                return;

            // Get all AutoSize columns
            var columns = new List<GridViewColumn>();
            double specifiedWidth = 0;
            foreach (var col in grid.Columns)
            {
                if ((bool)col.GetValue(AutoSizeColumnProperty))
                    columns.Add(col);
                else
                    specifiedWidth += col.ActualWidth;
            }

            // Give them a fair share of the remaining space
            foreach (var col in columns)
            {
                var newWidth = (listView.ActualWidth - specifiedWidth) / columns.Count;
                if (newWidth >= 0)
                {
                    col.Width = newWidth > 5 ? newWidth - 5 : newWidth;
                }
            }
        }
    }
}