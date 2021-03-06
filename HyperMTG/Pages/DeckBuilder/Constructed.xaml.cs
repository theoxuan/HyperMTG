﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using FirstFloor.ModernUI.Windows.Controls;
using HyperKore.Utilities;
using HyperMTG.ViewModel;

namespace HyperMTG.Pages
{
	/// <summary>
	///     Interaction logic for Constructed.xaml
	/// </summary>
	public partial class Constructed
	{
		public Constructed()
		{
			InitializeComponent();
		}

		private void FrameworkElement_OnTargetUpdated(object sender, DataTransferEventArgs e)
		{
			BBCodeBlock block = sender as BBCodeBlock;
			if (block == null)
			{
				return;
			}

			if (string.IsNullOrWhiteSpace(block.BBCode))
			{
				return;
			}

			IEnumerable<string> result = block.BBCode.ManaSplit();
			block.Inlines.Clear();
			foreach (string str in result)
			{
				if (Regex.IsMatch(str, "[{.+}]"))
				{
					Style style = Application.Current.TryFindResource("Mana" + Regex.Replace(str, "{|}", "").ToUpper()) as Style;
					if (style != null)
					{
						UserControl control = new UserControl
						{
							Style = style,
							Width = block.FontSize,
							Height = block.FontSize,
						};
						block.Inlines.Add(new InlineUIContainer(control));
					}
					else
					{
						block.Inlines.Add(str);
					}
				}
				else
				{
					block.Inlines.Add(str);
				}
			}
		}

		private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			DeckBuiderViewModel deckBuiderViewModel = DataContext as DeckBuiderViewModel;
			if (deckBuiderViewModel == null) return;
			if (deckBuiderViewModel.FilterCommand.CanExecute(null))
			{
				deckBuiderViewModel.FilterCommand.Execute(null);
			}
		}
	}
}