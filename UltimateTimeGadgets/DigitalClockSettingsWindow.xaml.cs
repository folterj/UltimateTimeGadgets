using System.Windows;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for DigitalClockSettingsWindow.xaml
	/// </summary>
	public partial class DigitalClockSettingsWindow : SettingsWindow
	{
		DigitalClockControl timeControl;

		public DigitalClockSettingsWindow(TimeControl timeControl, ControlSettings settings)
		{
			this.timeControl = (DigitalClockControl)timeControl;
			this.settings = settings;

			InitializeComponent();

			fromSettings();
			initDone = true;
		}

		public override void fromSettings()
		{
			// convert from settings to form
			if (!updating)
			{
				updating = true;

				string format = settings.format;

				hour24Check.IsChecked = format.Contains("H");
				secondsCheck.IsChecked = format.Contains("s");
				zerosCheck.IsChecked = format.ToLower().Contains("hh");
				utcCheck.IsChecked = format.ToLower().Contains("z");

				timeControl.setFormat(format);

				updating = false;
			}
		}

		public override void toSettings()
		{
			// convert from form to settings
			if (initDone && !updating)
			{
				updating = true;

				string format = "";

				if ((bool)secondsCheck.IsChecked)
				{
					format = ":ss";
				}
				if ((bool)hour24Check.IsChecked)
				{
					if ((bool)zerosCheck.IsChecked)
					{
						format = "HH:mm" + format;
					}
					else
					{
						format = "H:mm" + format;
					}
				}
				else
				{
					if ((bool)zerosCheck.IsChecked)
					{
						format = "hh:mm" + format + " tt";
					}
					else
					{
						format = "h:mm" + format + " tt";
					}
				}
				if ((bool)utcCheck.IsChecked)
				{
					format += " zzz";
				}
				settings.format = format;
				timeControl.setFormat(format);

				updating = false;
			}
		}

		private void hour24Check_CheckChanged(object sender, RoutedEventArgs e)
		{
			toSettings();
		}

		private void secondsCheck_CheckChanged(object sender, RoutedEventArgs e)
		{
			toSettings();
		}

		private void zerosCheck_CheckChanged(object sender, RoutedEventArgs e)
		{
			toSettings();
		}

		private void utcCheck_CheckChanged(object sender, RoutedEventArgs e)
		{
			toSettings();
		}

	}
}
