namespace REPL.Service
{
    using System;
    using REPL.Contracts;
    using REPL.Engine;
    using REPL.SyntaxAnalyzer;
    using System.Collections.Generic;

    public class ReplService : IReplService
    {
        public EvalResult EvalCS(Guid sessionId, string code) {
            var analyzer = new ReplAnalyzerCS(code);
            if (!analyzer.IsCompleteSubmission()) {
                return new EvalResult(sessionId, "Submission is not completed!", new List<DiagnosticsResult>(), true);
            }
            var engine = ReplRepository.GetCSEngine(sessionId, e => {
                e.InitEngineWithAssembly(typeof(ReplService).Assembly);
            });
            return engine.Eval(code);
        }
    }
}
