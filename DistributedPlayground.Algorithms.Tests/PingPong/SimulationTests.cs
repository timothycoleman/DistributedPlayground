namespace DistributedPlayground.Tests.PingPong;

using DistributedPlayground.Infrastructure;
using DistributedPlayground.Infrastructure.Interceptors;
using DistributedPlayground.PingPong;

public class SimulationTests
{
	[Fact]
	public void simple()
	{
		var clock = new Clock();

		var network = new Network(
			clock,
			Interceptors.Random(
				random: new Random(0),
				minDelay: 5,
				maxDelay: 5,
				percentLoss: 0,
				out _),
			new Pinger(),
			new Ponger());

		new Simulation(clock, network).RunUntil(new Instant(100));

		Assert.Equal(10, network.MessagesDelivered.OfType<Messages.Ping>().Count());
		Assert.Equal(10, network.MessagesDelivered.OfType<Messages.Pong>().Count());
	}

	[Fact]
	public void simulate_network_partitions_manually()
	{
		var clock = new Clock();

		var network = new Network(
			clock,
			Interceptors.Random(
				random: new Random(0),
				minDelay: 5,
				maxDelay: 5,
				percentLoss: 0,
				out var partitionManager),
			new Pinger(),
			new Ponger());

		var sim = new Simulation(clock, network);

		// advance the simulation with no partition. expect pings and pongs
		sim.RunUntil(new Instant(100));

		Assert.Equal(10, network.MessagesDelivered.OfType<Messages.Ping>().Count());
		Assert.Equal(10, network.MessagesDelivered.OfType<Messages.Pong>().Count());

		// separate the pinger and ponger into different partitions.
		// advance the simuation but expect no more pings and pongs
		partitionManager.Partition(
			new[] { "Pinger" },
			new[] { "Ponger" });
		sim.RunMore(new Delay(100));

		Assert.Equal(10, network.MessagesDelivered.OfType<Messages.Ping>().Count());
		Assert.Equal(10, network.MessagesDelivered.OfType<Messages.Pong>().Count());

		// clear the partitions and advance the simuation. expect more pings and pongs
		partitionManager.ClearPartitions();
		sim.RunMore(new Delay(100));

		Assert.Equal(20, network.MessagesDelivered.OfType<Messages.Ping>().Count());
		Assert.Equal(20, network.MessagesDelivered.OfType<Messages.Pong>().Count());
	}

	[Fact]
	public void simulate_network_partitions_automatically()
	{
		var clock = new Clock();

		var network = new Network(
			clock,
			Interceptors.Random(
				random: new Random(0),
				minDelay: 5,
				maxDelay: 5,
				percentLoss: 0,
				out var partitionManager),
			new Pinger(),
			new Ponger(),
			new PartitioningProcess(partitionManager, Pinger.Name));

		var sim = new Simulation(clock, network);

		// Partitioning process automatically creates and repairs partitions
		sim.RunUntil(new Instant(300));

		Assert.Equal(19, network.MessagesDelivered.OfType<Messages.Ping>().Count());
		Assert.Equal(17, network.MessagesDelivered.OfType<Messages.Pong>().Count());
	}
}
