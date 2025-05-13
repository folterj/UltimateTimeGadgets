using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for DaylightdialControl.xaml
	/// </summary>
	public partial class DaylightdialControl : TimeControl
	{
		public double sunRiseHours = 0;
		public double sunSetHours = 0;

		double radius = 0.885;
		double handWidth = 0.05;

		WriteableBitmap outImage;
		UInt32[,] outImageData;

		int width, height;

		bool initialised = false;


		public DaylightdialControl()
		{
			InitializeComponent();
			init();

			defaultWidth = 100;
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

			outImageData = new UInt32[height, width];
			outImage = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

			MainImage.Source = outImage;
			initialised = true;
			redraw();
		}

		void datetimeModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			redraw();
		}

		void update()
		{
			int dayOfYear = datetimeModel.datetime.DayOfYear;
			double daylightAdjustment = -datetimeModel.getDstHours();
			double lat = datetimeModel.lat;
			double lon = datetimeModel.lon;

			double clockSunHours = Sun.calcSunHours0(datetimeModel, Sun.clockHorAngle);

			double meridian = datetimeModel.timeZone.BaseUtcOffset.TotalHours;
			double longitudeAdjustment = lon / 15 - meridian;

			double eot = Sun.calcEOT(dayOfYear);

			TimeSpan clockSunRise = Sun.getSunRise(clockSunHours, longitudeAdjustment, daylightAdjustment, eot);
			TimeSpan clockSunSet = Sun.getSunSet(clockSunHours, longitudeAdjustment, daylightAdjustment, eot);

			sunRiseHours = clockSunRise.TotalHours;
			sunSetHours = clockSunSet.TotalHours;
		}

		public void redraw()
		{
			double hours, angle, dist, dx, dy;
			double lineWidth;

			if (!initialised)
				return;

			lineWidth = Math.Sqrt(width * height) * handWidth;

			axle.Width = lineWidth;
			axle.Height = lineWidth;
			hand.StrokeThickness = lineWidth / 2;

			update();

			hours = datetimeModel.datetime.TimeOfDay.TotalHours;
			angle = -hours / 24 * 360 - 90;	// negative angle to make move clockwise; 0 hours = 270 degrees
			setHand(hand, angle, radius);

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					outImageData[y, x] = 0;	// clear

					dx = x - width / 2;
					dy = -(y - height / 2);
					dist = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
					if (dist < width * radius / 2)
					{
						angle = -Math.Atan2(dy, dx) * Sun.rad2deg - 90;	// negative angle to make move clockwise; subtract 90 degrees for hours offset
						if (angle < 0)
						{
							angle += 360;
						}
						if (angle > 360)
						{
							angle -= 360;
						}
						hours = 24 * angle / 360;	// angle: 0 ... 360
						if (hours < sunRiseHours || hours > sunSetHours)
						{
							// night
							outImageData[y, x] = 0x7F000000;
						}
					}
				}
			}
			writePixelData(outImage, outImageData);
		}

		public void writePixelData(WriteableBitmap bitmap, UInt32[,] pixelData)
		{
			int bytesPerPixel = outImage.Format.BitsPerPixel / 8;
			bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixelData, width * bytesPerPixel, 0, 0);
		}
		
		void setHand(Line hand, double angledeg, double len)
		{
			double angle = angledeg * Sun.deg2rad;	// subtract 90 degrees for 0 degrees / 12' offset
			double cx = width / 2;
			double cy = height / 2;
			double x, y;

			x = cx + Math.Cos(angle) / 2 * len * width;
			y = cy - Math.Sin(angle) / 2 * len * height;

			hand.X1 = cx;
			hand.Y1 = cy;
			hand.X2 = x;
			hand.Y2 = y;
		}

	}
}
