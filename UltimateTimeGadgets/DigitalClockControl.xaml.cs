using System.Windows.Media;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for DigitalClockControl.xaml
	/// </summary>
	public partial class DigitalClockControl : TimeControl
	{
		DateTimeFormatter datetimeFormatter = new DateTimeFormatter();
		string format = "HH:mm:ss";

		public DigitalClockControl()
		{
			defaultWidth = 60;
			defaultHeight = 20;

			InitializeComponent();
			setFormat(format);
			DataContext = datetimeFormatter;
		}

		public override void setDateTime(DateTimeModel datetimeModel)
		{
			this.datetimeModel = datetimeModel;
			datetimeFormatter.setModel(datetimeModel);
		}

		public string getFormat()
		{
			return format;
		}

		public void setFormat(string format)
		{
			string fullFormat = "";
			string[] parts;
			bool utc = false;

			this.format = format;
			utc = format.ToLower().Contains("z");
			parts = format.Split(' ');
			foreach (string part in parts)
			{
				if (!utc || part.Contains("z"))
				{
					// normal and utc offset
					fullFormat += "{0:" + part + "} ";
				}
				else
				{
					// utc time
					fullFormat += "{2:" + part + "} ";
				}
			}
			datetimeFormatter.setFormat(fullFormat);
		}

		public override void updateStyle(ColorStyle style, Color color, Brush brush, Brush background)
		{
			clockText.Foreground = brush;
			textBorder.Background = background;
		}

	}
}
