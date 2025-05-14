
namespace UltimateTimeGadgets
{
	public class Location
	{
		public string name;
		public string nameAsc;
		public double lat;
		public double lon;
		public string countryCode;
		public int population;
		public int elevation;
		public int elevation0;
		public string timeZoneId;

		public Location()
		{
			name = "";
		}

		public Location(string name, string nameAsc,
						double lat, double lon,
						string countryCode, int population,
						int elevation, int elevation0,
						string timeZoneId)
		{
			this.name = name;
			this.nameAsc = nameAsc;
			this.lat = lat;
			this.lon = lon;
			this.countryCode = countryCode;
			this.population = population;
			this.elevation = elevation;
			this.elevation0 = elevation0;
			this.timeZoneId = timeZoneId;
		}

		public bool isEmpty()
		{
			return (name == "");
		}

		public Location(string[] parts)
		{
			name = parts[0];
			nameAsc = parts[1];
			double.TryParse(parts[2], out lat);
			double.TryParse(parts[3], out lon);
			countryCode = parts[4];
			int.TryParse(parts[5], out population);
			int.TryParse(parts[6], out elevation);
			int.TryParse(parts[7], out elevation0);
			timeZoneId = parts[8];
		}

		public override string ToString()
		{
			//return base.ToString();
			return name;
		}
	}
}
