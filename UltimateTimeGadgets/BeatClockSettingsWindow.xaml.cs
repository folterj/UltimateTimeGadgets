using System.Windows;
using System.Windows.Controls;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for BeatClockSettingsWindow.xaml
	/// </summary>
	public partial class BeatClockSettingsWindow : SettingsWindow
	{
		public BeatClockControl timeControl;

		public BeatClockSettingsWindow(TimeControl timeControl, ControlSettings settings)
		{
			this.timeControl = (BeatClockControl)timeControl;
			this.settings = settings;

			InitializeComponent();

			fromSettings();
		}

		public override void fromSettings()
		{
			bool showAtSign = settings.format.Contains(Constants.showAtSignString);
			int decimals = settings.index;

			timeControl.setFormat(decimals, showAtSign);
			decimalsCombo.SelectedIndex = decimals;
			atSignCheck.IsChecked = showAtSign;
		}

		public override void toSettings()
		{
			bool showAtSign = (bool)atSignCheck.IsChecked;
			int decimals = decimalsCombo.SelectedIndex;

			settings.format = "";
			if (showAtSign)
			{
				settings.format += Constants.showAtSignString;
			}
			settings.index = decimals;
			timeControl.setFormat(decimals, showAtSign);
		}

		private void decimalsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			toSettings();
		}

		private void atSignCheck_CheckChanged(object sender, RoutedEventArgs e)
		{
			toSettings();
		}

	}
}
