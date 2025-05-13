
namespace UltimateTimeGadgets
{
	public class Timezone
	{
		public string countryCode;
		public string timeZoneId;
		public double janTimeOffset;
		public double julTimeOffset;
		public double rawTimeOffset;
		public string winTimeZoneId;

		public Timezone()
		{
		}

		public Timezone(string countryCode, string timeZoneId,
						double janTimeOffset, double julTimeOffset, double rawTimeOffset,
						string winTimeZoneId)
		{
			this.countryCode = countryCode;
			this.timeZoneId = timeZoneId;
			this.janTimeOffset = janTimeOffset;
			this.julTimeOffset = julTimeOffset;
			this.rawTimeOffset = rawTimeOffset;
			this.winTimeZoneId = winTimeZoneId;
		}

		public Timezone(string[] parts)
		{
			countryCode = parts[0];
			timeZoneId = parts[1];
			double.TryParse(parts[2], out janTimeOffset);
			double.TryParse(parts[3], out julTimeOffset);
			double.TryParse(parts[4], out rawTimeOffset);
			winTimeZoneId = parts[5];
		}
	}
}
