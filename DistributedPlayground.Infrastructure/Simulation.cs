namespace DistributedPlayground.Infrastructure;

public class Simulation
{
	private readonly Clock _clock;
	private readonly Network _network;

	public Simulation(Clock clock, Network network)
	{
		_clock = clock;
		_network = network;
		_network.Start();
	}

	public void RunMore(Delay delay)
	{
		RunUntil(_clock.Now + delay);
	}

	public void RunUntil(Instant until)
	{
		while (_clock.Now <= until)
			Tick();
	}

	public void RunUntil(Func<bool> condition)
	{
		while (!condition())
			Tick();
	}


	public void RunUntilIdle()
	{
		while (!_network.IsEmpty)
			Tick();
	}

	private void Tick()
	{
		_network.Tick();
		_clock.Tick();
	}
}
