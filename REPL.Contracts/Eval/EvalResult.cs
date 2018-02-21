namespace REPL.Contracts.Eval
{
    using Microsoft.CodeAnalysis;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

	[DataContract(Name = "eval_result")]
	public class EvalResult {

		#region Constructors: Public        

		public EvalResult(Guid sessionId, string message, List<DiagnosticsResult> diagnostics, bool hasError = false) {
			SessionId = sessionId;
			StringResult = message;
			Diagnostics = diagnostics;
			HasError = hasError;
		}

		#endregion

		[DataMember(Name = "session_id", IsRequired = true, Order = 0)]
		public Guid SessionId { get; set; }


		[DataMember(Name = "string_result")]
		public string StringResult { get; set; }


		[DataMember(Name = "diagnostics")]
		public List<DiagnosticsResult> Diagnostics { get; set; }


		[DataMember(Name = "has_error")]
		public bool HasError { get; set; }

		public bool HasWarnings {
			get {
				return Diagnostics?.Any(x => x.Severity == DiagnosticSeverity.Warning) ?? false;
			}
		}
		public override string ToString() {
			return StringResult;
		}

		public static EvalResult Error(Guid sessionId, string message) {
			return new EvalResult(sessionId, message, new List<DiagnosticsResult>(), true);
		}

		public static EvalResult Instance(Guid sessionId, string message, List<DiagnosticsResult> diagnostics,
				bool hasError = false) {
			return new EvalResult(sessionId, message, diagnostics, hasError);
		}

		public static EvalResult Instance(Guid sessionId, string message) {
			return new EvalResult(sessionId, message, new List<DiagnosticsResult>(), false);
		}

		public static EvalResult Empty(Guid sessionId) {
			return new EvalResult(sessionId, string.Empty, new List<DiagnosticsResult>(), false);
		}
    }
}
