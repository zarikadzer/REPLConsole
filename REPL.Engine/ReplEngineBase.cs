namespace REPL.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis.Scripting;
    using REPL.Contracts;
    using Microsoft.CodeAnalysis;
    using REPL.Contracts.Eval;

    public abstract class ReplEngineBase : IReplEngine
    {
        private static readonly object _lockObject = new object();

        protected static Dictionary<Guid, Tuple<Script, ScriptState>> ScriptSessions =
            new Dictionary<Guid, Tuple<Script, ScriptState>>();

        public event EventHandler<string> OnOutput;

        public event EventHandler<string> OnError;

        public ReplEngineBase(Guid sessionId) {
            SessionId = sessionId;
        }

        public Guid SessionId { get; set; }

        private void HandleEvent(EventHandler<string> eventHandler, string response) {
            var h = eventHandler;
            if (h != null) {
                h.Invoke(this, response);
            }
        }

        protected void HandleOutputEvent(string message) => HandleEvent(OnOutput, message);
        protected void HandleErrorEvent(string message) => HandleEvent(OnError, message);
        public abstract Tuple<Script, ScriptState> GetScriptSession(string command, ScriptOptions options = null);
        public abstract void InitEngineWithAssembly(Assembly parentAssembly);

        public EvalResult Eval(string command) {
            if (string.IsNullOrWhiteSpace(command)) {
                return null;
            }
            var session = GetScriptSession(command);
            var script = session.Item1;
            var scriptState = session.Item2;
            var diagnostics = script.Compile();
            var hasErrors = diagnostics.Any(x => x.Severity == DiagnosticSeverity.Error
                || (x.Severity == DiagnosticSeverity.Warning && x.IsWarningAsError));
            var hasWarnings = diagnostics.Any(x => x.Severity == DiagnosticSeverity.Warning);
            var diagResult = diagnostics.Select(x => new DiagnosticsResult(x.ToString(), x.Severity)).ToList();

            if (hasErrors) {
                return new EvalResult(SessionId, string.Empty, diagResult, true);
            }

            try {
                var result = scriptState != null
                    ? script.RunFromAsync(scriptState).GetAwaiter().GetResult()
                    : script.RunAsync().GetAwaiter().GetResult();

                if (result.Exception == null) {
                    ScriptSessions[SessionId] = new Tuple<Script, ScriptState>(script, result);
                }
                return new EvalResult(SessionId, result.ReturnValue?.ToString(), diagResult, false);
            } catch (Exception ex) {
                return new EvalResult(SessionId, ex.Message, new List<DiagnosticsResult>(), true);
            }
        }

        public void Reset(Assembly parentAssembly) {
            lock (_lockObject) {
                if (ScriptSessions.ContainsKey(SessionId)) {
                    var scriptState = ScriptSessions[SessionId].Item2;
                    foreach (var v in scriptState.Variables) {
                        v.Value = null;
                    }
                    ScriptSessions.Remove(SessionId);
                    GC.Collect();
                    GC.Collect();
                    GC.Collect(2, GCCollectionMode.Forced);
                    InitEngineWithAssembly(parentAssembly);
                }
            }
        }

        public EvalResult ReplayAll() {
            if (!ScriptSessions.ContainsKey(SessionId)) {
                return null;
            }
            var script = ScriptSessions[SessionId].Item1;
            var diagnostics = script.Compile();
            var diagResult = diagnostics.Select(x => new DiagnosticsResult(x.ToString(), x.Severity)).ToList();
            try {
                var result = script.RunAsync().GetAwaiter().GetResult();
                if (result.Exception == null) {
                    ScriptSessions[SessionId] = new Tuple<Script, ScriptState>(script, result);
                }
                return new EvalResult(SessionId, result.ReturnValue?.ToString(), diagResult, false);
            } catch (Exception ex) {
                return new EvalResult(SessionId, ex.Message, new List<DiagnosticsResult>(), true);
            }
        }
    }
}
