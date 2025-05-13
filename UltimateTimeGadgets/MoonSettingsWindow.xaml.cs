using System.Windows;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for MoonSettingsWindow.xaml
	/// </summary>
	public partial class MoonSettingsWindow : SettingsWindow
	{
		MoonVisControl timeControl;

		public MoonSettingsWindow(TimeControl timeControl, ControlSettings settings)
		{
			this.timeControl = (MoonVisControl)timeControl;
			this.settings = settings;

			InitializeComponent();

			fromSettings();
		}

		public override void fromSettings()
		{
			bool showInfo = settings.format.Contains(Constants.showInfoString);

			timeControl.setInfoVisible(showInfo);
			infoCheck.IsChecked = showInfo;
		}

		public override void toSettings()
		{
			bool showInfo = (bool)infoCheck.IsChecked;

			settings.format = "";
			if (showInfo)
			{
				settings.format += Constants.showInfoString;
			}
			timeControl.setInfoVisible(showInfo);
		}

		private void infoCheck_CheckChanged(object sender, RoutedEventArgs e)
		{
			toSettings();
		}

	}
}
