using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for TimeZoneMapControl.xaml
	/// </summary>
	public partial class TimeZoneMapControl : TimeControl
	{
		LocationStore locationStore;

		int width0, height0;
		int width, height;

		double lon0, lon1;
		double lonp0, lonp1;
		double lata0, lata1, lata2, lata3;

		Point mapPos = new Point();

		bool initialised = false;
		bool dragged = false;

		Point zoomPos = new Point();
		double zoom = 1;

		Point dragPos = new Point();

		public bool mouseOverControl = false;

		public TimeZoneMapControl()
		{
			InitializeComponent();
			init();

			width0 = (int)MainImage.Source.Width;
			height0 = (int)MainImage.Source.Height;
		}

		public void setLocationStore(LocationStore locationStore)
		{
			this.locationStore = locationStore;
		}

		void setLonLat(double lon, double lat)
		{
			datetimeModel.setLonLat(lon, lat);
			notifyLocationSelected();
		}

		public double getLon()
		{
			return datetimeModel.lon;
		}

		public double getLat()
		{
			return datetimeModel.lat;
		}

		void init()
		{
			datetimeModel.LonLatChanged += new System.ComponentModel.PropertyChangedEventHandler(datetimeModel_PropertyChanged);
			mapPos.X = datetimeModel.lon;
			mapPos.Y = datetimeModel.lat;
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			// only able to read width/height properties after an image is loaded in control
			width = (int)MainImage.ActualWidth;
			height = (int)MainImage.ActualHeight;

			horGuide.X2 = width;
			vertGuide.Y2 = height;

			lon0 = -157.5;
			lon1 = 180;
			lonp0 = (double)146 / width0 * width;
			lonp1 = (double)3840 / width0 * width;

			lata0 = -0.66393;
			lata1 = 84.258131;
			lata2 = 1.672046;
			lata3 = 5874.138677;

			initialised = true;

			update();
		}

		public override void setDateTime(DateTimeModel datetimeModel)
		{
			this.datetimeModel = datetimeModel;
			init();
			update();
		}

		void datetimeModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			mapPos.X = datetimeModel.lon;
			mapPos.Y = datetimeModel.lat;
			update();
		}

		void update()
		{
			if (!initialised)
				return;

			Point tempPos = new Point();
			tempPos.X = lon2pix(mapPos.X);
			tempPos.Y = lat2pix(mapPos.Y);

			Point screenPos = MainImage.TranslatePoint(tempPos, MainGrid);
			double x = screenPos.X;
			double y = screenPos.Y;

			vertCross.X1 = x;
			vertCross.X2 = x;
			vertCross.Y1 = y - 5;
			vertCross.Y2 = y + 5;

			horCross.Y1 = y;
			horCross.Y2 = y;
			horCross.X1 = x - 5;
			horCross.X2 = x + 5;
		}

		public event PropertyChangedEventHandler locationSelected;

		void notifyLocationSelected()
		{
			if (locationSelected != null)
			{
				locationSelected(this, new PropertyChangedEventArgs("lon/lat"));
			}
		}

		private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			dragged = false;
			dragPos = e.GetPosition(MainGrid);
		}

		public void MainGrid_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (!dragged)
			{
				Point imagePos = e.GetPosition(MainImage);
				mapPos.X = pix2lon(imagePos.X);
				mapPos.Y = pix2lat(imagePos.Y);
				setLonLat(mapPos.X, mapPos.Y);
			}
		}

		private void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			Point gridPos = e.GetPosition(MainGrid);
			Point imagePos = e.GetPosition(MainImage);

			horGuide.Y1 = gridPos.Y;
			horGuide.Y2 = gridPos.Y;
			horGuide.Visibility = Visibility.Visible;

			vertGuide.X1 = gridPos.X;
			vertGuide.X2 = gridPos.X;
			vertGuide.Visibility = Visibility.Visible;

			if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
			{
				// dragging
				dragged = true;
				double dx = gridPos.X - dragPos.X;
				double dy = gridPos.Y - dragPos.Y;
				zoomPos.X -= dx / zoom;
				zoomPos.Y -= dy / zoom;
				MainImage.RenderTransform = new ScaleTransform(zoom, zoom, zoomPos.X, zoomPos.Y);
				dragPos = gridPos;
			}
			mouseOverControl = true;
		}

		private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			horGuide.Visibility = Visibility.Hidden;
			vertGuide.Visibility = Visibility.Hidden;
			mouseOverControl = false;
		}

		private void Grid_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
		{
			int d = e.Delta / 120;
			double f = Math.Pow(2, d);
			zoomPos = e.GetPosition(MainImage);
			if (zoom < 1.1 && f < 1)
				return;
			zoom *= f;
			if (zoom < 1.1)
			{
				zoom = 1;
				MainImage.RenderTransform = null;
			}
			else
			{
				MainImage.RenderTransform = new ScaleTransform(zoom, zoom, zoomPos.X, zoomPos.Y);
			}
			update();	// won't have new zoom/coordinates yet
		}

		private void MainImage_LayoutUpdated(object sender, EventArgs e)
		{
			// called after RenderTransform is finished
			update();
		}

		double lon2pix(double lon)
		{
			return lonp0 + (lon - lon0) / (lon1 - lon0) * (lonp1 - lonp0);
		}

		double lat2pix(double lat)
		{
			// x = (PI - asin((y-a0)/a1) - a2) / (2 * PI / a3)
			double pix = (Math.PI - Math.Asin((lat - lata0) / lata1) - lata2) / (2 * Math.PI / lata3);
			return pix / height0 * height;
		}

		double pix2lon(double pix)
		{
			return lon0 + (pix - lonp0) / (lonp1 - lonp0) * (lon1 - lon0);
		}

		double pix2lat(double pix0)
		{
			// y = a0 + a1 * sin(2 * PI * x / a3 + a2)
			double pix = pix0 / height * height0;
			return lata0 + lata1 * Math.Sin(2 * Math.PI * pix / lata3 + lata2);
		}

	}
}
