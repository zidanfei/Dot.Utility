using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Utility.Media.Diagram
{
    public interface ILayerAbilities
    {
        bool CanCopyObjects();

        bool CanDeleteObjects();

        bool CanEditObjects();

        bool CanInsertObjects();

        bool CanLinkObjects();

        bool CanMoveObjects();

        bool CanReshapeObjects();

        bool CanResizeObjects();

        bool CanSelectObjects();

        void SetModifiable(bool b);


        bool AllowCopy { get; set; }

        bool AllowDelete { get; set; }

        bool AllowEdit { get; set; }

        bool AllowInsert { get; set; }

        bool AllowLink { get; set; }

        bool AllowMove { get; set; }

        bool AllowReshape { get; set; }

        bool AllowResize { get; set; }

        bool AllowSelect { get; set; }

    }
}
