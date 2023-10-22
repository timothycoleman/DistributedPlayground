using DistributedPlayground.Infrastructure.Interceptors;
using DistributedPlayground.Infrastructure.Messages;

namespace DistributedPlayground.Infrastructure;

public class Network : IHandle
{
	private readonly IClock _clock;
	private readonly Send _scheduledSend;
	private readonly DeliveryQueue _deliveryQueue;
	private readonly Deliver _deliver;
	private readonly Dictionary<string, ILogicalProcess> _processes = new();

	public bool IsEmpty => _deliveryQueue.IsEmpty;

	public List<(Instant HandledAt, Message Message)> Handlings { get; } = new();
	public List<(Instant DeliveredAt, Message Message)> Deliveries { get; } = new();

	public IReadOnlyList<Message> MessagesHandled => Handlings.Select(x => x.Message).ToArray();
	public IReadOnlyList<Message> MessagesDelivered => Deliveries.Select(x => x.Message).ToArray();

	public Network(
		IClock clock,
		InterceptorPair interceptors,
		params ILogicalProcess[] processes)
	{
		_clock = clock;
		_deliveryQueue = new(_clock);

		_scheduledSend = interceptors.SendInterceptor
			.Then(SendingInterceptors.EnsurePositiveDelay)
			.Then(_deliveryQueue.Schedule);

		_deliver = interceptors.DeliveryInterceptor
			.Then(Deliver);

		foreach (var process in processes)
		{
			_processes[process.Id] = process;
			process.Output = this;
		}
	}

	public Network Start()
	{
		foreach (var component in _processes)
			component.Value.Start();
		return this;
	}

	// Send the message, taking into account network latency, failures, duplications, etc.
	public void Handle(Message msg)
	{
		Handlings.Add((_clock.Now, msg));

		if (msg is Callback callback)
		{
			// direct on the sendqueue not via the scheduler. callbacks happen reliably.
			_deliveryQueue.Schedule(callback.Delay, msg);
		}
		else if (msg.To == Message.All || msg.To == Message.AllOthers)
		{
			// when sending to all, schedule each one separately so that some can fail while others succeed
			foreach (var component in _processes)
			{
				if (msg.To == Message.AllOthers && component.Key == msg.From)
					continue;

				_scheduledSend(Delay.None, msg with { To = component.Key });
			}
		}
		else
		{
			_scheduledSend(Delay.None, msg);
		}
	}

	public void Tick()
	{
		var messagesToSend = _deliveryQueue.ForInstant(_clock.Now);
		foreach (var msg in messagesToSend)
		{
			_deliver(msg);
		}
	}

	private void Deliver(Message msg)
	{
		if (_processes.TryGetValue(msg.To, out var process))
		{
			process.Handle(msg);
			Deliveries.Add((_clock.Now, msg));
		}
	}
}
