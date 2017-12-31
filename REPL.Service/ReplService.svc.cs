namespace REPL.Service
{
	using System;
	using REPL.Contracts;
	using REPL.Engine;
	using REPL.SyntaxAnalyzer;

	public class ReplService : IReplService
    {
        public EvalResult EvalCS(Guid sessionId, string code) {
			var analyzer = new ReplAnalyzerCS(code);
			if (!analyzer.IsCompleteSubmission()) {
				return new EvalResult("Submission is not completed!", true);
			}
			var engine = ReplRepository.GetCSEngine(sessionId);
			engine.InitEngineWithAssembly(typeof(ReplService).Assembly);
			return engine.Eval(code);
        }
    }
}
