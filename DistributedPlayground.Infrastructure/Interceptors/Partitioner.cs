namespace DistributedPlayground.Infrastructure.Interceptors;

public class Partitioner
{
	private Dictionary<string, int> _partitions = new();

	public void ClearPartitions()
	{
		_partitions = new();
	}

	public void Partition(params string[][] partitions)
	{
		ClearPartitions();

		for (var partitionIndex = 0; partitionIndex < partitions.Length; partitionIndex++)
		{
			var partition = partitions[partitionIndex];
			foreach (var processId in partition)
			{
				_partitions[processId] = partitionIndex;
			}
		}
	}

	public void Partition(params string[] processIds)
	{
		ClearPartitions();

		foreach (var processId in processIds)
		{
			_partitions[processId] = 0;
		}
	}

	public void Intercept(Message message, Deliver next)
	{
		if (PartitionOf(message.From) != PartitionOf(message.To))
			return;

		next(message);
	}

	private int PartitionOf(string id)
	{
		if (!_partitions.TryGetValue(id, out var partition))
			return -1;
		return partition;
	}
}
