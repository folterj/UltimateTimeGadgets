using System;

namespace UltimateTimeGadgets
{
	class Sun
	{
		public static double deg2rad = Math.PI / 180;
		public static double rad2deg = 1 / deg2rad;

		public static double clockHorAngle = 0.8333;
		public static double civilHorAngle = 6;

		public static double calcSunHours0(DateTimeModel datetimeModel, double horAngle)
		{
			double sunHours = 0;
			DateTime datetime = datetimeModel.datetime.DateTime;
			double lat = datetimeModel.lat;
			double lon = datetimeModel.lon;
			double elevation = 0;

			double meridian = datetimeModel.timeZone.BaseUtcOffset.TotalHours;
			double longitudeAdjustment = lon / 15 - meridian;

			double correctedHours = datetime.TimeOfDay.TotalHours + longitudeAdjustment;

			int correctedYear, correctedMonth;
			if (datetime.Month > 2)
			{
				correctedYear = datetime.Year;
				correctedMonth = datetime.Month - 3;
			}
			else
			{
				correctedYear = datetime.Year - 1;
				correctedMonth = datetime.Month + 9;
			}

			double t = (correctedHours / 24 + datetime.Day + Math.Round(30.6 * correctedMonth) + Math.Floor(365.25 * (correctedYear - 1976)) - 8707.5) / 36525;
			double m = (357.528 + 35999.05 * t) % 360;
			double c = (1.915 * Math.Sin(m * deg2rad)) + (0.020 * Math.Sin(2 * m * deg2rad));
			double l = (280.460 + (36000.770 * t) + c) % 360;
			double alpha = l - 2.466 * Math.Sin(2 * l * deg2rad) + 0.053 * Math.Sin(4 * l * deg2rad);
			double obliquity = 23.4393 - 0.013 * t;
			double declination = Math.Atan(Math.Tan(obliquity * deg2rad) * Math.Sin(alpha * deg2rad)) * rad2deg;

			double hourLsot = -(Math.Sin(lat * deg2rad) * Math.Sin(declination * deg2rad) - Math.Sin((-horAngle - 0.0347 * Math.Sqrt(elevation)) * deg2rad)) /
								(Math.Cos(lat * deg2rad) * Math.Cos(declination * deg2rad));

			if (hourLsot > -1 && hourLsot < 1)
			{
				sunHours = rad2deg * Math.Acos(hourLsot) / 15 * 2;
			}
			return sunHours;
		}

		public static double calcSunHours(DateTimeModel datetimeModel, double horAngle)
		{
			DateTime datetime = datetimeModel.datetime.DateTime;
			int dayOfYear = datetimeModel.datetime.DayOfYear;
			double lat = datetimeModel.lat;

			double p = Math.Asin(0.39795 * Math.Cos(0.2163108 + 2 * Math.Atan(0.9671396 * Math.Tan(0.00860 * (dayOfYear - 186)))));
			double clockSunHours = 24 - (24 / Math.PI) * Math.Acos((Math.Sin(horAngle * deg2rad) + Math.Sin(lat * deg2rad) * Math.Sin(p)) / Math.Cos(lat * deg2rad) * Math.Cos(p));
			return clockSunHours;
		}

		public static double calcSunHours2(DateTimeModel datetimeModel, double horAngle)
		{
			double lat = datetimeModel.lat;
			double lon = datetimeModel.lon;

			//JulianCalendar j = new JulianCalendar();
			//int jd = j.GetDayOfYear(DateTime.Now);

			double jd2000 = 2451545.0009;

			double jd = DateTime.Now.Date.ToOADate() + 2415019;		// example: 31 dec 2012 (00:00) = 2456293.0
			double n0 = (jd - jd2000) - lon / 360;
			double n = n0 + 0.5;

			// double j = 2451545.0009 + lon / 360 + n;
			double j = jd + 0.5;	// today at noon

			double m = (357.5291 + 0.98560028 * (j - jd2000)) % 360;
			double c = 1.9148 * Math.Sin(m * deg2rad) + 0.0200 * Math.Sin(2 * m * deg2rad) + 0.0003 * Math.Sin(3 * m * deg2rad);
			double l = (m + 102.9372 + c + 180) % 360;

			double jt = j + 0.0053 * Math.Sin(m * deg2rad) - 0.0069 * Math.Sin(2 * l * deg2rad);

			double declination = Math.Asin(Math.Sin(l * deg2rad) * Math.Sin(23.45 * deg2rad)) * rad2deg;	// in degrees

			double w0 = Math.Acos((Math.Sin(-horAngle * deg2rad) - Math.Sin(lat * deg2rad) * Math.Sin(declination * deg2rad)) / (Math.Cos(lat * deg2rad) * Math.Cos(declination * deg2rad))) * rad2deg; // in degrees

			//double jset = jd2000 + (w0 + lat) / 360 + n + 0.0053 * Math.Sin(m * deg2rad) - 0.0069 * Math.Sin(2 * l * deg2rad);
			//double jset = j + w0 / 360 + 0.0053 * Math.Sin(m * deg2rad) - 0.0069 * Math.Sin(2 * l * deg2rad);

			//DateTime dset = DateTime.FromOADate(jset - 2415019);

			double clockSunHours = (w0 / 360 + 0.0053 * Math.Sin(m * deg2rad) - 0.0069 * Math.Sin(2 * l * deg2rad)) * 24 * 2;

			return clockSunHours;
		}

		public static TimeSpan getSunRise(double sunHours, double longitudeAdjustment, double daylightAdjustment, double eot)
		{
			return getSunRiseSet(sunHours, longitudeAdjustment, daylightAdjustment, eot, true);
		}

		public static TimeSpan getSunSet(double sunHours, double longitudeAdjustment, double daylightAdjustment, double eot)
		{
			return getSunRiseSet(sunHours, longitudeAdjustment, daylightAdjustment, eot, false);
		}

		public static TimeSpan getSunRiseSet(double sunHours, double longitudeAdjustment, double daylightAdjustment, double eot, bool rise)
		{
			double riseSet = 12 - longitudeAdjustment - daylightAdjustment - eot;
			if (rise)
			{
				riseSet -= sunHours / 2;
			}
			else
			{
				riseSet += sunHours / 2;
			}
			while (riseSet < 0) riseSet += 24;
			while (riseSet > 24) riseSet -= 24;
			int riseSetHour = (int)riseSet;
			riseSet -= riseSetHour;
			riseSet *= 60;
			int riseSetMin = (int)riseSet;
			riseSet -= riseSetMin;
			riseSet *= 60;
			int riseSetSec = (int)riseSet;
			return new TimeSpan(riseSetHour, riseSetMin, riseSetSec);
		}

		public static double calcEOT(int dayOfYear)
		{
			double w = 360 / 365.34;	// the Earth's mean angular orbital velocity in degrees per day
			int d = dayOfYear - 1;	// date in days starting at zero on January 1
			double a = w * (d + 10);	// angle the earth would move on its orbit at its average speed from the December solstice to date d
			double b = a + (360 / Math.PI) * 0.0167 * Math.Sin(w * deg2rad * (d - 2));	// angle the Earth moves from the solstice to date d
			double c = (a - Math.Atan(Math.Tan(b * deg2rad) / Math.Cos(23.44 * deg2rad)) * rad2deg) / 180;	// difference between the angles moved at mean speed, and at the corrected speed projected onto the equatorial plane
			double eot = 720 * (c - Math.Round(c));	// EoT is the equation of time in minutes (common units)
			return eot / 60;						// convert to hours
		}

		public static int daysInYear(int year)
		{
			if (DateTime.IsLeapYear(year))
			{
				return 366;
			}
			else
			{
				return 365;
			}
		}

	}
}
