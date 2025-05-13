using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Resources;

namespace UltimateTimeGadgets
{
	public class LocationStore
	{
		public List<string> locationList = new List<string>();
		public List<Location> locations = new List<Location>();
		public List<Country> countries = new List<Country>();
		public List<Timezone> timezoneCodes = new List<Timezone>();

		public LocationStore()
		{
			readCountries();
			readTimezones();
			readLocation();
			createLocationList();
		}

		void readLocation()
		{
			Uri uri = new Uri("/UltimateTimeGadgets;component/Resources/locations.csv", UriKind.Relative);
			StreamResourceInfo res = Application.GetResourceStream(uri);
			StreamReader reader = new StreamReader(res.Stream);
			string s = reader.ReadToEnd();
			reader.Close();

			string[] lines = s.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			string[] parts;

			locations.Clear();

			foreach (string line in lines)
			{
				parts = line.Split(',');
				locations.Add(new Location(parts));
			}
		}

		void readCountries()
		{
			Uri uri = new Uri("/UltimateTimeGadgets;component/Resources/countries.csv", UriKind.Relative);
			StreamResourceInfo res = Application.GetResourceStream(uri);
			StreamReader reader = new StreamReader(res.Stream);
			string s = reader.ReadToEnd();
			reader.Close();

			string[] lines = s.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			string[] parts;

			countries.Clear();

			foreach (string line in lines)
			{
				parts = line.Split(',');
				countries.Add(new Country(parts));
			}
		}

		void readTimezones()
		{
			Uri uri = new Uri("/UltimateTimeGadgets;component/Resources/timezones.csv", UriKind.Relative);
			StreamResourceInfo res = Application.GetResourceStream(uri);
			StreamReader reader = new StreamReader(res.Stream);
			string s = reader.ReadToEnd();
			reader.Close();

			string[] lines = s.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			string[] parts;

			timezoneCodes.Clear();

			foreach (string line in lines)
			{
				parts = line.Split(',');
				timezoneCodes.Add(new Timezone(parts));
			}
		}

		void createLocationList()
		{
			locationList.Clear();

			foreach (Location location in locations)
			{
				locationList.Add(locationToString(location));
			}
		}

		public string locationToString(Location location)
		{
			string locationstring = "";
			string countrystring = "";

			if (location != null)
			{
				foreach (Country country in countries)
				{
					if (country.countryCode == location.countryCode)
					{
						countrystring = country.name;
					}
				}
				locationstring = location.name;
				if (countrystring != "")
				{
					locationstring += " (" + countrystring + ")";
				}
			}
			return locationstring;
		}

		public Location getLocationFromString(string locationMatch)
		{
			int foundi = -1;
			int i = 0;

			foreach (string locationString in locationList)
			{
				if (locationString == locationMatch)
				{
					foundi = i;
				}
				i++;
			}

			if (foundi >= 0)
			{
				return locations[foundi];
			}
			return null;
		}

		public Location findNearestLocation(double lon, double lat)
		{
			Location nearestLocation = new Location();
			double mindist = 1000;
			double dist;

			foreach (Location location in locations)
			{
				dist = Math.Sqrt(Math.Pow(location.lon - lon, 2) + Math.Pow(location.lat - lat, 2));
				if (dist < mindist)
				{
					mindist = dist;
					nearestLocation = location;
				}
			}
			return nearestLocation;
		}

		public Location findCountryLocation(string countryString)
		{
			Location bestLocation = new Location();
			Country countryFound = new Country();
			bool found = false;

			// search for exact match
			foreach (Country country in countries)
			{
				if (country.name.ToLower() == countryString.ToLower())
				{
					countryFound = country;
					found = true;
					break;
				}
			}

			if (!found)
			{
				// search for contained match
				foreach (Country country in countries)
				{
					if (country.name.ToLower().Contains(countryString.ToLower()))
					{
						countryFound = country;
						found = true;
						break;
					}
				}
			}

			if (found)
			{
				foreach (Location location in locations)
				{
					if (location.name.ToLower() == countryFound.capital.ToLower() &&
						location.countryCode == countryFound.countryCode)
					{
						bestLocation = location;
						break;
					}
				}
			}

			return bestLocation;
		}
		
		public string findWinTimeZoneLocation(Location location)
		{
			string winTimeZoneId = "";

			foreach (Timezone timezoneCode in timezoneCodes)
			{
				if (timezoneCode.timeZoneId == location.timeZoneId)
				{
					winTimeZoneId = timezoneCode.winTimeZoneId;
				}
			}
			return winTimeZoneId;
		}

	}
}
