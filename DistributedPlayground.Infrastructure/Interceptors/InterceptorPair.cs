namespace DistributedPlayground.Infrastructure.Interceptors;

public readonly struct InterceptorPair
{
	public InterceptorPair(InterceptSending sendInterceptor, InterceptDelivery deliveryInterceptor)
	{
		SendInterceptor = sendInterceptor;
		DeliveryInterceptor = deliveryInterceptor;
	}

	public InterceptSending SendInterceptor { get; }
	public InterceptDelivery DeliveryInterceptor { get; }
}
