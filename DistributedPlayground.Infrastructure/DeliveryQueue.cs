namespace DistributedPlayground.Infrastructure;

public class DeliveryQueue
{
	private readonly SortedDictionary<Instant, List<Message>> _queue = new();
	private readonly IClock _clock;

	public DeliveryQueue(IClock clock)
	{
		_clock = clock;
	}

	public bool IsEmpty => _queue.Count == 0;

	public void Schedule(Delay delay, Message message)
	{
		if (delay <= Delay.None)
			throw new InvalidOperationException("Delay must be positive");

		var instant = _clock.Now.Add(delay);
		if (!_queue.TryGetValue(instant, out var list))
		{
			list = new List<Message>();
			_queue.Add(instant, list);
		}

		list.Add(message);
	}

	public IReadOnlyList<Message> ForInstant(Instant instant)
	{
		if (!_queue.Remove(instant, out var list))
			return Array.Empty<Message>();

		return list;
	}
}
