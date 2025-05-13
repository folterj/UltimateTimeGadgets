using System;
using System.Reflection;
using System.Windows;
using Microsoft.Win32;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for GeneralSettingsWindow.xaml
	/// </summary>
	public partial class GeneralSettingsWindow : Window
	{
		ControlSettingsList settings;
		ControlSettingsList localSettings = new ControlSettingsList();

		GalleryWindow gallery;

		bool updating = false;

		public GeneralSettingsWindow(ControlSettingsList settings, GalleryWindow gallery)
		{
			InitializeComponent();
			styleCombo.ItemsSource = Enum.GetValues(typeof(ColorStyle));

			this.settings = settings;
			this.gallery = gallery;
			localSettings.copyGeneralSettings(this.settings);

			fromSettings();
		}

		public void fromSettings()
		{
			// convert from settings to form
			if (!updating)
			{
				updating = true;

				styleCombo.SelectedItem = localSettings.colorStyle;
				lockedCheck.IsChecked = localSettings.gadgetsLocked;
				onTopCheck.IsChecked = localSettings.gadgetsOnTop;
				startupCheck.IsChecked = localSettings.windowsStartup;

				updating = false;
			}
		}

		public void toSettings()
		{
			// convert from form to settings
			if (!updating)
			{
				updating = true;

				localSettings.colorStyle = (ColorStyle)styleCombo.SelectedItem;
				localSettings.gadgetsLocked = (bool)lockedCheck.IsChecked;
				localSettings.gadgetsOnTop = (bool)onTopCheck.IsChecked;
				localSettings.windowsStartup = (bool)startupCheck.IsChecked;

				updating = false;
			}
		}

		private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.OldValue == false && (bool)e.NewValue == true)
			{
				fromSettings();
			}
		}

		private void okButton_Click(object sender, RoutedEventArgs e)
		{
			toSettings();
			settings.copyGeneralSettings(localSettings);
			gallery.setGadgetsStyle(settings.colorStyle);
			gallery.setGadgetsLocked(settings.gadgetsLocked);
			gallery.setGadgetsOnTop(settings.gadgetsOnTop);
			settings.save();
			registerInStartup(settings.windowsStartup);
			Hide();
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			Hide();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Hide();
			e.Cancel = true;
		}

		private void registerInStartup(bool isChecked)
		{
			RegistryKey key = Registry.CurrentUser.OpenSubKey(Constants.regKey, true);
			Assembly curAssembly = Assembly.GetExecutingAssembly();
			string name = curAssembly.GetName().Name;
			string path = "\"" + curAssembly.Location + "\" " + Constants.noGalleryOption;

			if (isChecked)
			{
				if (key.GetValue(name) == null)
				{
					// key doesn't exist; create
					key.SetValue(name, path);
				}
			}
			else
			{
				if (key.GetValue(name) != null)
				{
					// key exists; delete
					key.DeleteValue(name);
				}
			}
		}

	}
}
