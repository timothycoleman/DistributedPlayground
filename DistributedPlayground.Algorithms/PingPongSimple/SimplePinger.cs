using DistributedPlayground.Infrastructure;

namespace DistributedPlayground.PingPongSimple;

public class SimplePinger : LogicalProcess
{
	public const string Name = "Pinger";

	public SimplePinger() : base(Name)
	{ }

	public override void Handle(Message msg) => HandleImpl((dynamic)msg);

	public override void Start()
	{
		SendPing();
	}

	private void SendPing()
	{
		Send(
			to: SimplePonger.Name,
			message: new Messages.Ping());
	}

	private void HandleImpl(Messages.Pong msg)
	{
		SendPing();
	}
}
