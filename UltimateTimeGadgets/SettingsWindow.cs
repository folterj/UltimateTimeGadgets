using System.Windows;

namespace UltimateTimeGadgets
{
	public abstract class SettingsWindow : Window
	{
		public ControlSettings settings;
		SettingsObserver settingsObserver;

		public string oldFormat;
		public int oldIndex;

		public bool updating = false;
		public bool initDone = false;

		public abstract void fromSettings();

		public abstract void toSettings();

		public void setSettingsObserver(SettingsObserver settingsObserver)
		{
			this.settingsObserver = settingsObserver;
		}

		private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.OldValue == false && (bool)e.NewValue == true)
			{
				oldFormat = settings.format;
				oldIndex = settings.index;
				fromSettings();
			}
		}

		public void okButton_Click(object sender, RoutedEventArgs e)
		{
			Hide();
			if (settingsObserver != null)
			{
				settingsObserver.settingsChanged();
			}
		}

		public void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			settings.format = oldFormat;
			settings.index = oldIndex;
			fromSettings();
			Hide();
		}

		public void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Hide();
			e.Cancel = true;
		}

	}
}
