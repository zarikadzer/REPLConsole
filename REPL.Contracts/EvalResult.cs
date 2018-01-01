namespace REPL.Contracts
{
    using Microsoft.CodeAnalysis;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(Name = "eval_result")]
    [Serializable]
    public class EvalResult
    {

        #region Constructors: Public

        public EvalResult() {
        }

        public EvalResult(string message, bool hasError = false) {
            StringResult = message;
            HasError = hasError;
            Diagnostics = new List<DiagnosticsResult>();
        }

        public EvalResult(string message, List<DiagnosticsResult> diagnostics, bool hasError = false) : this(message, hasError) {
            Diagnostics = diagnostics;
        }

        #endregion

        [DataMember(Name = "string_result")]
        public string StringResult { get; set; }


        [DataMember(Name = "diagnostics")]
        public List<DiagnosticsResult> Diagnostics { get; set; }


        [DataMember(Name = "has_error")]
        public bool HasError { get; set; }

        public bool HasWarnings
        {
            get {
                return Diagnostics?.Any(x => x.Severity == DiagnosticSeverity.Warning) ?? false;
            }
        }
        public override string ToString() {
            return StringResult;
        }
    }
}
