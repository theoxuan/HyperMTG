﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;

//Event Design: http://msdn.microsoft.com/en-us/library/ms229011.aspx
// Reference - http://www.codeproject.com/Articles/165368/WPF-MVVM-Quick-Start-Tutorial

namespace HyperMTG.Helper
{
	[Serializable]
	public abstract class ObservableObject : INotifyPropertyChanged
	{
		[field: NonSerialized]
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, e);
			}
			
		}

		protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpresssion)
		{
			string propertyName = PropertySupport.ExtractPropertyName(propertyExpresssion);
			RaisePropertyChanged(propertyName);
		}

		protected void RaisePropertyChanged(string propertyName)
		{
			VerifyPropertyName(propertyName);
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Warns the developer if this Object does not have a public property with
		/// the specified flavor. This method does not exist in a Release build.
		/// </summary>
		[Conditional("DEBUG")]
		[DebuggerStepThrough]
		public void VerifyPropertyName(string propertyName)
		{
			// verify that the property flavor matches a real,  
			// public, instance property on this Object.
			if (TypeDescriptor.GetProperties(this)[propertyName] == null)
			{
				Debug.Fail("Invalid property flavor: " + propertyName);
			}
			
		}
	}
}
