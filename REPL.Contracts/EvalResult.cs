using System;
using System.Runtime.Serialization;

namespace REPL.Contracts
{
	[DataContract(Name = "eval_result")]
	public class EvalResult
	{
		public EvalResult() {
		}

		public EvalResult(string message, bool hasError = false) {
			StringResult = message;
			HasError = hasError;
		}

		[DataMember(Name = "string_result")]
		public string StringResult { get; set; }

		[DataMember(Name = "has_error")]
		public bool HasError { get; set; }

		public override string ToString() {
			return StringResult;
		}
	}
}
