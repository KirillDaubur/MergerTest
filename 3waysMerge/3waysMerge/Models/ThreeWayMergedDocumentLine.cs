using _3waysMerge.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace _3waysMerge.Models
{
    public class ThreeWayMergedDocumentLine
    {
        public PatentType Parent { get; set; }
        public string Text { get; set; }
        public ThreeWayMergeDocumentLineState State { get; set; }
    }
}
