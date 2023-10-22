using DistributedPlayground.Infrastructure.Interceptors;

namespace DistributedPlayground.Infrastructure.Tests.Interceptors;

public class DuplicateInterceptorTests
{
	[Fact]
	public void intercepts_correctly()
	{
		// given
		var originalMessage = new Message();
		var scheduledItems = new List<(Delay Delay, Message Message)>();

		var sut = SendingInterceptors
			.Duplicate((_, _) => 2)
			.Then((d, m) => scheduledItems.Add((d, m)));

		// when
		sut(new Delay(1), originalMessage);

		// then
		Assert.Collection(
			scheduledItems,
			(item) =>
			{
				Assert.Equal(new Delay(1), item.Delay);
				Assert.Equal(originalMessage, item.Message);
			},
			(item) =>
			{
				Assert.Equal(new Delay(1), item.Delay);
				Assert.Equal(originalMessage, item.Message);
			});
	}
}
