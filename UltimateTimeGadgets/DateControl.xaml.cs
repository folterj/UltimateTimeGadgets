using System.Windows.Media;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for DateControl.xaml
	/// </summary>
	public partial class DateControl : TimeControl
	{
		DateTimeFormatter datetimeFormatter = new DateTimeFormatter();
		string format = "d/M/yyyy";

		public DateControl()
		{
			defaultWidth = 80;
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

		public void setFormat(string format)
		{
			string fullFormat = "";
			string[] parts;

			this.format = format;
			parts = format.Split(' ');
			foreach (string part in parts)
			{
				if (part.Contains("n"))
				{
					fullFormat += "{1:d} ";
				}
				else
				{
					fullFormat += "{0:" + part + "} ";
				}
			}
			datetimeFormatter.setFormat(fullFormat);
		}

		public override void updateStyle(ColorStyle style, Color color, Brush brush, Brush background)
		{
			dateText.Foreground = brush;
			textBorder.Background = background;
		}

	}
}
