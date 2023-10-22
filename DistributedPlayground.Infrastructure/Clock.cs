namespace DistributedPlayground.Infrastructure;

public class Clock : IClock
{
	public Instant Now { get; set; } = new Instant(0);

	public void Tick()
	{
		Now = Now.Add(new Delay(1));
	}
}
