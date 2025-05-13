using System.Windows;
using System.Windows.Controls;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for DateSettingsWindow.xaml
	/// </summary>
	public partial class DateSettingsWindow : SettingsWindow
	{
		DateControl timeControl;

		public DateSettingsWindow(TimeControl timeControl, ControlSettings settings)
		{
			this.timeControl = (DateControl)timeControl;
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
				int y = format.ToLower().LastIndexOf("y");
				int m = format.ToLower().LastIndexOf("m");
				int d = format.ToLower().LastIndexOf("d");

				timeControl.setFormat(format);

				if (d < m && m < y)
				{
					// d/m/y
					formatCombo.SelectedIndex = 1;
				}
				else if (m < d && d < y)
				{
					// m/d/y
					formatCombo.SelectedIndex = 2;
				}
				else
				{
					// y/m/d
					formatCombo.SelectedIndex = 0;
				}

				if (format.Contains("yyyy"))
				{
					yearCombo.SelectedIndex = 0;
				}
				else
				{
					yearCombo.SelectedIndex = 1;
				}

				if (format.Contains("MMMM"))
				{
					monthCombo.SelectedIndex = 1;
				}
				else if (format.Contains("MMM"))
				{
					monthCombo.SelectedIndex = 2;
				}
				else
				{
					monthCombo.SelectedIndex = 0;
				}

				if (format.Contains("dddd"))
				{
					weekDayCheck.IsChecked = true;
					weekDayCombo.SelectedIndex = 0;
				}
				else if (format.Contains("ddd"))
				{
					weekDayCheck.IsChecked = true;
					weekDayCombo.SelectedIndex = 1;
				}
				else if (format.Contains("n"))
				{
					weekDayCheck.IsChecked = true;
					weekDayCombo.SelectedIndex = 2;
				}
				else
				{
					weekDayCheck.IsChecked = false;
				}

				string[] parts = format.Split(new char[] { ' ', '/' });
				foreach (string part in parts)
				{
					if (part == "dd")
					{
						zerosCheck.IsChecked = true;
					}
					if (part == "d")
					{
						zerosCheck.IsChecked = false;
					}
				}
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
				string year, month, date;
				string separator = " ";

				if (yearCombo.SelectedIndex == 1)
				{
					year = "yy";
				}
				else
				{
					year = "yyyy";
				}

				switch (monthCombo.SelectedIndex)
				{
					case 1: month = "MMMM"; break;
					case 2: month = "MMM"; break;
					default: month = "MM"; separator = "/"; break;
				}

				if (!(bool)zerosCheck.IsChecked)
				{
					if (monthCombo.SelectedIndex == 0)
					{
						month = "M";
					}
					date = "d";
				}
				else
				{
					date = "dd";
				}

				switch (formatCombo.SelectedIndex)
				{
					case 1: format = string.Format("{0}{3}{1}{3}{2}", date, month, year, separator); break;
					case 2: format = string.Format("{0}{3}{1}{3}{2}", month, date, year, separator); break;
					default: format = string.Format("{0}{3}{1}{3}{2}", year, month, date, separator); break;
				}

				if ((bool)weekDayCheck.IsChecked)
				{
					if (weekDayCombo.SelectedIndex == 0)
					{
						format = "dddd " + format;
					}
					else if (weekDayCombo.SelectedIndex == 1)
					{
						format = "ddd " + format;
					}
				}

				settings.format = format;
				timeControl.setFormat(format);

				if ((bool)weekDayCheck.IsChecked && weekDayCombo.SelectedIndex == 2)
				{
					format = "n " + format;
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

		private void yearCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			toSettings();
		}

		private void monthCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			toSettings();
		}

		private void weekDayCheck_CheckChanged(object sender, RoutedEventArgs e)
		{
			weekDayCombo.IsEnabled = (bool)weekDayCheck.IsChecked;
			if (weekDayCombo.IsEnabled && weekDayCombo.SelectedIndex < 0)
			{
				weekDayCombo.SelectedIndex = 0;
			}
			toSettings();
		}

		private void weekDayCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			toSettings();
		}

		private void zerosCheck_CheckChanged(object sender, RoutedEventArgs e)
		{
			toSettings();
		}

	}
}
