using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;

namespace WalletWasabi.Gui.ManagedDialogs
{
	internal class ManagedFileChooser : UserControl
	{
		private Control _quickLinksRoot;
		private ListBox _filesView;

		public ManagedFileChooser()
		{
			AvaloniaXamlLoader.Load(this);
			AddHandler(PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
			_quickLinksRoot = this.FindControl<Control>("QuickLinks");
			_filesView = this.FindControl<ListBox>("Files");
		}

		private ManagedFileChooserViewModel Model => DataContext as ManagedFileChooserViewModel;

		private void OnPointerPressed(object sender, PointerPressedEventArgs e)
		{
			if (!((e.Source as StyledElement)?.DataContext is ManagedFileChooserItemViewModel model))
			{
				return;
			}

			var isQuickLink = _quickLinksRoot.IsLogicalParentOf(e.Source as Control);
			if (e.ClickCount == 2 || isQuickLink)
			{
				if (model.IsDirectory)
				{
					Model?.Navigate(model.Path);
				}
				else
				{
					Model?.SelectSingleFile(model);
				}

				e.Handled = true;
			}
		}

		protected override async void OnDataContextChanged(EventArgs e)
		{
			base.OnDataContextChanged(e);

			if (DataContext is null)
			{
				return;
			}

			var model = DataContext as ManagedFileChooserViewModel;

			var preselected = model.SelectedItems.FirstOrDefault();
			if (preselected is null)
			{
				return;
			}

			//Let everything to settle down and scroll to selected item
			await Task.Delay(100);

			if (preselected != model.SelectedItems.FirstOrDefault())
			{
				return;
			}

			// Workaround for ListBox bug, scroll to the previous file
			var indexOfPreselected = model.Items.IndexOf(preselected);

			if (indexOfPreselected > 1)
			{
				_filesView.ScrollIntoView(model.Items[indexOfPreselected - 1]);
			}
		}
	}
}
