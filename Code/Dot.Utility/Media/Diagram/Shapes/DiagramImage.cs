using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.IO;
using System.Drawing.Imaging;
using System.Globalization;
using System.Resources;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class DiagramImage : DiagramShape
    {
        static DiagramImage()
        {
            DiagramImage.myDefaultImageList = null;
            DiagramImage.myDefaultResourceManager = null;
        }

        public DiagramImage()
        {
            this.myAlignment = 2;
            this.myResourceManager = DiagramImage.DefaultResourceManager;
            this.myName = null;
            this.myImageList = DiagramImage.DefaultImageList;
            this.myIndex = -1;
            this.myImage = null;
            base.InternalFlags &= -33;
            base.InternalFlags |= 0x100000;
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0x641:
                    {
                        this.Image = (System.Drawing.Image)e.GetValue(undo);
                        return;
                    }
                case 0x642:
                    {
                        this.ResourceManager = (System.Resources.ResourceManager)e.GetValue(undo);
                        return;
                    }
                case 0x643:
                    {
                        this.Name = (string)e.GetValue(undo);
                        return;
                    }
                case 0x644:
                    {
                        this.Alignment = e.GetInt(undo);
                        return;
                    }
                case 0x645:
                    {
                        this.AutoResizes = (bool)e.GetValue(undo);
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        private System.Drawing.Image ConvertIconToImage(Icon icon)
        {
            return icon.ToBitmap();
        }

        private System.Drawing.Image ConvertObjectToImage(object obj)
        {
            if (obj != null)
            {
                System.Drawing.Image image1 = obj as System.Drawing.Image;
                if (image1 != null)
                {
                    return image1;
                }
                Icon icon1 = obj as Icon;
                if (icon1 != null)
                {
                    return this.ConvertIconToImage(icon1);
                }
                byte[] buffer1 = obj as byte[];
                if (buffer1 != null)
                {
                    MemoryStream stream1 = new MemoryStream(buffer1);
                    return System.Drawing.Image.FromStream(stream1);
                }
            }
            return null;
        }

        public override RectangleF ExpandPaintBounds(RectangleF rect, DiagramView view)
        {
            if (this.Shadowed)
            {
                SizeF ef1 = this.GetShadowOffset(view);
                if (ef1.Width < 0f)
                {
                    rect.X += ef1.Width;
                    rect.Width -= ef1.Width;
                }
                else
                {
                    rect.Width += ef1.Width;
                }
                if (ef1.Height < 0f)
                {
                    rect.Y += ef1.Height;
                    rect.Height -= ef1.Height;
                    return rect;
                }
                rect.Height += ef1.Height;
            }
            return rect;
        }

        private void GetImage()
        {
            if (this.myImage == null)
            {
                this.myImage = this.LoadImage();
                if ((this.myImage != null) && ((base.Width == 0f) || (base.Height == 0f)))
                {
                    this.UpdateSize();
                }
            }
        }

        public virtual System.Drawing.Image LoadImage()
        {
            int num1 = this.Index;
            if (num1 >= 0)
            {
                System.Windows.Forms.ImageList list1 = this.ImageList;
                if ((list1 != null) && (num1 < list1.Images.Count))
                {
                    return list1.Images[num1];
                }
                if ((DiagramImage.DefaultImageList != null) && (num1 < DiagramImage.DefaultImageList.Images.Count))
                {
                    return DiagramImage.DefaultImageList.Images[num1];
                }
            }
            string text1 = this.Name;
            if (text1 == null)
            {
                return null;
            }
            System.Drawing.Image image1 = null;
            if (this.ResourceManager != null)
            {
                try
                {
                    object obj1 = this.ResourceManager.GetObject(text1, CultureInfo.CurrentCulture);
                    image1 = this.ConvertObjectToImage(obj1);
                }
                catch (MissingManifestResourceException)
                {
                }
            }
            if (image1 == null)
            {
                if (DiagramImage.DefaultResourceManager != null)
                {
                    try
                    {
                        object obj2 = DiagramImage.DefaultResourceManager.GetObject(text1, CultureInfo.CurrentCulture);
                        image1 = this.ConvertObjectToImage(obj2);
                    }
                    catch (MissingManifestResourceException)
                    {
                    }
                }
                if (image1 == null)
                {
                    try
                    {
                        image1 = System.Drawing.Image.FromFile(text1);
                    }
                    catch (OutOfMemoryException exception1)
                    {
                        DiagramShape.Trace("LoadImage: " + exception1.ToString());
                    }
                    catch (IOException exception2)
                    {
                        DiagramShape.Trace("LoadImage: " + exception2.ToString());
                    }
                    catch (ArgumentException exception3)
                    {
                        DiagramShape.Trace("LoadImage: " + exception3.ToString());
                    }
                }
            }
            return image1;
        }

        public override void Paint(Graphics g, DiagramView view)
        {
            RectangleF ef1 = this.Bounds;
            System.Drawing.Image image1 = this.Image;
            int num1 = this.Index;
            if ((image1 == null) && (num1 >= 0))
            {
                System.Windows.Forms.ImageList list1 = view.ImageList;
                if ((list1 != null) && (num1 < list1.Images.Count))
                {
                    image1 = list1.Images[num1];
                }
            }
            if (image1 != null)
            {
                if (this.Shadowed)
                {
                    SizeF ef2 = this.GetShadowOffset(view);
                    ColorMatrix matrix1 = new ColorMatrix();
                    matrix1.Matrix00 = 0f;
                    matrix1.Matrix11 = 0f;
                    matrix1.Matrix22 = 0f;
                    SolidBrush brush1 = this.GetShadowBrush(view) as SolidBrush;
                    if (brush1 != null)
                    {
                        Color color1 = brush1.Color;
                        matrix1.Matrix30 = ((float)color1.R) / 255f;
                        matrix1.Matrix31 = ((float)color1.G) / 255f;
                        matrix1.Matrix32 = ((float)color1.B) / 255f;
                        matrix1.Matrix33 = ((float)color1.A) / 255f;
                    }
                    else
                    {
                        matrix1.Matrix30 = 0.5f;
                        matrix1.Matrix31 = 0.5f;
                        matrix1.Matrix32 = 0.5f;
                        matrix1.Matrix33 = 0.5f;
                    }
                    ImageAttributes attributes1 = new ImageAttributes();
                    attributes1.SetColorMatrix(matrix1);
                    g.DrawImage(image1, new Rectangle((int)(ef1.X + ef2.Width), (int)(ef1.Y + ef2.Height), (int)ef1.Width, (int)ef1.Height), 0, 0, image1.Size.Width, image1.Size.Height, GraphicsUnit.Pixel, attributes1);
                }
                ef1 = this.Bounds;
                g.DrawImage(image1, ef1);
            }
        }

        private void ResetImage()
        {
            this.myImage = null;
        }

        public override void SetSizeKeepingLocation(SizeF s)
        {
            RectangleF ef1 = this.Bounds;
            ef1.Width = s.Width;
            ef1.Height = s.Height;
            PointF tf1 = this.Location;
            RectangleF ef2 = this.SetRectangleSpotLocation(ef1, this.Alignment, tf1);
            this.Bounds = ef2;
        }

        private void UpdateSize()
        {
            if (this.AutoResizes)
            {
                System.Drawing.Image image1 = this.Image;
                if (image1 != null)
                {
                    SizeF ef1 = base.Size;
                    SizeF ef2 = new SizeF((float)image1.Size.Width, (float)image1.Size.Height);
                    if (ef1 != ef2)
                    {
                        this.SetSizeKeepingLocation(ef2);
                    }
                }
            }
        }


        [Description("The image alignment"), DefaultValue(2), Category("Appearance")]
        public virtual int Alignment
        {
            get
            {
                return this.myAlignment;
            }
            set
            {
                int num1 = this.myAlignment;
                if (num1 != value)
                {
                    this.myAlignment = value;
                    this.Changed(0x644, num1, null, DiagramShape.NullRect, value, null, DiagramShape.NullRect);
                }
            }
        }

        [Category("Behavior"), Description("Whether the bounds are recalculated when the image changes."), DefaultValue(true)]
        public virtual bool AutoResizes
        {
            get
            {
                return ((base.InternalFlags & 0x100000) != 0);
            }
            set
            {
                bool flag1 = (base.InternalFlags & 0x100000) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        base.InternalFlags |= 0x100000;
                    }
                    else
                    {
                        base.InternalFlags &= -1048577;
                    }
                    this.Changed(0x645, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("The initial ImageList for newly constructed GoImage objects.")]
        public static System.Windows.Forms.ImageList DefaultImageList
        {
            get
            {
                return DiagramImage.myDefaultImageList;
            }
            set
            {
                DiagramImage.myDefaultImageList = value;
            }
        }

        [Description("The initial ResourceManager for newly constructed GoImage objects.")]
        public static System.Resources.ResourceManager DefaultResourceManager
        {
            get
            {
                return DiagramImage.myDefaultResourceManager;
            }
            set
            {
                DiagramImage.myDefaultResourceManager = value;
            }
        }

        [Category("Appearance"), Description("The Image displayed by this GoImage.")]
        public virtual System.Drawing.Image Image
        {
            get
            {
                this.GetImage();
                return this.myImage;
            }
            set
            {
                this.GetImage();
                System.Drawing.Image image1 = this.myImage;
                if (image1 != value)
                {
                    this.myImage = value;
                    this.Changed(0x641, 0, image1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    this.UpdateSize();
                }
            }
        }

        [Description("The ImageList used to hold a collection of images, selected by Index."), DefaultValue((string)null), Category("Appearance")]
        public virtual System.Windows.Forms.ImageList ImageList
        {
            get
            {
                return this.myImageList;
            }
            set
            {
                System.Windows.Forms.ImageList list1 = this.myImageList;
                if (list1 != value)
                {
                    this.myImageList = value;
                    this.ResetImage();
                    this.Changed(0x646, 0, list1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    this.UpdateSize();
                }
            }
        }

        [Category("Appearance"), Description("The index of the image in an ImageList."), DefaultValue(-1)]
        public virtual int Index
        {
            get
            {
                return this.myIndex;
            }
            set
            {
                int num1 = this.myIndex;
                if (num1 != value)
                {
                    this.myIndex = value;
                    this.ResetImage();
                    this.Changed(0x647, num1, null, DiagramShape.NullRect, value, null, DiagramShape.NullRect);
                    this.UpdateSize();
                }
            }
        }

        public override PointF Location
        {
            get
            {
                return this.GetSpotLocation(this.Alignment);
            }
            set
            {
                this.SetSpotLocation(this.Alignment, value);
            }
        }

        [Description("The Resource name or filename for loading images."), Category("Appearance"), DefaultValue((string)null)]
        public virtual string Name
        {
            get
            {
                return this.myName;
            }
            set
            {
                string text1 = this.myName;
                if (text1 != value)
                {
                    this.myName = value;
                    this.ResetImage();
                    this.Changed(0x643, 0, text1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    this.UpdateSize();
                }
            }
        }

        [Category("Appearance"), DefaultValue((string)null), Description("The ResourceManager used to look up and load images by Name.")]
        public virtual System.Resources.ResourceManager ResourceManager
        {
            get
            {
                return this.myResourceManager;
            }
            set
            {
                System.Resources.ResourceManager manager1 = this.myResourceManager;
                if (manager1 != value)
                {
                    this.myResourceManager = value;
                    this.ResetImage();
                    this.Changed(0x642, 0, manager1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    this.UpdateSize();
                }
            }
        }


        public const int ChangedAlignment = 0x644;
        public const int ChangedAutoResizes = 0x645;
        public const int ChangedImage = 0x641;
        public const int ChangedImageList = 0x646;
        public const int ChangedIndex = 0x647;
        public const int ChangedName = 0x643;
        public const int ChangedResourceManager = 0x642;
        private const int flagAutoResizes = 0x100000;
        private int myAlignment;
        private static System.Windows.Forms.ImageList myDefaultImageList;
        private static System.Resources.ResourceManager myDefaultResourceManager;
        [NonSerialized]
        private System.Drawing.Image myImage;
        [NonSerialized]
        private System.Windows.Forms.ImageList myImageList;
        private int myIndex;
        private string myName;
        [NonSerialized]
        private System.Resources.ResourceManager myResourceManager;
    }
}
