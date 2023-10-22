namespace DistributedPlayground.Tests.PingPongSimple;

using DistributedPlayground.Algorithms.Tests.Infrastructure;
using DistributedPlayground.PingPongSimple;

public class SimplePingerTests
{
	private readonly SimplePinger _sut;
	private readonly Runner _runner;

	public SimplePingerTests()
	{
		_sut = new SimplePinger();
		_runner = new Runner(_sut);
		_sut.Start();
	}

	[Fact]
	public void on_start_send_ping() =>
		_runner.Run(def => def
			.Given()
			.When()
			.ThenTo(
				SimplePonger.Name,
				new Messages.Ping()));

	[Fact]
	public void on_pong_reply_ping() =>
		_runner.Run(def => def
			.Given()
			.WhenFrom(
				SimplePonger.Name,
				new Messages.Pong())
			.ThenTo(
				SimplePonger.Name,
				new Messages.Ping()));
}
