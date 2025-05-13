
namespace UltimateTimeGadgets
{
	public class Country
	{
		public string countryCode;
		public string name;
		public string capital;
		public string continent;

		public Country()
		{
		}

		public Country(string countryCode, string name, string capital, string continent)
		{
			this.countryCode = countryCode;
			this.name = name;
			this.capital = capital;
			this.continent = continent;
		}

		public Country(string[] parts)
		{
			countryCode = parts[0];
			name = parts[1];
			capital = parts[2];
			continent = parts[3];
		}
	}
}
