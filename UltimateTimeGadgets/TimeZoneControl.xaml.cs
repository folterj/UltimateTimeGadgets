
namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for TimeZoneControl.xaml
	/// </summary>
	public partial class TimeZoneControl : TimeControl
	{
		string format = "{0}";
		TimeZoneFormatter timeZoneFormatter = new TimeZoneFormatter();

		public TimeZoneControl()
		{
			InitializeComponent();
			timezoneCombo.ItemsSource = timeZoneFormatter.timeZones;
			timeZoneFormatter.setFormat(format);
			DataContext = timeZoneFormatter;
		}

		public override void setDateTime(DateTimeModel datetimeModel)
		{
			this.datetimeModel = datetimeModel;
			timeZoneFormatter.setModel(datetimeModel);
		}

	}
}
