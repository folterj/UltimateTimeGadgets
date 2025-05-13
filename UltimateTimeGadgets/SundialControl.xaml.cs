using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for SundialControl.xaml
	/// </summary>
	public partial class SundialControl : TimeControl
	{
		double handLen = 0.88;
		double handWidth = 0.05;

		Size size;

		SunRiseSetControl sunRiseSetControl = new SunRiseSetControl();	// use sun/rise set control

		public SundialControl()
		{
			defaultWidth = 100;
			defaultHeight = 100;

			InitializeComponent();

			init();
			loadDialImage(false);
			redraw();
		}

		public void loadDialImage(bool south)
		{
			string dialString;

			if (south)
			{
				dialString = "Resources/sundialimage/sundial_south.png";
			}
			else
			{
				dialString = "Resources/sundialimage/sundial_north.png";
			}
			MainImage.Source = new BitmapImage(new Uri(dialString, UriKind.Relative));
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
			sunRiseSetControl.setDateTime(datetimeModel);
			init();
			redraw();
		}

		public void redraw()
		{
			TimeSpan time = datetimeModel.getDateTimeWithoutDST().TimeOfDay;
			TimeSpan time0 = datetimeModel.datetime.TimeOfDay;
			TimeSpan sunRise = sunRiseSetControl.clockSunRise;
			TimeSpan sunSet = sunRiseSetControl.clockSunSet;
			double angle;
			double lat = datetimeModel.lat;
			double lineWidth = Math.Sqrt(ActualWidth * ActualHeight) * handWidth;
			bool obscureDisc = false;

			axle.Width = lineWidth;
			axle.Height = lineWidth;
			obscure.Width = handLen * ActualWidth;
			obscure.Height = handLen * ActualHeight;

			if (time0 > sunRise && time0 < sunSet)
			{
				if (lat < 0)
				{
					// southern hemisphere
					angle = -time.TotalHours * 15;
				}
				else
				{
					// northern hemisphere
					angle = time.TotalHours * 15;
				}
				loadDialImage((lat < 0));
				setHand(hand, angle, handLen);
				hand.StrokeThickness = lineWidth;
				hand.Visibility = Visibility.Visible;
				obscure.Fill = Brushes.Transparent;
			}
			else
			{
				// no sun ; no hand / shadow
				hand.Visibility = Visibility.Hidden;
				obscureDisc = true;
			}

			if (time.TotalHours < 4 || time.TotalHours > 20)
			{
				obscureDisc = true;
			}

			if (obscureDisc)
			{
				obscure.Fill = new SolidColorBrush(Color.FromArgb(127, 0, 0, 0));
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
