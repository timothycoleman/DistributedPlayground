namespace DistributedPlayground.Infrastructure.Messages;

public record Callback(Delay Delay) : Message;

public record Callback<T>(Delay Delay, T Message) : Callback(Delay);
