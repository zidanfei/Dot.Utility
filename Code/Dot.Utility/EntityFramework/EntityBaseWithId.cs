using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.EntityFramework
{

    [Serializable]
    public class EntityBaseWithId<T>
    {

        public virtual T Id { get; set; }
    }

    [Serializable]
    public class EntityBaseWithId : EntityBaseWithId<int>
    {

    }
}
