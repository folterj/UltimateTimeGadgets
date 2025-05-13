using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for EarthVisControl.xaml
	/// </summary>
	public partial class EarthVisControl : TimeControl
	{
		const double deg2rad = Math.PI / 180;
		const double rad2deg = 1 / deg2rad;

		BitmapImage dayBitmapImage, nightBitmapImage, nightTransBitmapImage;
		WriteableBitmap outImage;
		UInt32[,] dayImageData, nightImageData, nightTransImageData, outImageData;

		int width, height;

		bool initialised = false;

		public EarthVisControl()
		{
			defaultWidth = 200;
			defaultHeight = 100;

			InitializeComponent();
			init();
		}

		void init()
		{
			datetimeModel.MinutesChanged += new System.ComponentModel.PropertyChangedEventHandler(datetimeModel_PropertyChanged);
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

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			// only able to read width/height properties after an image is loaded in control
			width = (int)MainImage.ActualWidth;
			height = (int)MainImage.ActualHeight;

			dayBitmapImage = loadImageResize(new Uri("Resources/earth/earth_day.png", UriKind.Relative), width, height);
			nightBitmapImage = loadImageResize(new Uri("Resources/earth/earth_night.png", UriKind.Relative), width, height);
			nightTransBitmapImage = loadImageResize(new Uri("Resources/earth/earth_night_trans.png", UriKind.Relative), width, height);
			dayImageData = getPixelData(dayBitmapImage);
			nightImageData = getPixelData(nightBitmapImage);
			nightTransImageData = getPixelData(nightTransBitmapImage);

			outImageData = new UInt32[height, width];
			outImage = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

			MainImage.Source = outImage;
			initialised = true;
			redraw();
		}

		BitmapImage loadImageResize(Uri source, int width, int height)
		{
			BitmapImage image = new BitmapImage();
			image.BeginInit();
			image.UriSource = source;
			image.DecodePixelWidth = width;
			image.DecodePixelHeight = height;
			image.EndInit();

			// force sync load
			MainImage.Source = image;

			return image;
		}

		public UInt32[,] getPixelData(BitmapSource source)
		{
			UInt32[,] outData = new UInt32[height, width];
			UInt32[] byteData = new UInt32[height * width];
			int bytesPerPixel = source.Format.BitsPerPixel / 8;
			int widthInBytes = bytesPerPixel * width;
			int i = 0;

			source.CopyPixels(byteData, widthInBytes, 0);

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					outData[y, x] = byteData[i++];
				}
			}
			return outData;
		}

		public void writePixelData(WriteableBitmap bitmap, UInt32[,] pixelData)
		{
			int bytesPerPixel = outImage.Format.BitsPerPixel / 8;
			bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixelData, width * bytesPerPixel, 0, 0);
		}

		void datetimeModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			redraw();
		}

		void redraw()
		{
			if (!initialised)
				return;

			DateTime datetime = datetimeModel.utcdatetime.DateTime;
			double t = datetime.TimeOfDay.TotalHours + 6;
			while (t < 0)
			{
				t += 24;
			}
			while (t > 24)
			{
				t -= 24;
			}
			t /= 24;
			Vector3D d = new Vector3D(Math.Sin(2 * Math.PI * t), 0, Math.Cos(2 * Math.PI * t));	// pointing from earth to sun
			double obsTilt = 23.5 * Math.Cos(2 * Math.PI * (datetime.DayOfYear - 172) / 365);	// tilt
			Vector3D r = new Vector3D(0, Math.Tan(2 * Math.PI * obsTilt / 360), 0);				// season offset
			Vector3D rd = r + d;																// corrected d
			rd.Normalize();
			Vector3D p = new Vector3D();														// earth normal
			double illumination, fractLight;
			UInt32 dayColor, nightColor, outColor;
			double angu, angv;
			double lon0, lat0;

			for (int x = 0; x < width; x++)
			{
				lon0 = (double)x / width;
				angu = 2 * Math.PI * lon0;

				for (int y = 0; y < height; y++)
				{
					lat0 = (double)y / (2 * height) - 1;
					angv = 2 * Math.PI * lat0;

					p.X = Math.Sin(angv) * Math.Cos(angu);
					p.Y = Math.Cos(angv);
					p.Z = Math.Sin(angv) * Math.Sin(angu);
					p.Normalize();

					illumination = Vector3D.DotProduct(rd, p);

					dayColor = dayImageData[y, x];
					if (style == ColorStyle.Opaque)
					{
						nightColor = nightImageData[y, x];
					}
					else
					{
						//nightColor = nightTransImageData[y, x];
						nightColor = 0x00000000;
					}
					if (illumination < -0.1)
					{
						// illumination < -0.1
						fractLight = 0;
					}
					else if (illumination < 0.1)
					{
						// -0.1 > illumination > 0.1
						fractLight = (illumination + 0.1) / 0.2 / 2;			// normalise to 0 - 0.5
					}
					else
					{
						// 0.1 > illumination > 1
						fractLight = 0.5 + ((illumination - 0.1)) / 0.9 / 2;	// normalise to 0.5 - 1
					}
					outColor = colorCombine(nightColor, dayColor, fractLight);
					if (illumination > -0.01 && illumination < 0.01)
					{
						outColor = colorAdd(outColor, colorMultiply(colorToInt(Colors.Yellow), 0.2));
					}
					if (illumination > 0.97)
					{
						// sun (reflection)
						outColor = colorAdd(outColor, colorMultiply(colorToInt(Colors.Yellow), 0.2));
					}
					outImageData[y, x] = outColor;
				}
			}
			writePixelData(outImage, outImageData);
		}

		UInt32 colorCombine(UInt32 color1, UInt32 color2, double factor)
		{
			UInt32 newColor = 0;
			UInt32 c, c1, c2;
			UInt32 mask = 0xFF;

			for (int i = 0; i < 4; i++)
			{
				c1 = color1 & mask;
				c2 = color2 & mask;
				c = (UInt32)(c1 * (1 - factor) + c2 * factor);
				if (c > mask)
				{
					c = mask;
				}
				newColor |= (c & mask);
				mask *= 0x100;
			}
			return newColor;
		}

		UInt32 colorAdd(UInt32 color1, UInt32 color2)
		{
			UInt32 newColor = 0;
			UInt32 c, c1, c2;
			UInt32 mask = 0xFF;

			for (int i = 0; i < 4; i++)
			{
				c1 = color1 & mask;
				c2 = color2 & mask;
				c = c1 + c2;
				if (c > mask)
				{
					c = mask;
				}
				newColor |= (c & mask);
				mask *= 0x100;
			}
			return newColor;
		}

		UInt32 colorMultiply(UInt32 color, double factor)
		{
			UInt32 newColor = 0;
			UInt32 c;
			UInt32 mask = 0xFF;

			for (int i = 0; i < 4; i++)
			{
				c = (color & mask);
				if (i < 3)
				{
					// skip MSB/alpha bits
					c = (UInt32)(c * factor);
					if (c > mask)
					{
						c = mask;
					}
				}
				newColor |= (c & mask);
				mask *= 0x100;
			}
			return newColor;
		}

		UInt32 colorToInt(Color color)
		{
			UInt32 newColor = 0;

			newColor = color.B + (color.G + (color.R + (UInt32)color.A * 0x100) * 0x100) * 0x100;

			return newColor;
		}

		private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			UserControl_Loaded(sender, e);
		}

	}
}
