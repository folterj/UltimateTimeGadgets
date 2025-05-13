using System.ComponentModel;

namespace UltimateTimeGadgets
{
	class BeatTimeFormatter : INotifyPropertyChanged
	{
		DateTimeModel datetimeModel;
		string format = "@ {0}";

		public string beattimestring
		{
			get { return string.Format(format, datetimeModel.getBeatTime()); }
		}

		public BeatTimeFormatter()
		{
			setModel(new DateTimeModel(false));
			init();
		}

		public BeatTimeFormatter(DateTimeModel datetimeModel)
		{
			setModel(datetimeModel);
			init();
		}

		public void setModel(DateTimeModel datetimeModel)
		{
			this.datetimeModel = datetimeModel;
			init();
		}

		void init()
		{
			datetimeModel.SecondsChanged += new PropertyChangedEventHandler(datetimeModel_PropertyChanged);
			datetimeModel.TimeZoneChanged += new PropertyChangedEventHandler(datetimeModel_PropertyChanged);
		}

		public void setFormat(string format)
		{
			this.format = format;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(string info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		void datetimeModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			NotifyPropertyChanged("beattimestring");
		}

	}
}
