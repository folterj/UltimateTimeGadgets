using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for MoonVisControl.xaml
	/// </summary>
	public partial class MoonVisControl : TimeControl, INotifyPropertyChanged
	{
		double moonAge = 0;
		double moonFactor = 0;
		double moonPhase = 0;

		string format = "Age: {0:F1} Lit: {1:P1} ({2})";

		public string moonstring
		{
			get { return string.Format(format, moonAge, moonPhase, getMoonAgeDesc(moonFactor)); }
		}

		public MoonVisControl()
		{
			defaultWidth = 100;
			defaultHeight = 100;

			InitializeComponent();
			DataContext = this;

			setInfoVisible(false);

			init();
			loadMoonImage(0, false);
			update();
		}

		public void setInfoVisible(bool visible)
		{
			if (visible)
			{
				textRow.Height = new GridLength(28);
				textBorder.Visibility = Visibility.Visible;
			}
			else
			{
				textRow.Height = new GridLength(0);
				textBorder.Visibility = Visibility.Hidden;
			}
		}

		public override void updateStyle(ColorStyle style, Color color, Brush brush, Brush background)
		{
			moonLabel.Foreground = brush;
			textBorder.Background = background;
			update();
		}

		void init()
		{
			datetimeModel.MinutesChanged += new System.ComponentModel.PropertyChangedEventHandler(datetimeModel_PropertyChanged);
			datetimeModel.LonLatChanged += new System.ComponentModel.PropertyChangedEventHandler(datetimeModel_PropertyChanged);
		}

		public override void setDateTime(DateTimeModel datetimeModel)
		{
			this.datetimeModel = datetimeModel;
			init();
			update();
		}

		public void loadMoonImage(int moonimagei, bool south)
		{
			BitmapImage image = new BitmapImage();
			string moonImageFile;
			string addTrans = "";

			if (style != ColorStyle.Opaque)
			{
				addTrans = "trans";
			}
			moonImageFile = string.Format("Resources/moon{1}/moon{0:00}.png", moonimagei, addTrans);

			image.BeginInit();
			if (south)
			{
				image.Rotation = Rotation.Rotate180;
			}
			image.UriSource = new Uri(moonImageFile, UriKind.Relative);
			image.EndInit();
			MainImage.Source = image;
		}

		void datetimeModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			update();
		}

		void update()
		{
			DateTime datetime = datetimeModel.datetime.DateTime;
			double lat = datetimeModel.lat;
			double lon = datetimeModel.lon;

			calcMoonAge(datetime);
			moonFactor = moonAge / 29.5305888610;
			moonPhase = (1 - Math.Cos(2 * Math.PI * moonFactor)) / 2;
			int moonImagei = (int)Math.Round(moonFactor * 30);	// 0.5 -> moon15; 0 / 1 -> moon00
			if (moonImagei > 29)
				moonImagei = 0;
			loadMoonImage(moonImagei, (lat < 0));
			NotifyPropertyChanged("moonstring");
		}

		void calcMoonAge(DateTime datetime)
		{
			TimeSpan dt = datetime - new DateTime(2000, 1, 1);
			int n = (int)((dt.TotalDays - 5.597661) / 29.5305888610);
			double dnewmoon = 5.597661 + 29.5305888610 * n + (102.026 * 1e-12) * Math.Pow(n, 2) - 0.000739 - (235 * 1e-12) * Math.Pow(n, 2);
			moonAge = dt.TotalDays - dnewmoon;
		}

		string getMoonAgeDesc(double moonFactor)
		{
			string age = "new moon";

			if (moonFactor > 0.0167)
			{
				age = "waxing crescent";
			}
			if (moonFactor > 0.233)
			{
				age = "first quarter";
			}
			if (moonFactor > 0.267)
			{
				age = "waxing gibbous";
			}
			if (moonFactor > 0.483)
			{
				age = "full moon";
			}
			if (moonFactor > 0.517)
			{
				age = "waning gibbous";
			}
			if (moonFactor > 0.733)
			{
				age = "last quarter";
			}
			if (moonFactor > 0.767)
			{
				age = "waning crescent";
			}
			if (moonFactor > 0.983)
			{
				age = "new moon";
			}
			return age;
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
