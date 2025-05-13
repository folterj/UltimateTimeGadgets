using System.Windows.Controls;

namespace UltimateTimeGadgets
{
	/// <summary>
	/// Interaction logic for SeasonVisSettingsWindow.xaml
	/// </summary>
	public partial class SeasonVisSettingsWindow : SettingsWindow
	{
		SeasonVisControl timeControl;

		public SeasonVisSettingsWindow(TimeControl timeControl, ControlSettings settings)
		{
			this.timeControl = (SeasonVisControl)timeControl;
			this.settings = settings;

			InitializeComponent();

			fromSettings();
		}

		public override void fromSettings()
		{
			int index = settings.index;

			timeControl.setBackground(index);
			backgroundCombo.SelectedIndex = index - 1;
		}

		public override void toSettings()
		{
			int index = backgroundCombo.SelectedIndex + 1;

			settings.index = index;
			timeControl.setBackground(index);
		}

		private void clockFaceCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			toSettings();
		}

	}
}
