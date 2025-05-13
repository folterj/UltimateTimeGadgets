using System.Windows;
using System.Windows.Controls;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for AnalogClockSettingsWindow.xaml
	/// </summary>
	public partial class AnalogClockSettingsWindow : SettingsWindow
	{
		AnalogClockControl timeControl;

		public AnalogClockSettingsWindow(TimeControl timeControl, ControlSettings settings)
		{
			this.timeControl = (AnalogClockControl)timeControl;
			this.settings = settings;

			InitializeComponent();

			fromSettings();
		}

		public override void fromSettings()
		{
			// convert from settings to form
			if (!updating)
			{
				updating = true;

				bool showSecondHand = settings.format.Contains(Constants.showSecondHandString);
				bool showMinuteHand = settings.format.Contains(Constants.showMinuteHandString);
				int clockFace = settings.index;

				timeControl.setSecondsHand(showSecondHand);
				timeControl.setMinutesHand(showMinuteHand);
				timeControl.setClockFace(clockFace);
				secondsCheck.IsChecked = showSecondHand;
				minutesCheck.IsChecked = showMinuteHand;
				clockFaceCombo.SelectedIndex = clockFace - 1;

				updating = false;
			}
		}

		public override void toSettings()
		{
			// convert from form to settings
			if (!updating)
			{
				updating = true;

				bool showSecondHand = (bool)secondsCheck.IsChecked;
				bool showMinuteHand = (bool)minutesCheck.IsChecked;
				int clockFace = clockFaceCombo.SelectedIndex + 1;

				settings.format = "";
				if (showSecondHand)
				{
					settings.format += Constants.showSecondHandString;
				}
				if (showMinuteHand)
				{
					settings.format += Constants.showMinuteHandString;
				}
				settings.index = clockFace;
				timeControl.setSecondsHand(showSecondHand);
				timeControl.setMinutesHand(showMinuteHand);
				timeControl.setClockFace(clockFace);

				updating = false;
			}
		}

		private void secondsCheck_CheckChanged(object sender, RoutedEventArgs e)
		{
			toSettings();
		}

		private void minutesCheck_CheckChanged(object sender, RoutedEventArgs e)
		{
			bool mins = (bool)minutesCheck.IsChecked;
			secondsCheck.IsEnabled = mins;
			if (!mins)
			{
				secondsCheck.IsChecked = false;
			}
			toSettings();
		}

		private void clockFaceCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			toSettings();
		}

		private void colorPicker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			toSettings();
		}

	}
}
