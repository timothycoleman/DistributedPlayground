namespace DistributedPlayground.Infrastructure;

public interface IHandle
{
	void Handle(Message message);
}
