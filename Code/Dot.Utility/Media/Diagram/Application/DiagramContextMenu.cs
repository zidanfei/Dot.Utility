using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Dot.Utility.Media.Diagram
{
    [ToolboxItem(false), DesignTimeVisible(false)]
    public class DiagramContextMenu : ContextMenu
    {
        public DiagramContextMenu(DiagramView view)
        {
            this._View = null;
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }
            this._View = view;
        }

        public static DiagramView GetView(MenuItem m)
        {
            if (m != null)
            {
                Menu menu1 = m.Parent;
                if (menu1 is MenuItem)
                {
                    return DiagramContextMenu.GetView((MenuItem)menu1);
                }
                if (menu1 is DiagramContextMenu)
                {
                    return ((DiagramContextMenu)menu1).View;
                }
            }
            return null;
        }

        private DiagramView _View;
        public DiagramView View
        {
            get
            {
                return this._View;
            }
        }
    }
}
