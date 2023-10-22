using DistributedPlayground.Infrastructure;
using DistributedPlayground.Infrastructure.Interceptors;
using DistributedPlayground.Infrastructure.Messages;

namespace DistributedPlayground.Tests.PingPong;

// example to show how to have partitions managed by a process
public class PartitioningProcess : LogicalProcess
{
	public const string Name = "Partitioner";

	private readonly Delay _delay = new(100);
	private readonly Partitioner _partitioner;
	private readonly string _idToPartition;

	public PartitioningProcess(Partitioner partitioner, string idToPartition) : base(Name)
	{
		_partitioner = partitioner;
		_idToPartition = idToPartition;
	}

	public override void Handle(Message message) => Handle((dynamic)message);

	public override void Start()
	{
		SendToSelf(new Callback<Rift>(_delay, new Rift()));
	}

	public void Handle(Callback<Rift> message)
	{
		_partitioner.Partition(_idToPartition);
		SendToSelf(new Callback<Repair>(_delay, new Repair()));
	}

	public void Handle(Callback<Repair> message)
	{
		_partitioner.ClearPartitions();
		SendToSelf(new Callback<Rift>(_delay, new Rift()));
	}

	public record Rift : Message;
	public record Repair : Message;
}
