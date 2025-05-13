using System.Windows.Media;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for TimeZoneDispControl.xaml
	/// </summary>
	public partial class TimeZoneDispControl : TimeControl
	{
		string format = "{1}";
		TimeZoneFormatter timeZoneFormatter = new TimeZoneFormatter();

		public TimeZoneDispControl()
		{
			defaultWidth = 200;
			defaultHeight = 20;

			InitializeComponent();
			timeZoneFormatter.setFormat(format);
			DataContext = timeZoneFormatter;
		}

		public override void setDateTime(DateTimeModel datetimeModel)
		{
			this.datetimeModel = datetimeModel;
			timeZoneFormatter.setModel(datetimeModel);
		}

		public void setFormat(string format)
		{
			this.format = format;
			timeZoneFormatter.setFormat(format);
		}

		public override void updateStyle(ColorStyle style, Color color, Brush brush, Brush background)
		{
			timezoneText.Foreground = brush;
			textBorder.Background = background;
		}

	}
}
