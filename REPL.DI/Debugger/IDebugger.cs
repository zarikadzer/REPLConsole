using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.DI.Debugger
{
    public interface IDebugger
    {
        DebugInfo GetDebugInfo();
    }
}
