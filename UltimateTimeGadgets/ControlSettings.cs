using System;
using System.Windows;

namespace UltimateTimeGadgets
{
	[Serializable]
	public class ControlSettings
	{
		public ControlType type;

		public Point location = new Point();

		public double size = 1;

		public bool snapTop = false;
		public bool snapBottom = false;
		public bool snapLeft = false;
		public bool snapRight = false;

		public int screeni = 0;

		public string format = "";
		public int index = 0;

		public ControlSettings()
		{
		}

		public ControlSettings(ControlType type)
		{
			this.type = type;
		}

		public ControlSettings(Point location)
		{
			this.location = location;
		}

		public ControlSettings(ControlType type, Point location)
		{
			this.type = type;
			this.location = location;

			switch (type)
			{
				case ControlType.AnalogClock:
					format = Constants.showMinuteHandString + Constants.showSecondHandString;
					index = 1;
					break;

				case ControlType.BeatClock:
					format = Constants.showAtSignString;
					index = 1;
					break;

				case ControlType.SeasonsVis:
					index = 1;
					break;

				case ControlType.Date:
					format = "dd/MM/yyyy";
					break;

				case ControlType.DigitalClock:
					format = "HH:mm:ss";
					break;

				case ControlType.SunRiseSet:
					format = Constants.showRiseSetString + Constants.showClockString + Constants.showCivilString;
					break;

				case ControlType.TimeZoneDisp:
					format = "{0}";
					break;

				case ControlType.TimeAdjustment:
					format = Constants.formatDMYString + Constants.showTimeString;
					break;
			}
		}

		public void setType(ControlType type)
		{
			this.type = type;
		}

		public event EventHandler Removed;

		public void remove()
		{
			if (Removed != null)
			{
				Removed(this, EventArgs.Empty);
			}
		}

	}
}
