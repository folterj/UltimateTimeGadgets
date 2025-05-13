using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for ControlWindow.xaml
	/// </summary>
	public partial class ControlWindow : Window
	{
		Window settingsWindow;

		public ControlSettings settings;
		SettingsObserver settingsObserver;

		ColorStyle style = ColorStyle.Opaque;
		bool dragging = false;
		bool locked = false;
		bool onTop = false;

		Matrix dpiMatrix;
		double scaleX = 1;
		double scaleY = 1;

		public ControlWindow(TimeControl timeControl, Window settingsWindow, ControlSettings settings, SettingsObserver settingsObserver,
							ColorStyle style, bool dragging, bool locked, bool onTop, Matrix dpiMatrix)
		{
			InitializeComponent();

			// replace user control
			mainGrid.Children.Remove(this.timeControl);
			this.timeControl = timeControl;
			mainGrid.Children.Add(timeControl);
			Canvas.SetZIndex(timeControl, -1);	// send to back

			this.settingsWindow = settingsWindow;
			this.settings = settings;
			this.settingsObserver = settingsObserver;

			setStyle(style);
			this.dragging = dragging;
			setLocked(locked);
			setOnTop(onTop);

			this.dpiMatrix = dpiMatrix;
			scaleX = dpiMatrix.M11;
			scaleY = dpiMatrix.M22;
			this.dpiMatrix.Invert();

			updateSize();
			updateLocation();

			setUIvisible(IsMouseOver);
		}

		[Flags]
		public enum ExtendedWindowStyles
		{
			// ...
			WS_EX_TOOLWINDOW = 0x00000080,
			// ...
		}

		public enum GetWindowLongFields
		{
			// ...
			GWL_EXSTYLE = (-20),
			// ...
		}

		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

		public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
		{
			int error = 0;
			IntPtr result = IntPtr.Zero;
			// Win32 SetWindowLong doesn't clear error on success
			SetLastError(0);

			if (IntPtr.Size == 4)
			{
				// use SetWindowLong
				Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
				error = Marshal.GetLastWin32Error();
				result = new IntPtr(tempResult);
			}
			else
			{
				// use SetWindowLongPtr
				result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
				error = Marshal.GetLastWin32Error();
			}

			if ((result == IntPtr.Zero) && (error != 0))
			{
				throw new System.ComponentModel.Win32Exception(error);
			}

			return result;
		}

		[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
		private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
		private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

		private static int IntPtrToInt32(IntPtr intPtr)
		{
			return unchecked((int)intPtr.ToInt64());
		}

		[DllImport("kernel32.dll", EntryPoint = "SetLastError")]
		public static extern void SetLastError(int dwErrorCode);

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// Hide window from Alt-TAB
			WindowInteropHelper wndHelper = new WindowInteropHelper(this);

			int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);

			exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
			SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
		}

		void setUIvisible(bool visible)
		{
			Visibility vis;

			if (visible)
			{
				vis = Visibility.Visible;
			}
			else
			{
				vis = Visibility.Hidden;
			}
			closeButton.Visibility = vis;
			largerButton.Visibility = vis;
			smallerButton.Visibility = vis;
			if (settingsWindow != null)
			{
				settingsButton.Visibility = vis;
			}
			else
			{
				settingsButton.Visibility = Visibility.Hidden;
			}
		}

		public void setStyle(ColorStyle style)
		{
			this.style = style;
			timeControl.setStyle(style);

			Color color = Colors.Black;
			Brush brush = Brushes.Black;
			Brush background = null;

			if (style == ColorStyle.Light)
			{
				color = Colors.White;
			}
			brush = new SolidColorBrush(color);

			if (style == ColorStyle.Opaque)
			{
				background = new SolidColorBrush(Color.FromScRgb(0.5f, 1, 1, 1));
			}

			updateStyle(style, color, brush, background);
		}

		public virtual void updateStyle(ColorStyle style, Color color, Brush brush, Brush background)
		{
			// dummy; will be overwritten
		}

		public void setLocked(bool locked)
		{
			this.locked = locked;
			if (locked)
			{
				mainGrid.Background = Brushes.Transparent;
			}
			else
			{
				mainGrid.Background = new SolidColorBrush(Color.FromArgb(0x01, 0xFF, 0xFF, 0xFF));
			}
		}

		public void setOnTop(bool onTop)
		{
			this.onTop = onTop;
			this.Topmost = onTop;
		}

		private void Grid_MouseMove(object sender, MouseEventArgs e)
		{
			if (!locked)
			{
				setUIvisible(true);

				if (dragging)
				{
					updateMouseMove(e);
				}
			}
		}

		private void Grid_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!locked)
			{
				dragging = false;
				setUIvisible(false);
			}
		}

		private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			if (!locked)
			{
				settings.location = e.GetPosition(null);
				dragging = true;
			}
		}

		private void mainGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (!locked)
			{
				dragging = false;
				settingsObserver.settingsChanged();
			}
		}

		void updateMouseMove(MouseEventArgs e)
		{
			System.Windows.Forms.Screen[] screens = System.Windows.Forms.Screen.AllScreens;
			System.Drawing.Rectangle screenBounds;
			Point location = PointToScreen(e.GetPosition(this));
			double snapThreshold = 20;
			double maxSnapThreshold;
			
			maxSnapThreshold = ActualWidth / 2 - 5;
			if (snapThreshold > maxSnapThreshold)
			{
				snapThreshold = maxSnapThreshold;
			}
			maxSnapThreshold = ActualHeight / 2 - 5;
			if (snapThreshold > maxSnapThreshold)
			{
				snapThreshold = maxSnapThreshold;
			}

			// first determine which screen (monitor) location is in:
			settings.screeni = 0;
			for (int i = 0; i < screens.Length; i++)
			{
				screenBounds = screens[i].WorkingArea;

				if (screenBounds.Contains((int)location.X, (int)location.Y))
				{
					settings.screeni = i;
					break;
				}
			}

			screenBounds = screens[settings.screeni].WorkingArea;

			settings.location.X = location.X - ActualWidth * scaleX / 2;
			settings.location.Y = location.Y - ActualHeight * scaleY / 2;

			settings.snapLeft = (Math.Abs(settings.location.X - screenBounds.Left) < snapThreshold);
			settings.snapRight = (Math.Abs(settings.location.X + ActualWidth * scaleX - screenBounds.Right) < snapThreshold);
			settings.snapTop = (Math.Abs(settings.location.Y - screenBounds.Top) < snapThreshold);
			settings.snapBottom = (Math.Abs(settings.location.Y + ActualHeight * scaleY - screenBounds.Bottom) < snapThreshold);
			updateLocation();
		}

		void updateLocation()
		{
			System.Drawing.Rectangle screenBounds = getCurrentScreenBounds();

			if (settings.snapLeft)
				settings.location.X = screenBounds.Left;
			if (settings.snapRight)
				settings.location.X = screenBounds.Right - ActualWidth * scaleX;
			if (settings.snapTop)
				settings.location.Y = screenBounds.Top;
			if (settings.snapBottom)
				settings.location.Y = screenBounds.Bottom - ActualHeight * scaleY;

			Point setpos = dpiMatrix.Transform(settings.location);

			Left = setpos.X;
			Top = setpos.Y;
		}

		System.Drawing.Rectangle getCurrentScreenBounds()
		{
			System.Windows.Forms.Screen[] screens = System.Windows.Forms.Screen.AllScreens;
			if (settings.screeni >= screens.Length)
			{
				settings.screeni = screens.Length - 1;
			}
			System.Windows.Forms.Screen screen = screens[settings.screeni];
			return screen.WorkingArea;
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			updateLocation();
		}

		private void closeButton_Click(object sender, RoutedEventArgs e)
		{
			closeAll();
		}

		public void closeAll()
		{
			if (settingsWindow != null)
			{
				settingsWindow.Close();
			}
			settings.remove();
			settingsObserver.settingsChanged();
			Close();
		}

		private void largerButton_Click(object sender, RoutedEventArgs e)
		{
			settings.size *= 2;
			updateSize();
			settingsObserver.settingsChanged();
		}

		private void smallerButton_Click(object sender, RoutedEventArgs e)
		{
			settings.size /= 2;
			updateSize();
			settingsObserver.settingsChanged();
		}

		void updateSize()
		{
			System.Drawing.Rectangle screenBounds = getCurrentScreenBounds();
			double width, height;

			// minimum size
			if (settings.size < 1)
			{
				settings.size = 1;
			}

			// round to integral size
			double sizei = Math.Log10(settings.size) / Math.Log10(2);
			if (Math.Abs(sizei - Math.Round(sizei)) > 0.1)
			{
				// significant deviation from integral power; try highest power
				sizei = Math.Ceiling(sizei);
				settings.size = Math.Pow(2, sizei);
			}

			// new pixel size
			width = timeControl.defaultWidth * settings.size;
			height = timeControl.defaultHeight * settings.size;

			// height is at least control buttons height
			if (height < 76)
			{
				height = 76;
			}

			// maximum pixel size
			if (width > screenBounds.Width)
			{
				width = screenBounds.Width;
				settings.size = width / timeControl.defaultWidth;
				height = timeControl.defaultHeight * settings.size;
			}
			if (height > screenBounds.Height)
			{
				height = screenBounds.Height;
				settings.size = height / timeControl.defaultHeight;
				width = timeControl.defaultWidth * settings.size;
			}

			timeControl.Width = width;
			timeControl.Height = height;
		}

		private void settingsButton_Click(object sender, RoutedEventArgs e)
		{
			if (settingsWindow != null)
			{
				settingsWindow.Show();
			}
		}

	}
}
