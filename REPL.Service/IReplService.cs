namespace REPL.Service
{
	using System.ComponentModel;
	using System.ServiceModel;
	using System.ServiceModel.Web;
	using REPL.Contracts.Eval;
	
	[ServiceContract]
	public interface IReplService
	{
		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "repl/eval/cs", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[Description("Evaluates the C# code and returns the EvalResult object.")]
		EvalResult EvalCS(EvalRequest request);
	}
}
