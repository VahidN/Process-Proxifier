using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProcessProxifier.Behaviors
{
    /// <summary>
    /// Password watermarking code from: http://prabu-guru.blogspot.com/2010/06/how-to-add-watermark-text-to-textbox.html
    /// </summary>
    public class TextboxHelper : DependencyObject
    {
        public static readonly DependencyProperty IsMonitoringProperty = DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(TextboxHelper), new UIPropertyMetadata(false, onIsMonitoringChanged));
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(TextboxHelper), new UIPropertyMetadata(string.Empty));
        public static readonly DependencyProperty TextLengthProperty = DependencyProperty.RegisterAttached("TextLength", typeof(int), typeof(TextboxHelper), new UIPropertyMetadata(0));
        public static readonly DependencyProperty ClearTextButtonProperty = DependencyProperty.RegisterAttached("ClearTextButton", typeof(bool), typeof(TextboxHelper), new FrameworkPropertyMetadata(false, clearTextChanged));
        public static readonly DependencyProperty SelectAllOnFocusProperty = DependencyProperty.RegisterAttached("SelectAllOnFocus", typeof(bool), typeof(TextboxHelper), new FrameworkPropertyMetadata(false));

        private static readonly DependencyProperty HasTextProperty = DependencyProperty.RegisterAttached("HasText", typeof(bool), typeof(TextboxHelper), new FrameworkPropertyMetadata(false));

        public static void SetSelectAllOnFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectAllOnFocusProperty, value);
        }

        public static bool GetSelectAllOnFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectAllOnFocusProperty);
        }

        public static void SetIsMonitoring(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMonitoringProperty, value);
        }

        public static string GetWatermark(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }

        private static void setTextLength(DependencyObject obj, int value)
        {
            obj.SetValue(TextLengthProperty, value);
            obj.SetValue(HasTextProperty, value >= 1);
        }

        public bool HasText
        {
            get { return (bool)GetValue(HasTextProperty); }
        }

        static void onIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox)
            {
                var txtBox = d as TextBox;

                if ((bool)e.NewValue)
                {
                    txtBox.TextChanged += textChanged;
                    txtBox.GotFocus += textBoxGotFocus;
                }
                else
                {
                    txtBox.TextChanged -= textChanged;
                    txtBox.GotFocus -= textBoxGotFocus;
                }
            }
            else if (d is PasswordBox)
            {
                var passBox = d as PasswordBox;

                if ((bool)e.NewValue)
                {
                    passBox.PasswordChanged += passwordChanged;
                    passBox.GotFocus += passwordGotFocus;
                }
                else
                {
                    passBox.PasswordChanged -= passwordChanged;
                    passBox.GotFocus -= passwordGotFocus;
                }
            }
        }

        static void textChanged(object sender, TextChangedEventArgs e)
        {
            var txtBox = sender as TextBox;
            if (txtBox == null)
                return;
            setTextLength(txtBox, txtBox.Text.Length);
        }

        static void passwordChanged(object sender, RoutedEventArgs e)
        {
            var passBox = sender as PasswordBox;
            if (passBox == null)
                return;
            setTextLength(passBox, passBox.Password.Length);
        }

        static void textBoxGotFocus(object sender, RoutedEventArgs e)
        {
            var txtBox = sender as TextBox;
            if (txtBox == null)
                return;
            if (GetSelectAllOnFocus(txtBox))
            {
                txtBox.Dispatcher.BeginInvoke((Action)(txtBox.SelectAll));
            }
        }

        static void passwordGotFocus(object sender, RoutedEventArgs e)
        {
            var passBox = sender as PasswordBox;
            if (passBox == null)
                return;
            if (GetSelectAllOnFocus(passBox))
            {
                passBox.Dispatcher.BeginInvoke((Action)(passBox.SelectAll));
            }
        }

        public static bool GetClearTextButton(DependencyObject d)
        {
            return (bool)d.GetValue(ClearTextButtonProperty);
        }

        public static void SetClearTextButton(DependencyObject obj, bool value)
        {
            obj.SetValue(ClearTextButtonProperty, value);
        }

        private static void clearTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textbox = d as TextBox;
            if (textbox != null)
            {
                if ((bool)e.NewValue)
                {
                    textbox.Loaded += textBoxLoaded;
                }
                else
                {
                    textbox.Loaded -= textBoxLoaded;
                }
            }
            var passbox = d as PasswordBox;
            if (passbox != null)
            {
                if ((bool)e.NewValue)
                {
                    passbox.Loaded += passBoxLoaded;
                }
                else
                {
                    passbox.Loaded -= passBoxLoaded;
                }
            }
        }

        static void passBoxLoaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is PasswordBox))
                return;

            var passbox = sender as PasswordBox;
            if (passbox.Style == null)
                return;

            var template = passbox.Template;
            if (template == null)
                return;

            var button = template.FindName("PART_ClearText", passbox) as Button;
            if (button == null)
                return;

            if (GetClearTextButton(passbox))
            {
                button.Click += clearPassClicked;
            }
            else
            {
                button.Click -= clearPassClicked;
            }
        }


        static void textBoxLoaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox))
                return;

            var textbox = sender as TextBox;
            if (textbox.Style == null)
                return;

            var template = textbox.Template;
            if (template == null)
                return;

            var button = template.FindName("PART_ClearText", textbox) as Button;
            if (button == null)
                return;

            if (GetClearTextButton(textbox))
            {
                button.Click += clearTextClicked;
            }
            else
            {
                button.Click -= clearTextClicked;
            }
        }

        static void clearTextClicked(object sender, RoutedEventArgs e)
        {
            var button = ((Button)sender);
            var parent = VisualTreeHelper.GetParent(button);
            while (!(parent is TextBox))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            ((TextBox)parent).Clear();
        }

        static void clearPassClicked(object sender, RoutedEventArgs e)
        {
            var button = ((Button)sender);
            var parent = VisualTreeHelper.GetParent(button);
            while (!(parent is PasswordBox))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            ((PasswordBox)parent).Clear();
        }
    }
}