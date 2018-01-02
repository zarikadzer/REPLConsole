namespace REPL.DI.Debugger
{
    public class ClassDebugger : IDebugger
    {
        public ClassDebugger(IDebuggable instance) {
            Instance = instance;
        }
        public IDebuggable Instance { get; set; }

        public DebugInfo GetDebugInfo() {
            return Instance.GetDebugInfo();
        }
    }
}
