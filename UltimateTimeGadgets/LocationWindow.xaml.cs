using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for LocationWindow.xaml
	/// </summary>
	public partial class LocationWindow : Window
	{
		DateTimeModel timezoneModel = new DateTimeModel(false);

		LocationStore locationStore;
		DateTimeModel datetimeModel;
		ControlSettingsList settings;

		List<string> locationMatches = new List<string>();
		List<int> locationIndices = new List<int>();

		List<string> tooMany = new List<string> { "..." };

		bool updating = false;

		public LocationWindow(DateTimeModel datetimeModel, LocationStore locationStore, ControlSettingsList settings)
		{
			InitializeComponent();
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Close,
				new ExecutedRoutedEventHandler(delegate(object sender, ExecutedRoutedEventArgs args) { this.Close(); })));

			this.datetimeModel = datetimeModel;
			this.locationStore = locationStore;
			this.settings = settings;

			timezoneModel.copy(this.datetimeModel);

			timeZoneMap.setLocationStore(this.locationStore);
			timeZoneMap.setDateTime(timezoneModel);
			timeZone.setDateTime(timezoneModel);
			lonLat.setDateTime(timezoneModel);
			locationCombo.Text = locationStore.locationToString(datetimeModel.location);
			if (locationCombo.Text == "")
			{
				locationSelected(false);
			}
		}

		private void okButton_Click(object sender, RoutedEventArgs e)
		{
			datetimeModel.copy(timezoneModel);

			settings.location = locationStore.locationToString(datetimeModel.location);
			settings.lon = datetimeModel.lon;
			settings.lat = datetimeModel.lat;
			settings.timeZoneId = datetimeModel.timeZone.Id;

			settings.save();

			Hide();
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			Hide();
		}

		private void openCombo(bool open)
		{
			locationCombo.StaysOpenOnEdit = open;
			locationCombo.IsDropDownOpen = open;
		}

		private void locationCombo_GotFocus(object sender, RoutedEventArgs e)
		{
			openCombo(true);
		}

		private void locationCombo_LostFocus(object sender, RoutedEventArgs e)
		{
			openCombo(false);
		}

		private void locationCombo_KeyDown(object sender, KeyEventArgs e)
		{
			openCombo(true);
		}

		private void locationCombo_TextChanged(object sender, TextChangedEventArgs e)
		{
			string match = locationCombo.Text;
			int i = 0;
			int maxMatch = 100;

			if (!updating)
			{
				locationCombo.ItemsSource = null;
				locationMatches.Clear();
				locationIndices.Clear();

				if (match != "")
				{
					foreach (string location in locationStore.locationList)
					{
						if (location.ToLower().Contains(match.ToLower()))
						{
							locationMatches.Add(location);
							locationIndices.Add(i);
						}
						i++;
						if (locationMatches.Count > maxMatch)
						{
							// performance: stop searching as too many hits to display anyway
							break;
						}
					}
				}
				// only show matches when narrowed down significantly
				if (locationMatches.Count > maxMatch)
				{
					// matches > 100
					locationCombo.ItemsSource = tooMany;
				}
				else if (locationMatches.Count > 0)
				{
					// 0 < matches < 100
					locationCombo.ItemsSource = locationMatches;
				}
				else
				{
					// matches = 0
					locationCombo.ItemsSource = null;
				}
			}
		}

		private void locationCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// location selected from location combo
			if (!updating)
			{
				// don't execute if currently updating
				int i = locationCombo.SelectedIndex;

				if (i >= 0)
				{
					Location location = locationStore.locations[locationIndices[i]];
					setLocationTimeZone(location);
					timezoneModel.setLocation(location);
				}
				proximityLabel.Content = "";
			}
		}

		private void timeZoneMap_locationSelected(object sender, PropertyChangedEventArgs e)
		{
			locationSelected(true);
		}

		private void locationSelected(bool timeZonefromNearLocation)
		{
			// new location selected using mouse
			Location nearestLocation;
			string label = "";

			// find nearest location
			nearestLocation = locationStore.findNearestLocation(timeZoneMap.getLon(), timeZoneMap.getLat());
			if (nearestLocation != null)
			{
				label = locationStore.locationToString(nearestLocation);
				label += string.Format(" Lon: {0:F1} Lat: {1:F1}", nearestLocation.lon, nearestLocation.lat);
			}
			proximityLabel.Content = label;

			// ignore combobox selection change event
			updating = true;
			locationCombo.ItemsSource = null;
			locationCombo.Text = Constants.customLocation;
			updating = false;

			timezoneModel.location = null;
			timezoneModel.setLonLat(timeZoneMap.getLon(), timeZoneMap.getLat());
			if (timeZonefromNearLocation)
			{
				setLocationTimeZone(nearestLocation);
			}
		}

		void setLocationTimeZone(Location location)
		{
			string winTimeZoneId = locationStore.findWinTimeZoneLocation(location);
			if (winTimeZoneId != "")
			{
				timezoneModel.setTimeZone(TimeZoneInfo.FindSystemTimeZoneById(winTimeZoneId));
			}
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			Hide();
			e.Cancel = true;
		}

	}
}
