using System;
using System.Drawing;
namespace ThoughtWorks.QRCode.Codec.Data
{
	public class QRCodeBitmapImage : QRCodeImage
	{
		private Bitmap image;
		public virtual int Width
		{
			get
			{
				return this.image.Width;
			}
		}
		public virtual int Height
		{
			get
			{
				return this.image.Height;
			}
		}
		public QRCodeBitmapImage(Bitmap image)
		{
			this.image = image;
		}
		public virtual int getPixel(int x, int y)
		{
			return this.image.GetPixel(x, y).ToArgb();
		}
	}
}
