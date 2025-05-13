using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for GalleryWindow.xaml
	/// </summary>
	public partial class GalleryWindow : Window
	{
		DateTimeModel datetimeModel;

		List<ControlWindow> timeWindows = new List<ControlWindow>();

		ControlSettingsList controlSettingsList;

		Matrix dpiMatrix = new Matrix();

		public GalleryWindow(DateTimeModel datetimeModel, ControlSettingsList controlSettingsList)
		{
			this.datetimeModel = datetimeModel;
			this.controlSettingsList = controlSettingsList;

			InitializeComponent();

			addControls();
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct MARGINS
		{
			public int cxLeftWidth;      // width of left border that retains its size
			public int cxRightWidth;     // width of right border that retains its size
			public int cyTopHeight;      // height of top border that retains its size
			public int cyBottomHeight;   // height of bottom border that retains its size
		};

		[DllImport("DwmApi.dll")]
		private static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMarInset);

		[DllImport("DwmApi.dll", PreserveSig = false)]
		public static extern bool DwmIsCompositionEnabled();

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (Environment.OSVersion.Version.Major >= 6 && DwmIsCompositionEnabled())
			{
				// full glass window
				WindowInteropHelper windowInteropHelper = new WindowInteropHelper(this);
				IntPtr myHwnd = windowInteropHelper.Handle;
				HwndSource mainWindowSrc = System.Windows.Interop.HwndSource.FromHwnd(myHwnd);

				mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

				MARGINS margins = new MARGINS()
				{
					cxLeftWidth = -1,
					cxRightWidth = -1,
					cyBottomHeight = -1,
					cyTopHeight = -1
				};

				DwmExtendFrameIntoClientArea(myHwnd, ref margins);
			}
			else
			{
				// incompatible with glass window
				Background = Brushes.Gray;
			}

			// get current DPI
			PresentationSource source = PresentationSource.FromVisual(this);
			if (source != null)
			{
				dpiMatrix = source.CompositionTarget.TransformToDevice;
			}

			// load saved controls
			foreach (ControlSettings settings in controlSettingsList.controls)
			{
				createControl(settings, false);
			}
		}

		public void setGadgetsStyle(ColorStyle style)
		{
			foreach (ControlWindow timeWindow in timeWindows)
			{
				timeWindow.setStyle(style);
			}
		}

		public void setGadgetsLocked(bool gadgetsLocked)
		{
			foreach (ControlWindow timeWindow in timeWindows)
			{
				timeWindow.setLocked(gadgetsLocked);
			}
		}

		public void setGadgetsOnTop(bool gadgetsOnTop)
		{
			foreach (ControlWindow timeWindow in timeWindows)
			{
				timeWindow.setOnTop(gadgetsOnTop);
			}
		}

		private void addControls()
		{
			DigitalClockControl digitalClock = new DigitalClockControl();
			addControl(digitalClock, "Digital Clock", digitalClock_MouseLeftButtonDown);

			DateControl date = new DateControl();
			addControl(date, "Date", date_MouseLeftButtonDown);

			SunRiseSetControl sunRiseSet = new SunRiseSetControl();
			addControl(sunRiseSet, "Sun Rise/Set", sunRiseSet_MouseLeftButtonDown);

			AnalogClockControl analogClock = new AnalogClockControl();
			addControl(analogClock, "Analog Clock", analogClock_MouseLeftButtonDown);

			MoonVisControl moonVis = new MoonVisControl();
			addControl(moonVis, "Moon Phase", moonVis_MouseLeftButtonDown);

			EarthVisControl earthVis = new EarthVisControl();
			addControl(earthVis, "Earth Light", earthVis_MouseLeftButtonDown);

			BeatClockControl beatClock = new BeatClockControl();
			addControl(beatClock, "Beat Time", beatClock_MouseLeftButtonDown);

			DaylightdialControl daylightDial = new DaylightdialControl();
			addControl(daylightDial, "Daylight", daylightDial_MouseLeftButtonDown);

			DaylightVisControl daylightVis = new DaylightVisControl();
			addControl(daylightVis, "Yearly Daylight", daylightVis_MouseLeftButtonDown);

			DayPartControl dayPart = new DayPartControl();
			addControl(dayPart, "Day part", dayPart_MouseLeftButtonDown);

			SundialControl sundial = new SundialControl();
			addControl(sundial, "Sundial", sundial_MouseLeftButtonDown);

			TimeZoneDispControl timeZone = new TimeZoneDispControl();
			addControl(timeZone, "Time Zone", timeZone_MouseLeftButtonDown);
	
			SeasonsControl seasons = new SeasonsControl();
			addControl(seasons, "Seasons", seasons_MouseLeftButtonDown);

			SeasonVisControl seasonsVis = new SeasonVisControl();
			addControl(seasonsVis, "Season Dial", seasonsVis_MouseLeftButtonDown);

			TimeAdjustmentControl timeAdjustment = new TimeAdjustmentControl();
			addControl(timeAdjustment, "DST Adjustment", timeAdjustment_MouseLeftButtonDown);
		}

		private void addControl(TimeControl timeControl, string label, MouseButtonEventHandler eventHandler)
		{
			double width;
			Grid controlGrid = new Grid();
			Border paddingBorder = new Border();
			paddingBorder.Padding = new Thickness(2);

			Border controlBorder = new Border();
			controlBorder.BorderThickness = new Thickness(3);
			controlBorder.CornerRadius = new CornerRadius(5);
			controlBorder.Padding = new Thickness(2);
			controlBorder.MouseEnter += new MouseEventHandler(controlBorder_MouseEnter);
			controlBorder.MouseLeave += new MouseEventHandler(controlBorder_MouseLeave);
			controlBorder.MouseLeftButtonDown += new MouseButtonEventHandler(eventHandler);
			setBorder(controlBorder, false);

			if (timeControl.defaultWidth > 100)
			{
				width = 200;
			}
			else
			{
				width = 100;
			}
			timeControl.Width = width;
			timeControl.Height = 100;

			TextBlock text = new TextBlock(new Run(label));
			text.TextAlignment = TextAlignment.Center;
			text.TextWrapping = TextWrapping.Wrap;

			Viewbox textBox = new Viewbox();
			textBox.HorizontalAlignment = HorizontalAlignment.Center;
			textBox.VerticalAlignment = VerticalAlignment.Bottom;
			textBox.Width = width;
			textBox.Height = 20;
			textBox.Child = text;

			RowDefinition controlRow = new RowDefinition();
			controlRow.Height = new GridLength(100);
			controlGrid.RowDefinitions.Add(controlRow);
			RowDefinition labelRow = new RowDefinition();
			controlGrid.RowDefinitions.Add(labelRow);

			controlGrid.Children.Add(timeControl);
			timeControl.SetValue(Grid.RowProperty, 0);
			controlGrid.Children.Add(textBox);
			textBox.SetValue(Grid.RowProperty, 1);

			controlBorder.Child = controlGrid;

			paddingBorder.Child = controlBorder;

			mainPanel.Children.Add(paddingBorder);

			timeControl.setDateTime(datetimeModel);
			timeControl.setStyle(ColorStyle.Opaque);
		}

		void controlBorder_MouseEnter(object sender, MouseEventArgs e)
		{
			Border border = (Border)sender;
			setBorder(border, true);
		}

		void controlBorder_MouseLeave(object sender, MouseEventArgs e)
		{
			Border border = (Border)sender;
			setBorder(border, false);
		}

		void setBorder(Border border, bool over)
		{
			SolidColorBrush brush;

			if (over)
			{
				border.BorderBrush = Brushes.White;

				brush = new SolidColorBrush(Colors.White);
				brush.Opacity = 0.5;
				border.Background = brush;
			}
			else
			{
				brush = new SolidColorBrush(Colors.White);
				brush.Opacity = 0.25;
				border.BorderBrush = brush;

				border.Background = null;
			}
		}

		private void createControl(ControlSettings settings, bool dragging)
		{
			switch (settings.type)
			{
				case ControlType.DigitalClock: createDigitalClock(settings, dragging); break;
				case ControlType.AnalogClock: createAnalogClock(settings, dragging); break;
				case ControlType.DayPart: createDayPart(settings, dragging); break;
				case ControlType.BeatClock: createBeatClock(settings, dragging); break;
				case ControlType.Date: createDate(settings, dragging); break;
				case ControlType.SunRiseSet: createSunRiseSet(settings, dragging); break;
				case ControlType.DaylightDial: createDaylightDial(settings, dragging); break;
				case ControlType.DaylightVis: createDaylightVis(settings, dragging); break;
				case ControlType.MoonVis: createMoonVis(settings, dragging); break;
				case ControlType.EarthVis: createEarthVis(settings, dragging); break;
				case ControlType.SeasonsVis: createSeasonsVis(settings, dragging); break;
				case ControlType.Seasons: createSeasons(settings, dragging); break;
				case ControlType.SunDial: createSundial(settings, dragging); break;
				case ControlType.TimeZoneDisp: createTimeZoneDisp(settings, dragging); break;
				case ControlType.TimeAdjustment: createTimeAdjustment(settings, dragging); break;
			}
		}

		private void createDigitalClock(ControlSettings settings, bool dragging)
		{
			TimeControl timeControl = new DigitalClockControl();
			SettingsWindow settingsWindow = new DigitalClockSettingsWindow(timeControl, settings);
			createControlWindow(timeControl, settingsWindow, settings, dragging);
		}

		private void createDate(ControlSettings settings, bool dragging)
		{
			TimeControl timeControl = new DateControl();
			SettingsWindow settingsWindow = new DateSettingsWindow(timeControl, settings);
			createControlWindow(timeControl, settingsWindow, settings, dragging);
		}

		private void createSunRiseSet(ControlSettings settings, bool dragging)
		{
			TimeControl timeControl = new SunRiseSetControl();
			SettingsWindow settingsWindow = new SunRiseSetSettingsWindow(timeControl, settings);
			createControlWindow(timeControl, settingsWindow, settings, dragging);
		}

		private void createAnalogClock(ControlSettings settings, bool dragging)
		{
			TimeControl timeControl = new AnalogClockControl();
			SettingsWindow settingsWindow = new AnalogClockSettingsWindow(timeControl, settings);
			createControlWindow(timeControl, settingsWindow, settings, dragging);
		}

		private void createMoonVis(ControlSettings settings, bool dragging)
		{
			TimeControl timeControl = new MoonVisControl();
			SettingsWindow settingsWindow = new MoonSettingsWindow(timeControl, settings);
			createControlWindow(timeControl, settingsWindow, settings, dragging);
		}

		private void createEarthVis(ControlSettings settings, bool dragging)
		{
			TimeControl timeControl = new EarthVisControl();
			createControlWindow(timeControl, null, settings, dragging);
		}

		private void createBeatClock(ControlSettings settings, bool dragging)
		{
			TimeControl timeControl = new BeatClockControl();
			SettingsWindow settingsWindow = new BeatClockSettingsWindow(timeControl, settings);
			createControlWindow(timeControl, settingsWindow, settings, dragging);
		}

		private void createSeasonsVis(ControlSettings settings, bool dragging)
		{
			TimeControl timeControl = new SeasonVisControl();
			SettingsWindow settingsWindow = new SeasonVisSettingsWindow(timeControl, settings);
			createControlWindow(timeControl, settingsWindow, settings, dragging);
		}

		private void createSeasons(ControlSettings settings, bool dragging)
		{
			TimeControl timeControl = new SeasonsControl();
			createControlWindow(timeControl, null, settings, dragging);
		}

		private void createSundial(ControlSettings settings, bool dragging)
		{
			TimeControl timeControl = new SundialControl();
			createControlWindow(timeControl, null, settings, dragging);
		}

		private void createTimeZoneDisp(ControlSettings settings, bool dragging)
		{
			TimeControl timeControl = new TimeZoneDispControl();
			SettingsWindow settingsWindow = new TimeZoneDispSettingsWindow(timeControl, settings);
			createControlWindow(timeControl, settingsWindow, settings, dragging);
		}

		private void createTimeAdjustment(ControlSettings settings, bool dragging)
		{
			TimeControl timeControl = new TimeAdjustmentControl();
			SettingsWindow settingsWindow = new TimeAdjustmentSettingsWindow(timeControl, settings);
			createControlWindow(timeControl, settingsWindow, settings, dragging);
		}

		private void createDayPart(ControlSettings settings, bool dragging)
		{
			TimeControl timeControl = new DayPartControl();
			createControlWindow(timeControl, null, settings, dragging);
		}

		private void createDaylightDial(ControlSettings settings, bool dragging)
		{
			TimeControl timeControl = new DaylightdialControl();
			createControlWindow(timeControl, null, settings, dragging);
		}

		private void createDaylightVis(ControlSettings settings, bool dragging)
		{
			TimeControl timeControl = new DaylightVisControl();
			createControlWindow(timeControl, null, settings, dragging);
		}

		private void createControlWindow(TimeControl timeControl, SettingsWindow settingsWindow, ControlSettings settings, bool dragging)
		{
			timeControl.setDateTime(datetimeModel);
			settings.Removed += new EventHandler(settings_Removed);

			ControlWindow timeWindow = new ControlWindow(timeControl,
														settingsWindow,
														settings,
														controlSettingsList,
														controlSettingsList.colorStyle,
														dragging,
														controlSettingsList.gadgetsLocked && !dragging,
														controlSettingsList.gadgetsOnTop,
														dpiMatrix);

			timeWindow.Closed += new EventHandler(timeWindow_Closed);

			timeWindows.Add(timeWindow);

			timeWindow.Show();
			if (settingsWindow != null)
			{
				settingsWindow.Owner = timeWindow;
				settingsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				settingsWindow.setSettingsObserver(controlSettingsList);
			}
		}

		public void closeAll()
		{
			foreach (ControlWindow control in timeWindows.ToArray())
			{
				control.closeAll();
			}
		}

		void settings_Removed(object sender, EventArgs e)
		{
			ControlSettings settings = (ControlSettings)sender;
			controlSettingsList.controls.Remove(settings);
		}

		void timeWindow_Closed(object sender, EventArgs e)
		{
			ControlWindow timeWindow = (ControlWindow)sender;
			timeWindows.Remove(timeWindow);
		}

		private void digitalClock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ControlSettings settings = new ControlSettings(ControlType.DigitalClock, PointToScreen(e.GetPosition(this)));
			controlSettingsList.controls.Add(settings);
			createDigitalClock(settings, true);
		}

		private void date_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ControlSettings settings = new ControlSettings(ControlType.Date, PointToScreen(e.GetPosition(this)));
			controlSettingsList.controls.Add(settings);
			createDate(settings, true);
		}

		private void sunRiseSet_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ControlSettings settings = new ControlSettings(ControlType.SunRiseSet, PointToScreen(e.GetPosition(this)));
			controlSettingsList.controls.Add(settings);
			createSunRiseSet(settings, true);
		}

		private void analogClock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ControlSettings settings = new ControlSettings(ControlType.AnalogClock, PointToScreen(e.GetPosition(this)));
			controlSettingsList.controls.Add(settings);
			createAnalogClock(settings, true);
		}

		private void moonVis_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ControlSettings settings = new ControlSettings(ControlType.MoonVis, PointToScreen(e.GetPosition(this)));
			controlSettingsList.controls.Add(settings);
			createMoonVis(settings, true);
		}

		private void earthVis_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ControlSettings settings = new ControlSettings(ControlType.EarthVis, PointToScreen(e.GetPosition(this)));
			controlSettingsList.controls.Add(settings);
			createEarthVis(settings, true);
		}

		private void beatClock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ControlSettings settings = new ControlSettings(ControlType.BeatClock, PointToScreen(e.GetPosition(this)));
			controlSettingsList.controls.Add(settings);
			createBeatClock(settings, true);
		}

		private void seasonsVis_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ControlSettings settings = new ControlSettings(ControlType.SeasonsVis, PointToScreen(e.GetPosition(this)));
			controlSettingsList.controls.Add(settings);
			createSeasonsVis(settings, true);
		}

		private void seasons_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ControlSettings settings = new ControlSettings(ControlType.Seasons, PointToScreen(e.GetPosition(this)));
			controlSettingsList.controls.Add(settings);
			createSeasons(settings, true);
		}

		private void sundial_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ControlSettings settings = new ControlSettings(ControlType.SunDial, PointToScreen(e.GetPosition(this)));
			controlSettingsList.controls.Add(settings);
			createSundial(settings, true);
		}

		private void timeZone_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ControlSettings settings = new ControlSettings(ControlType.TimeZoneDisp, PointToScreen(e.GetPosition(this)));
			controlSettingsList.controls.Add(settings);
			createTimeZoneDisp(settings, true);
		}

		private void timeAdjustment_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ControlSettings settings = new ControlSettings(ControlType.TimeAdjustment, PointToScreen(e.GetPosition(this)));
			controlSettingsList.controls.Add(settings);
			createTimeAdjustment(settings, true);
		}

		private void dayPart_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ControlSettings settings = new ControlSettings(ControlType.DayPart, PointToScreen(e.GetPosition(this)));
			controlSettingsList.controls.Add(settings);
			createDayPart(settings, true);
		}

		private void daylightDial_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ControlSettings settings = new ControlSettings(ControlType.DaylightDial, PointToScreen(e.GetPosition(this)));
			controlSettingsList.controls.Add(settings);
			createDaylightDial(settings, true);
		}

		private void daylightVis_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ControlSettings settings = new ControlSettings(ControlType.DaylightVis, PointToScreen(e.GetPosition(this)));
			controlSettingsList.controls.Add(settings);
			createDaylightVis(settings, true);
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			Hide();
			e.Cancel = true;
		}

	}
}
