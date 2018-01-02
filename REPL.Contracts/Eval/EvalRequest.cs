namespace REPL.Contracts.Eval
{
	using System;
	using System.Runtime.Serialization;

	[DataContract(Name = "eval_request")]
	public class EvalRequest
	{
		[DataMember(Name = "session_id", IsRequired = true)]
		public Guid SessionId { get; set; }

		[DataMember(Name = "code")]
		public string Code { get; set; }
	}
}