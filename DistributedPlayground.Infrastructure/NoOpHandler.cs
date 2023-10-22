namespace DistributedPlayground.Infrastructure;

public class NoOpHandler : IHandle
{
	public static readonly NoOpHandler Instance = new();
	public void Handle(Message message)
	{ }
}
