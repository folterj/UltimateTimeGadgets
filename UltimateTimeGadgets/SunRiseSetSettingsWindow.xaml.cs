using System.Windows.Controls;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for SunRiseSetSettingsWindow.xaml
	/// </summary>
	public partial class SunRiseSetSettingsWindow : SettingsWindow
	{
		SunRiseSetControl timeControl;

		public SunRiseSetSettingsWindow(TimeControl timeControl, ControlSettings settings)
		{
			this.timeControl = (SunRiseSetControl)timeControl;
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

				bool showRiseSet = settings.format.Contains(Constants.showRiseSetString);
				bool showHours = settings.format.Contains(Constants.showHoursString);
				bool showCountdown = settings.format.Contains(Constants.showCountdownString);
				bool showClock = settings.format.Contains(Constants.showClockString);
				bool showCivil = settings.format.Contains(Constants.showCivilString);

				timeControl.setFormat(showRiseSet, showHours, showCountdown, showClock, showCivil);

				if (showHours)
				{
					typeCombo.SelectedIndex = 1;
				}
				else if (showCountdown)
				{
					typeCombo.SelectedIndex = 2;
				}
				else
				{
					typeCombo.SelectedIndex = 0;
				}
				if (showClock && !showCivil)
				{
					clockCivilCombo.SelectedIndex = 1;
				}
				else if (showCivil && !showClock)
				{
					clockCivilCombo.SelectedIndex = 2;
				}
				else
				{
					clockCivilCombo.SelectedIndex = 0;
				}

				updating = false;
			}
		}

		public override void toSettings()
		{
			// convert from form to settings
			if (!updating)
			{
				updating = true;

				bool showRiseSet = false;
				bool showHours = false;
				bool showCountdown = false;
				bool showClock = false;
				bool showCivil = false;

				switch (typeCombo.SelectedIndex)
				{
					case 1: showHours = true; break;
					case 2: showCountdown = true; break;
					default: showRiseSet = true; break;
				}
				switch (clockCivilCombo.SelectedIndex)
				{
					case 1: showClock = true; break;
					case 2: showCivil = true; break;
					default: showClock = true; showCivil = true; break;
				}

				settings.format = "";
				if (showRiseSet)
				{
					settings.format += Constants.showRiseSetString;
				}
				if (showHours)
				{
					settings.format += Constants.showHoursString;
				}
				if (showCountdown)
				{
					settings.format += Constants.showCountdownString;
				}
				if (showClock)
				{
					settings.format += Constants.showClockString;
				}
				if (showCivil)
				{
					settings.format += Constants.showCivilString;
				}
				timeControl.setFormat(showRiseSet, showHours, showCountdown, showClock, showCivil);

				updating = false;
			}
		}

		private void displayCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			toSettings();
		}

		private void clockCivilCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			toSettings();
		}

	}
}
