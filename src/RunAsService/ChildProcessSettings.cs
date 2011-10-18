using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunAsService
{
    public class ChildProcessSettings
    {
        public string FileName { get; set; }
        public string Arguments { get; set; }
        public string WorkingDirectory { get; set; }
    }
}
