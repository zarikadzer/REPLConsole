using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.DI.Debugger
{
    public interface IDebuggable
    {
        DebugInfo GetDebugInfo();
    }

    public class DebugInfo
    {
        public DebugInfo(string message, int code = 0)
        {
            Message = message;
            Code = code;
        }
        public string Message { get; set; }
        public int Code { get; set; }

        public override string ToString()
        {
            return $"{DateTime.UtcNow} {Code.ToString().PadLeft(8)} {Message}";
        }
    }
}
