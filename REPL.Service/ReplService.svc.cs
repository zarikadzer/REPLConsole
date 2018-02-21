namespace REPL.Service
{
	using System.Linq;
	using System.ServiceModel.Activation;
	using Microsoft.CodeAnalysis;
	using REPL.Contracts;
	using REPL.Contracts.Eval;
	using REPL.Engine;
	using REPL.SyntaxAnalyzer;

	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class ReplService : IReplService
	{
		public EvalResult EvalCS(EvalRequest request) {
			var analyzer = new ReplAnalyzerCS(request.Code);
			if (!analyzer.IsCompleteSubmission()) {
				return EvalResult.Error(request.SessionId, "Submission is not completed!");
			}
			var engine = ReplRepository.GetCSEngine(request.SessionId, e => {
				e.InitEngineWithAssembly(typeof(ReplService).Assembly);
			});
			return engine.Eval(request.Code);
		}

		public EvalResult ValidateCS(EvalRequest request) {
			var analyzer = new ReplAnalyzerCS(request.Code);
			if (!analyzer.IsCompleteSubmission()) {
				return EvalResult.Error(request.SessionId, "Submission is not completed!");
			}
			var engine = ReplRepository.GetCSEngine(request.SessionId, e => {
				e.InitEngineWithAssembly(typeof(ReplService).Assembly);
			});
			var diagnostics = engine.Validate(request.Code, out var script, out var scriptState);
			var hasErrors = diagnostics.Any(x => x.Severity == DiagnosticSeverity.Error || (x.Severity == DiagnosticSeverity.Warning && x.IsWarningAsError));
			var diagResult = diagnostics.Select(x => new DiagnosticsResult(x.ToString(), x.Severity)).ToList();
			return EvalResult.Instance(request.SessionId, string.Empty, diagResult, hasErrors);
		}

		public EvalResult ResetCS(EvalRequest request) {
			var response = EvalResult.Empty(request.SessionId);
			response.StringResult = $"Total Memory: {System.GC.GetTotalMemory(false)} bytes -> ";
			var engine = ReplRepository.GetCSEngine(request.SessionId, e => {
				e.Reset(typeof(ReplService).Assembly);
			});
			response.StringResult += $"{System.GC.GetTotalMemory(true)} bytes";
			return response;
		}
	}
}
