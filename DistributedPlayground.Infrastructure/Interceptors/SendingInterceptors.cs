namespace DistributedPlayground.Infrastructure.Interceptors;

public static class SendingInterceptors
{
	public static InterceptSending AbsoluteDelay(Func<Delay, Message, Delay> getDelay) =>
		(delay, message, next) =>
			next(getDelay(delay, message), message);

	public static InterceptSending RelativeDelay(Func<Delay, Message, Delay> getDelay) =>
		(delay, message, next) =>
			next(delay + getDelay(delay, message), message);

	public static InterceptSending Duplicate(Func<Delay, Message, int> getNumDuplicates) =>
		(delay, message, next) =>
		{
			var numDuplicates = getNumDuplicates(delay, message);
			for (var i = 0; i < numDuplicates; i++)
			{
				next(delay, message);
			}
		};

	public static InterceptSending Filter(Func<Delay, Message, bool> keep) =>
		(delay, message, next) =>
		{
			if (keep(delay, message))
				next(delay, message);
		};

	public static InterceptSending EnsurePositiveDelay { get; } =
		(delay, message, next) =>
		{
			if (delay <= Delay.None)
				delay = new Delay(1);

			next(delay, message);
		};

	public static InterceptSending None { get; } =
		(delay, message, next) =>
			next(delay, message);
}
