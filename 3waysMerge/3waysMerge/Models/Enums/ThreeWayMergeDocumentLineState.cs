using System;
using System.Collections.Generic;
using System.Text;

namespace _3waysMerge.Models.Enums
{
    public enum ThreeWayMergeDocumentLineState
    {
        None,
        Equal,
        Modified,
        New,
        Removed,
        Conflict
    }
}
