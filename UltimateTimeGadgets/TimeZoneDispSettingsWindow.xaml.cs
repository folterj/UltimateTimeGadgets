using System.Windows.Controls;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for TimeZoneDispSettingsWindow.xaml
	/// </summary>
	public partial class TimeZoneDispSettingsWindow : SettingsWindow
	{
		TimeZoneDispControl timeControl;

		public TimeZoneDispSettingsWindow(TimeControl timeControl, ControlSettings settings)
		{
			this.timeControl = (TimeZoneDispControl)timeControl;
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

				string format = settings.format;

				if (format.Contains("1"))
				{
					formatCombo.SelectedIndex = 1;
				}
				else if (format.Contains("2"))
				{
					formatCombo.SelectedIndex = 2;
				}
				else
				{
					formatCombo.SelectedIndex = 0;
				}

				timeControl.setFormat(format);

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

				switch (formatCombo.SelectedIndex)
				{
					case 1: format += "{1}"; break;
					case 2: format += "UTC{2:'+'00;'-'00}:{3:00}"; break;
					default: format += "{0}"; break;
				}

				settings.format = format;
				timeControl.setFormat(format);

				updating = false;
			}
		}

		private void formatCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			toSettings();
		}

	}
}
