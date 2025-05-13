using System;
using System.ComponentModel;

namespace UltimateTimeGadgets
{
	class DateTimeFormatter : INotifyPropertyChanged
	{
		DateTimeModel datetimeModel;
		string format = "{0}";

		public string datetimestring
		{
			get
			{
				int dayi = 0;
				// set day of the week number correctly
				DayOfWeek day = datetimeModel.datetime.DayOfWeek;
				switch (day)
				{
					case DayOfWeek.Sunday: dayi = 1; break;
					case DayOfWeek.Monday: dayi = 2; break;
					case DayOfWeek.Tuesday: dayi = 3; break;
					case DayOfWeek.Wednesday: dayi = 4; break;
					case DayOfWeek.Thursday: dayi = 5; break;
					case DayOfWeek.Friday: dayi = 6; break;
					case DayOfWeek.Saturday: dayi = 7; break;
				}

				string dayPart;
				// set day part
				int hour = datetimeModel.datetime.TimeOfDay.Hours;
				if (hour < 6)
				{
					dayPart = "night";
				}
				else if (hour < 12)
				{
					dayPart = "morning";
				}
				else if (hour < 18)
				{
					dayPart = "afternoon";
				}
				else
				{
					dayPart = "evening";
				}

				return string.Format(format, datetimeModel.datetime, dayi, datetimeModel.utcdatetime, dayPart);
			}
		}

		public DateTimeFormatter()
		{
			setModel(new DateTimeModel(false));
			init();
		}

		public DateTimeFormatter(DateTimeModel datetimeModel)
		{
			setModel(datetimeModel);
			init();
		}

		public void setModel(DateTimeModel datetimeModel)
		{
			this.datetimeModel = datetimeModel;
			init();
		}

		void init()
		{
			datetimeModel.SecondsChanged += new PropertyChangedEventHandler(datetimeModel_PropertyChanged);
			datetimeModel.TimeZoneChanged += new PropertyChangedEventHandler(datetimeModel_PropertyChanged);
		}

		public void setFormat(string format)
		{
			this.format = format;
			NotifyPropertyChanged("datetimestring");
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		void datetimeModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			NotifyPropertyChanged("datetimestring");
		}

	}
}
