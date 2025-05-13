
namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for LonLatControl.xaml
	/// </summary>
	public partial class LonLatControl : TimeControl
	{
		string format = "{0:F1}";
		LonLatFormatter lonLatFormatter = new LonLatFormatter();

		public LonLatControl()
		{
			InitializeComponent();
			lonLatFormatter.setFormat(format);
			DataContext = lonLatFormatter;
		}

		public override void setDateTime(DateTimeModel datetimeModel)
		{
			this.datetimeModel = datetimeModel;
			lonLatFormatter.setModel(datetimeModel);
		}

	}
}
