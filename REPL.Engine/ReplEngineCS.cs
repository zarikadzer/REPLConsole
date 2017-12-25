namespace REPL.Engine
{
    using System;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;
    using System.IO;

    public class ReplEngineCS : ReplEngineBase
    {
        public ReplEngineCS(Guid sessionId) : base(sessionId)
        {
        }

        public static implicit operator WeakReference<ReplEngineBase>(ReplEngineCS instance) {
            return new WeakReference<ReplEngineBase>(instance);
        }

        public override void InitEngine() {
            string appDir = Path.GetDirectoryName(typeof(ReplEngineCS).Assembly.ManifestModule.FullyQualifiedName);

            //All dependend assemblies
            foreach (var dll in Directory.EnumerateFiles(appDir, "*.dll")) {
                var loadDllCmd = $"#r \"{dll}\"";
                var initRefsResult = Eval(loadDllCmd);
                HandleOutputEvent(loadDllCmd + initRefsResult);
            }

            //Add reference to the executable assembly
            var loadExeCmd = $"#r \"{appDir}\\{AppDomain.CurrentDomain.FriendlyName}\"";
            var initExeResult = Eval(loadExeCmd);
            HandleOutputEvent(loadExeCmd + initExeResult);

            //Initial references and usings
            var lines = File.ReadAllLines($"{appDir}\\InitInteractiveBase.csx");
            foreach (var line in lines) {
                var initResult = Eval(line);
                HandleOutputEvent(line + initExeResult);
            }
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
