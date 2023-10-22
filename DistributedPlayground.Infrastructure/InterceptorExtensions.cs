namespace DistributedPlayground.Infrastructure;

public static class InterceptorExtensions
{
	public static InterceptSending Then(
		this InterceptSending x,
		InterceptSending y) =>
		(delay, msg, next) => x(delay, msg, (d, m) => y(d, m, next));

	public static Send Then(this InterceptSending x, Send y) =>
		(delay, msg) => x(delay, msg, y);

	public static InterceptDelivery Then(this InterceptDelivery x, InterceptDelivery y) =>
		(msg, next) => x(msg, m => y(m, next));

	public static Deliver Then(this InterceptDelivery x, Deliver y) =>
		msg => x(msg, y);
}
