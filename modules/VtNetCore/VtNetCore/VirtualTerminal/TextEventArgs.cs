using System;
using System.Collections.Generic;
using System.Text;

namespace VtNetCore.VirtualTerminal
{
    public class TextEventArgs : EventArgs
    {
        public string Text { get; set; }
    }
}
