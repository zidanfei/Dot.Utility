using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class DiagramControl : DiagramShape
    {
        public DiagramControl()
        {
            this.myControlType = null;
            this.myEditedObject = null;
            this.myMap = null;
        }

        public override void Changed(int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
        {
            if (!base.SuspendsUpdates)
            {
                base.Changed(subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
                if (subhint == 0x3eb)
                {
                    IDictionaryEnumerator enumerator1 = this.Map.GetEnumerator();
                    while (enumerator1.MoveNext())
                    {
                        DictionaryEntry entry1 = enumerator1.Entry;
                        DiagramView view1 = (DiagramView)entry1.Key;
                        Control control1 = (Control)entry1.Value;
                        if (view1 != null)
                        {
                            bool flag1 = this.CanView();
                            if (!flag1 && (control1 != null))
                            {
                                control1.Visible = false;
                            }
                        }
                    }
                }
            }
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            if (e.SubHint == 0x76d)
            {
                this.ControlType = (Type)e.GetValue(undo);
            }
            else
            {
                base.ChangeValue(e, undo);
            }
        }

        public override DiagramShape CopyObject(CopyDictionary env)
        {
            DiagramControl control1 = (DiagramControl)base.CopyObject(env);
            control1.myEditedObject = (DiagramShape)env[this.myEditedObject];
            control1.myMap = null;
            return control1;
        }

        [PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\r\n               version=\"1\">\r\n   <IPermission class=\"System.Security.Permissions.UIPermission, mscorlib, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                version=\"1\"\r\n                Window=\"AllWindows\"/>\r\n</PermissionSet>\r\n")]
        public virtual Control CreateControl(DiagramView view)
        {
            Type type1 = this.ControlType;
            if (type1 == null)
            {
                return null;
            }
            Control control1 = (Control)Activator.CreateInstance(type1);
            RectangleF ef1 = this.Bounds;
            Rectangle rectangle1 = view.ConvertDocToView(ef1);
            control1.Bounds = rectangle1;
            IDiagramControl obj1 = control1 as IDiagramControl;
            if (obj1 != null)
            {
                obj1.DiagramView = view;
                obj1.DiagramControl = this;
            }
            return control1;
        }

        public virtual void DisposeControl(Control comp, DiagramView view)
        {
            if ((comp != null) && (view != null))
            {
                if (view.EditControl != this)
                {
                    view.RemoveDiagramControl(this, comp);
                    comp.Dispose();
                }
                else
                {
                    comp.Visible = false;
                }
            }
        }

        public override void DoEndEdit(DiagramView view)
        {
            DiagramShape obj1 = this.EditedObject;
            if (obj1 != null)
            {
                obj1.DoEndEdit(view);
            }
        }

        public virtual Control FindControl(DiagramView view)
        {
            return (Control)this.Map[view];
        }

        public virtual Control GetControl(DiagramView view)
        {
            Control control1 = this.FindControl(view);
            if (control1 == null)
            {
                control1 = this.CreateControl(view);
                if (control1 != null)
                {
                    this.Map[view] = control1;
                    view.AddGoControl(this, control1);
                }
            }
            return control1;
        }

        protected override void OnLayerChanged(DiagramLayer oldLayer, DiagramLayer newLayer, DiagramShape mainObj)
        {
            base.OnLayerChanged(oldLayer, newLayer, mainObj);
            if (((oldLayer != null) && (newLayer == null)) && oldLayer.IsInDocument)
            {
                DiagramDocument document1 = oldLayer.Document;
                IDictionaryEnumerator enumerator1 = this.Map.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    DictionaryEntry entry1 = enumerator1.Entry;
                    DiagramView view1 = (DiagramView)entry1.Key;
                    Control control1 = (Control)entry1.Value;
                    if ((view1 != null) && (control1 != null))
                    {
                        this.DisposeControl(control1, view1);
                    }
                }
                this.Map.Clear();
            }
            else if (((oldLayer != null) && (newLayer == null)) && oldLayer.IsInView)
            {
                DiagramView view2 = oldLayer.View;
                Control control2 = this.FindControl(view2);
                if (control2 != null)
                {
                    this.Map.Remove(view2);
                    if (control2 != null)
                    {
                        this.DisposeControl(control2, view2);
                    }
                }
            }
        }

        public override void Paint(Graphics g, DiagramView view)
        {
            Control control1 = this.GetControl(view);
            if (control1 != null)
            {
                RectangleF ef1 = this.Bounds;
                Rectangle rectangle1 = view.ConvertDocToView(ef1);
                control1.Bounds = rectangle1;
                control1.Visible = true;
            }
        }

        [Description("The Type used to specify which Control to create when first displayed in a DiagramView.")]
        public virtual Type ControlType
        {
            get
            {
                return this.myControlType;
            }
            set
            {
                Type type1 = this.myControlType;
                if (type1 != value)
                {
                    this.myControlType = value;
                    this.Changed(0x76d, 0, type1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("The DiagramShape for which this control is acting as an editor.")]
        public virtual DiagramShape EditedObject
        {
            get
            {
                return this.myEditedObject;
            }
            set
            {
                this.myEditedObject = value;
            }
        }

        [Description("The Hashtable that maps GoViews to Controls for this DiagramControl.")]
        public Hashtable Map
        {
            get
            {
                if (this.myMap == null)
                {
                    this.myMap = new Hashtable();
                }
                return this.myMap;
            }
        }


        public const int ChangedControlType = 0x76d;
        private Type myControlType;
        private DiagramShape myEditedObject;
        [NonSerialized]
        private Hashtable myMap;
    }
}
