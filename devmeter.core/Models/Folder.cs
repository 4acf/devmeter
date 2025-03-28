﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devmeter.ui.Core.Models
{
    public class Folder : FilesystemObject
    {
        public int ChildrenLinesOfCode { get; set; }
        public int ChildrenLinesOfWhitespace { get; set; }
        public List<FilesystemObject> FilesystemObjects { get; set; } = [];
    }
}
