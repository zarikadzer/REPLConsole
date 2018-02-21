namespace REPL.Service
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.ServiceModel;
	using System.ServiceModel.Activation;
	using System.ServiceModel.Web;
	using Microsoft.CodeAnalysis;
	using REPL.Contracts;
	using REPL.Contracts.Eval;
	using REPL.Engine;
	using REPL.SyntaxAnalyzer;

	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class ReplService : IReplService
	{
		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "cs/eval", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[Description("Evaluates the C# code and returns the EvalResult object.")]
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

		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "cs/validate", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[Description("Validates the C# code and returns the EvalResult object.")]
		public EvalResult ValidateCS(EvalRequest request) {
			var analyzer = new ReplAnalyzerCS(request.Code);
			if (!analyzer.IsCompleteSubmission()) {
				return new EvalResult(request.SessionId, "Submission is not completed!", new List<DiagnosticsResult>(), true);
			}
			var engine = ReplRepository.GetCSEngine(request.SessionId, e => {
				e.InitEngineWithAssembly(typeof(ReplService).Assembly);
			});
			var diagnostics = engine.Validate(request.Code, out var script, out var scriptState);
			var hasErrors = diagnostics.Any(x => x.Severity == DiagnosticSeverity.Error || (x.Severity == DiagnosticSeverity.Warning && x.IsWarningAsError));
			var diagResult = diagnostics.Select(x => new DiagnosticsResult(x.ToString(), x.Severity)).ToList();
			return new EvalResult(request.SessionId, string.Empty, diagResult, hasErrors);
		}


	}
}
