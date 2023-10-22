using DistributedPlayground.Infrastructure;

namespace DistributedPlayground.PingPong;

public class Ponger : LogicalProcess
{
	public const string Name = "Ponger";

	public Ponger() : base(Name)
	{ }

	public override void Handle(Message msg) => HandleImpl((dynamic)msg);

	public override void Start()
	{ }

	public void HandleImpl(Messages.Ping msg)
	{
		Send(
			to: msg.From,
			message: new Messages.Pong(Number: msg.Number));
	}
}
