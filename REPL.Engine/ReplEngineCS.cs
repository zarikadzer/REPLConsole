namespace REPL.Engine
{
    using System;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;

    public class ReplEngineCS : ReplEngineBase
    {
        public ReplEngineCS(Guid sessionId) : base(sessionId)
        {
        }

        public override Script GetScriptSession(string command, ScriptOptions options = null)
        {
            if (!ScriptSessions.TryGetValue(SessionId, out var existedScript))
            {
                var newScript = CSharpScript.Create(command, options);
                return newScript;
            }
            var next = existedScript.ContinueWith(command, options);
            return next;
        }
    }
}
