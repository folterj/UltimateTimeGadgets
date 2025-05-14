using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : System.Windows.Application
	{
		DateTimeModel datetimeModel;
		LocationStore locationStore;
		ControlSettingsList settings;

		GalleryWindow galleryWindow;
		LocationWindow locationWindow;
		GeneralSettingsWindow settingsWindow;
		AboutBox about;

		NotifyIcon trayIcon;
		MenuItem[] menuItems;

		bool init = false;

		App()
		{
			Process currentProcess = Process.GetCurrentProcess();
			if (Process.GetProcessesByName(currentProcess.ProcessName).Length > 1)
			{
				// other process with same name already exists (currently more than 1 running); exit
				Shutdown();
				return;
			}

			datetimeModel = new DateTimeModel(true);
			locationStore = new LocationStore();
			settings = new ControlSettingsList();

			bool showGallery = true;
			string[] args = Environment.GetCommandLineArgs();

			for (int index = 1; index < args.Length; index++)
			{
				if (args[index].ToLower() == Constants.noGalleryOption.ToLower())
				{
					showGallery = false;
				}
			}

			createTrayIcon();

			loadControlSettings();
			updateLocation();

			galleryWindow = new GalleryWindow(datetimeModel, settings);
			locationWindow = new LocationWindow(datetimeModel, locationStore, settings);
			settingsWindow = new GeneralSettingsWindow(settings, galleryWindow);
			about = new AboutBox();

			if (!showGallery)
			{
				setGalleryWindowVisible(false);
			}
			galleryWindow.Show();	// force Loaded to be called
			if (!showGallery)
			{
				galleryWindow.Hide();
			}
            about.startCheckUpdates();
			init = true;
		}

		private void setGalleryWindowVisible(bool visible)
		{
			if (visible)
			{
				galleryWindow.WindowState = WindowState.Normal;
			}
			else
			{
				galleryWindow.WindowState = WindowState.Minimized;
			}
			galleryWindow.ShowInTaskbar = visible;
		}

		public bool loadControlSettings()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ControlSettingsList));
			try
			{
				using (var reader = new StreamReader(settings.filename))
				{
					settings = (ControlSettingsList)serializer.Deserialize(reader);
				}
				return true;
			}
			catch (Exception)
			{
				// read/parse error
			}
			return false;
		}

		void updateLocation()
		{
			string locations;
			TimeZoneInfo timeZone = TimeZoneInfo.Local;
			Location location;
			bool settingsUpdated = false;

			// get from settings
			locations = settings.location;
			if (settings.timeZoneId != "")
			{
				timeZone = TimeZoneInfo.FindSystemTimeZoneById(settings.timeZoneId);
			}
			
			if (locations == "" && settings.lon == 0 && settings.lat == 0)
			{
				// location not set
				RegionInfo info = RegionInfo.CurrentRegion;
				location = locationStore.findLocation(info.EnglishName, timeZone);
				if (location != null)
				{
					if (location.name != "")
					{
						datetimeModel.setLocation(location);
					}
				}
				settingsUpdated = true;
			}
			else if (locations != "")
			{
				location = locationStore.getLocationFromString(locations);
				datetimeModel.setLocation(location);
				timeZone = locationStore.getLocationTimeZone(location);
			}
			else
			{
				datetimeModel.setLonLat(settings.lon, settings.lat);
			}

			datetimeModel.setTimeZone(timeZone);

			if (settingsUpdated)
			{
				settings.location = locationStore.locationToString(datetimeModel.location);
				settings.lon = datetimeModel.lon;
				settings.lat = datetimeModel.lat;
				settings.timeZoneId = datetimeModel.timeZone.Id;

				settings.save();
			}
		}

		void createTrayIcon()
		{
			trayIcon = new NotifyIcon();
			trayIcon.Icon = UltimateTimeGadgets.Properties.Resources.UltimateTimeGadgetsTrayIcon;
			trayIcon.Text = "Ultimate Time Gadgets";

			menuItems = new MenuItem[]
			{
				new MenuItem("Gadget Gallery", new EventHandler(gallery_Click)),
				new MenuItem("Location Setup", new EventHandler(location_Click)),
				new MenuItem("General Settings", new EventHandler(settings_Click)),
				new MenuItem("Close Gadgets", new EventHandler(closeAll_Click)),
				new MenuItem("About", new EventHandler(about_Click)),
				new MenuItem("Exit", new EventHandler(exit_Click))
			};
			trayIcon.ContextMenu = new ContextMenu(menuItems);
			trayIcon.DoubleClick += new EventHandler(gallery_Click);
			trayIcon.Visible = true;
		}

		void removeTrayIcon()
		{
			trayIcon.Dispose();
			trayIcon.Icon = null;
		}

		private void gallery_Click(Object sender, System.EventArgs e)
		{
			setGalleryWindowVisible(true);
			galleryWindow.Show();
		}

		private void location_Click(Object sender, System.EventArgs e)
		{
			locationWindow.Show();
		}

		private void settings_Click(Object sender, System.EventArgs e)
		{
			settingsWindow.Show();
		}

		private void about_Click(Object sender, System.EventArgs e)
		{
			about.Show();
		}

		private void closeAll_Click(Object sender, System.EventArgs e)
		{
			galleryWindow.closeAll();
		}

		private void exit_Click(Object sender, System.EventArgs e)
		{
			Shutdown();
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			exit();
		}

		private void exit()
		{
			if (init)
			{
				removeTrayIcon();
				// don't save settings when windows is shutting down
			}
		}

	}
}
