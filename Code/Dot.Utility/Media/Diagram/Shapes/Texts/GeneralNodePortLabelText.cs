using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class GeneralNodePortLabelText : DiagramText
    {
        public GeneralNodePortLabelText()
        {
            this.myPort = null;
            this.Selectable = false;
            this.Editable = true;
            this.FontSize = DiagramText.DefaultFontSize - 2f;
        }


        public GeneralNodePort Port
        {
            get
            {
                return this.myPort;
            }
            set
            {
                this.myPort = value;
            }
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                if (this.Text != value)
                {
                    base.Text = value;
                    if (this.Port != null)
                    {
                        this.Port.Name = this.Text;
                    }
                }
            }
        }


        private GeneralNodePort myPort;
    }
}
