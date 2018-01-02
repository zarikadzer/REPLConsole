namespace REPLConsole
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Text;
	using System.Threading;
	using REPL.SyntaxAnalyzer;
	using REPL.DI.Debugger;
	using System.Linq;
	using Microsoft.CodeAnalysis;
	using REPL.Engine;

	public class Program
	{
		private static ConsoleIO _console = ConsoleIO.Default;
		static void Main(string[] args) {
			DoSomeWork();
			var sessionId = Guid.NewGuid();
			var engine = ReplRepository.GetCSEngine(sessionId, e => {
				e.OnOutput += Engine_OnOutput;
				e.OnError += Engine_OnError;
				e.InitEngineWithAssembly(typeof(Program).Assembly);
			});

			_console.WriteLineInfo("------------------------------------------------------------------------");
			_console.WriteLineInfo($" New code session started with Id: {sessionId}");
			_console.WriteLineInfo("------------------------------------------------------------------------");

			ProcessRepl(engine, sessionId);
		}

		private static void Engine_OnError(object sender, string e) {
			_console.WriteErrorLine(e);
		}

		private static void Engine_OnOutput(object sender, string message) {
			_console.Out.WriteLine(message);
		}

		public static void ProcessRepl(ReplEngineBase engine, Guid sessionId) {
			StringBuilder inputString = null;
			while (inputString == null || inputString.ToString().ToLower().TrimEnd() != "exit") {
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
				if (evalResult == null) {
					continue;
				}

				if (!evalResult.HasError && !evalResult.HasWarnings) {
					if (!string.IsNullOrEmpty(evalResult.ToString())) {
						_console.Out.WriteLine(evalResult);
					}
				}

				foreach (var d in evalResult.Diagnostics) {
					switch (d.Severity) {
						case DiagnosticSeverity.Error:
							_console.WriteErrorLine(d.Message as Object);
							break;
						case DiagnosticSeverity.Warning:
							_console.WriteWarningLine(d.Message as Object);
							break;
						default:
							_console.WriteLineInfo(d.Message as Object);
							break;
					}
				}
			}
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

	public class A : IDebuggable
	{
		public string Value { get; set; }
		public string Value2 { get; set; }

		public virtual DebugInfo GetDebugInfo() {
			return new DebugInfo(Value);
		}

	}

}
