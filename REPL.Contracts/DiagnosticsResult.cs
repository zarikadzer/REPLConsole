namespace REPL.Contracts
{
    using System;
    using System.Runtime.Serialization;
    using Microsoft.CodeAnalysis;

    [DataContract(Name = "diagnostics_result")]
    [Serializable]
    public class DiagnosticsResult
    {

        #region Constructors: Public

        public DiagnosticsResult() {
        }

        public DiagnosticsResult(string message, DiagnosticSeverity severity) {
            Message = message;
            Severity = severity;
        }

        #endregion

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "severity")]
        public DiagnosticSeverity Severity { get; set; }
    }
}