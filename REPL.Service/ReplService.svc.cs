namespace REPL.Service
{
    using System;

    public class ReplService : IReplService
    {
        public string Eval(Guid sessionId, string code) {
            //
            // TODO: Add the implementation of a single REPL iteration.
            //
            return string.Format("You entered: {0} {1}", sessionId, code);
        }

    }
}
