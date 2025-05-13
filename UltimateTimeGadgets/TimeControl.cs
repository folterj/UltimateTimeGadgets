using System.Windows.Controls;
using System.Windows.Media;

namespace UltimateTimeGadgets
{
	public class TimeControl : UserControl
	{
		protected DateTimeModel datetimeModel = new DateTimeModel(false);

		// set in derived classes: defaultWidth, defaultHeight 
		public double defaultWidth;
		public double defaultHeight;

		public ColorStyle style = ColorStyle.Opaque;

		public virtual void setDateTime(DateTimeModel datetimeModel)
		{
			// normally overwritten
			this.datetimeModel = datetimeModel;
		}

		public void setStyle(ColorStyle style)
		{
			this.style = style;

			Color color = Colors.Black;
			Brush brush = Brushes.Black;
			Brush background = null;

			if (style == ColorStyle.Light)
			{
				color = Colors.White;
			}
			brush = new SolidColorBrush(color);

			if (style == ColorStyle.Opaque)
			{
				background = new SolidColorBrush(Color.FromScRgb(0.5f, 1, 1, 1));
			}

			updateStyle(style, color, brush, background);
		}

		public virtual void updateStyle(ColorStyle style, Color color, Brush brush, Brush background)
		{
			// dummy; will be overwritten
		}

	}
}
