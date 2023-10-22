using DistributedPlayground.Infrastructure.Interceptors;

namespace DistributedPlayground.Infrastructure.Tests.Interceptors;

public class InterceptionExtensionsTests
{
	[Fact]
	public void then_works_for_send()
	{
		// given
		var results = new List<(Delay, Message)>();
		var send = SendingInterceptors.None
			.Then(SendingInterceptors.AbsoluteDelay((d, _) => d * 2))
			.Then(SendingInterceptors.AbsoluteDelay((d, _) => d + new Delay(1)))
			.Then((d, m) => results.Add((d, m)));

		// when
		send(new Delay(4), new Message());

		// then
		Assert.Equal((new Delay(9), new Message()), results.Single());
	}
}
