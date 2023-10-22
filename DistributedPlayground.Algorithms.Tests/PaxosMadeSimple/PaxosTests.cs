namespace DistributedPlayground.Tests.PaxosMadeSimple;

using System;
using DistributedPlayground.Infrastructure;
using DistributedPlayground.Infrastructure.Interceptors;
using DistributedPlayground.PaxosMadeSimple;

public class Tests
{
	[Fact]
	public void works_with_reliable_network()
	{
		Run(Interceptors.None);
	}

	[Fact]
	public void works_with_unreliable_network()
	{
		Run(Interceptors.Random(
			random: new Random(0),
			minDelay: 5,
			maxDelay: 10,
			percentLoss: 50,
			out _));
	}

	private static void Run(InterceptorPair interceptors)
	{
		var clock = new Clock();
		var clusterSize = 3;

		var nodeIndex = 0;
		var nodes = new[]
		{
			new Node($"Node{nodeIndex}", nodeIndex++, clusterSize),
			new Node($"Node{nodeIndex}", nodeIndex++, clusterSize),
			new Node($"Node{nodeIndex}", nodeIndex++, clusterSize),
		};

		var network = new Network(clock, interceptors, nodes);

		new Simulation(clock, network).RunUntilIdle();

		// choose a value
		Assert.NotNull(nodes[0].ChosenValue);

		// value chosen is on the expected values
		Assert.Contains(nodes[0].ChosenValue!, new[] { "Node1", "Node2", "Node3" });

		// other nodes choose the same value
		Assert.Equal(nodes[0].ChosenValue, nodes[1].ChosenValue);
		Assert.Equal(nodes[0].ChosenValue, nodes[2].ChosenValue);
	}
}
