using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for AnalogClockControl.xaml
	/// </summary>
	public partial class AnalogClockControl : TimeControl
	{
		bool minutesEnabled = true;
		bool secondsEnabled = true;

		double hourLen = 0.667;	// about 2/3 of long hand
		double minLen = 1;
		double secLen = 1;

		double[] handLength = new double[] { 0.8, 0.8, 0.65, 0.8, 0.8, 0.8 };
		double[] handWidth = new double[] { 0.01, 0.01, 0.01, 0.01, 0.01, 0.02 };

		int clockFace = 1;
		Brush styleBrush = Brushes.Black;

		Size size;

		public AnalogClockControl()
		{
			defaultWidth = 100;
			defaultHeight = 100;

			InitializeComponent();
			init();
			setClockFace(clockFace);
		}

		public void setClockFace(int clockFace)
		{
			if (clockFace < 1)
				clockFace = 1;
			if (clockFace > 6)
				clockFace = 6;
			this.clockFace = clockFace;
			update();
		}

		public override void updateStyle(ColorStyle style, Color color, Brush brush, Brush background)
		{
			styleBrush = brush;
			update();
		}

		public void update()
		{
			string imageString = string.Format("Resources/clockface/face{0}_{1}.png", clockFace, style.ToString().ToLower());
			if (!loadImage(imageString))
			{
				imageString = string.Format("Resources/clockface/face{0}.png", clockFace);
				loadImage(imageString);
			}
			hourHand.Stroke = styleBrush;
			minHand.Stroke = styleBrush;
			secHand.Stroke = styleBrush;
			axle.Fill = styleBrush;
			redraw();
		}

		public bool loadImage(string path)
		{
			try
			{
				BitmapImage source = new BitmapImage(new Uri(path, UriKind.Relative));
				MainImage.Source = source;
				source.Freeze();
				int test = source.PixelWidth;	// the only wat to test load: this will throw an exception
				return true;
			}
			catch (Exception)
			{
			}
			return false;
		}

		public void setSecondsHand(bool enabled)
		{
			secondsEnabled = enabled;
			redraw();
		}

		public void setMinutesHand(bool enabled)
		{
			minutesEnabled = enabled;
			redraw();
		}

		void init()
		{
			datetimeModel.SecondsChanged += new System.ComponentModel.PropertyChangedEventHandler(datetimeModel_PropertyChanged);
			datetimeModel.TimeZoneChanged += new System.ComponentModel.PropertyChangedEventHandler(datetimeModel_PropertyChanged);
		}

		void datetimeModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			redraw();
		}

		public override void setDateTime(DateTimeModel datetimeModel)
		{
			this.datetimeModel = datetimeModel;
			init();
			redraw();
		}

		public void redraw()
		{
			TimeSpan time = datetimeModel.datetime.TimeOfDay;
			double hourangle = time.TotalHours * 30;
			double minangle = time.TotalMinutes * 6;
			double secangle = time.Seconds * 6;

			double hourLineLength = hourLen * handLength[clockFace - 1];
			double minLineLength = minLen * handLength[clockFace - 1];
			double secLineLength = secLen * handLength[clockFace - 1];

			double hourLineWidth = Math.Sqrt(ActualWidth * ActualHeight) * handWidth[clockFace - 1];
			double minLineWidth = hourLineWidth * 0.67;
			double secLineWidth = hourLineWidth * 0.33;

			axle.Width = hourLineWidth * 2;
			axle.Height = hourLineWidth * 2;

			setHand(hourHand, hourangle, hourLineLength);
			hourHand.StrokeThickness = hourLineWidth;

			if (minutesEnabled)
			{
				setHand(minHand, minangle, minLineLength);
				minHand.StrokeThickness = minLineWidth;
				minHand.Visibility = Visibility.Visible;
			}
			else
			{
				minHand.Visibility = Visibility.Hidden;
			}
			if (secondsEnabled)
			{
				setHand(secHand, secangle, secLineLength);
				secHand.StrokeThickness = secLineWidth;
				secHand.Visibility = Visibility.Visible;
			}
			else
			{
				secHand.Visibility = Visibility.Hidden;
			}
		}

		void setHand(Line hand, double angledeg, double len)
		{
			double angle = (angledeg - 90) / 180 * Math.PI;	// subtract 90 degrees for 0 degrees / 12' offset
			double w = size.Width;
			double h = size.Height;
			double cx = w / 2;
			double cy = h / 2;
			double x, y;

			x = cx + Math.Cos(angle) / 2 * len * w;
			y = cy + Math.Sin(angle) / 2 * len * h;

			hand.X1 = cx;
			hand.Y1 = cy;
			hand.X2 = x;
			hand.Y2 = y;
		}

		private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			size = e.NewSize;
			redraw();
		}

	}
}
