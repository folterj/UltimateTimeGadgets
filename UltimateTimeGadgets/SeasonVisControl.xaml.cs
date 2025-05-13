using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for SeasonControl.xaml
	/// </summary>
	public partial class SeasonVisControl : TimeControl
	{
		double handLen = 0.8;
		double lineWidth = 0.01;

		int background = 3;
		Brush styleBrush = Brushes.Black;

		Size size;

		public SeasonVisControl()
		{
			defaultWidth = 100;
			defaultHeight = 100;

			InitializeComponent();
			init();
			setBackground(background);
		}

		public void setBackground(int background)
		{
			if (background < 1)
				background = 1;
			if (background > 4)
				background = 4;
			this.background = background;
			update();
		}

		public override void updateStyle(ColorStyle style, Color color, Brush brush, Brush background)
		{
			styleBrush = brush;
			update();
		}

		public void update()
		{
			string imageString = string.Format("Resources/seasons/seasons{0}_{1}.png", background, style.ToString().ToLower());
			if (!loadImage(imageString))
			{
				imageString = string.Format("Resources/seasons/seasons{0}.png", background);
				loadImage(imageString);
			}
			hand.Stroke = styleBrush;
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

		void init()
		{
			datetimeModel.MinutesChanged += new System.ComponentModel.PropertyChangedEventHandler(datetimeModel_PropertyChanged);
			datetimeModel.TimeZoneChanged += new System.ComponentModel.PropertyChangedEventHandler(datetimeModel_PropertyChanged);
			datetimeModel.LonLatChanged += new System.ComponentModel.PropertyChangedEventHandler(datetimeModel_PropertyChanged);
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
			double lineWidth0 = Math.Sqrt(ActualWidth * ActualHeight) * lineWidth;
			int yearDay;
			double yearDays;
			double angle;
			bool south = (datetimeModel.lat < 0);

			yearDay = datetimeModel.datetime.DayOfYear + 10;	// difference between 21 of 3rd month and quarter year
			yearDays = 365.24;
			angle = yearDay / yearDays * 360;

			if (south)
			{
				angle += 180;
			}
			if (angle > 360)
			{
				angle -= 360;
			}

			axle.Width = lineWidth0 * 2;
			axle.Height = lineWidth0 * 2;

			setHand(hand, angle, handLen);
			hand.StrokeThickness = lineWidth0;
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
