using DistributedPlayground.Infrastructure;

namespace DistributedPlayground.PingPongSimple;

public static class Messages
{
	public record Ping() : Message;

	public record Pong() : Message;
}
