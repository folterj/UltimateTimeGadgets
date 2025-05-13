using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace UltimateTimeGadgets
{
	[Serializable]
	public class ControlSettingsList : SettingsObserver
	{
		public string location = "";
		public double lon = 0;
		public double lat = 0;
		public string timeZoneId = "";

		public ColorStyle colorStyle = ColorStyle.Opaque;
		public bool gadgetsLocked = false;
		public bool gadgetsOnTop = false;
		public bool windowsStartup = false;

		public List<ControlSettings> controls = new List<ControlSettings>();

		[XmlIgnore]
		public string filename = "";

		public ControlSettingsList()
		{
			try
			{
				filename = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + Constants.settingsPathName;
				// automatically creates (complete) path if not exists
				Directory.CreateDirectory(filename);
				filename += "\\" + Constants.settingsFileName;
			}
			catch (Exception)
			{
				filename = "";
			}
		}

		public void copyGeneralSettings(ControlSettingsList settings)
		{
			this.colorStyle = settings.colorStyle;
			this.gadgetsLocked = settings.gadgetsLocked;
			this.gadgetsOnTop = settings.gadgetsOnTop;
			this.windowsStartup = settings.windowsStartup;
		}

		public void save()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ControlSettingsList));
			using (var writer = new StreamWriter(filename))
			{
				serializer.Serialize(writer, this);
			}
		}

		public void settingsChanged()
		{
			save();
		}

	}
}
