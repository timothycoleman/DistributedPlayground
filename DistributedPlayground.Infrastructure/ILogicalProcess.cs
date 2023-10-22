namespace DistributedPlayground.Infrastructure;

public interface ILogicalProcess : IHandle
{
	string Id { get; }

	IHandle Output { set; }

	void Start();
}
