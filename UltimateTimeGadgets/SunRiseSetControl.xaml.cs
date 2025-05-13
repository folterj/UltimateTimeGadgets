using System;
using System.ComponentModel;
using System.Windows.Media;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for SunRiseSetControl.xaml
	/// </summary>
	public partial class SunRiseSetControl : TimeControl, INotifyPropertyChanged
	{
		public double clockSunHours = 0;
		public double civilSunHours = 0;
		public TimeSpan clockSunRise = new TimeSpan();
		public TimeSpan clockSunSet = new TimeSpan();
		public TimeSpan civilSunRise = new TimeSpan();
		public TimeSpan civilSunSet = new TimeSpan();

		string nextSunRiseSetString = "";
		string nextDawnDuskString = "";
		TimeSpan nextSunRiseSetTime = new TimeSpan();
		TimeSpan nextDawnDuskTime = new TimeSpan();

		string sunrisesetFormat = "";

		public string sunrisesetstring
		{
			get { return string.Format(sunrisesetFormat,
										clockSunRise, clockSunSet, clockSunHours, 24 - clockSunHours, nextSunRiseSetString, nextSunRiseSetTime,
										civilSunRise, civilSunSet, civilSunHours, 24 - civilSunHours, nextDawnDuskString, nextDawnDuskTime);
			}
		}

		public SunRiseSetControl()
		{
			defaultWidth = 200;
			defaultHeight = 40;

			InitializeComponent();
			DataContext = this;
			init();
			update();

			setFormat(true, false, false, true, true);
		}

		public void setFormat(bool showRiseSet, bool showHours, bool showCountdown, bool showClock, bool showCivil)
		{
			string format = "";

			if (showClock)
			{
				if (showRiseSet)
				{
					// rise / set times
					format += "Sunrise: {0:hh\\:mm} Sunset: {1:hh\\:mm}";
				}
				if (showHours)
				{
					// hours day / night
					format += "Day: {2:F1} [h] Night: {3:F1} [h]";
				}
				if (showCountdown)
				{
					// hours left before next rise / set
					format += "{4}: {5:h\\:mm} [h:m]";
				}
			}


			if (format != "")
			{
				format += "\n";
			}

			if (showCivil)
			{
				if (showRiseSet)
				{
					// rise / set times
					format += "Dawn: {6:hh\\:mm} Dusk: {7:hh\\:mm}";
				}
				if (showHours)
				{
					// hours day / night
					format += "Light: {8:F1} [h] Dark: {9:F1} [h]";
				}
				if (showCountdown)
				{
					// hours left before next rise / set
					format += "{10}: {11:h\\:mm} [h:m]";
				}
			}
			
			sunrisesetFormat = format;
			update();
		}

		public override void updateStyle(ColorStyle style, Color color, Brush brush, Brush background)
		{
			sunRiseSetText.Foreground = brush;
			textBorder.Background = background;
		}

		void init()
		{
			datetimeModel.MinutesChanged += new System.ComponentModel.PropertyChangedEventHandler(datetimeModel_PropertyChanged);
			datetimeModel.TimeZoneChanged += new System.ComponentModel.PropertyChangedEventHandler(datetimeModel_PropertyChanged);
			datetimeModel.LonLatChanged += new System.ComponentModel.PropertyChangedEventHandler(datetimeModel_PropertyChanged);
		}

		public override void setDateTime(DateTimeModel datetimeModel)
		{
			this.datetimeModel = datetimeModel;
			init();
			update();
		}

		void datetimeModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			update();
		}

		void update()
		{
			DateTime datetime = datetimeModel.datetime.DateTime;
			int dayOfYear = datetimeModel.datetime.DayOfYear;
			double daylightAdjustment = -datetimeModel.getDstHours();
			double lat = datetimeModel.lat;
			double lon = datetimeModel.lon;
			TimeSpan time = datetime.TimeOfDay;
			TimeSpan hours24 = new TimeSpan(24, 0, 0);

			clockSunHours = Sun.calcSunHours0(datetimeModel, Sun.clockHorAngle);
			civilSunHours = Sun.calcSunHours0(datetimeModel, Sun.civilHorAngle);

			double meridian = datetimeModel.timeZone.BaseUtcOffset.TotalHours;
			double longitudeAdjustment = lon / 15 - meridian;

			double eot = Sun.calcEOT(dayOfYear);

			clockSunRise = Sun.getSunRise(clockSunHours, longitudeAdjustment, daylightAdjustment, eot);
			clockSunSet = Sun.getSunSet(clockSunHours, longitudeAdjustment, daylightAdjustment, eot);
			civilSunRise = Sun.getSunRise(civilSunHours, longitudeAdjustment, daylightAdjustment, eot);
			civilSunSet = Sun.getSunSet(civilSunHours, longitudeAdjustment, daylightAdjustment, eot);

			TimeSpan timeToRise = clockSunRise - time;
			TimeSpan timeToSet = clockSunSet - time;
			TimeSpan timeToDawn = civilSunRise - time;
			TimeSpan timeToDusk = civilSunSet - time;

			if (timeToSet < TimeSpan.Zero)
			{
				// move to next day
				timeToRise = timeToRise.Add(hours24);
				timeToSet = timeToSet.Add(hours24);
			}
			if (timeToDusk < TimeSpan.Zero)
			{
				// move to next day
				timeToDawn = timeToDawn.Add(hours24);
				timeToDusk = timeToDusk.Add(hours24);
			}

			if (timeToRise >= TimeSpan.Zero)
			{
				nextSunRiseSetString = "Sunrise in";
				nextSunRiseSetTime = timeToRise;
			}
			else
			{
				nextSunRiseSetString = "Sunset in";
				nextSunRiseSetTime = timeToSet;
			}

			if (timeToDawn >= TimeSpan.Zero)
			{
				nextDawnDuskString = "Dawn in";
				nextDawnDuskTime = timeToDawn;
			}
			else
			{
				nextDawnDuskString = "Dusk in";
				nextDawnDuskTime = timeToDusk;
			}

			NotifyPropertyChanged("sunrisesetstring");
		}
		
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

	}
}
