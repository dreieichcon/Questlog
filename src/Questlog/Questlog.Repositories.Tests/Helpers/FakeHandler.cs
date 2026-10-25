using System.Net;
using System.Text;

namespace Questlog.Repositories.Tests.Helpers;

public sealed class FakeHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> responder) : HttpMessageHandler
{
	public List<HttpRequestMessage> Requests { get; } = [];

	public static FakeHandler Responding(HttpStatusCode status, string content = "")
		=> new(_ => Task.FromResult(new HttpResponseMessage(status) { Content = new StringContent(content, Encoding.UTF8) }));

	public static FakeHandler Throwing(Exception exception)
		=> new(_ => throw exception);

	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
	{
		Requests.Add(request);
		return responder(request);
	}
}
