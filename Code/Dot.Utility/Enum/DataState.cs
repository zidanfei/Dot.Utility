using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility
{
    public enum DataState
    {
        // Summary:
        //     The row has been created but is not part of any System.Data.DataRowCollection.
        //     A System.Data.DataRow is in this state immediately after it has been created
        //     and before it is added to a collection, or if it has been removed from a
        //     collection.
        Detached = 1,
        //
        // Summary:
        //     The row has not changed since System.Data.DataRow.AcceptChanges() was last
        //     called.
        Unchanged = 2,
        //
        // Summary:
        //     The row has been added to a System.Data.DataRowCollection, and System.Data.DataRow.AcceptChanges()
        //     has not been called.
        Added = 4,
        //
        // Summary:
        //     The row was deleted using the System.Data.DataRow.Delete() method of the
        //     System.Data.DataRow.
        Deleted = 8,
        //
        // Summary:
        //     The row has been modified and System.Data.DataRow.AcceptChanges() has not
        //     been called.
        Modified = 16,
    }
}
