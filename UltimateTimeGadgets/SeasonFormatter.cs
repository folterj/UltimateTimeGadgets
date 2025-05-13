using System;
using System.ComponentModel;

namespace UltimateTimeGadgets
{
	class SeasonFormatter : INotifyPropertyChanged
	{
		DateTimeModel datetimeModel;
		string format = "{0}";

		int season1Day = 0;
		int season2Day = 0;
		int season3Day = 0;
		int season4Day = 0;

		public string seasonstring
		{
			get
			{
				int day = datetimeModel.datetime.Date.DayOfYear;
				double lat = datetimeModel.lat;
				int season;
				string s = "-";

				if (day >= season4Day)
				{
					season = 4;	// winter
				}
				else if (day >= season3Day)
				{
					season = 3;	// autumn
				}
				else if (day >= season2Day)
				{
					season = 2;	// summer
				}
				else if (day >= season1Day)
				{
					season = 1;	// spring
				}
				else
				{
					season = 4;	// winter
				}
				if (lat >= 0)
				{
					// north
					switch (season)
					{
						case 1: s = "Spring"; break;
						case 2: s = "Summer"; break;
						case 3: s = "Autumn"; break;
						case 4: s = "Winter"; break;
					}
				}
				else
				{
					// south
					switch (season)
					{
						case 1: s = "Autumn"; break;
						case 2: s = "Winter"; break;
						case 3: s = "Spring"; break;
						case 4: s = "Summer"; break;
					}
				}
				return string.Format(format, s);
			}
		}

		public SeasonFormatter()
		{
			setModel(new DateTimeModel(false));
			init();
		}

		public SeasonFormatter(DateTimeModel datetimeModel)
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
			season1Day = getDay(3, 21);		// spring
			season2Day = getDay(6, 21);		// summer
			season3Day = getDay(9, 21);		// autumn
			season4Day = getDay(12, 21);	// winter

			datetimeModel.TimeZoneChanged += new PropertyChangedEventHandler(datetimeModel_PropertyChanged);
		}

		int getDay(int month, int day)
		{
			int days = day;

			for (int m = 1; m < month; m++)
			{
				days += DateTime.DaysInMonth(datetimeModel.datetime.Year, m);
			}
			return days;
		}

		public void setFormat(string format)
		{
			this.format = format;
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
			NotifyPropertyChanged("seasonstring");
		}

	}
}
