using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;

namespace UltimateTimeGadgets
{
	public class DateTimeModel
	{
		public DateTimeOffset datetime = DateTimeOffset.Now;	// local time (and offset)
		public DateTimeOffset utcdatetime = DateTimeOffset.UtcNow;
		public TimeZoneInfo timeZone = TimeZoneInfo.Local;		// custom time zone
		DispatcherTimer secondTimer = new DispatcherTimer();
		DispatcherTimer minuteTimer = new DispatcherTimer();
		public Location location = new Location();
		public double lon = 0;
		public double lat = 0;
		public double elevation = 0;

		TimeZoneInfo beatTimeZone;

		public ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();

		bool autoUpdate = false;

		int lastSecond = -1;
		int lastMinute = -1;

		public DateTimeModel(bool autoUpdate)
		{
			this.autoUpdate = autoUpdate;

			string standardName = "Swatch Time";
			string displayName = "(UTC+01:00) Swatch Time";
			TimeZoneInfo beatTimeZone0 = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");							// Swatch / Biel time zone
			beatTimeZone = TimeZoneInfo.CreateCustomTimeZone(standardName, beatTimeZone0.BaseUtcOffset, displayName, standardName);

			if (autoUpdate)
			{
				datetime = DateTimeOffset.Now;
				timeZone = TimeZoneInfo.Local;
				secondTimer.Tick += new EventHandler(secondTimer_Tick);
				secondTimer.Interval = TimeSpan.FromMilliseconds(100);
				secondTimer.Start();
			}
		}

		public void copy(DateTimeModel datetimeModel)
		{
			this.datetime = datetimeModel.datetime;
			this.setLocation(datetimeModel.location);
			this.setTimeZone(datetimeModel.timeZone);
			this.setLonLat(datetimeModel.lon, datetimeModel.lat);
			this.elevation = datetimeModel.elevation;
		}

		void secondTimer_Tick(object sender, EventArgs e)
		{
			if (autoUpdate)
			{
				DateTimeOffset now = DateTimeOffset.Now;
				if (now.Second != lastSecond)
				{
					lastSecond = now.Second;
					datetime = TimeZoneInfo.ConvertTime(now, timeZone);
					utcdatetime = DateTimeOffset.UtcNow;

					NotifySecondsChanged("datetime");
					if (now.Minute != lastMinute)
					{
						lastMinute = now.Minute;
						datetime = TimeZoneInfo.ConvertTime(now, timeZone);
						NotifyMinutesChanged("datetime");
					}
				}
			}
		}

		public DateTimeOffset getDateTimeWithoutDST()
		{
			return getDateTimeWithoutDST(datetime, timeZone);
		}

		public DateTimeOffset getDateTimeWithoutDST(DateTimeOffset datetime0, TimeZoneInfo timeZone0)
		{
			string standardName = "Local Time no DST";
			TimeZoneInfo localNoDstTimeZone = TimeZoneInfo.CreateCustomTimeZone(standardName, timeZone0.BaseUtcOffset, standardName, standardName);
			DateTimeOffset plainDateTime = TimeZoneInfo.ConvertTime(datetime0, localNoDstTimeZone);
			return plainDateTime;
		}

		public double getBeatTime()
		{
			DateTimeOffset beattime = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, beatTimeZone);
			return beattime.TimeOfDay.TotalHours / 24 * 1000;
		}

		public event PropertyChangedEventHandler SecondsChanged;

		private void NotifySecondsChanged(String info)
		{
			if (SecondsChanged != null)
			{
				SecondsChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		public event PropertyChangedEventHandler MinutesChanged;

		private void NotifyMinutesChanged(String info)
		{
			if (MinutesChanged != null)
			{
				MinutesChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		public void setTimeZone(TimeZoneInfo timeZone)
		{
			this.timeZone = timeZone;
			NotifyTimeZoneChanged("timeZone");
		}

		public event PropertyChangedEventHandler TimeZoneChanged;

		private void NotifyTimeZoneChanged(String info)
		{
			if (TimeZoneChanged != null)
			{
				TimeZoneChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		public void setLocation(Location location)
		{
			this.location = location;
			if (location != null)
			{
				setLonLat(location.lon, location.lat);
				elevation = location.elevation0;
			}
		}

		public double getDstHours()
		{
			double delta = 0;

			if (timeZone.IsDaylightSavingTime(datetime.DateTime))
			{
				TimeZoneInfo.AdjustmentRule[] adjustments = timeZone.GetAdjustmentRules();
				int year = datetime.Year;
				foreach (TimeZoneInfo.AdjustmentRule adjustment in adjustments)
				{
					if (adjustment.DateStart.Year <= year && adjustment.DateEnd.Year >= year)
					{
						delta = adjustment.DaylightDelta.TotalHours;
					}
				}

			}
			return delta;
		}

		public void setLonLat(double lon, double lat)
		{
			this.lon = lon;
			this.lat = lat;
			updateLonLat();
		}

		public void updateLonLat()
		{
			NotifyLonLatChanged("lon");
			NotifyLonLatChanged("lat");
		}

		public void getTimeZoneFromLon()
		{
			double timeOffset = lon / 15;
			double dif;
			double mindif = 1000;
			TimeZoneInfo bestTimeZone = TimeZoneInfo.Local;

			foreach (TimeZoneInfo timeZone0 in timeZones)
			{
				dif = Math.Abs(timeZone0.BaseUtcOffset.TotalHours - timeOffset);
				if (dif < mindif)
				{
					mindif = dif;
					bestTimeZone = timeZone0;
				}
			}
			setTimeZone(bestTimeZone);
		}

		public event PropertyChangedEventHandler LonLatChanged;

		public void NotifyLonLatChanged(String info)
		{
			if (LonLatChanged != null)
			{
				LonLatChanged(this, new PropertyChangedEventArgs(info));
			}
		}

	}
}
