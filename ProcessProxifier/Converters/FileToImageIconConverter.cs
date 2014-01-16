using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ProcessProxifier.Converters
{
    public class FileToImageIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            var path = value.ToString();
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return DependencyProperty.UnsetValue;

            using (var sysicon = Icon.ExtractAssociatedIcon(path))
            {
                return sysicon == null ? 
                    DependencyProperty.UnsetValue : 
                    Imaging.CreateBitmapSourceFromHIcon(sysicon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}