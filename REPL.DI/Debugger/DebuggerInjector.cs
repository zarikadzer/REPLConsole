namespace REPL.DI.Debugger
{
    using System;
    public class DebuggerInjector<T> : InjectorBase<T, IDebugger> where T : IDebuggable
    {
    }
}
