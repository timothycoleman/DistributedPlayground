namespace DistributedPlayground.Infrastructure;

public record class Message
{
	public const string All = "$all";
	public const string AllOthers = "$all-others";
	public const string Unknown = "$unknown";

	public string From { get; init; } = Unknown;

	public string To { get; init; } = Unknown;
}
