using System.Windows.Media;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for BeatClockControl.xaml
	/// </summary>
	public partial class BeatClockControl : TimeControl
	{
		BeatTimeFormatter beattimeFormatter = new BeatTimeFormatter();
		string format = "@ {0:F1}";

		public BeatClockControl()
		{
			defaultWidth = 60;
			defaultHeight = 20;

			InitializeComponent();
			beattimeFormatter.setFormat(format);
			DataContext = beattimeFormatter;
		}

		public override void setDateTime(DateTimeModel datetimeModel)
		{
			this.datetimeModel = datetimeModel;
			beattimeFormatter.setModel(this.datetimeModel);
		}

		public void setFormat(int decimals, bool atSign)
		{
			string format = "{0:F" + decimals.ToString() + "}";
			if (atSign)
				format = "@ " + format;
			this.format = format;
			beattimeFormatter.setFormat(format);
		}

		public override void updateStyle(ColorStyle style, Color color, Brush brush, Brush background)
		{
			clockText.Foreground = brush;
			textBorder.Background = background;
		}

	}
}
