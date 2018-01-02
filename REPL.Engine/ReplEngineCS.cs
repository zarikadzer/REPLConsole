namespace REPL.Engine
{
    using System;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;
    using System.IO;
    using System.Reflection;
    using System.Linq;

    public class ReplEngineCS : ReplEngineBase
    {
        public ReplEngineCS(Guid sessionId) : base(sessionId) {
        }

        public static implicit operator WeakReference<ReplEngineBase>(ReplEngineCS instance) {
            return new WeakReference<ReplEngineBase>(instance);
        }

        public override void InitEngineWithAssembly(Assembly parentAssembly) {
            string appDir = Path.GetDirectoryName(parentAssembly.GetFiles().FirstOrDefault().Name);

            //All dependend assemblies
            foreach (var dll in Directory.EnumerateFiles(appDir, "*.dll")) {
                var loadDllCmd = $"#r \"{dll}\"";
                var initRefsResult = Eval(loadDllCmd);
                HandleOutputEvent(loadDllCmd + initRefsResult);
            }

            //Add reference to the executable assembly
            var loadExeCmd = $"#r \"{parentAssembly.GetFiles().FirstOrDefault().Name}\"";
            var initExeResult = Eval(loadExeCmd);
            HandleOutputEvent(loadExeCmd + initExeResult);

            //Initial references and usings
            var lines = GetInitFileContents().Split('\n').ToList();
            if (lines != null && lines.Count > 1) {
                lines.RemoveAt(0);
            }
            foreach (var line in lines) {
                if (line != null && line.Trim() == "") {
                    continue;
                }
                var initResult = Eval(line);
                HandleOutputEvent(line + initResult);
            }
        }

        public override Tuple<Script, ScriptState> GetScriptSession(string command, ScriptOptions options = null) {
            if (!ScriptSessions.TryGetValue(SessionId, out var existedSession)) {
                var newSession = new Tuple<Script, ScriptState>(CSharpScript.Create(command, options), null);
                return newSession;
            }
            var existedScript = existedSession.Item1;
            var next = existedScript.ContinueWith(command, options);
            return new Tuple<Script, ScriptState>(next, existedSession.Item2);
        }

        private string GetInitFileContents() {
            var resourceName = "REPL.Engine.Properties.Resources.resources";
            using (var stream = GetType().Assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream)) {
                return reader.ReadToEnd();
            }
        }

    }
}
