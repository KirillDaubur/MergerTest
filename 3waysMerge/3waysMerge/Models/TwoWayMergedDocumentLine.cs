using _3waysMerge.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace _3waysMerge.Models
{
    public class TwoWayMergedDocumentLine
    {
        public int ParentDocumentLineNumer { get; set; }
        public string Text { get; set; }
        public TwoWayMergeDocumentLineState State { get; set; }
    }
}
