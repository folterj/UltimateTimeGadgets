using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace UltimateTimeGadgets
{
	class TimeZoneFormatter : INotifyPropertyChanged
	{
		DateTimeModel datetimeModel;
		string format = "{0}";

		public ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();

		public string timezonestring
		{
			get
			{
				TimeZoneInfo timeZone = datetimeModel.timeZone;
				return string.Format(format, timeZone, timeZone.Id, timeZone.BaseUtcOffset.Hours, timeZone.BaseUtcOffset.Minutes);
			}
			set
			{
				foreach (TimeZoneInfo timeZone in timeZones)
				{
					if (timeZone.ToString() == value)
					{
						datetimeModel.setTimeZone(timeZone);
					}
				}
			}
		}

		public TimeZoneFormatter()
		{
			setModel(new DateTimeModel(false));
			init();
		}

		public TimeZoneFormatter(DateTimeModel datetimeModel)
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
			datetimeModel.TimeZoneChanged += new PropertyChangedEventHandler(datetimeModel_PropertyChanged);
		}

		public void setFormat(string format)
		{
			this.format = format;
			NotifyPropertyChanged("timezonestring");
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
			NotifyPropertyChanged("timezonestring");
		}
	}
}
