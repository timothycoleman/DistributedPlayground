namespace DistributedPlayground.Infrastructure.Interceptors;

public static class DeliveryInterceptors
{
	public static InterceptDelivery None { get; } =
		(message, next) => next(message);

	public static InterceptDelivery Partition(out Partitioner partitioner)
	{
		partitioner = new Partitioner();
		return partitioner.Intercept;
	}
}
