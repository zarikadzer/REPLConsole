namespace REPL.Engine
{
	using REPL.Contracts;

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
