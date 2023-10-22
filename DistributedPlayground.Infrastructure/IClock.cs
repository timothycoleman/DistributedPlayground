namespace DistributedPlayground.Infrastructure;

public interface IClock
{
	Instant Now { get; }
}
