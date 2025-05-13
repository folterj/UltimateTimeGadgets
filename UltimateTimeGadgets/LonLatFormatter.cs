using System;
using System.ComponentModel;

namespace UltimateTimeGadgets
{
	class LonLatFormatter : INotifyPropertyChanged
	{
		DateTimeModel datetimeModel;
		string format = "{0}";

		public string lonstring
		{
			get { return string.Format(format, datetimeModel.lon); }
			set { double.TryParse(value, out datetimeModel.lon); datetimeModel.updateLonLat(); datetimeModel.getTimeZoneFromLon(); }
		}

		public string latstring
		{
			get { return string.Format(format, datetimeModel.lat); }
			set { double.TryParse(value, out datetimeModel.lat); datetimeModel.updateLonLat(); }
		}

		public LonLatFormatter()
		{
			setModel(new DateTimeModel(false));
			init();
		}

		public LonLatFormatter(DateTimeModel datetimeModel)
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
			datetimeModel.LonLatChanged += new PropertyChangedEventHandler(datetimeModel_PropertyChanged);
		}

		public void setFormat(string format)
		{
			this.format = format;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		void datetimeModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			NotifyPropertyChanged("lonstring");
			NotifyPropertyChanged("latstring");
		}

	}
}
