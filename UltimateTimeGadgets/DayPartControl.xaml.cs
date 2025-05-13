using System.Windows.Media;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for DayPartControl.xaml
	/// </summary>
	public partial class DayPartControl : TimeControl
	{
		DateTimeFormatter datetimeFormatter = new DateTimeFormatter();

		public DayPartControl()
		{
			defaultWidth = 60;
			defaultHeight = 20;

			InitializeComponent();
			datetimeFormatter.setFormat("{3}");
			DataContext = datetimeFormatter;
		}

		public override void setDateTime(DateTimeModel datetimeModel)
		{
			this.datetimeModel = datetimeModel;
			datetimeFormatter.setModel(datetimeModel);
		}

		public override void updateStyle(ColorStyle style, Color color, Brush brush, Brush background)
		{
			clockText.Foreground = brush;
			textBorder.Background = background;
		}

	}
}
