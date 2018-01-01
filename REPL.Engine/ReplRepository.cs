namespace REPL.Engine
{
    using System;
    using System.Collections.Generic;

    public class ReplRepository
    {
        private static readonly object _lockObject = new object();
        private static Dictionary<Guid, WeakReference<ReplEngineBase>> _replEngines =
                new Dictionary<Guid, WeakReference<ReplEngineBase>>();

        public static ReplEngineBase GetCSEngine(Guid sessionId) {
            lock (_lockObject) {
                if (_replEngines.TryGetValue(sessionId, out var weakObject)
                    && weakObject.TryGetTarget(out var replEngine)) {
                    return replEngine;
                }
                var engine = new ReplEngineCS(sessionId);
                _replEngines[sessionId] = engine;
                return engine;
            }
        }


    }
}
