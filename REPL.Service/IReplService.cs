namespace REPL.Service
{
	using REPL.Contracts;
	using System;
    using System.ComponentModel;
    using System.ServiceModel;
	using System.ServiceModel.Web;

	[ServiceContract]
    public interface IReplService
    {
        [OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "repl/eval/cs", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        [Description("Evaluates the C# code and returns the EvalResult object.")]
		EvalResult EvalCS(Guid sessionId, string code);
    }
}
