using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.DI.Debugger
{
    public class DebuggerInjector<T> : InjectorBase<T, IDebugger> where T: IDebuggable
    {
    }
}
