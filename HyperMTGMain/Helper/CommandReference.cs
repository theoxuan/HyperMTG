﻿using System;
using System.Windows;
using System.Windows.Input;

namespace HyperMTG.Helper
{
	public class CommandReference : Freezable, ICommand
	{
		public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof (ICommand),
			typeof (CommandReference), new PropertyMetadata(OnCommandChanged));

		public ICommand Command
		{
			get { return (ICommand) GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		#region ICommand Members

		public bool CanExecute(object parameter)
		{
			if (Command != null)
				return Command.CanExecute(parameter);
			return false;
		}

		public void Execute(object parameter)
		{
			Command.Execute(parameter);
		}

		public event EventHandler CanExecuteChanged;

		#endregion

		private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CommandReference commandReference = d as CommandReference;
			ICommand oldCommand = e.OldValue as ICommand;
			ICommand newCommand = e.NewValue as ICommand;
			if (oldCommand != null)
			{
				oldCommand.CanExecuteChanged -= commandReference.CanExecuteChanged;
			}
			if (newCommand != null)
			{
				newCommand.CanExecuteChanged += commandReference.CanExecuteChanged;
			}
		}

		#region Freezable

		protected override Freezable CreateInstanceCore()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}