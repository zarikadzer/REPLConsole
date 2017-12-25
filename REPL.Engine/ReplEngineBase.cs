namespace REPL.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis.Scripting;

    public abstract class ReplEngineBase : IReplEngine
    {
        protected static Dictionary<Guid, Script> ScriptSessions = new Dictionary<Guid, Script>();

        public event EventHandler<string> OnOutput;

        public event EventHandler<string> OnError;

        public ReplEngineBase(Guid sessionId)
        {
            SessionId = sessionId;
        }

        public Guid SessionId { get; set; }

        public abstract Script GetScriptSession(string command, ScriptOptions options = null);
        public abstract void InitEngine();

        protected void HandleOutputEvent(string message) => HandleEvent(OnOutput, message);
        protected void HandleErrorEvent(string message) => HandleEvent(OnError, message);

        private void HandleEvent(EventHandler<string> eventHandler, string response) {
            var h = eventHandler;
            if (h == null) {
                h.Invoke(this, response);
            }
        }

        public EvalResult Eval(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return null;
            }
            var script = GetScriptSession(command);
            var diagnostics = script.Compile();
            if (!diagnostics.IsEmpty)
            {
                return new EvalResult {
                    StringResult = diagnostics.Select(x => x.ToString()).Aggregate((x, y) => $"{x}\r\n{y}"),
                    HasError = true
                };
            }

            try
            {
                var result = script.RunAsync().GetAwaiter().GetResult();
                if (result.Exception == null)
                {
                    ScriptSessions[SessionId] = script;
                }
                return new EvalResult
                {
                    StringResult = result.ReturnValue?.ToString(),
                    HasError = false
                };
            }
            catch (Exception ex)
            {
                return new EvalResult
                {
                    StringResult = ex.Message,
                    HasError = true
                };
            }
        }
    }
}
