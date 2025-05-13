using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace UltimateTimeGadgets
{
	partial class AboutBox : Form
	{
		public AboutBox()
		{
			InitializeComponent();
			this.Text = String.Format("About {0}", AssemblyTitle);
			this.labelProductName.Text = AssemblyProduct;
			this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
			this.labelCopyright.Text = AssemblyCopyright;
			this.labelCompanyName.Text = AssemblyCompany;
			this.textBoxDescription.Text = AssemblyDescription;
		}

		#region Assembly Attribute Accessors

		public string AssemblyTitle
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				if (attributes.Length > 0)
				{
					AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
					if (titleAttribute.Title != "")
					{
						return titleAttribute.Title;
					}
				}
				return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}

		public string AssemblyVersion
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}

		public string AssemblyDescription
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyDescriptionAttribute)attributes[0]).Description;
			}
		}

		public string AssemblyProduct
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyProductAttribute)attributes[0]).Product;
			}
		}

		public string AssemblyCopyright
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}

		public string AssemblyCompany
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyCompanyAttribute)attributes[0]).Company;
			}
		}
		#endregion

		private void textBoxDescription_LinkClicked(object sender, LinkClickedEventArgs e)
		{
			try
			{
				Process.Start(e.LinkText);
			}
			catch (Exception)
			{
			}
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			Hide();
		}

        string getUrl(string url)
        {
            string content = "";

            WebRequest request = WebRequest.Create(url);
            request.UseDefaultCredentials = true;
            request.Timeout = 30000;	// takes max approx 2.5 seconds
            ((HttpWebRequest)request).Accept = "*/*";
            ((HttpWebRequest)request).UserAgent = "compatible";	// essential!
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            content = reader.ReadToEnd();
            reader.Close();
            responseStream.Close();
            response.Close();

            return content;
        }

        bool openWebLink(string link)
        {
            try
            {
                Process.Start(link);
                return true;
            }
            catch (Exception e)
            {
                Clipboard.SetText(link);
                MessageBox.Show("Error: " + e.Message + "\nPlease use your preferred browser to navigate to this link manually\n(This link has been copied to the clipboard - Select Paste in your browser address bar)", "Application error");
            }
            return false;
        }

        int compareVersions(string version1, string version2)
        {
            int comp = 0;
            string[] versions1 = version1.Split('.');
            string[] versions2 = version2.Split('.');
            int v1, v2;

            for (int i = 0; i < version1.Length && i < versions2.Length; i++)
            {
                int.TryParse(versions1[i], out v1);
                int.TryParse(versions2[i], out v2);

                if (v2 > v1)
                {
                    comp = 1;
                    break;
                }
                else if (v2 < v1)
                {
                    comp = -1;
                    break;
                }
            }
            return comp;
        }

        private void checkUpdateButton_Click(object sender, EventArgs e)
        {
            startCheckUpdates();
        }

        public void startCheckUpdates()
        {
            Thread updateThread = new Thread(checkUpdates);
            updateThread.Start();
        }

        private void checkUpdates()
        {
            string webVersion = "";
            string assemblyName;

            try
            {
                AssemblyName currentAssembly = Assembly.GetExecutingAssembly().GetName();
                assemblyName = Assembly.GetExecutingAssembly().GetName().Name.ToLower();
                webVersion = getUrl(AssemblyDescription + "/files/" + assemblyName + "ver");
                if (webVersion != "")
                {
                    if (compareVersions(AssemblyVersion, webVersion) > 0)
                    {
                        if (MessageBox.Show("New version available to download\nGo to download page now?", AssemblyTitle,
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            openWebLink(AssemblyDescription);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        
        private void AboutBox_FormClosing(object sender, FormClosingEventArgs e)
		{
			Hide();
			e.Cancel = true;
		}

	}
}
