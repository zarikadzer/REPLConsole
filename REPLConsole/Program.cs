namespace REPLConsole
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading;
    using REPL.DI.Debugger;
    using REPL.Engine;
    using REPL.SyntaxAnalyzer;

    public class Program
    {
        private static ConsoleIO _console = ConsoleIO.Default;
        static void Main(string[] args)
        {
            DoSomeWork();
            var sessionId = Guid.NewGuid();
            var engine = new ReplEngineCS(sessionId);
            ProcessRepl(engine, sessionId);
        }

        public static void ProcessRepl(ReplEngineCS engine, Guid sessionId)
        {
            InitRepl(engine, sessionId);

            StringBuilder inputString = null;
            while (inputString==null || inputString.ToString() .ToLower().TrimEnd() != "exit") {
                _console.WriteInfo("> ");
                bool isSubmissionCompleted = false;
                inputString = new StringBuilder("");
                do {
                    //Read
                    inputString.AppendLine(_console.In.ReadLine());
                    var analyzer = new ReplAnalyzerCS(inputString.ToString());
                    isSubmissionCompleted = analyzer.IsCompleteSubmission();
                    if (!isSubmissionCompleted) {
                        _console.WriteInfo(".\t");
                    }
                } while (!isSubmissionCompleted);

                //Eval
                var evalResult = engine.Eval(inputString.ToString());

                //Print
                if (evalResult?.HasError ?? false) {
                    _console.WriteErrorLine(evalResult);
                } else {
                    if (!string.IsNullOrEmpty(evalResult?.ToString())) {
                        _console.Out.WriteLine(evalResult);
                    }
                }
            }
        }

        public static void InitRepl(ReplEngineCS engine, Guid sessionId) {
            string appDir = Path.GetDirectoryName(typeof(Program).Assembly.ManifestModule.FullyQualifiedName);

            //All dependend assemblies
            foreach (var dll in Directory.EnumerateFiles(appDir, "*.dll")) {
                var loadDllCmd = $"#r \"{dll}\"";
                var initRefsResult = engine.Eval(loadDllCmd);
                _console.Out.WriteLine(loadDllCmd + initRefsResult);
            }

            //Add reference to the executable assembly
            var loadExeCmd = $"#r \"{appDir}\\{AppDomain.CurrentDomain.FriendlyName}\"";
            var initExeResult = engine.Eval(loadExeCmd);
            _console.Out.WriteLine(loadExeCmd + initExeResult);

            //Initial references and usings
            var lines = File.ReadAllLines($"{appDir}\\InitInteractiveBase.csx");
            foreach(var line in lines)
            {
                var initResult = engine.Eval(line);
                _console.Out.WriteLine(line + initExeResult);
            }
            
            _console.WriteLineInfo("------------------------------------------------------------------------");
            _console.WriteLineInfo($" New code session started with Id: {sessionId}");
            _console.WriteLineInfo("------------------------------------------------------------------------");
        }

        #region Some work...

        public static DebuggerInjector<A> aInjector = new DebuggerInjector<A>();
        public static A aInstance;

        private static void DoSomeWork() {
            aInstance = new A {
                Value = "Message 15" + Guid.NewGuid(),
                Value2 = "Some additional message 16"
            };

            aInjector.Bind(typeof(A), new ClassDebugger(aInstance));

            var action = new Action(() => {
                while (true) {
                    var debugInfo = aInjector.Get(aInstance.GetType()).GetDebugInfo();
                    Debug.WriteLine(debugInfo);
                    Thread.Sleep(1000);
                }
            });

            action.BeginInvoke(null, null);
        } 

        #endregion
    }

    public class A: IDebuggable
    {
        public string Value { get; set; }
        public string Value2 { get; set; }

        public virtual DebugInfo GetDebugInfo()
        {
            return new DebugInfo(Value);
        }
        
    }
 
}
