using DistributedPlayground.Infrastructure;

namespace DistributedPlayground.PingPong;

public static class Messages
{
	public record Ping(int Number) : Message;

	public record Pong(int Number) : Message;
}
