namespace REPL.Service
{
	using REPL.Contracts;
	using System;
	using System.ServiceModel;
	using System.ServiceModel.Web;

	[ServiceContract]
    public interface IReplService
    {
        [OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "repl/eval", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
		EvalResult Eval(Guid sessionId, string code);
    }
}
