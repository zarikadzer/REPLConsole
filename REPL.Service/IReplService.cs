namespace REPL.Service
{
	using System.ComponentModel;
	using System.ServiceModel;
	using System.ServiceModel.Activation;
	using System.ServiceModel.Web;
	using REPL.Contracts.Eval;

	[ServiceContract]
	public interface IReplService
	{
		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "cs/eval", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[Description("Evaluates the C# code and returns the EvalResult object.")]
		EvalResult EvalCS(EvalRequest request);

		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "cs/validate", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[Description("Validates the C# code and returns the EvalResult object.")]
		EvalResult ValidateCS(EvalRequest request);
	}
}
