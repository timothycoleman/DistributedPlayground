using DistributedPlayground.Infrastructure;
using DistributedPlayground.Infrastructure.Messages;

namespace DistributedPlayground.PingPong;

public class Pinger : LogicalProcess
{
	public const string Name = "Pinger";

	private int _currentPingNumber = -1;

	public Pinger() : base(Name)
	{ }

	public override void Handle(Message msg) => HandleImpl((dynamic)msg);

	public override void Start()
	{
		SendPing();
	}

	private void SendPing()
	{
		// Send a ping to Ponger.Name.
		// We also receive the same message wrapped in a TimerCallback after a delay
		// so that we can retry if we want.
		Send(
			to: Ponger.Name,
			message: new Messages.Ping(Number: ++_currentPingNumber),
			callbackIn: new Delay(20));
	}

	private void HandleImpl(Messages.Pong msg)
	{
		if (msg.Number != _currentPingNumber)
		{
			// this pong is not for the current ping. ignore.
			return;
		}

		SendPing();
	}

	private void HandleImpl(Callback<Messages.Ping> msg)
	{
		if (msg.Message.Number != _currentPingNumber)
		{
			// we have sent another ping since. nothing to do
			return;
		}

		// timeout for the current ping. send another one.
		SendPing();
	}
}
