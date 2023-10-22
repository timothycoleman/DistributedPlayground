namespace DistributedPlayground.Infrastructure.Interceptors;

public static class Interceptors
{
	public static InterceptorPair None { get; } =
		new InterceptorPair(
			SendingInterceptors.None,
			DeliveryInterceptors.None);

	public static InterceptorPair Random(
		Random random,
		int minDelay,
		int maxDelay,
		int percentLoss,
		out Partitioner partitioner)
	{
		return new InterceptorPair(
			SendingInterceptors.None
				.Then(SendingInterceptors.Filter(
					(_, _) => random.Next(100) >= percentLoss))
				.Then(SendingInterceptors.AbsoluteDelay(
					(_, _) => new Delay(random.Next(minDelay, maxDelay)))),
			DeliveryInterceptors.None
				.Then(DeliveryInterceptors.Partition(out partitioner)));
	}
}
