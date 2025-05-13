using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for TimeAdjustmentControl.xaml
	/// </summary>
	public partial class TimeAdjustmentControl : TimeControl, INotifyPropertyChanged
	{
		TimeZoneInfo.TransitionTime transitionStart = new TimeZoneInfo.TransitionTime();
		TimeZoneInfo.TransitionTime transitionEnd = new TimeZoneInfo.TransitionTime();
		string rawStart = "";
		string rawEnd = "";
		DateTime start = new DateTime();
		DateTime end = new DateTime();
		double delta = 0;
		string nextadjustmentString = "";
		DateTime nextadjustmentDate = new DateTime();

		string timeadjustmentFormat = "";

		public string timeadjustmentstring
		{
			get
			{
				if (delta != 0)
				{
					return string.Format(timeadjustmentFormat, start, end, delta, nextadjustmentString, nextadjustmentDate, rawStart, rawEnd);
				}
				return "No Adjustment";
			}
		}

		public TimeAdjustmentControl()
		{
			defaultWidth = 200;
			defaultHeight = 40;

			InitializeComponent();
			DataContext = this;
			init();
			update();

			setFormat("d/m/y", false, false, false, false, false);
		}

		public void setFormat(string dateFormat, bool showAll, bool showRaw, bool showYear, bool showTime, bool showAdjustment)
		{
			string fullformat = "";
			string format = "";

			if (showRaw && (transitionStart.IsFixedDateRule || transitionEnd.IsFixedDateRule))
			{
				showRaw = false;
				showAll = true;
			}

			if (!showRaw)
			{
				if (dateFormat == Constants.formatMDYString)
				{
					if (showYear)
					{
						format = "MM/dd/yyyy";
					}
					else
					{
						format = "MM/dd";
					}
				}
				else if (dateFormat == Constants.formatYMDString)
				{
					if (showYear)
					{
						format = "yyyy/MM/dd";
					}
					else
					{
						format = "MM/dd";
					}
				}
				else
				{
					if (showYear)
					{
						format = "dd/MM/yyyy";
					}
					else
					{
						format = "dd/MM";
					}
				}
			}

			if (showTime)
			{
				format += " HH:mm";
			}

			if (showRaw)
			{
				fullformat += "DST Start: {5}";
				if (showTime)
				{
					fullformat += " at{0:" + format + "}";
				}
				if (showAdjustment)
				{
					fullformat += " ({2} Hours)";
				}
				fullformat += "\nDST End: {6}";
				if (showTime)
				{
					fullformat += " at{1:" + format + "}";
				}
				if (showAdjustment)
				{
					fullformat += " ({2} Hours)";
				}
			}
			else if (showAll)
			{
				fullformat += "DST Start: {0:" + format + "}";
				if (showAdjustment)
				{
					fullformat += " ({2} Hours)";
				}
				fullformat += "\nDST End: {1:" + format + "}";
				if (showAdjustment)
				{
					fullformat += " ({2} Hours)";
				}
			}
			else
			{
				fullformat = "{3}{4:" + format + "}";
				if (showAdjustment)
				{
					fullformat += " ({2} Hours)";
				}
			}

			timeadjustmentFormat = fullformat;
			update();
		}

		public override void updateStyle(ColorStyle style, Color color, Brush brush, Brush background)
		{
			timeAdjustmentText.Foreground = brush;
			textBorder.Background = background;
		}

		void init()
		{
			datetimeModel.TimeZoneChanged += new System.ComponentModel.PropertyChangedEventHandler(datetimeModel_PropertyChanged);
		}

		public override void setDateTime(DateTimeModel datetimeModel)
		{
			this.datetimeModel = datetimeModel;
			init();
			update();
		}

		void datetimeModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			update();
		}

		void update()
		{
			DateTime dateTime = datetimeModel.datetime.DateTime;
			TimeZoneInfo timeZoneInfo = datetimeModel.timeZone;
			TimeZoneInfo.AdjustmentRule[] adjustments = timeZoneInfo.GetAdjustmentRules();
			DateTime nextstart = new DateTime();
			DateTime nextend = new DateTime();
			int year = datetimeModel.datetime.Year;

			start = new DateTime();
			end = new DateTime();
			nextstart = new DateTime();
			nextend = new DateTime();
			delta = 0;

			foreach (TimeZoneInfo.AdjustmentRule adjustment in adjustments)
			{
				if (adjustment.DateStart.Year <= year && adjustment.DateEnd.Year >= year)
				{
					transitionStart = adjustment.DaylightTransitionStart;
					transitionEnd = adjustment.DaylightTransitionEnd;
					rawStart = getRuleString(transitionStart);
					rawEnd = getRuleString(transitionEnd);
					start = getDateTime(transitionStart, year);
					end = getDateTime(transitionEnd, year);
					nextstart = getDateTime(transitionStart, year + 1);
					nextend = getDateTime(transitionEnd, year + 1);
					delta = adjustment.DaylightDelta.TotalHours;
				}
			}

			if (dateTime < start && (start < end || dateTime > end))
			{
				// not yet DST
				nextadjustmentString = "Clock forward: ";
				nextadjustmentDate = start;
			}
			else if (dateTime < end)
			{
				// currently in DST
				nextadjustmentString = "Clock backward: ";
				nextadjustmentDate = end;
			}
			else if (nextstart < nextend)
			{
				// after DST; before DST next year
				nextadjustmentString = "Clock forward: ";
				nextadjustmentDate = nextstart;
			}
			else
			{
				// after DST; before DST next year
				nextadjustmentString = "Clock backward: ";
				nextadjustmentDate = nextend;
			}

			NotifyPropertyChanged("timeadjustmentstring");
		}

		private DateTime getDateTime(TimeZoneInfo.TransitionTime transition,int year)
		{
			DateTime dateTime;
			DateTime time = transition.TimeOfDay;
			int month = transition.Month;
			int day = transition.Day;

			if (!transition.IsFixedDateRule)
			{
				// For non-fixed date rules, get local calendar
				Calendar cal = CultureInfo.CurrentCulture.Calendar;
				// Get first day of week for transition 
				// For example, the 3rd week starts no earlier than the 15th of the month 
				int startOfWeek = transition.Week * 7 - 6;
				// What day of the week does the month start on? 
				int firstDayOfWeek = (int)cal.GetDayOfWeek(new DateTime(year, transition.Month, 1));
				// Determine how much start date has to be adjusted 
				int changeDayOfWeek = (int)transition.DayOfWeek;

				if (firstDayOfWeek <= changeDayOfWeek)
					day = startOfWeek + (changeDayOfWeek - firstDayOfWeek);
				else
					day = startOfWeek + (7 - firstDayOfWeek + changeDayOfWeek);

				// Adjust for months with no fifth week 
				if (day > cal.GetDaysInMonth(year, transition.Month))
					day -= 7;
			}
			dateTime = new DateTime(year, month, day, time.Hour, time.Minute, time.Second);

			return dateTime;
		}

		string getRuleString(TimeZoneInfo.TransitionTime transition)
		{
			DateTimeFormatInfo dateFormat = CultureInfo.CurrentCulture.DateTimeFormat;
			string nth = "";

			if (!transition.IsFixedDateRule)
			{
				switch (transition.Week)
				{
					case 2: nth = "second"; break;
					case 3: nth = "third"; break;
					case 4: nth = "fourth"; break;
					case 5: nth = "last"; break;
					default: nth = "first"; break;
				}
				return string.Format("{0} {1} of {2}", nth, transition.DayOfWeek, dateFormat.GetMonthName(transition.Month));
			}
			return "";
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
