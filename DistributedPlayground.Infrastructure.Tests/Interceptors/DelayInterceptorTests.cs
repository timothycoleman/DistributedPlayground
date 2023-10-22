using DistributedPlayground.Infrastructure.Interceptors;

namespace DistributedPlayground.Infrastructure.Tests.Interceptors;

public class DelayInterceptorTests
{
	[Theory]
	[InlineData(1, 3)]
	[InlineData(2, 6)]
	public void relative_delay(long originalDelay, long expectedDelay)
	{
		// given
		var originalMessage = new Message();
		var scheduledItems = new List<(Delay Delay, Message Message)>();

		var sut = SendingInterceptors
			.RelativeDelay((delay, _) => delay * 2)
			.Then((d, m) => scheduledItems.Add((d, m)));

		// when
		sut(new Delay(originalDelay), originalMessage);

		// then
		Assert.Collection(
			scheduledItems,
			(item) =>
			{
				Assert.Equal(new Delay(expectedDelay), item.Delay);
				Assert.Equal(originalMessage, item.Message);
			});
	}

	[Theory]
	[InlineData(1, 2)]
	[InlineData(2, 4)]
	public void absolute_delay(long originalDelay, long expectedDelay)
	{
		// given
		var originalMessage = new Message();
		var scheduledItems = new List<(Delay Delay, Message Message)>();

		var sut = SendingInterceptors
			.AbsoluteDelay((delay, _) => delay * 2)
			.Then((d, m) => scheduledItems.Add((d, m)));

		// when
		sut(new Delay(originalDelay), originalMessage);

		// then
		Assert.Collection(
			scheduledItems,
			(item) =>
			{
				Assert.Equal(new Delay(expectedDelay), item.Delay);
				Assert.Equal(originalMessage, item.Message);
			});
	}
}
