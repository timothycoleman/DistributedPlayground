namespace DistributedPlayground.Tests.PingPong;

using DistributedPlayground.Algorithms.Tests.Infrastructure;
using DistributedPlayground.Infrastructure;
using DistributedPlayground.PingPong;

public class PingerTests
{
	private readonly Pinger _sut;
	private readonly Runner _runner;

	public PingerTests()
	{
		_sut = new Pinger();
		_runner = new Runner(_sut);
		_sut.Start();
	}

	[Fact]
	public void on_start_send_ping() =>
		_runner.Run(def => def
			.Given()
			.When()
			.Then(
				to: Ponger.Name,
				new Messages.Ping(0),
				withCallbackAfter: new Delay(20)));

	[Fact]
	public void on_pong_reply_ping() =>
		_runner.Run(def => def
			.Given()
			.WhenFrom(
				Ponger.Name,
				new Messages.Pong(0))
			.Then(
				Ponger.Name,
				new Messages.Ping(1),
				withCallbackAfter: new Delay(20)));

	[Fact]
	public void on_timeout_send_ping() =>
		_runner.Run(def => def
			.Given()
			.WhenCallback(
				Ponger.Name,
				new Messages.Ping(0))
			.Then(
				Ponger.Name,
				new Messages.Ping(1),
				withCallbackAfter: new Delay(20)));

	[Fact]
	public void on_timeout_after_pong_do_nothing() =>
		_runner.Run(def => def
			.Given(
				new Messages.Pong(0))
			.WhenCallback(
				Ponger.Name,
				new Messages.Ping(0))
			.Then());
}
