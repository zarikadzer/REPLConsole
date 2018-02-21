namespace REPL.Engine
{
	using REPL.Contracts.Eval;
	using System.Reflection;

	public interface IReplEngine
	{
		/// <summary>
		/// Executes the c# command without session.
		/// </summary>
		/// <param name="sessionId">The <see cref="Guid"/> of the commands sequence.</param>
		/// <param name="command">C# code.</param>
		/// <returns>String result.</returns>
		EvalResult Eval(string command);
		/// <summary>
		/// Removes the whole script and all variables.
		/// </summary>
		/// <param name="parentAssembly">An executing assembly to initialize new state.</param>
		void Reset(Assembly parentAssembly);

		/// <summary>
		/// Replays all the submissions from the very beginning of the script.
		/// </summary>
		/// <returns>All string results with all diagnostics.</returns>
		EvalResult ReplayAll();
	}
}
