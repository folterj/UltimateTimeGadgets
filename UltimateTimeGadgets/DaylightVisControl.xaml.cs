using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for DaylightVisControl.xaml
	/// </summary>
	public partial class DaylightVisControl : TimeControl
	{
		public double sunRiseHours = 0;
		public double sunSetHours = 0;

		int width, height;
		double fontSize;

		bool initialised = false;


		public DaylightVisControl()
		{
			InitializeComponent();
			init();

			defaultWidth = 200;
			defaultHeight = 100;
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
			redraw();
		}

		public override void updateStyle(ColorStyle style, Color color, Brush brush, Brush background)
		{
			redraw();
		}

		private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			// only able to read width/height properties after resize event
			width = (int)e.NewSize.Width;
			height = (int)e.NewSize.Height;

			initialised = true;
			redraw();
		}

		void datetimeModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			redraw();
		}

		void calculate(int dayOfYear)
		{
			DateTimeModel dayOfYearModel = new DateTimeModel(false);
			double daylightAdjustment;
			double lat = datetimeModel.lat;
			double lon = datetimeModel.lon;

			dayOfYearModel.copy(datetimeModel);
			dayOfYearModel.datetime = new DateTime(datetimeModel.datetime.Year, 1, 1) + new TimeSpan(dayOfYear, 0, 0, 0);	// dayOfYear: 0-based
			
			daylightAdjustment = -dayOfYearModel.getDstHours();	// optional?

			double clockSunHours = Sun.calcSunHours0(dayOfYearModel, Sun.clockHorAngle);

			double meridian = datetimeModel.timeZone.BaseUtcOffset.TotalHours;
			double longitudeAdjustment = lon / 15 - meridian;

			double eot = Sun.calcEOT(dayOfYear);

			TimeSpan clockSunRise = Sun.getSunRise(clockSunHours, longitudeAdjustment, daylightAdjustment, eot);
			TimeSpan clockSunSet = Sun.getSunSet(clockSunHours, longitudeAdjustment, daylightAdjustment, eot);

			sunRiseHours = clockSunRise.TotalHours;
			sunSetHours = clockSunSet.TotalHours;
		}

		void redraw()
		{
			int year, dayOfYear, maxDays;
			double vx0, vx, vy;
			double fontHeight, vfontHeight;
			DateTime date;
			string s;

			if (!initialised)
				return;

			fontSize = height / 24;
			fontHeight = fontSize * 1.25;
			vfontHeight = fontHeight / height;

			year = datetimeModel.datetime.Year;
			maxDays = Sun.daysInYear(year);

			MainCanvas.Children.Clear();

			if (style == ColorStyle.Opaque)
			{
				fillBackground(Brushes.White);
			}

			for (int hour = 0; hour < 24; hour++)
			{
				vy = (double)hour / 24;
				addLine(0, vy, 1, vy, Brushes.LightGray);

				s = string.Format("{0}", hour);
				vy += vfontHeight;
				addText(0, vy, s);
			}
			for (int month = 0; month < 12; month++)
			{
				vx = (double)month / 12;
				addLine(vx, 0, vx, 1, Brushes.LightGray);

				vx += 0.3 / 12;
				date = new DateTime(year, month + 1, 1);
				s = string.Format("{0:MMM}", date);
				addText(vx, vfontHeight, s);
			}

			for (dayOfYear = 0; dayOfYear < maxDays; dayOfYear++)
			{
				vx0 = (double)(dayOfYear - 1) / maxDays;
				vx = (double)dayOfYear / maxDays;
				calculate(dayOfYear);
				vy = sunRiseHours / 24;
				addLine(vx0, vy, vx, vy, Brushes.Black);

				vy = sunSetHours / 24;
				addLine(vx0, vy, vx, vy, Brushes.Black);
			}

			dayOfYear = datetimeModel.datetime.DayOfYear;
			vx = (double)dayOfYear / maxDays;
			addLine(vx, 0, vx, 1, Brushes.Blue);
		}

		void addLine(double vx1, double vy1, double vx2, double vy2, Brush stroke)
		{
			Line line = new Line();
			line.X1 = calcX(vx1);
			line.Y1 = calcY(vy1);
			line.X2 = calcX(vx2);
			line.Y2 = calcY(vy2);
			line.Stroke = stroke;

			MainCanvas.Children.Add(line);
		}

		void addText(double vx, double vy, string s)
		{
			TextBlock text = new TextBlock();
			text.Text = s;
			text.FontSize = fontSize;

			MainCanvas.Children.Add(text);

			Canvas.SetLeft(text, calcX(vx));
			Canvas.SetTop(text, calcY(vy));
		}

		void fillBackground(Brush fill)
		{
			Rectangle rect = new Rectangle();
			rect.Fill = fill;
			rect.Width = width;
			rect.Height = height;

			MainCanvas.Children.Add(rect);

			Canvas.SetLeft(rect, 0);
			Canvas.SetTop(rect, 0);
		}

		double calcX(double vx)
		{
			return vx * width;
		}

		double calcY(double vy)
		{
			return (1 - vy) * height;
		}

	}
}
