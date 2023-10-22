namespace DistributedPlayground.Tests.PingPongSimple;

using DistributedPlayground.Infrastructure;
using DistributedPlayground.Infrastructure.Interceptors;
using DistributedPlayground.PingPongSimple;

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
			new SimplePinger(),
			new SimplePonger());

		new Simulation(clock, network).RunUntil(new Instant(100));

		Assert.Equal(10, network.MessagesDelivered.OfType<Messages.Ping>().Count());
		Assert.Equal(10, network.MessagesDelivered.OfType<Messages.Pong>().Count());
	}
}
