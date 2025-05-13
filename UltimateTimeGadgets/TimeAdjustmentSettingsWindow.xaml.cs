using System.Windows;
using System.Windows.Controls;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for TimeAdjustmentSettingsWindow.xaml
	/// </summary>
	public partial class TimeAdjustmentSettingsWindow : SettingsWindow
	{
		TimeAdjustmentControl timeControl;

		public TimeAdjustmentSettingsWindow(TimeControl timeControl, ControlSettings settings)
		{
			this.timeControl = (TimeAdjustmentControl)timeControl;
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

				string format = "";
				bool showAll = settings.format.Contains(Constants.showAllString);
				bool showRaw = settings.format.Contains(Constants.showRawString);
				bool showYear = settings.format.Contains(Constants.showYearString);
				bool showTime = settings.format.Contains(Constants.showTimeString);
				bool showAdjustment = settings.format.Contains(Constants.showAdjustmentString);

				if (settings.format.Contains(Constants.formatMDYString))
				{
					formatCombo.SelectedIndex = 1;
					format = Constants.formatMDYString;
				}
				else if (settings.format.Contains(Constants.formatYMDString))
				{
					formatCombo.SelectedIndex = 2;
					format = Constants.formatYMDString;
				}
				else
				{
					formatCombo.SelectedIndex = 0;
					format = Constants.formatDMYString;
				}

				timeControl.setFormat(format, showAll, showRaw, showYear, showTime, showAdjustment);

				if (showAll)
				{
					displayCombo.SelectedIndex = 0;
				}
				else if (showRaw)
				{
					displayCombo.SelectedIndex = 1;
				}
				else
				{
					displayCombo.SelectedIndex = 2;
				}

				yearCheck.IsChecked = showYear;
				timeCheck.IsChecked = showTime;
				adjustmentCheck.IsChecked = showAdjustment;

				updating = false;
			}
		}

		public override void toSettings()
		{
			// convert from form to settings
			if (!updating)
			{
				updating = true;

				string format = "";
				bool showAll = false;
				bool showRaw = false;
				bool showYear = false;
				bool showTime = false;
				bool showAdjustment = false;

				switch (formatCombo.SelectedIndex)
				{
					case 1: format = Constants.formatMDYString; break;
					case 2: format = Constants.formatYMDString; break;
					default: format = Constants.formatDMYString; break;
				}

				switch (displayCombo.SelectedIndex)
				{
					case 0: showAll = true; break;
					case 1: showRaw = true; break;
					default: showAll = false; break;
				}

				showYear = (bool)yearCheck.IsChecked;
				showTime = (bool)timeCheck.IsChecked;
				showAdjustment = (bool)adjustmentCheck.IsChecked;

				settings.format = format;
				if (showAll)
				{
					settings.format += Constants.showAllString;
				}
				if (showRaw)
				{
					settings.format += Constants.showRawString;
				}
				if (showYear)
				{
					settings.format += Constants.showYearString;
				}
				if (showTime)
				{
					settings.format += Constants.showTimeString;
				}
				timeControl.setFormat(format, showAll, showRaw, showYear, showTime, showAdjustment);

				updating = false;
			}
		}

		private void formatCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			toSettings();
		}

		private void displayCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			toSettings();
		}

		private void yearCheck_CheckChanged(object sender, RoutedEventArgs e)
		{
			toSettings();
		}

		private void timeCheck_CheckChanged(object sender, RoutedEventArgs e)
		{
			toSettings();
		}

		private void adjustmentCheck_CheckChanged(object sender, RoutedEventArgs e)
		{
			toSettings();
		}

	}
}
