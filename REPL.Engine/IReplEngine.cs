namespace REPL.Engine
{
    public class EvalResult
    {
        public string StringResult { get; set; }
        public bool HasError { get; set; }

        public override string ToString()
        {
            return StringResult;
        }
    }

    public interface IReplEngine
    {
        /// <summary>
        /// Executes the c# command without session.
        /// </summary>
        /// <param name="sessionId">The <see cref="Guid"/> of the commands sequence.</param>
        /// <param name="command">C# code.</param>
        /// <returns>String result.</returns>
        EvalResult Eval(string command);
    }
}
