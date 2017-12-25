using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.DI.Debugger
{
    public class ClassDebugger : IDebugger
    {
        public ClassDebugger(IDebuggable instance)
        {
            Instance = instance;
        }
        public IDebuggable Instance { get; set; }

        public DebugInfo GetDebugInfo()
        {
            return Instance.GetDebugInfo();
        }
    }
}
