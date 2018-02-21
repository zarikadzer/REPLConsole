namespace REPL.Service
{
	using REPL.Contracts;
	using REPL.Engine;
	using REPL.SyntaxAnalyzer;
	using System.Collections.Generic;
	using REPL.Contracts.Eval;

	public class ReplService : IReplService
	{
		public EvalResult EvalCS(EvalRequest request) {
			var analyzer = new ReplAnalyzerCS(request.Code);
			if (!analyzer.IsCompleteSubmission()) {
				return new EvalResult(request.SessionId, "Submission is not completed!", new List<DiagnosticsResult>(), true);
			}
			var engine = ReplRepository.GetCSEngine(request.SessionId, e => {
				e.InitEngineWithAssembly(typeof(ReplService).Assembly);
			});
			return engine.Eval(request.Code);
		}
	}
}
