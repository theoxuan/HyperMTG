﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HyperMTG.Helper
{
	public class VisibaleConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return parameter == null
				? ((bool) value ? Visibility.Visible : Visibility.Collapsed)
				: (!(bool) value ? Visibility.Visible : Visibility.Collapsed);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}

		#endregion
	}
}