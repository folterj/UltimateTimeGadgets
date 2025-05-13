using System.Windows.Media;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for SeasonsControl.xaml
	/// </summary>
	public partial class SeasonsControl : TimeControl
	{
		string format = "{0}";
		SeasonFormatter seasonFormatter = new SeasonFormatter();

		public SeasonsControl()
		{
			defaultWidth = 60;
			defaultHeight = 20;

			InitializeComponent();
			seasonFormatter.setFormat(format);
			DataContext = seasonFormatter;
		}

		public override void setDateTime(DateTimeModel datetimeModel)
		{
			this.datetimeModel = datetimeModel;
			seasonFormatter.setModel(datetimeModel);
		}

		public void setFormat(string format)
		{
			this.format = format;
			seasonFormatter.setFormat(format);
		}

		public override void updateStyle(ColorStyle style, Color color, Brush brush, Brush background)
		{
			seasonText.Foreground = brush;
			textBorder.Background = background;
		}

	}
}
