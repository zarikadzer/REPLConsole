using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.Engine
{
    public class ReplRepository
    {
        private static readonly object _lockObject = new object();
        private static Dictionary<Guid, WeakReference<ReplEngineBase>> _replEngines =
                new Dictionary<Guid, WeakReference<ReplEngineBase>>();

        public static ReplEngineBase GetCSEngine(Guid sessionId)
        {
            lock (_lockObject)
            {
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
