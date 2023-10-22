using DistributedPlayground.Infrastructure;

namespace DistributedPlayground.PingPongSimple;

public class SimplePonger : LogicalProcess
{
	public const string Name = "Ponger";

	public SimplePonger() : base(Name)
	{ }

	public override void Handle(Message msg) => HandleImpl((dynamic)msg);

	public override void Start()
	{ }

	public void HandleImpl(Messages.Ping msg)
	{
		Send(
			to: msg.From,
			message: new Messages.Pong());
	}
}
